namespace LPRMock.Models
{
    public class PrintJob
    {
        public PrintJob()
        {
            timestamp = DateTime.Now;
        }

        public string PrinterQueue { get; set; }
        public int ControlfileLength { get; set; }
        public string ControlfileName { get; set; }
        public string SourceHost { get; set; }
        public int DataFileLength { get; set; }
        public string DataFileName { get; set; }
        public string JobName { get; set; }
        public int JobNumber { get; set; }
        public string User { get; set; }
        public string FileName { get; set; }
        public string Payload { get; set; } = string.Empty;
        public string? RejectReason { get; set; }
        public DateTime timestamp { get; private set; }
    }
}
