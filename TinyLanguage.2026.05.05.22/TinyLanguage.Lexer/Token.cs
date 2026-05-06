namespace TinyLanguage.Lexer
{
    // Immutable record representing a single lexed token.
    // Value holds the raw source text of the token (the lexeme).
    // Line is the 1-based source line number where the token starts.
    public record Token(string Value, TokenType Type, int Line);
}
