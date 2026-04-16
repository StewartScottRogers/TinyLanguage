namespace TinyLanguage.Lexer
{
    // Every terminal symbol in the BNF grammar is represented here as a
    // distinct enum value. The lexer emits exactly one TokenType per token.
    public enum TokenType
    {
        // ----------------------------------------------------------------
        // Literals
        // ----------------------------------------------------------------

        // An integer literal: decimal, binary (0b…), octal (0o…), or hex (0x…)
        IntegerLiteral,

        // A floating-point literal: digits "." digits
        FloatLiteral,

        // A string literal delimited by double-quotes or single-quotes
        StringLiteral,

        // The boolean literal "true"
        True,

        // The boolean literal "false"
        False,

        // ----------------------------------------------------------------
        // Identifier
        // ----------------------------------------------------------------

        // Any identifier that is not a keyword: [a-zA-Z][a-zA-Z0-9_]*
        Identifier,

        // ----------------------------------------------------------------
        // Keywords — control flow
        // ----------------------------------------------------------------

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

        // ----------------------------------------------------------------
        // Keywords — declarations
        // ----------------------------------------------------------------

        Let,
        Var,
        Const,
        Enum,

        // ----------------------------------------------------------------
        // Keywords — functions and classes
        // ----------------------------------------------------------------

        Function,
        Return,
        Class,
        Extends,
        Implements,
        Constructor,
        New,
        Static,

        // ----------------------------------------------------------------
        // Keywords — modules
        // ----------------------------------------------------------------

        Module,
        Import,
        As,
        Export,

        // ----------------------------------------------------------------
        // Keywords — exceptions
        // ----------------------------------------------------------------

        Try,
        Catch,
        Finally,
        Throw,

        // ----------------------------------------------------------------
        // Keywords — pattern matching
        // ----------------------------------------------------------------

        Match,
        When,

        // ----------------------------------------------------------------
        // Keywords — I/O
        // ----------------------------------------------------------------

        Print,
        Input,

        // ----------------------------------------------------------------
        // Keywords — logical operators (word forms)
        // ----------------------------------------------------------------

        Not,
        And,
        Or,

        // ----------------------------------------------------------------
        // Keywords — type operations
        // ----------------------------------------------------------------

        Is,

        // "as" is already defined above; it serves double duty for type
        // assertions and module aliases.

        // ----------------------------------------------------------------
        // Keywords — types
        // ----------------------------------------------------------------

        Int,
        Float,      // type keyword "float" (different from FloatLiteral)
        String,     // type keyword "string"
        Bool,
        Array,
        Object,
        Null,
        Void,
        Map,

        // ----------------------------------------------------------------
        // Operators
        // ----------------------------------------------------------------

        // Assignment
        ColonEquals,        // :=

        // Compound assignment operators
        PlusEqual,          // +=
        MinusEqual,         // -=
        StarEqual,          // *=
        SlashEqual,         // /=
        PercentEqual,       // %=
        StarStarEqual,      // **=
        SlashSlashEqual,    // //=
        AmpersandEqual,     // &=
        PipeEqual,          // |=
        CaretEqual,         // ^=
        LessLessEqual,      // <<=
        GreaterGreaterEqual, // >>=

        // Increment / decrement
        PlusPlus,           // ++
        MinusMinus,         // --

        // Equality — only valid inside annotations and enum bodies per spec §1.2
        SingleEqual,        // =

        // Arithmetic
        Plus,               // +
        Minus,              // -
        Star,               // *
        Slash,              // /
        Percent,            // %
        StarStar,           // **  (exponentiation)
        SlashSlash,         // //  (floor division — never a comment per spec §1.1)
        Ampersand,          // &   (string concatenation)

        // Comparison
        EqualEqual,         // ==
        BangEqual,          // !=
        Less,               // <
        Greater,            // >
        LessEqual,          // <=
        GreaterEqual,       // >=

        // Logical (symbol forms)
        AmpAmp,             // &&
        PipePipe,           // ||

        // Arrow / fat arrow
        Arrow,              // ->  (return type annotation)
        FatArrow,           // =>  (pattern case body)

        // Ternary
        Question,           // ?
        Colon,              // :

        // Member access
        Dot,                // .

        // Separators / punctuation
        Comma,              // ,
        Semicolon,          // ;

        // Annotation prefix
        At,                 // @

        // Wildcard pattern
        Underscore,         // _

        // Pattern alternation
        Pipe,               // |

        // ----------------------------------------------------------------
        // Delimiters
        // ----------------------------------------------------------------

        LeftParen,          // (
        RightParen,         // )
        LeftBrace,          // {
        RightBrace,         // }
        LeftBracket,        // [
        RightBracket,       // ]

        // ----------------------------------------------------------------
        // Special
        // ----------------------------------------------------------------

        // Produced for any character not recognised by the lexer
        Unknown,

        // The final token produced after all source has been consumed
        EndOfFile
    }
}
