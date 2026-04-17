using System;

namespace TinyLanguage.Lexer;

/// <summary>
/// Thrown when the lexer encounters source text that cannot be tokenised —
/// for example an unterminated string literal or an invalid escape sequence.
/// </summary>
public sealed class LexerException : Exception
{
    /// <summary>1-based source line number where the error was detected.</summary>
    public readonly int Line;

    public LexerException(string message, int line)
        : base($"Lexer error at line {line}: {message}")
    {
        Line = line;
    }
}
