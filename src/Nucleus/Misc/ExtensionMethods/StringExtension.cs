namespace Nucleus.Misc.ExtensionMethods;

public static class StringExtension
{
    extension(string input)
    {
        public byte[] AsByteArray()
        {
            return Encoding.UTF8.GetBytes(input);
        }

        public Stream AsStream()
        {
            return new MemoryStream(input.AsByteArray());
        }
    }
}