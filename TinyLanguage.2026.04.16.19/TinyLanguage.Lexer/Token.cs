namespace TinyLanguage.Lexer;

// Represents a single token produced by the Lexer.
// Line is the 1-based source line where the token starts.
public sealed record Token(TokenType Type, string Value, int Line);
