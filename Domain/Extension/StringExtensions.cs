namespace Domain.Extension
{
    public static class StringExtensions
    {
        public static string Masked(this string source, int start = 6, int count= 6)
        {
            return source.Masked('x', start, count);
        }

        private static string Masked(this string source, char maskValue, int start, int count)
        {
            var firstPart = source.Substring(0, start);
            var lastPart = source.Substring(start + count);
            var middlePart = new string(maskValue, count);

            return firstPart + middlePart + lastPart;
        }
    }
}
