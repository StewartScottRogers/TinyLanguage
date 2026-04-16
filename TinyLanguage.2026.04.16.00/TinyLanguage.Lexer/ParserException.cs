namespace TinyLanguage.Lexer
{
    public sealed class ParserException : System.Exception
    {
        public readonly int Line;
        public ParserException(string message, int line) : base($"Line {line}: {message}") { Line = line; }
    }
}
