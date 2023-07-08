namespace LPRMock
{
    public static class ExtensionMethods
    {
        public static void Acknowledge(this Stream stream)
        {
            stream.Write(new byte[] { 0 }, 0, 1);
            stream.Flush();
        }
    }
}
