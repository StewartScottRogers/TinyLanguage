using System;

namespace TinyLanguage.Lexer;

/// <summary>
/// Thrown when the parser encounters a token sequence that does not match
/// any BNF production. Carries the 1-based source line number where the
/// error was detected.
/// </summary>
public sealed class ParserException : Exception
{
    /// <summary>1-based source line number where the error was detected.</summary>
    public readonly int Line;

    public ParserException(string message, int line)
        : base($"Line {line}: {message}")
    {
        Line = line;
    }
}
