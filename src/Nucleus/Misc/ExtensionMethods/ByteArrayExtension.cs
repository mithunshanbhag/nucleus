namespace Nucleus.Misc.ExtensionMethods;

public static class ByteArrayExtension
{
    extension(byte[] bytes)
    {
        public string AsString()
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public Stream AsStream()
        {
            return new MemoryStream(bytes);
        }
    }
}