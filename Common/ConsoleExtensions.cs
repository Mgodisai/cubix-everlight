namespace Common
{
    public static class ConsoleExtensions
    {
        private static void WriteInColor(string message, ConsoleColor color, bool needNewLine)
        {
            Console.ForegroundColor = color;
            if (needNewLine)
                Console.WriteLine(message);
            else
                Console.Write(message);

            Console.ResetColor();
        }

        public static void WriteLineSuccess(string? message)
        {
            WriteInColor(message ?? string.Empty, ConsoleColor.Green, true);
        }

        public static void WriteLineError(string? message)
        {
            WriteInColor(message ?? string.Empty, ConsoleColor.Red, true);
        }

        public static void WriteLineWarning(string? message)
        {
            WriteInColor(message ?? string.Empty, ConsoleColor.Yellow, true);
        }
    }
}
