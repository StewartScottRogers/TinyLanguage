namespace TinyLanguage.Lexer;

/// <summary>
/// Enumerates every token type produced by the TinyLanguage lexer.
/// Organised by category: keywords, type-keywords, operators, delimiters,
/// literals, identifiers, and special tokens.
/// </summary>
public enum TokenType
{
    // ── Keywords ─────────────────────────────────────────────────────────
    Let,
    Var,
    Const,
    If,
    Then,
    Else,
    End,
    While,
    For,
    To,
    Step,
    Foreach,
    In,
    Do,
    Break,
    Continue,
    Return,
    Print,
    Input,
    Function,
    Lambda,
    Class,
    New,
    This,
    Module,
    Import,
    Export,
    Try,
    Catch,
    Finally,
    Throw,
    Match,
    Case,
    Default,
    Enum,
    True,
    False,
    Null,
    And,
    Or,
    Not,
    Is,
    As,
    Static,
    Constructor,    // capital-C Constructor keyword (class constructor definitions)
    Extends,
    Implements,
    When,
    Switch,

    // ── Type keywords ─────────────────────────────────────────────────────
    // These appear in type annotations and cast expressions.
    TypeInt,
    TypeFloat,
    TypeString,
    TypeBool,
    TypeArray,
    TypeObject,
    TypeVoid,
    TypeMap,

    // ── Operators ─────────────────────────────────────────────────────────
    Assign,             // :=
    SingleEqual,        // =   (annotation params, enum value assignments only)
    Plus,               // +
    Minus,              // -
    Star,               // *
    Slash,              // /
    DoubleSlash,        // //  (floor division — NOT a comment)
    DoubleStar,         // **  (exponentiation)
    Percent,            // %
    Ampersand,          // &   (string concatenation)
    Pipe,               // |   (pattern alternation)
    Caret,              // ^
    Tilde,              // ~
    ShiftLeft,          // <<
    ShiftRight,         // >>
    EqualEqual,         // ==
    NotEqual,           // !=
    Less,               // <
    Greater,            // >
    LessEqual,          // <=
    GreaterEqual,       // >=
    DoubleAmpersand,    // &&
    DoublePipe,         // ||
    Arrow,              // ->  (return type annotation)
    FatArrow,           // =>  (pattern-match arm)
    Question,           // ?   (ternary / nullable type suffix)
    Colon,              // :
    PlusAssign,         // +=
    MinusAssign,        // -=
    StarAssign,         // *=
    SlashAssign,        // /=

    // ── Delimiters / punctuation ──────────────────────────────────────────
    LeftParen,          // (
    RightParen,         // )
    LeftBracket,        // [
    RightBracket,       // ]
    LeftBrace,          // {
    RightBrace,         // }
    Comma,              // ,
    Dot,                // .
    Semicolon,          // ;
    At,                 // @  (annotation prefix)

    // ── Literals ─────────────────────────────────────────────────────────
    IntegerLiteral,     // 42, 0b101, 0o17, 0xFF
    FloatLiteral,       // 3.14
    StringLiteral,      // "hello" or 'hello'

    // ── Identifier ───────────────────────────────────────────────────────
    Identifier,         // user-defined names

    // ── Special ──────────────────────────────────────────────────────────
    Unknown,            // unrecognised character
    EndOfFile           // signals end of token stream
}
