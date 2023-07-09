using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using LPRMock.Models;

namespace LPRMock.Services
{
    public class LPRService : BackgroundService
    {
        const int bufferSize = 512;
        private readonly int port;
        private Byte[] bytes = new Byte[bufferSize];
        private ILogger<LPRService> logger;

        private TcpListener server;

        public LPRService(ILogger<LPRService> logger, IConfiguration configuration)
        {
            this.logger = logger;

            int _confPort = default;
            try
            {
                _confPort = configuration.GetValue<int>("LPRPort");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new ArgumentException("Configuration.LPRPort");
            }

            this.port = _confPort != default ? _confPort : 515;


            IPAddress localAddr;
            string _confIP = string.Empty;
            try
            {
                _confIP = configuration.GetValue<string>("LPRIP")?.Trim();
                if (string.IsNullOrEmpty(_confIP))
                {
                    logger.LogWarning("Configuration.LPRIP is invalid using default 127.0.0.1");
                    _confIP = "127.0.0.1";
                }

                localAddr = IPAddress.Parse(_confIP);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                throw new ArgumentException("Configuration.LPRIP");
            }


            server = new TcpListener(localAddr, port);
        }

        public string? BoundEndpoint => server.LocalEndpoint.ToString();


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Factory.StartNew(() => RunLPR(stoppingToken), TaskCreationOptions.LongRunning);
        }

        private void RunLPR(CancellationToken stoppingToken)
        {
            string data = null;
            string[] splits, subsplits;
            string commandPayload = null;
            char commandCode = '\0';

            server.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using TcpClient client = server.AcceptTcpClient();

                    data = null;
                    splits = null;
                    subsplits = null;
                    commandCode = '\0';
                    int readByteCount;

                    // 0 normal
                    // 1 control file
                    // 2 data file
                    ushort mode = 0;

                    NetworkStream stream = client.GetStream();

                    PrintJob? printJob = new PrintJob();

                    while (stream.Socket.Connected && !stoppingToken.IsCancellationRequested)
                    {
                        data = string.Empty;
                        do
                        {
                            readByteCount = stream.Read(bytes, 0, bytes.Length);
                            data += Encoding.ASCII.GetString(bytes, 0, readByteCount);
                        } while (readByteCount != 0 && readByteCount == bufferSize);

                        if (mode == 2)
                        {
                            printJob.Payload = data;
                            mode = 0;
                            stream.Acknowledge();
                            break;
                        }


                        splits = data.Split('\n');
                        splits = splits.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        foreach (string split in splits)
                        {
                            commandCode = split[0];
                            commandPayload = split.Substring(1);
                            switch (commandCode)
                            {
                                case '\u0002': // 2
                                {
                                    if (string.IsNullOrEmpty(printJob.PrinterQueue))
                                    {
                                        printJob.PrinterQueue = commandPayload;

                                        if (PrintFilter.allowedPrinternames.Any())
                                        {
                                            if (!PrintFilter.allowedPrinternames.Contains(commandPayload))
                                            {
                                                stream.Refuse();
                                                stream.Dispose();

                                                printJob.RejectReason = nameof(PrintFilter.allowedPrinternames);
                                                break;
                                            }
                                        }
                                        stream.Acknowledge();
                                        break;
                                    }
                                    else
                                    {
                                        // Receive control file
                                        mode = 1;
                                        subsplits = commandPayload.Split(" ");
                                        printJob.ControlfileLength = int.Parse(subsplits[0]);
                                        printJob.ControlfileName = subsplits[1];

                                        stream.Acknowledge();
                                        break;
                                    }
                                }
                                case 'H':
                                {
                                    // Host name (source)
                                    printJob.SourceHost = commandPayload;

                                    if (PrintFilter.allowedHosts.Any())
                                    {
                                        if (!PrintFilter.allowedHosts.Contains(commandPayload))
                                        {
                                            stream.Refuse();
                                            stream.Dispose();
                                            printJob.RejectReason = nameof(PrintFilter.allowedHosts);
                                        }
                                    }
                                    break;
                                }

                                case '\u0050': // P
                                {
                                    printJob.User = commandPayload;

                                    if (PrintFilter.allowedUsers.Any())
                                    {
                                        if (!PrintFilter.allowedUsers.Contains(commandPayload))
                                        {
                                            stream.Refuse();
                                            stream.Dispose();
                                            printJob.RejectReason = nameof(PrintFilter.allowedUsers);
                                        }
                                    }
                                    break;
                                }

                                case 'J': // Job Name
                                {
                                    printJob.JobName = commandPayload;
                                    break;
                                }

                                case '\u0003': //Receive data file
                                {
                                    subsplits = commandPayload.Split(" ");
                                    printJob.DataFileLength = int.Parse(subsplits[0]);

                                    var regex = new Regex(@"^dfA(\d+).+$");
                                    var match = regex.Match(subsplits[1]);

                                    printJob.JobNumber = int.Parse(match.Groups[1].Value);
                                    printJob.DataFileName = subsplits[1];

                                    stream.Acknowledge();
                                    mode = 2;
                                    break;
                                }

                                case '\u004E': // N - Name of source file
                                {
                                    printJob.FileName = commandPayload;
                                    break;
                                }


                                case 'C': // Class for banner page
                                case 'L':
                                {
                                    // ignored
                                    break;
                                }
                            }

                            switch (mode)
                            {
                                case 1:
                                case 2:
                                {
                                    if (split.EndsWith('\0'))
                                    {
                                        mode = 0;

                                        stream.Acknowledge();
                                        printJob.Payload = printJob.Payload.TrimEnd('\0');
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    Program.Jobs.Add(printJob);
                    
                }
                catch (SocketException e)
                {
                    logger.LogCritical("SocketException: {0}", e);
                }
            }

            server.Stop();
        }
    }
}