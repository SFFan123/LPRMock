namespace LPRMock.Models
{
    public static class PrintFilter
    {
        public static List<string> allowedPrinternames { get; set; }

        public static List<string> allowedUsers { get; set; }

        public static List<string> allowedHosts { get; set; }

        static PrintFilter()
        {
            allowedPrinternames = new List<string>();
            allowedUsers = new List<string>();
            allowedHosts = new List<string>();
        }
    }
}
