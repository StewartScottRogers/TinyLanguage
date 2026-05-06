using System;

namespace TinyLanguage.Lexer
{
    // Exception thrown when the parser encounters a malformed token stream.
    // The 'line' parameter (lowerCamelCase, same name as the variable) records
    // the 1-based source line number where the error occurred.
    public class ParserException : Exception
    {
        public int Line { get; }

        public ParserException(string message, int line)
            : base($"Parser error at line {line}: {message}")
        {
            Line = line;
        }
    }
}
