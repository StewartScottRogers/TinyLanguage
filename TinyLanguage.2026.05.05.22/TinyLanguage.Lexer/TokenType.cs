namespace TinyLanguage.Lexer
{
    // Enumeration of every token type produced by the TinyLanguage lexer.
    // Keyword entries match their exact source spelling (case-sensitive).
    // Operator and punctuation entries use descriptive names.
    public enum TokenType
    {
        // ---------------------------------------------------------------
        // Declaration keywords
        // ---------------------------------------------------------------
        Let,
        Var,
        Const,
        Enum,
        Function,
        Return,
        Class,
        Extends,
        Implements,
        Constructor,   // Capital C — matches the BNF production exactly
        New,
        Static,
        Module,
        Import,
        Export,
        As,

        // ---------------------------------------------------------------
        // Control-flow keywords
        // ---------------------------------------------------------------
        If,
        Then,
        Else,
        End,
        While,
        Do,
        For,
        To,
        Step,
        Foreach,
        In,
        Break,
        Continue,
        Switch,
        Case,
        Default,

        // ---------------------------------------------------------------
        // I/O keywords
        // ---------------------------------------------------------------
        Print,
        Input,

        // ---------------------------------------------------------------
        // Exception keywords
        // ---------------------------------------------------------------
        Try,
        Catch,
        Finally,
        Throw,

        // ---------------------------------------------------------------
        // Pattern-matching keywords
        // ---------------------------------------------------------------
        Match,
        When,

        // ---------------------------------------------------------------
        // Logical keywords
        // ---------------------------------------------------------------
        And,
        Or,
        Not,
        Is,

        // ---------------------------------------------------------------
        // Self-reference keyword
        // ---------------------------------------------------------------
        This,

        // ---------------------------------------------------------------
        // Built-in type-name keywords
        // ---------------------------------------------------------------
        Int,
        Float,
        String,
        Bool,
        Array,
        Object,
        Map,
        Void,

        // ---------------------------------------------------------------
        // Literal-value keywords
        // ---------------------------------------------------------------
        Null,
        True,
        False,

        // ---------------------------------------------------------------
        // Identifiers
        // ---------------------------------------------------------------
        Identifier,

        // ---------------------------------------------------------------
        // Assignment operators
        // ---------------------------------------------------------------
        Assign,         // :=
        SingleEqual,    // =   (valid only in enum value lists and annotation param lists)

        // ---------------------------------------------------------------
        // Arithmetic operators
        // ---------------------------------------------------------------
        Plus,           // +
        Minus,          // -
        Star,           // *
        Slash,          // /
        Percent,        // %
        StarStar,       // **
        SlashSlash,     // //   (floor division — NEVER a comment)
        Amp,            // &    (string concatenation)

        // ---------------------------------------------------------------
        // Comparison operators
        // ---------------------------------------------------------------
        EqualEqual,     // ==
        NotEqual,       // !=
        Less,           // <
        Greater,        // >
        LessEqual,      // <=
        GreaterEqual,   // >=

        // ---------------------------------------------------------------
        // Logical operators
        // ---------------------------------------------------------------
        AmpAmp,         // &&
        PipePipe,       // ||
        Pipe,           // |   (pattern alternation — NOT a comment)

        // ---------------------------------------------------------------
        // Arrow operators
        // ---------------------------------------------------------------
        Arrow,          // ->
        FatArrow,       // =>

        // ---------------------------------------------------------------
        // Miscellaneous operators and punctuation
        // ---------------------------------------------------------------
        Question,       // ?
        Colon,          // :
        Dot,            // .
        Comma,          // ,
        Semicolon,      // ;
        AtSign,         // @

        // ---------------------------------------------------------------
        // Delimiters
        // ---------------------------------------------------------------
        LeftParen,      // (
        RightParen,     // )
        LeftBrace,      // {
        RightBrace,     // }
        LeftBracket,    // [
        RightBracket,   // ]

        // ---------------------------------------------------------------
        // Unary / special punctuation
        // ---------------------------------------------------------------
        Bang,           // !
        Underscore,     // _   (wildcard pattern)

        // ---------------------------------------------------------------
        // Number literals
        // ---------------------------------------------------------------
        IntegerLiteral,
        FloatLiteral,
        BinaryLiteral,
        OctalLiteral,
        HexLiteral,

        // ---------------------------------------------------------------
        // String literal
        // ---------------------------------------------------------------
        StringLiteral,

        // ---------------------------------------------------------------
        // Error / end
        // ---------------------------------------------------------------
        Unknown,
        EndOfFile
    }
}
