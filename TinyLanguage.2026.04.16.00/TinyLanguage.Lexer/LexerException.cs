using System;

namespace TinyLanguage.Lexer
{
    // Thrown by the Lexer when it encounters source text it cannot tokenise —
    // for example an unknown character or an unterminated string literal.
    // The Line property carries the 1-based source line where the problem was
    // found so that the caller can produce a useful error message.
    public class LexerException : Exception
    {
        public int Line { get; }

        public LexerException(string message, int line)
            : base($"Line {line}: {message}")
        {
            Line = line;
        }
    }
}
