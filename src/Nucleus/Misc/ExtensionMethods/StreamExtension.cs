namespace Nucleus.Misc.ExtensionMethods;

public static class StreamExtension
{
    extension(Stream stream)
    {
        public byte[] AsByteArray()
        {
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        public string AsString()
        {
            return Encoding.UTF8.GetString(stream.AsByteArray());
        }
    }
}