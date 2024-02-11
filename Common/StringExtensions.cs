namespace Common
{
    public static class StringExtensions
    {
        public static string TruncateString(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : string.Concat(input.AsSpan(0, maxLength), "...");
        }
    }
}
