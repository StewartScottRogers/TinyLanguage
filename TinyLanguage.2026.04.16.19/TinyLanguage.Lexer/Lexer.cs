using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLanguage.Lexer;

/// <summary>
/// Converts a TinyLanguage source string into a flat sequence of <see cref="Token"/> objects.
/// <para>
/// Key rules from the language specification:
/// <list type="bullet">
///   <item><c>#</c> starts a line comment — text through end-of-line is discarded.</item>
///   <item><c>//</c> is the floor-division operator — it is never a comment.</item>
///   <item><c>:=</c> is the only assignment operator; bare <c>=</c> is <see cref="TokenType.SingleEqual"/>.</item>
///   <item>Every reserved word always produces its keyword token — never <see cref="TokenType.Identifier"/>.</item>
///   <item>Identifiers may not start with an underscore (coding-style rule).</item>
/// </list>
/// </para>
/// </summary>
public sealed class Lexer
{
    // The complete source text being lexed.
    private readonly string Source;

    // Current read position within Source.
    private int Position;

    // Current 1-based line number (incremented on every '\n').
    private int Line;

    /// <summary>Initialises the lexer with the given source text.</summary>
    public Lexer(string source)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Position = 0;
        Line = 1;
    }

    // ── Public entry point ────────────────────────────────────────────────

    /// <summary>
    /// Tokenises the entire source and returns every token including the
    /// terminal <see cref="TokenType.EndOfFile"/> token.
    /// </summary>
    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();

        while (true)
        {
            Token token = NextToken();
            tokens.Add(token);
            if (token.Type == TokenType.EndOfFile)
            {
                break;
            }
        }

        return tokens;
    }

    // ── Core scan loop ────────────────────────────────────────────────────

    /// <summary>Scans and returns the next meaningful token.</summary>
    private Token NextToken()
    {
        // Skip whitespace (spaces, tabs, carriage returns, and newlines).
        // Newlines increment the line counter.
        SkipWhitespace();

        if (Position >= Source.Length)
        {
            return MakeToken(TokenType.EndOfFile, string.Empty);
        }

        char current = Source[Position];

        // ── Line comment: # runs to end of line ───────────────────────────
        if (current == '#')
        {
            SkipLineComment();
            return NextToken();
        }

        // ── Multi-line block comment: /* ... */ ───────────────────────────
        if (current == '/' && Peek(1) == '*')
        {
            SkipBlockComment();
            return NextToken();
        }

        // ── String literals ───────────────────────────────────────────────
        if (current == '"' || current == '\'')
        {
            return ReadStringLiteral(current);
        }

        // ── Number literals ───────────────────────────────────────────────
        if (char.IsDigit(current))
        {
            return ReadNumber();
        }

        // ── Identifiers and keywords ──────────────────────────────────────
        // Identifiers start with a letter (spec forbids leading underscore).
        if (char.IsLetter(current))
        {
            return ReadIdentifierOrKeyword();
        }

        // ── Operators and punctuation ─────────────────────────────────────
        return ReadOperatorOrPunctuation();
    }

    // ── Whitespace and comment helpers ────────────────────────────────────

    private void SkipWhitespace()
    {
        while (Position < Source.Length)
        {
            char ch = Source[Position];
            if (ch == '\n')
            {
                Line++;
                Position++;
            }
            else if (ch == ' ' || ch == '\t' || ch == '\r')
            {
                Position++;
            }
            else
            {
                break;
            }
        }
    }

    private void SkipLineComment()
    {
        // Consume everything up to (but not including) the newline.
        while (Position < Source.Length && Source[Position] != '\n')
        {
            Position++;
        }
        // The newline itself will be consumed by the next SkipWhitespace call.
    }

    private void SkipBlockComment()
    {
        int startLine = Line;
        Position += 2; // skip /*

        while (Position < Source.Length)
        {
            if (Source[Position] == '\n')
            {
                Line++;
                Position++;
            }
            else if (Source[Position] == '*' && Peek(1) == '/')
            {
                Position += 2; // skip */
                return;
            }
            else
            {
                Position++;
            }
        }

        throw new LexerException("Unterminated block comment", startLine);
    }

    // ── Number reading ────────────────────────────────────────────────────

    private Token ReadNumber()
    {
        int startLine = Line;
        int start = Position;

        // Check for 0b / 0o / 0x prefixes.
        if (Source[Position] == '0' && Position + 1 < Source.Length)
        {
            char prefix = Source[Position + 1];
            if (prefix == 'b' || prefix == 'B')
            {
                return ReadPrefixedInteger(start, startLine, "01", "binary");
            }
            if (prefix == 'o' || prefix == 'O')
            {
                return ReadPrefixedInteger(start, startLine, "01234567", "octal");
            }
            if (prefix == 'x' || prefix == 'X')
            {
                return ReadHexInteger(start, startLine);
            }
        }

        // Consume all leading digits.
        while (Position < Source.Length && char.IsDigit(Source[Position]))
        {
            Position++;
        }

        // Check for a decimal point followed by at least one digit → float.
        // The spec requires at least one digit on each side of '.'.
        if (Position < Source.Length && Source[Position] == '.'
            && Position + 1 < Source.Length && char.IsDigit(Source[Position + 1]))
        {
            Position++; // consume '.'
            while (Position < Source.Length && char.IsDigit(Source[Position]))
            {
                Position++;
            }
            return new Token(TokenType.FloatLiteral, Source.Substring(start, Position - start), startLine);
        }

        return new Token(TokenType.IntegerLiteral, Source.Substring(start, Position - start), startLine);
    }

    private Token ReadPrefixedInteger(int start, int startLine, string allowedDigits, string description)
    {
        Position += 2; // skip '0b' / '0o'

        if (Position >= Source.Length || allowedDigits.IndexOf(Source[Position]) < 0)
        {
            throw new LexerException($"Invalid {description} literal: expected a {description} digit", startLine);
        }

        while (Position < Source.Length && allowedDigits.IndexOf(Source[Position]) >= 0)
        {
            Position++;
        }

        return new Token(TokenType.IntegerLiteral, Source.Substring(start, Position - start), startLine);
    }

    private Token ReadHexInteger(int start, int startLine)
    {
        Position += 2; // skip '0x'

        if (Position >= Source.Length || !IsHexDigit(Source[Position]))
        {
            throw new LexerException("Invalid hex literal: expected a hex digit after 0x", startLine);
        }

        while (Position < Source.Length && IsHexDigit(Source[Position]))
        {
            Position++;
        }

        return new Token(TokenType.IntegerLiteral, Source.Substring(start, Position - start), startLine);
    }

    private static bool IsHexDigit(char ch)
    {
        return (ch >= '0' && ch <= '9')
            || (ch >= 'a' && ch <= 'f')
            || (ch >= 'A' && ch <= 'F');
    }

    // ── String reading ────────────────────────────────────────────────────

    private Token ReadStringLiteral(char delimiter)
    {
        int startLine = Line;
        Position++; // consume opening quote

        StringBuilder value = new StringBuilder();

        while (Position < Source.Length && Source[Position] != delimiter)
        {
            char ch = Source[Position];

            if (ch == '\n')
            {
                throw new LexerException("Unterminated string literal (newline inside string)", startLine);
            }

            if (ch == '\\')
            {
                Position++; // consume backslash
                if (Position >= Source.Length)
                {
                    throw new LexerException("Unterminated escape sequence at end of input", startLine);
                }

                char escaped = Source[Position];
                switch (escaped)
                {
                    case 'n':  value.Append('\n'); break;
                    case 't':  value.Append('\t'); break;
                    case '\\': value.Append('\\'); break;
                    case '"':  value.Append('"');  break;
                    case '\'': value.Append('\''); break;
                    default:
                        throw new LexerException(
                            $"Unknown escape sequence '\\{escaped}'", startLine);
                }

                Position++;
            }
            else
            {
                value.Append(ch);
                Position++;
            }
        }

        if (Position >= Source.Length)
        {
            throw new LexerException("Unterminated string literal", startLine);
        }

        Position++; // consume closing quote
        return new Token(TokenType.StringLiteral, value.ToString(), startLine);
    }

    // ── Identifier / keyword reading ──────────────────────────────────────

    private Token ReadIdentifierOrKeyword()
    {
        int startLine = Line;
        int start = Position;

        // Consume letter-digit-underscore sequences.
        while (Position < Source.Length
               && (char.IsLetterOrDigit(Source[Position]) || Source[Position] == '_'))
        {
            Position++;
        }

        string word = Source.Substring(start, Position - start);

        // Map the word to a keyword token type, or Identifier if not reserved.
        TokenType tokenType = MapKeyword(word);
        return new Token(tokenType, word, startLine);
    }

    /// <summary>
    /// Returns the <see cref="TokenType"/> for a reserved word, or
    /// <see cref="TokenType.Identifier"/> if the word is not reserved.
    /// Per implementation note 5, keywords always win — the lexer never
    /// returns Identifier for a reserved word.
    /// </summary>
    private static TokenType MapKeyword(string word)
    {
        switch (word)
        {
            // ── Control flow keywords ─────────────────────────────────────
            case "if":       return TokenType.If;
            case "then":     return TokenType.Then;
            case "else":     return TokenType.Else;
            case "end":      return TokenType.End;
            case "while":    return TokenType.While;
            case "do":       return TokenType.Do;
            case "for":      return TokenType.For;
            case "to":       return TokenType.To;
            case "step":     return TokenType.Step;
            case "foreach":  return TokenType.Foreach;
            case "in":       return TokenType.In;
            case "break":    return TokenType.Break;
            case "continue": return TokenType.Continue;
            case "switch":   return TokenType.Switch;
            case "case":     return TokenType.Case;
            case "default":  return TokenType.Default;
            case "match":    return TokenType.Match;
            case "when":     return TokenType.When;

            // ── Declaration keywords ──────────────────────────────────────
            case "let":      return TokenType.Let;
            case "var":      return TokenType.Var;
            case "const":    return TokenType.Const;
            case "enum":     return TokenType.Enum;

            // ── Function / class / module ─────────────────────────────────
            case "function":     return TokenType.Function;
            case "lambda":       return TokenType.Lambda;
            case "return":       return TokenType.Return;
            case "class":        return TokenType.Class;
            case "extends":      return TokenType.Extends;
            case "implements":   return TokenType.Implements;
            case "Constructor":  return TokenType.Constructor; // capital C — per spec
            case "new":          return TokenType.New;
            case "this":         return TokenType.This;
            case "static":       return TokenType.Static;
            case "module":       return TokenType.Module;
            case "import":       return TokenType.Import;
            case "export":       return TokenType.Export;

            // ── I/O keywords ──────────────────────────────────────────────
            case "print":  return TokenType.Print;
            case "input":  return TokenType.Input;

            // ── Exception keywords ────────────────────────────────────────
            case "try":     return TokenType.Try;
            case "catch":   return TokenType.Catch;
            case "finally": return TokenType.Finally;
            case "throw":   return TokenType.Throw;

            // ── Logical operators (word forms) ────────────────────────────
            case "and": return TokenType.And;
            case "or":  return TokenType.Or;
            case "not": return TokenType.Not;

            // ── Type check / assert ───────────────────────────────────────
            case "is": return TokenType.Is;
            case "as": return TokenType.As;

            // ── Boolean and null literals ─────────────────────────────────
            case "true":  return TokenType.True;
            case "false": return TokenType.False;
            case "null":  return TokenType.Null;

            // ── Type keywords ─────────────────────────────────────────────
            case "int":    return TokenType.TypeInt;
            case "float":  return TokenType.TypeFloat;
            case "string": return TokenType.TypeString;
            case "bool":   return TokenType.TypeBool;
            case "array":  return TokenType.TypeArray;
            case "object": return TokenType.TypeObject;
            case "void":   return TokenType.TypeVoid;
            case "map":    return TokenType.TypeMap;

            // ── Not a keyword: user-defined identifier ────────────────────
            default: return TokenType.Identifier;
        }
    }

    // ── Operator / punctuation reading ────────────────────────────────────

    private Token ReadOperatorOrPunctuation()
    {
        int startLine = Line;
        char ch = Source[Position];

        switch (ch)
        {
            // ── Simple single-character tokens ────────────────────────────
            case '(': Position++; return MakeToken(TokenType.LeftParen,    "(");
            case ')': Position++; return MakeToken(TokenType.RightParen,   ")");
            case '[': Position++; return MakeToken(TokenType.LeftBracket,  "[");
            case ']': Position++; return MakeToken(TokenType.RightBracket, "]");
            case '{': Position++; return MakeToken(TokenType.LeftBrace,    "{");
            case '}': Position++; return MakeToken(TokenType.RightBrace,   "}");
            case ',': Position++; return MakeToken(TokenType.Comma,        ",");
            case '.': Position++; return MakeToken(TokenType.Dot,          ".");
            case ';': Position++; return MakeToken(TokenType.Semicolon,    ";");
            case '@': Position++; return MakeToken(TokenType.At,           "@");
            case '%': Position++; return MakeToken(TokenType.Percent,      "%");
            case '^': Position++; return MakeToken(TokenType.Caret,        "^");
            case '~': Position++; return MakeToken(TokenType.Tilde,        "~");
            case '?': Position++; return MakeToken(TokenType.Question,     "?");

            // ── + or += ───────────────────────────────────────────────────
            case '+':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.PlusAssign, "+=");
                }
                return MakeToken(TokenType.Plus, "+");

            // ── - or -= or -> ─────────────────────────────────────────────
            case '-':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.MinusAssign, "-=");
                }
                if (Position < Source.Length && Source[Position] == '>')
                {
                    Position++;
                    return MakeToken(TokenType.Arrow, "->");
                }
                return MakeToken(TokenType.Minus, "-");

            // ── * or *= or ** ─────────────────────────────────────────────
            case '*':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.StarAssign, "*=");
                }
                if (Position < Source.Length && Source[Position] == '*')
                {
                    Position++;
                    return MakeToken(TokenType.DoubleStar, "**");
                }
                return MakeToken(TokenType.Star, "*");

            // ── / or /= or // ─────────────────────────────────────────────
            // IMPORTANT: // is floor-division, not a comment (spec note 1).
            case '/':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.SlashAssign, "/=");
                }
                if (Position < Source.Length && Source[Position] == '/')
                {
                    Position++;
                    return MakeToken(TokenType.DoubleSlash, "//");
                }
                return MakeToken(TokenType.Slash, "/");

            // ── = or == ──────────────────────────────────────────────────
            case '=':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.EqualEqual, "==");
                }
                if (Position < Source.Length && Source[Position] == '>')
                {
                    Position++;
                    return MakeToken(TokenType.FatArrow, "=>");
                }
                // Bare '=' is SingleEqual — valid only inside annotations and enums.
                return MakeToken(TokenType.SingleEqual, "=");

            // ── ! or != ──────────────────────────────────────────────────
            case '!':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.NotEqual, "!=");
                }
                // Bare '!' is not in the grammar — produce Unknown.
                return MakeToken(TokenType.Unknown, "!");

            // ── < or <= or << ─────────────────────────────────────────────
            case '<':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.LessEqual, "<=");
                }
                if (Position < Source.Length && Source[Position] == '<')
                {
                    Position++;
                    return MakeToken(TokenType.ShiftLeft, "<<");
                }
                return MakeToken(TokenType.Less, "<");

            // ── > or >= or >> ─────────────────────────────────────────────
            case '>':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.GreaterEqual, ">=");
                }
                if (Position < Source.Length && Source[Position] == '>')
                {
                    Position++;
                    return MakeToken(TokenType.ShiftRight, ">>");
                }
                return MakeToken(TokenType.Greater, ">");

            // ── & or && ──────────────────────────────────────────────────
            case '&':
                Position++;
                if (Position < Source.Length && Source[Position] == '&')
                {
                    Position++;
                    return MakeToken(TokenType.DoubleAmpersand, "&&");
                }
                return MakeToken(TokenType.Ampersand, "&");

            // ── | or || ──────────────────────────────────────────────────
            case '|':
                Position++;
                if (Position < Source.Length && Source[Position] == '|')
                {
                    Position++;
                    return MakeToken(TokenType.DoublePipe, "||");
                }
                return MakeToken(TokenType.Pipe, "|");

            // ── : or := ──────────────────────────────────────────────────
            case ':':
                Position++;
                if (Position < Source.Length && Source[Position] == '=')
                {
                    Position++;
                    return MakeToken(TokenType.Assign, ":=");
                }
                return MakeToken(TokenType.Colon, ":");

            // ── Unknown character ─────────────────────────────────────────
            default:
                Position++;
                return new Token(TokenType.Unknown, ch.ToString(), startLine);
        }
    }

    // ── Utility helpers ───────────────────────────────────────────────────

    /// <summary>
    /// Peeks at the character <paramref name="offset"/> positions ahead of the
    /// current position without advancing.  Returns '\0' if out of range.
    /// </summary>
    private char Peek(int offset)
    {
        int index = Position + offset;
        return index < Source.Length ? Source[index] : '\0';
    }

    /// <summary>Creates a token at the current line with the given type and value.</summary>
    private Token MakeToken(TokenType tokenType, string value)
    {
        return new Token(tokenType, value, Line);
    }
}
