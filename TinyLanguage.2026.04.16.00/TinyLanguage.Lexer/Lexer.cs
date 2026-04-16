using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TinyLanguage.Lexer
{
    // Converts TinyLanguage source text into a flat list of Token values.
    // The caller may supply source as a TextReader (preferred for large files)
    // or a plain string (convenient for tests).
    //
    // Key rules from the spec (Build.Solution.md):
    //   Note 1 : '#' is the only line-comment character.
    //            '//' is ALWAYS floor-division — never a comment.
    //   Note 2 : ':=' is the only assignment operator.
    //            Bare '=' is SingleEqual (valid only in annotation/enum contexts).
    //   Note 5 : Every reserved word always produces its keyword TokenType;
    //            reserved words are never returned as Identifier.
    //
    // Line numbers are 1-based and stored in every token.
    // Unterminated string literals throw LexerException.
    // Unrecognised characters produce TokenType.Unknown (no throw).
    public class Lexer
    {
        // ------------------------------------------------------------------
        // Keyword table
        // Populated once as a static field — shared across all Lexer instances.
        // Look-up happens after a letter-started word is read in full, so the
        // longest-match rule is automatically satisfied.
        // ------------------------------------------------------------------
        private static readonly Dictionary<string, TokenType> Keywords =
            new Dictionary<string, TokenType>
            {
                { "if",          TokenType.If },
                { "then",        TokenType.Then },
                { "else",        TokenType.Else },
                { "end",         TokenType.End },
                { "while",       TokenType.While },
                { "do",          TokenType.Do },
                { "for",         TokenType.For },
                { "to",          TokenType.To },
                { "step",        TokenType.Step },
                { "foreach",     TokenType.Foreach },
                { "in",          TokenType.In },
                { "break",       TokenType.Break },
                { "continue",    TokenType.Continue },
                { "switch",      TokenType.Switch },
                { "case",        TokenType.Case },
                { "default",     TokenType.Default },
                { "let",         TokenType.Let },
                { "var",         TokenType.Var },
                { "const",       TokenType.Const },
                { "enum",        TokenType.Enum },
                { "function",    TokenType.Function },
                { "return",      TokenType.Return },
                { "class",       TokenType.Class },
                { "extends",     TokenType.Extends },
                { "implements",  TokenType.Implements },
                { "Constructor", TokenType.Constructor },
                { "new",         TokenType.New },
                { "static",      TokenType.Static },
                { "module",      TokenType.Module },
                { "import",      TokenType.Import },
                { "as",          TokenType.As },
                { "export",      TokenType.Export },
                { "try",         TokenType.Try },
                { "catch",       TokenType.Catch },
                { "finally",     TokenType.Finally },
                { "throw",       TokenType.Throw },
                { "match",       TokenType.Match },
                { "when",        TokenType.When },
                { "print",       TokenType.Print },
                { "input",       TokenType.Input },
                { "not",         TokenType.Not },
                { "and",         TokenType.And },
                { "or",          TokenType.Or },
                { "is",          TokenType.Is },
                { "true",        TokenType.True },
                { "false",       TokenType.False },
                { "null",        TokenType.Null },
                { "int",         TokenType.Int },
                { "float",       TokenType.Float },
                { "string",      TokenType.String },
                { "bool",        TokenType.Bool },
                { "array",       TokenType.Array },
                { "object",      TokenType.Object },
                { "void",        TokenType.Void },
                { "map",         TokenType.Map },
            };

        // ------------------------------------------------------------------
        // Internal state
        // ------------------------------------------------------------------

        // Full source text.  We read the TextReader to completion once so that
        // we can index into Source at arbitrary offsets (needed for look-ahead).
        private readonly string Source;
        private int Position;       // index of the next character to consume
        private int CurrentLine;    // 1-based line number at Position

        // ------------------------------------------------------------------
        // Public entry points
        // ------------------------------------------------------------------

        // Preferred for file input: reads via a TextReader (stream-based).
        public static List<Token> Tokenise(TextReader reader)
        {
            string sourceText = reader.ReadToEnd();
            Lexer lexer = new Lexer(sourceText);
            return lexer.TokeniseAll();
        }

        // Convenient for tests and REPL use.
        public static List<Token> Tokenise(string sourceText)
        {
            Lexer lexer = new Lexer(sourceText);
            return lexer.TokeniseAll();
        }

        private Lexer(string sourceText)
        {
            Source = sourceText;
            Position = 0;
            CurrentLine = 1;
        }

        // ------------------------------------------------------------------
        // Core loop
        // ------------------------------------------------------------------

        private List<Token> TokeniseAll()
        {
            List<Token> tokens = new List<Token>();

            while (true)
            {
                SkipWhitespaceAndComments();

                if (Position >= Source.Length)
                {
                    tokens.Add(new Token(TokenType.EndOfFile, string.Empty, CurrentLine));
                    break;
                }

                Token next = ReadNextToken();
                tokens.Add(next);
            }

            return tokens;
        }

        // ------------------------------------------------------------------
        // Whitespace and comment skipping
        // ------------------------------------------------------------------

        // Advances Position past whitespace and '#' line comments.
        // Newlines increment CurrentLine.  CRLF is handled by skipping CR and
        // letting the following LF do the line-number increment.
        private void SkipWhitespaceAndComments()
        {
            while (Position < Source.Length)
            {
                char current = Source[Position];

                if (current == '\n')
                {
                    CurrentLine++;
                    Position++;
                }
                else if (current == '\r')
                {
                    Position++;
                }
                else if (current == ' ' || current == '\t')
                {
                    Position++;
                }
                else if (current == '#')
                {
                    // Line comment: skip from '#' up to (but not including) the newline.
                    // The newline is left for the next iteration so CurrentLine increments.
                    Position++;
                    while (Position < Source.Length && Source[Position] != '\n')
                    {
                        Position++;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        // ------------------------------------------------------------------
        // Single-token dispatch
        // ------------------------------------------------------------------

        private Token ReadNextToken()
        {
            int tokenLine = CurrentLine;
            char current = Source[Position];

            // String literals begin with a quote character.
            if (current == '"' || current == '\'')
            {
                return ReadStringLiteral(tokenLine);
            }

            // Numeric literals begin with a decimal digit.
            if (char.IsDigit(current))
            {
                return ReadNumericLiteral(tokenLine);
            }

            // Identifiers and keywords begin with a letter.
            if (char.IsLetter(current))
            {
                return ReadIdentifierOrKeyword(tokenLine);
            }

            // All remaining characters are single- or two-character operators.
            // Consume the leading character now; multi-char forms peek ahead.
            Position++;

            switch (current)
            {
                case '+':
                    return new Token(TokenType.Plus, "+", tokenLine);

                case '-':
                    // '->' return-type arrow
                    if (Position < Source.Length && Source[Position] == '>')
                    {
                        Position++;
                        return new Token(TokenType.Arrow, "->", tokenLine);
                    }
                    return new Token(TokenType.Minus, "-", tokenLine);

                case '*':
                    // '**' exponentiation
                    if (Position < Source.Length && Source[Position] == '*')
                    {
                        Position++;
                        return new Token(TokenType.StarStar, "**", tokenLine);
                    }
                    return new Token(TokenType.Star, "*", tokenLine);

                case '/':
                    // '//' floor-division — NEVER a comment (spec note 1)
                    if (Position < Source.Length && Source[Position] == '/')
                    {
                        Position++;
                        return new Token(TokenType.SlashSlash, "//", tokenLine);
                    }
                    return new Token(TokenType.Slash, "/", tokenLine);

                case '%':
                    return new Token(TokenType.Percent, "%", tokenLine);

                case '&':
                    // '&&' logical-and (symbol form)
                    if (Position < Source.Length && Source[Position] == '&')
                    {
                        Position++;
                        return new Token(TokenType.AmpAmp, "&&", tokenLine);
                    }
                    return new Token(TokenType.Ampersand, "&", tokenLine);

                case '|':
                    // '||' logical-or (symbol form)
                    if (Position < Source.Length && Source[Position] == '|')
                    {
                        Position++;
                        return new Token(TokenType.PipePipe, "||", tokenLine);
                    }
                    // Bare '|' — used as the pattern-alternation operator
                    return new Token(TokenType.Pipe, "|", tokenLine);

                case '=':
                    // '==' equality; '=>' fat-arrow; bare '=' single-equal
                    if (Position < Source.Length && Source[Position] == '=')
                    {
                        Position++;
                        return new Token(TokenType.EqualEqual, "==", tokenLine);
                    }
                    if (Position < Source.Length && Source[Position] == '>')
                    {
                        Position++;
                        return new Token(TokenType.FatArrow, "=>", tokenLine);
                    }
                    // Bare '=' is SingleEqual; valid inside annotations/enum bodies (note 2)
                    return new Token(TokenType.SingleEqual, "=", tokenLine);

                case '!':
                    // '!=' not-equal
                    if (Position < Source.Length && Source[Position] == '=')
                    {
                        Position++;
                        return new Token(TokenType.BangEqual, "!=", tokenLine);
                    }
                    return new Token(TokenType.Unknown, "!", tokenLine);

                case '<':
                    // '<=' less-or-equal
                    if (Position < Source.Length && Source[Position] == '=')
                    {
                        Position++;
                        return new Token(TokenType.LessEqual, "<=", tokenLine);
                    }
                    return new Token(TokenType.Less, "<", tokenLine);

                case '>':
                    // '>=' greater-or-equal
                    if (Position < Source.Length && Source[Position] == '=')
                    {
                        Position++;
                        return new Token(TokenType.GreaterEqual, ">=", tokenLine);
                    }
                    return new Token(TokenType.Greater, ">", tokenLine);

                case ':':
                    // ':=' assignment operator
                    if (Position < Source.Length && Source[Position] == '=')
                    {
                        Position++;
                        return new Token(TokenType.ColonEquals, ":=", tokenLine);
                    }
                    return new Token(TokenType.Colon, ":", tokenLine);

                case '?':
                    return new Token(TokenType.Question, "?", tokenLine);

                case '.':
                    return new Token(TokenType.Dot, ".", tokenLine);

                case ',':
                    return new Token(TokenType.Comma, ",", tokenLine);

                case ';':
                    return new Token(TokenType.Semicolon, ";", tokenLine);

                case '@':
                    return new Token(TokenType.At, "@", tokenLine);

                case '(':
                    return new Token(TokenType.LeftParen, "(", tokenLine);

                case ')':
                    return new Token(TokenType.RightParen, ")", tokenLine);

                case '{':
                    return new Token(TokenType.LeftBrace, "{", tokenLine);

                case '}':
                    return new Token(TokenType.RightBrace, "}", tokenLine);

                case '[':
                    return new Token(TokenType.LeftBracket, "[", tokenLine);

                case ']':
                    return new Token(TokenType.RightBracket, "]", tokenLine);

                case '_':
                    // Bare '_' is the wildcard pattern token.
                    // '_' followed by word characters is an illegal identifier
                    // (spec: identifiers must not begin with underscore).
                    if (Position < Source.Length &&
                        (char.IsLetterOrDigit(Source[Position]) || Source[Position] == '_'))
                    {
                        // Consume the full illegal token so the parser sees one Unknown.
                        StringBuilder illegal = new StringBuilder("_");
                        while (Position < Source.Length &&
                               (char.IsLetterOrDigit(Source[Position]) || Source[Position] == '_'))
                        {
                            illegal.Append(Source[Position]);
                            Position++;
                        }
                        return new Token(TokenType.Unknown, illegal.ToString(), tokenLine);
                    }
                    return new Token(TokenType.Underscore, "_", tokenLine);

                default:
                    return new Token(TokenType.Unknown, current.ToString(), tokenLine);
            }
        }

        // ------------------------------------------------------------------
        // String literal lexing
        // ------------------------------------------------------------------

        // Reads a complete string literal.  The returned Token.Value contains
        // the decoded content (surrounding quotes stripped, escapes resolved).
        //
        // Supported escape sequences (spec §Literals): \n \t \\ \" \'
        private Token ReadStringLiteral(int tokenLine)
        {
            char delimiter = Source[Position];
            Position++;     // consume opening quote

            StringBuilder content = new StringBuilder();

            while (true)
            {
                if (Position >= Source.Length)
                {
                    throw new LexerException(
                        string.Format("Unterminated string literal opened with '{0}'", delimiter),
                        tokenLine);
                }

                char ch = Source[Position];

                if (ch == delimiter)
                {
                    Position++;     // consume closing quote
                    break;
                }

                if (ch == '\\')
                {
                    Position++;     // consume backslash
                    if (Position >= Source.Length)
                    {
                        throw new LexerException(
                            "Unterminated escape sequence at end of file", tokenLine);
                    }
                    char escaped = Source[Position];
                    Position++;
                    switch (escaped)
                    {
                        case 'n':   content.Append('\n'); break;
                        case 't':   content.Append('\t'); break;
                        case '\\':  content.Append('\\'); break;
                        case '"':   content.Append('"');  break;
                        case '\'':  content.Append('\''); break;
                        default:
                            // Unknown escape — keep both characters for parser to report.
                            content.Append('\\');
                            content.Append(escaped);
                            break;
                    }
                    continue;
                }

                if (ch == '\n')
                {
                    throw new LexerException(
                        "Newline inside string literal is not allowed; use \\n instead",
                        CurrentLine);
                }

                content.Append(ch);
                Position++;
            }

            return new Token(TokenType.StringLiteral, content.ToString(), tokenLine);
        }

        // ------------------------------------------------------------------
        // Numeric literal lexing
        // ------------------------------------------------------------------

        // Reads a decimal integer, decimal float, binary (0b), octal (0o),
        // or hex (0x) literal.
        //
        // Float rule: requires at least one digit before AND after '.'.
        // "1." lexes as integer 1 then dot; ".5" lexes as dot then integer 5.
        private Token ReadNumericLiteral(int tokenLine)
        {
            // Detect non-decimal prefixes on a leading '0'.
            if (Source[Position] == '0' && Position + 1 < Source.Length)
            {
                char prefix = Source[Position + 1];
                if (prefix == 'b' || prefix == 'B')
                {
                    return ReadBinaryLiteral(tokenLine);
                }
                if (prefix == 'o' || prefix == 'O')
                {
                    return ReadOctalLiteral(tokenLine);
                }
                if (prefix == 'x' || prefix == 'X')
                {
                    return ReadHexLiteral(tokenLine);
                }
            }

            // Decimal integer or float.
            StringBuilder digits = new StringBuilder();
            while (Position < Source.Length && char.IsDigit(Source[Position]))
            {
                digits.Append(Source[Position]);
                Position++;
            }

            // Float check: '.' followed immediately by at least one digit.
            if (Position < Source.Length && Source[Position] == '.' &&
                Position + 1 < Source.Length && char.IsDigit(Source[Position + 1]))
            {
                digits.Append('.');
                Position++;     // consume '.'
                while (Position < Source.Length && char.IsDigit(Source[Position]))
                {
                    digits.Append(Source[Position]);
                    Position++;
                }
                return new Token(TokenType.FloatLiteral, digits.ToString(), tokenLine);
            }

            return new Token(TokenType.IntegerLiteral, digits.ToString(), tokenLine);
        }

        private Token ReadBinaryLiteral(int tokenLine)
        {
            Position += 2;      // consume '0b'
            StringBuilder digits = new StringBuilder("0b");
            while (Position < Source.Length &&
                   (Source[Position] == '0' || Source[Position] == '1'))
            {
                digits.Append(Source[Position]);
                Position++;
            }
            return new Token(TokenType.IntegerLiteral, digits.ToString(), tokenLine);
        }

        private Token ReadOctalLiteral(int tokenLine)
        {
            Position += 2;      // consume '0o'
            StringBuilder digits = new StringBuilder("0o");
            while (Position < Source.Length &&
                   Source[Position] >= '0' && Source[Position] <= '7')
            {
                digits.Append(Source[Position]);
                Position++;
            }
            return new Token(TokenType.IntegerLiteral, digits.ToString(), tokenLine);
        }

        private Token ReadHexLiteral(int tokenLine)
        {
            Position += 2;      // consume '0x'
            StringBuilder digits = new StringBuilder("0x");
            while (Position < Source.Length && IsHexDigit(Source[Position]))
            {
                digits.Append(Source[Position]);
                Position++;
            }
            return new Token(TokenType.IntegerLiteral, digits.ToString(), tokenLine);
        }

        // Returns true when ch is a valid hexadecimal digit character.
        private static bool IsHexDigit(char ch)
        {
            return (ch >= '0' && ch <= '9') ||
                   (ch >= 'a' && ch <= 'f') ||
                   (ch >= 'A' && ch <= 'F');
        }

        // ------------------------------------------------------------------
        // Identifier and keyword lexing
        // ------------------------------------------------------------------

        // Reads a letter-started word.  If the word appears in the keyword
        // table, the matching keyword TokenType is returned; otherwise the
        // token is an Identifier.  Per spec note 5, reserved words ALWAYS
        // produce keyword token types and are never emitted as Identifier.
        private Token ReadIdentifierOrKeyword(int tokenLine)
        {
            StringBuilder text = new StringBuilder();
            while (Position < Source.Length &&
                   (char.IsLetterOrDigit(Source[Position]) || Source[Position] == '_'))
            {
                text.Append(Source[Position]);
                Position++;
            }

            string word = text.ToString();

            TokenType keywordType;
            if (Keywords.TryGetValue(word, out keywordType))
            {
                return new Token(keywordType, word, tokenLine);
            }

            return new Token(TokenType.Identifier, word, tokenLine);
        }
    }
}
