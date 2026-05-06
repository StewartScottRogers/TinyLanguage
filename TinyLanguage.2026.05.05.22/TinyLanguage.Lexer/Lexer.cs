using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TinyLanguage.Lexer
{
    // Streaming lexer for TinyLanguage source code.
    //
    // Key rules from the specification (Build.Solution.md):
    //   Note 1  — '#' starts a line comment; '//' is ALWAYS floor-division, never a comment.
    //   Note 2  — ':=' is the only assignment operator; bare '=' is SingleEqual (not Unknown).
    //   Note 5  — Every reserved word always emits its keyword TokenType, never Identifier.
    //   Note 16 — 'static' emits TokenType.Static as its own token.
    //
    // Number literal rules:
    //   - Decimal float REQUIRES digits before AND after the dot (1.5 yes, .5 no, 1. no).
    //   - '0b' prefix → BinaryLiteral; '0o' → OctalLiteral; '0x' → HexLiteral.
    //
    // String literals accept both '"' and '\'' delimiters with escape sequences:
    //   \n  \t  \\  \"  \'
    //
    // The lexer reads the source character-by-character from a TextReader (stream-friendly).
    // It never loads the entire source into memory as a single string.
    public class Lexer
    {
        // Keyword table: maps every reserved word to its TokenType.
        // 'Constructor' has a capital C — matches the BNF production exactly.
        private static readonly Dictionary<string, TokenType> Keywords =
            new Dictionary<string, TokenType>
            {
                // Declarations
                { "let",         TokenType.Let         },
                { "var",         TokenType.Var         },
                { "const",       TokenType.Const       },
                { "enum",        TokenType.Enum        },
                { "function",    TokenType.Function    },
                { "return",      TokenType.Return      },
                { "class",       TokenType.Class       },
                { "extends",     TokenType.Extends     },
                { "implements",  TokenType.Implements  },
                { "Constructor", TokenType.Constructor },
                { "new",         TokenType.New         },
                { "static",      TokenType.Static      },
                { "module",      TokenType.Module      },
                { "import",      TokenType.Import      },
                { "export",      TokenType.Export      },
                { "as",          TokenType.As          },
                // Control flow
                { "if",          TokenType.If          },
                { "then",        TokenType.Then        },
                { "else",        TokenType.Else        },
                { "end",         TokenType.End         },
                { "while",       TokenType.While       },
                { "do",          TokenType.Do          },
                { "for",         TokenType.For         },
                { "to",          TokenType.To          },
                { "step",        TokenType.Step        },
                { "foreach",     TokenType.Foreach     },
                { "in",          TokenType.In          },
                { "break",       TokenType.Break       },
                { "continue",    TokenType.Continue    },
                { "switch",      TokenType.Switch      },
                { "case",        TokenType.Case        },
                { "default",     TokenType.Default     },
                // I/O
                { "print",       TokenType.Print       },
                { "input",       TokenType.Input       },
                // Exceptions
                { "try",         TokenType.Try         },
                { "catch",       TokenType.Catch       },
                { "finally",     TokenType.Finally     },
                { "throw",       TokenType.Throw       },
                // Pattern matching
                { "match",       TokenType.Match       },
                { "when",        TokenType.When        },
                // Logical
                { "and",         TokenType.And         },
                { "or",          TokenType.Or          },
                { "not",         TokenType.Not         },
                { "is",          TokenType.Is          },
                // Self-reference
                { "this",        TokenType.This        },
                // Type names
                { "int",         TokenType.Int         },
                { "float",       TokenType.Float       },
                { "string",      TokenType.String      },
                { "bool",        TokenType.Bool        },
                { "array",       TokenType.Array       },
                { "object",      TokenType.Object      },
                { "map",         TokenType.Map         },
                { "void",        TokenType.Void        },
                // Literal values
                { "null",        TokenType.Null        },
                { "true",        TokenType.True        },
                { "false",       TokenType.False       },
            };

        private readonly TextReader reader;
        private int currentLine;

        // Peek buffer: holds a single lookahead character.
        // -2 means the buffer is empty (not yet filled).
        // -1 means end-of-stream.
        private int peekBuffer;
        private bool hasPeek;

        // Constructs a lexer that reads from the given TextReader.
        // Prefer passing a StreamReader over a StringReader for large inputs.
        public Lexer(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this.reader = reader;
            currentLine = 1;
            peekBuffer = 0;
            hasPeek = false;
        }

        // Constructs a lexer that reads from a Stream using UTF-8 encoding.
        public Lexer(Stream stream)
            : this(new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: false))
        {
        }

        // Tokenises the entire source and returns a list of tokens.
        // The list always ends with an EndOfFile token.
        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            Token token = NextToken();
            while (token.Type != TokenType.EndOfFile)
            {
                tokens.Add(token);
                token = NextToken();
            }
            tokens.Add(token); // include the EndOfFile token
            return tokens;
        }

        // Returns the next token from the source stream.
        public Token NextToken()
        {
            SkipWhitespaceAndComments();

            int tokenLine = currentLine;
            int ch = Peek();

            if (ch == -1)
            {
                return new Token(string.Empty, TokenType.EndOfFile, tokenLine);
            }

            // Identifier or keyword
            if (IsLetter((char)ch))
            {
                return ReadIdentifierOrKeyword(tokenLine);
            }

            // Underscore — wildcard pattern (a single '_' not followed by letter/digit)
            // Note: identifiers cannot START with underscore per coding-style rules,
            // but '_' alone is the wildcard pattern token.
            if (ch == '_')
            {
                Consume();
                // If next char is letter or digit this forms an invalid identifier;
                // emit Unknown for robustness (the parser will reject it).
                if (IsLetterOrDigitOrUnderscore(Peek()))
                {
                    // Consume the rest of the invalid identifier-like token
                    StringBuilder builder = new StringBuilder("_");
                    while (IsLetterOrDigitOrUnderscore(Peek()))
                    {
                        builder.Append((char)Consume());
                    }
                    return new Token(builder.ToString(), TokenType.Unknown, tokenLine);
                }
                return new Token("_", TokenType.Underscore, tokenLine);
            }

            // Number literal
            if (IsDigit((char)ch))
            {
                return ReadNumber(tokenLine);
            }

            // String literal (double-quoted or single-quoted)
            if (ch == '"' || ch == '\'')
            {
                return ReadString(tokenLine);
            }

            // Operators and punctuation
            return ReadOperatorOrPunctuation(tokenLine);
        }

        // ---------------------------------------------------------------
        // Whitespace and comment handling
        // ---------------------------------------------------------------

        private void SkipWhitespaceAndComments()
        {
            while (true)
            {
                int ch = Peek();
                if (ch == -1)
                {
                    break;
                }

                if (ch == ' ' || ch == '\t' || ch == '\r')
                {
                    Consume();
                    continue;
                }

                if (ch == '\n')
                {
                    Consume();
                    currentLine++;
                    continue;
                }

                // '#' begins a line comment — skip to end of line (Note 1)
                if (ch == '#')
                {
                    Consume();
                    while (Peek() != '\n' && Peek() != -1)
                    {
                        Consume();
                    }
                    continue;
                }

                break;
            }
        }

        // ---------------------------------------------------------------
        // Identifier / keyword reader
        // ---------------------------------------------------------------

        private Token ReadIdentifierOrKeyword(int tokenLine)
        {
            StringBuilder builder = new StringBuilder();
            while (IsLetterOrDigitOrUnderscore(Peek()))
            {
                builder.Append((char)Consume());
            }
            string text = builder.ToString();

            TokenType keywordType;
            if (Keywords.TryGetValue(text, out keywordType))
            {
                return new Token(text, keywordType, tokenLine);
            }
            return new Token(text, TokenType.Identifier, tokenLine);
        }

        // ---------------------------------------------------------------
        // Number literal reader
        // ---------------------------------------------------------------

        private Token ReadNumber(int tokenLine)
        {
            // Check for 0b / 0o / 0x prefixes
            if (Peek() == '0')
            {
                int saved = Consume(); // consume '0'
                int next = Peek();

                if (next == 'b' || next == 'B')
                {
                    return ReadBinaryLiteral(tokenLine);
                }
                if (next == 'o' || next == 'O')
                {
                    return ReadOctalLiteral(tokenLine);
                }
                if (next == 'x' || next == 'X')
                {
                    return ReadHexLiteral(tokenLine);
                }

                // Plain decimal starting with '0'
                return ReadDecimalFromFirstDigit('0', tokenLine);
            }

            char firstDigit = (char)Consume();
            return ReadDecimalFromFirstDigit(firstDigit, tokenLine);
        }

        // Reads the rest of a decimal number given that 'firstDigit' was already consumed.
        private Token ReadDecimalFromFirstDigit(char firstDigit, int tokenLine)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(firstDigit);

            // Consume remaining integer digits
            while (IsDigit(Peek()))
            {
                builder.Append((char)Consume());
            }

            // Check for float: digits '.' digits — BOTH sides required (Note in spec)
            if (Peek() == '.' && IsDigitAt(LookAheadTwo()))
            {
                builder.Append((char)Consume()); // consume '.'
                while (IsDigit(Peek()))
                {
                    builder.Append((char)Consume());
                }
                return new Token(builder.ToString(), TokenType.FloatLiteral, tokenLine);
            }

            return new Token(builder.ToString(), TokenType.IntegerLiteral, tokenLine);
        }

        private Token ReadBinaryLiteral(int tokenLine)
        {
            Consume(); // consume 'b'
            StringBuilder builder = new StringBuilder("0b");
            if (Peek() != '0' && Peek() != '1')
            {
                throw new LexerException("Invalid binary literal: expected binary digit after '0b'", tokenLine);
            }
            while (Peek() == '0' || Peek() == '1')
            {
                builder.Append((char)Consume());
            }
            return new Token(builder.ToString(), TokenType.BinaryLiteral, tokenLine);
        }

        private Token ReadOctalLiteral(int tokenLine)
        {
            Consume(); // consume 'o'
            StringBuilder builder = new StringBuilder("0o");
            if (!IsOctalDigit(Peek()))
            {
                throw new LexerException("Invalid octal literal: expected octal digit after '0o'", tokenLine);
            }
            while (IsOctalDigit(Peek()))
            {
                builder.Append((char)Consume());
            }
            return new Token(builder.ToString(), TokenType.OctalLiteral, tokenLine);
        }

        private Token ReadHexLiteral(int tokenLine)
        {
            Consume(); // consume 'x'
            StringBuilder builder = new StringBuilder("0x");
            if (!IsHexDigit(Peek()))
            {
                throw new LexerException("Invalid hex literal: expected hex digit after '0x'", tokenLine);
            }
            while (IsHexDigit(Peek()))
            {
                builder.Append((char)Consume());
            }
            return new Token(builder.ToString(), TokenType.HexLiteral, tokenLine);
        }

        // ---------------------------------------------------------------
        // String literal reader
        // ---------------------------------------------------------------

        private Token ReadString(int tokenLine)
        {
            char delimiter = (char)Consume(); // consume opening '"' or '\''
            StringBuilder builder = new StringBuilder();
            builder.Append(delimiter);

            while (true)
            {
                int ch = Peek();
                if (ch == -1)
                {
                    throw new LexerException("Unterminated string literal", tokenLine);
                }
                if (ch == '\n')
                {
                    throw new LexerException("Unterminated string literal (newline in string)", currentLine);
                }

                Consume();

                if (ch == delimiter)
                {
                    // Closing delimiter found
                    builder.Append(delimiter);
                    break;
                }

                if (ch == '\\')
                {
                    // Escape sequence
                    int escaped = Peek();
                    if (escaped == -1)
                    {
                        throw new LexerException("Unterminated escape sequence in string literal", tokenLine);
                    }
                    Consume();
                    switch (escaped)
                    {
                        case 'n':  builder.Append("\\n");  break;
                        case 't':  builder.Append("\\t");  break;
                        case '\\': builder.Append("\\\\"); break;
                        case '"':  builder.Append("\\\""); break;
                        case '\'': builder.Append("\\'");  break;
                        default:
                            throw new LexerException(
                                $"Unknown escape sequence '\\{(char)escaped}' in string literal",
                                tokenLine);
                    }
                    continue;
                }

                builder.Append((char)ch);
            }

            return new Token(builder.ToString(), TokenType.StringLiteral, tokenLine);
        }

        // ---------------------------------------------------------------
        // Operator and punctuation reader
        // ---------------------------------------------------------------

        private Token ReadOperatorOrPunctuation(int tokenLine)
        {
            int ch = Consume();

            switch (ch)
            {
                case '+': return new Token("+", TokenType.Plus, tokenLine);
                case '-':
                    if (Peek() == '>')
                    {
                        Consume();
                        return new Token("->", TokenType.Arrow, tokenLine);
                    }
                    return new Token("-", TokenType.Minus, tokenLine);

                case '*':
                    if (Peek() == '*')
                    {
                        Consume();
                        return new Token("**", TokenType.StarStar, tokenLine);
                    }
                    return new Token("*", TokenType.Star, tokenLine);

                case '/':
                    // NOTE 1: '//' is ALWAYS floor-division — never a comment.
                    if (Peek() == '/')
                    {
                        Consume();
                        return new Token("//", TokenType.SlashSlash, tokenLine);
                    }
                    return new Token("/", TokenType.Slash, tokenLine);

                case '%': return new Token("%", TokenType.Percent, tokenLine);
                case '&':
                    if (Peek() == '&')
                    {
                        Consume();
                        return new Token("&&", TokenType.AmpAmp, tokenLine);
                    }
                    return new Token("&", TokenType.Amp, tokenLine);

                case '|':
                    if (Peek() == '|')
                    {
                        Consume();
                        return new Token("||", TokenType.PipePipe, tokenLine);
                    }
                    // Bare '|' is pattern alternation (Note 23) — NOT Unknown
                    return new Token("|", TokenType.Pipe, tokenLine);

                case '=':
                    if (Peek() == '=')
                    {
                        Consume();
                        return new Token("==", TokenType.EqualEqual, tokenLine);
                    }
                    if (Peek() == '>')
                    {
                        Consume();
                        return new Token("=>", TokenType.FatArrow, tokenLine);
                    }
                    // NOTE 2: bare '=' is SingleEqual (valid in enum and annotation contexts)
                    return new Token("=", TokenType.SingleEqual, tokenLine);

                case '!':
                    if (Peek() == '=')
                    {
                        Consume();
                        return new Token("!=", TokenType.NotEqual, tokenLine);
                    }
                    return new Token("!", TokenType.Bang, tokenLine);

                case '<':
                    if (Peek() == '=')
                    {
                        Consume();
                        return new Token("<=", TokenType.LessEqual, tokenLine);
                    }
                    return new Token("<", TokenType.Less, tokenLine);

                case '>':
                    if (Peek() == '=')
                    {
                        Consume();
                        return new Token(">=", TokenType.GreaterEqual, tokenLine);
                    }
                    return new Token(">", TokenType.Greater, tokenLine);

                case ':':
                    if (Peek() == '=')
                    {
                        Consume();
                        return new Token(":=", TokenType.Assign, tokenLine);
                    }
                    return new Token(":", TokenType.Colon, tokenLine);

                case '?': return new Token("?", TokenType.Question, tokenLine);
                case '.': return new Token(".", TokenType.Dot, tokenLine);
                case ',': return new Token(",", TokenType.Comma, tokenLine);
                case ';': return new Token(";", TokenType.Semicolon, tokenLine);
                case '@': return new Token("@", TokenType.AtSign, tokenLine);
                case '(': return new Token("(", TokenType.LeftParen, tokenLine);
                case ')': return new Token(")", TokenType.RightParen, tokenLine);
                case '{': return new Token("{", TokenType.LeftBrace, tokenLine);
                case '}': return new Token("}", TokenType.RightBrace, tokenLine);
                case '[': return new Token("[", TokenType.LeftBracket, tokenLine);
                case ']': return new Token("]", TokenType.RightBracket, tokenLine);

                default:
                    return new Token(((char)ch).ToString(), TokenType.Unknown, tokenLine);
            }
        }

        // ---------------------------------------------------------------
        // Character stream helpers
        // ---------------------------------------------------------------

        // Returns the next character without consuming it, or -1 at end-of-stream.
        private int Peek()
        {
            if (!hasPeek)
            {
                peekBuffer = reader.Read();
                hasPeek = true;
            }
            return peekBuffer;
        }

        // Consumes and returns the next character, or -1 at end-of-stream.
        private int Consume()
        {
            if (!hasPeek)
            {
                return reader.Read();
            }
            hasPeek = false;
            return peekBuffer;
        }

        // Returns the character two positions ahead without consuming either.
        // Uses a temporary read from the reader after consuming the peek.
        // NOTE: this temporarily discards one peek position; used only for
        // the decimal-float lookahead (digit '.' digit).
        // We achieve this by peeking two chars: current peek + reader.Peek().
        private int LookAheadTwo()
        {
            // reader.Peek() is the character AFTER the current peekBuffer
            // (already consumed or not yet consumed).
            if (hasPeek)
            {
                // peekBuffer is the immediate next char; reader.Peek() is the one after.
                return reader.Peek();
            }
            // If we have no buffered char, read and buffer it first.
            peekBuffer = reader.Read();
            hasPeek = true;
            return reader.Peek();
        }

        // ---------------------------------------------------------------
        // Character classification helpers
        // ---------------------------------------------------------------

        private static bool IsLetter(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        private static bool IsDigit(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsDigitAt(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsOctalDigit(int ch)
        {
            return ch >= '0' && ch <= '7';
        }

        private static bool IsHexDigit(int ch)
        {
            return (ch >= '0' && ch <= '9')
                || (ch >= 'a' && ch <= 'f')
                || (ch >= 'A' && ch <= 'F');
        }

        private static bool IsLetterOrDigitOrUnderscore(int ch)
        {
            return (ch >= 'a' && ch <= 'z')
                || (ch >= 'A' && ch <= 'Z')
                || (ch >= '0' && ch <= '9')
                || ch == '_';
        }
    }
}
