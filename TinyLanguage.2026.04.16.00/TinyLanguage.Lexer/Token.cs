namespace TinyLanguage.Lexer
{
    // An immutable value produced by the Lexer for every lexical unit of
    // source text. Three fields capture everything the parser needs:
    //   Type  — what kind of token this is (keyword, operator, literal, …)
    //   Value — the raw source text that was matched
    //   Line  — the 1-based source line where the token starts
    public record Token(TokenType Type, string Value, int Line);
}
