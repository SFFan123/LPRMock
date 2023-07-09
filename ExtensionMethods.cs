namespace LPRMock
{
    public static class ExtensionMethods
    {
        public static void Acknowledge(this Stream stream)
        {
            stream.Write(new byte[] { 0 }, 0, 1);
            stream.Flush();
        }
        public static void Refuse(this Stream stream)
        {
            stream.Write(new byte[] { 1 }, 0, 1);
            stream.Flush();
        }
    }
}
