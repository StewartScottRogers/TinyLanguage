using System;

namespace TinyLanguage.Lexer
{
    // Exception thrown when the lexer encounters invalid source text.
    // The 'line' parameter (lowerCamelCase, same name as the variable) records
    // the 1-based source line number where the error occurred.
    public class LexerException : Exception
    {
        public int Line { get; }

        public LexerException(string message, int line)
            : base($"Lexer error at line {line}: {message}")
        {
            Line = line;
        }
    }
}
