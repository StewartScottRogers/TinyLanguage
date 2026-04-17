using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLanguage.Lexer;

namespace TinyLanguage.UnitTests;

/// <summary>
/// Unit tests for the TinyLanguage Lexer.
/// Each test feeds a minimal source string and verifies the produced token types and values.
/// </summary>
[TestClass]
public sealed class LexerUnitTests
{
    // ── Helper ────────────────────────────────────────────────────────────

    /// <summary>Tokenises the source and returns the full token list including EndOfFile.</summary>
    private static List<Token> Lex(string source)
    {
        return new TinyLanguage.Lexer.Lexer(source).Tokenize();
    }

    /// <summary>
    /// Asserts that lexing <paramref name="source"/> produces exactly
    /// one meaningful token followed by EndOfFile.
    /// Returns the meaningful token for further assertions.
    /// </summary>
    private static Token LexSingle(string source, TokenType expectedType)
    {
        List<Token> tokens = Lex(source);
        Assert.AreEqual(2, tokens.Count,
            $"Expected exactly 2 tokens (one real + EOF) for source: {source}");
        Assert.AreEqual(expectedType, tokens[0].Type,
            $"Wrong token type for source: {source}");
        Assert.AreEqual(TokenType.EndOfFile, tokens[1].Type);
        return tokens[0];
    }

    // ================================================================
    // Keywords
    // ================================================================

    [TestMethod]
    public void Lexer_KeywordLet_ProducesLetToken()
    {
        Token token = LexSingle("let", TokenType.Let);
        Assert.AreEqual("let", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordVar_ProducesVarToken()
    {
        Token token = LexSingle("var", TokenType.Var);
        Assert.AreEqual("var", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordConst_ProducesConstToken()
    {
        Token token = LexSingle("const", TokenType.Const);
        Assert.AreEqual("const", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordIf_ProducesIfToken()
    {
        Token token = LexSingle("if", TokenType.If);
        Assert.AreEqual("if", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordThen_ProducesThenToken()
    {
        Token token = LexSingle("then", TokenType.Then);
        Assert.AreEqual("then", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordElse_ProducesElseToken()
    {
        Token token = LexSingle("else", TokenType.Else);
        Assert.AreEqual("else", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordEnd_ProducesEndToken()
    {
        Token token = LexSingle("end", TokenType.End);
        Assert.AreEqual("end", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordWhile_ProducesWhileToken()
    {
        Token token = LexSingle("while", TokenType.While);
        Assert.AreEqual("while", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordDo_ProducesDoToken()
    {
        Token token = LexSingle("do", TokenType.Do);
        Assert.AreEqual("do", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordFor_ProducesForToken()
    {
        Token token = LexSingle("for", TokenType.For);
        Assert.AreEqual("for", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordTo_ProducesToToken()
    {
        Token token = LexSingle("to", TokenType.To);
        Assert.AreEqual("to", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordStep_ProducesStepToken()
    {
        Token token = LexSingle("step", TokenType.Step);
        Assert.AreEqual("step", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordForeach_ProducesForeachToken()
    {
        Token token = LexSingle("foreach", TokenType.Foreach);
        Assert.AreEqual("foreach", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordIn_ProducesInToken()
    {
        Token token = LexSingle("in", TokenType.In);
        Assert.AreEqual("in", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordBreak_ProducesBreakToken()
    {
        Token token = LexSingle("break", TokenType.Break);
        Assert.AreEqual("break", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordContinue_ProducesContinueToken()
    {
        Token token = LexSingle("continue", TokenType.Continue);
        Assert.AreEqual("continue", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordReturn_ProducesReturnToken()
    {
        Token token = LexSingle("return", TokenType.Return);
        Assert.AreEqual("return", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordPrint_ProducesPrintToken()
    {
        Token token = LexSingle("print", TokenType.Print);
        Assert.AreEqual("print", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordInput_ProducesInputToken()
    {
        Token token = LexSingle("input", TokenType.Input);
        Assert.AreEqual("input", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordFunction_ProducesFunctionToken()
    {
        Token token = LexSingle("function", TokenType.Function);
        Assert.AreEqual("function", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordLambda_ProducesLambdaToken()
    {
        Token token = LexSingle("lambda", TokenType.Lambda);
        Assert.AreEqual("lambda", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordClass_ProducesClassToken()
    {
        Token token = LexSingle("class", TokenType.Class);
        Assert.AreEqual("class", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordNew_ProducesNewToken()
    {
        Token token = LexSingle("new", TokenType.New);
        Assert.AreEqual("new", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordThis_ProducesThisToken()
    {
        Token token = LexSingle("this", TokenType.This);
        Assert.AreEqual("this", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordModule_ProducesModuleToken()
    {
        Token token = LexSingle("module", TokenType.Module);
        Assert.AreEqual("module", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordImport_ProducesImportToken()
    {
        Token token = LexSingle("import", TokenType.Import);
        Assert.AreEqual("import", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordExport_ProducesExportToken()
    {
        Token token = LexSingle("export", TokenType.Export);
        Assert.AreEqual("export", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordTry_ProducesTryToken()
    {
        Token token = LexSingle("try", TokenType.Try);
        Assert.AreEqual("try", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordCatch_ProducesCatchToken()
    {
        Token token = LexSingle("catch", TokenType.Catch);
        Assert.AreEqual("catch", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordFinally_ProducesFinallyToken()
    {
        Token token = LexSingle("finally", TokenType.Finally);
        Assert.AreEqual("finally", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordThrow_ProducesThrowToken()
    {
        Token token = LexSingle("throw", TokenType.Throw);
        Assert.AreEqual("throw", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordMatch_ProducesMatchToken()
    {
        Token token = LexSingle("match", TokenType.Match);
        Assert.AreEqual("match", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordCase_ProducesCaseToken()
    {
        Token token = LexSingle("case", TokenType.Case);
        Assert.AreEqual("case", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordDefault_ProducesDefaultToken()
    {
        Token token = LexSingle("default", TokenType.Default);
        Assert.AreEqual("default", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordEnum_ProducesEnumToken()
    {
        Token token = LexSingle("enum", TokenType.Enum);
        Assert.AreEqual("enum", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordTrue_ProducesTrueToken()
    {
        Token token = LexSingle("true", TokenType.True);
        Assert.AreEqual("true", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordFalse_ProducesFalseToken()
    {
        Token token = LexSingle("false", TokenType.False);
        Assert.AreEqual("false", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordNull_ProducesNullToken()
    {
        Token token = LexSingle("null", TokenType.Null);
        Assert.AreEqual("null", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordAnd_ProducesAndToken()
    {
        Token token = LexSingle("and", TokenType.And);
        Assert.AreEqual("and", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordOr_ProducesOrToken()
    {
        Token token = LexSingle("or", TokenType.Or);
        Assert.AreEqual("or", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordNot_ProducesNotToken()
    {
        Token token = LexSingle("not", TokenType.Not);
        Assert.AreEqual("not", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordIs_ProducesIsToken()
    {
        Token token = LexSingle("is", TokenType.Is);
        Assert.AreEqual("is", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordAs_ProducesAsToken()
    {
        Token token = LexSingle("as", TokenType.As);
        Assert.AreEqual("as", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordStatic_ProducesStaticToken()
    {
        Token token = LexSingle("static", TokenType.Static);
        Assert.AreEqual("static", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordConstructor_ProducesConstructorToken()
    {
        Token token = LexSingle("Constructor", TokenType.Constructor);
        Assert.AreEqual("Constructor", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordExtends_ProducesExtendsToken()
    {
        Token token = LexSingle("extends", TokenType.Extends);
        Assert.AreEqual("extends", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordImplements_ProducesImplementsToken()
    {
        Token token = LexSingle("implements", TokenType.Implements);
        Assert.AreEqual("implements", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordWhen_ProducesWhenToken()
    {
        Token token = LexSingle("when", TokenType.When);
        Assert.AreEqual("when", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordSwitch_ProducesSwitchToken()
    {
        Token token = LexSingle("switch", TokenType.Switch);
        Assert.AreEqual("switch", token.Value);
    }

    // ── Type keywords ─────────────────────────────────────────────────────

    [TestMethod]
    public void Lexer_KeywordInt_ProducesTypeIntToken()
    {
        Token token = LexSingle("int", TokenType.TypeInt);
        Assert.AreEqual("int", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordFloat_ProducesTypeFloatToken()
    {
        Token token = LexSingle("float", TokenType.TypeFloat);
        Assert.AreEqual("float", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordString_ProducesTypeStringToken()
    {
        Token token = LexSingle("string", TokenType.TypeString);
        Assert.AreEqual("string", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordBool_ProducesTypeBoolToken()
    {
        Token token = LexSingle("bool", TokenType.TypeBool);
        Assert.AreEqual("bool", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordArray_ProducesTypeArrayToken()
    {
        Token token = LexSingle("array", TokenType.TypeArray);
        Assert.AreEqual("array", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordObject_ProducesTypeObjectToken()
    {
        Token token = LexSingle("object", TokenType.TypeObject);
        Assert.AreEqual("object", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordVoid_ProducesTypeVoidToken()
    {
        Token token = LexSingle("void", TokenType.TypeVoid);
        Assert.AreEqual("void", token.Value);
    }

    [TestMethod]
    public void Lexer_KeywordMap_ProducesTypeMapToken()
    {
        Token token = LexSingle("map", TokenType.TypeMap);
        Assert.AreEqual("map", token.Value);
    }

    // ================================================================
    // Operators
    // ================================================================

    [TestMethod]
    public void Lexer_Assign_ProducesAssignToken()
    {
        Token token = LexSingle(":=", TokenType.Assign);
        Assert.AreEqual(":=", token.Value);
    }

    [TestMethod]
    public void Lexer_SingleEqual_ProducesSingleEqualToken()
    {
        Token token = LexSingle("=", TokenType.SingleEqual);
        Assert.AreEqual("=", token.Value);
    }

    [TestMethod]
    public void Lexer_Plus_ProducesPlusToken()
    {
        Token token = LexSingle("+", TokenType.Plus);
        Assert.AreEqual("+", token.Value);
    }

    [TestMethod]
    public void Lexer_Minus_ProducesMinusToken()
    {
        Token token = LexSingle("-", TokenType.Minus);
        Assert.AreEqual("-", token.Value);
    }

    [TestMethod]
    public void Lexer_Star_ProducesStarToken()
    {
        Token token = LexSingle("*", TokenType.Star);
        Assert.AreEqual("*", token.Value);
    }

    [TestMethod]
    public void Lexer_Slash_ProducesSlashToken()
    {
        Token token = LexSingle("/", TokenType.Slash);
        Assert.AreEqual("/", token.Value);
    }

    [TestMethod]
    public void Lexer_DoubleSlash_ProducesDoubleSlashToken()
    {
        // // is floor-division — NEVER a comment (spec note 1).
        Token token = LexSingle("//", TokenType.DoubleSlash);
        Assert.AreEqual("//", token.Value);
    }

    [TestMethod]
    public void Lexer_DoubleStar_ProducesDoubleStarToken()
    {
        Token token = LexSingle("**", TokenType.DoubleStar);
        Assert.AreEqual("**", token.Value);
    }

    [TestMethod]
    public void Lexer_Percent_ProducesPercentToken()
    {
        Token token = LexSingle("%", TokenType.Percent);
        Assert.AreEqual("%", token.Value);
    }

    [TestMethod]
    public void Lexer_Ampersand_ProducesAmpersandToken()
    {
        Token token = LexSingle("&", TokenType.Ampersand);
        Assert.AreEqual("&", token.Value);
    }

    [TestMethod]
    public void Lexer_Pipe_ProducesPipeToken()
    {
        Token token = LexSingle("|", TokenType.Pipe);
        Assert.AreEqual("|", token.Value);
    }

    [TestMethod]
    public void Lexer_Caret_ProducesCaretToken()
    {
        Token token = LexSingle("^", TokenType.Caret);
        Assert.AreEqual("^", token.Value);
    }

    [TestMethod]
    public void Lexer_Tilde_ProducesTildeToken()
    {
        Token token = LexSingle("~", TokenType.Tilde);
        Assert.AreEqual("~", token.Value);
    }

    [TestMethod]
    public void Lexer_ShiftLeft_ProducesShiftLeftToken()
    {
        Token token = LexSingle("<<", TokenType.ShiftLeft);
        Assert.AreEqual("<<", token.Value);
    }

    [TestMethod]
    public void Lexer_ShiftRight_ProducesShiftRightToken()
    {
        Token token = LexSingle(">>", TokenType.ShiftRight);
        Assert.AreEqual(">>", token.Value);
    }

    [TestMethod]
    public void Lexer_EqualEqual_ProducesEqualEqualToken()
    {
        Token token = LexSingle("==", TokenType.EqualEqual);
        Assert.AreEqual("==", token.Value);
    }

    [TestMethod]
    public void Lexer_NotEqual_ProducesNotEqualToken()
    {
        Token token = LexSingle("!=", TokenType.NotEqual);
        Assert.AreEqual("!=", token.Value);
    }

    [TestMethod]
    public void Lexer_Less_ProducesLessToken()
    {
        Token token = LexSingle("<", TokenType.Less);
        Assert.AreEqual("<", token.Value);
    }

    [TestMethod]
    public void Lexer_Greater_ProducesGreaterToken()
    {
        Token token = LexSingle(">", TokenType.Greater);
        Assert.AreEqual(">", token.Value);
    }

    [TestMethod]
    public void Lexer_LessEqual_ProducesLessEqualToken()
    {
        Token token = LexSingle("<=", TokenType.LessEqual);
        Assert.AreEqual("<=", token.Value);
    }

    [TestMethod]
    public void Lexer_GreaterEqual_ProducesGreaterEqualToken()
    {
        Token token = LexSingle(">=", TokenType.GreaterEqual);
        Assert.AreEqual(">=", token.Value);
    }

    [TestMethod]
    public void Lexer_DoubleAmpersand_ProducesDoubleAmpersandToken()
    {
        Token token = LexSingle("&&", TokenType.DoubleAmpersand);
        Assert.AreEqual("&&", token.Value);
    }

    [TestMethod]
    public void Lexer_DoublePipe_ProducesDoublePipeToken()
    {
        Token token = LexSingle("||", TokenType.DoublePipe);
        Assert.AreEqual("||", token.Value);
    }

    [TestMethod]
    public void Lexer_Arrow_ProducesArrowToken()
    {
        Token token = LexSingle("->", TokenType.Arrow);
        Assert.AreEqual("->", token.Value);
    }

    [TestMethod]
    public void Lexer_FatArrow_ProducesFatArrowToken()
    {
        Token token = LexSingle("=>", TokenType.FatArrow);
        Assert.AreEqual("=>", token.Value);
    }

    [TestMethod]
    public void Lexer_Question_ProducesQuestionToken()
    {
        Token token = LexSingle("?", TokenType.Question);
        Assert.AreEqual("?", token.Value);
    }

    [TestMethod]
    public void Lexer_Colon_ProducesColonToken()
    {
        Token token = LexSingle(":", TokenType.Colon);
        Assert.AreEqual(":", token.Value);
    }

    [TestMethod]
    public void Lexer_PlusAssign_ProducesPlusAssignToken()
    {
        Token token = LexSingle("+=", TokenType.PlusAssign);
        Assert.AreEqual("+=", token.Value);
    }

    [TestMethod]
    public void Lexer_MinusAssign_ProducesMinusAssignToken()
    {
        Token token = LexSingle("-=", TokenType.MinusAssign);
        Assert.AreEqual("-=", token.Value);
    }

    [TestMethod]
    public void Lexer_StarAssign_ProducesStarAssignToken()
    {
        Token token = LexSingle("*=", TokenType.StarAssign);
        Assert.AreEqual("*=", token.Value);
    }

    [TestMethod]
    public void Lexer_SlashAssign_ProducesSlashAssignToken()
    {
        Token token = LexSingle("/=", TokenType.SlashAssign);
        Assert.AreEqual("/=", token.Value);
    }

    // ================================================================
    // Delimiters / punctuation
    // ================================================================

    [TestMethod]
    public void Lexer_LeftParen_ProducesLeftParenToken()
    {
        Token token = LexSingle("(", TokenType.LeftParen);
        Assert.AreEqual("(", token.Value);
    }

    [TestMethod]
    public void Lexer_RightParen_ProducesRightParenToken()
    {
        Token token = LexSingle(")", TokenType.RightParen);
        Assert.AreEqual(")", token.Value);
    }

    [TestMethod]
    public void Lexer_LeftBracket_ProducesLeftBracketToken()
    {
        Token token = LexSingle("[", TokenType.LeftBracket);
        Assert.AreEqual("[", token.Value);
    }

    [TestMethod]
    public void Lexer_RightBracket_ProducesRightBracketToken()
    {
        Token token = LexSingle("]", TokenType.RightBracket);
        Assert.AreEqual("]", token.Value);
    }

    [TestMethod]
    public void Lexer_LeftBrace_ProducesLeftBraceToken()
    {
        Token token = LexSingle("{", TokenType.LeftBrace);
        Assert.AreEqual("{", token.Value);
    }

    [TestMethod]
    public void Lexer_RightBrace_ProducesRightBraceToken()
    {
        Token token = LexSingle("}", TokenType.RightBrace);
        Assert.AreEqual("}", token.Value);
    }

    [TestMethod]
    public void Lexer_Comma_ProducesCommaToken()
    {
        Token token = LexSingle(",", TokenType.Comma);
        Assert.AreEqual(",", token.Value);
    }

    [TestMethod]
    public void Lexer_Dot_ProducesDotToken()
    {
        Token token = LexSingle(".", TokenType.Dot);
        Assert.AreEqual(".", token.Value);
    }

    [TestMethod]
    public void Lexer_Semicolon_ProducesSemicolonToken()
    {
        Token token = LexSingle(";", TokenType.Semicolon);
        Assert.AreEqual(";", token.Value);
    }

    [TestMethod]
    public void Lexer_At_ProducesAtToken()
    {
        Token token = LexSingle("@", TokenType.At);
        Assert.AreEqual("@", token.Value);
    }

    // ================================================================
    // Number literals
    // ================================================================

    [TestMethod]
    public void Lexer_IntegerLiteralZero_ProducesIntegerToken()
    {
        Token token = LexSingle("0", TokenType.IntegerLiteral);
        Assert.AreEqual("0", token.Value);
    }

    [TestMethod]
    public void Lexer_IntegerLiteralDecimal_ProducesIntegerToken()
    {
        Token token = LexSingle("42", TokenType.IntegerLiteral);
        Assert.AreEqual("42", token.Value);
    }

    [TestMethod]
    public void Lexer_IntegerLiteralHex_ProducesIntegerToken()
    {
        Token token = LexSingle("0xFF", TokenType.IntegerLiteral);
        Assert.AreEqual("0xFF", token.Value);
    }

    [TestMethod]
    public void Lexer_IntegerLiteralHexLowerCase_ProducesIntegerToken()
    {
        Token token = LexSingle("0xff", TokenType.IntegerLiteral);
        Assert.AreEqual("0xff", token.Value);
    }

    [TestMethod]
    public void Lexer_IntegerLiteralBinary_ProducesIntegerToken()
    {
        Token token = LexSingle("0b1010", TokenType.IntegerLiteral);
        Assert.AreEqual("0b1010", token.Value);
    }

    [TestMethod]
    public void Lexer_IntegerLiteralOctal_ProducesIntegerToken()
    {
        Token token = LexSingle("0o17", TokenType.IntegerLiteral);
        Assert.AreEqual("0o17", token.Value);
    }

    [TestMethod]
    public void Lexer_FloatLiteralPi_ProducesFloatToken()
    {
        Token token = LexSingle("3.14", TokenType.FloatLiteral);
        Assert.AreEqual("3.14", token.Value);
    }

    [TestMethod]
    public void Lexer_FloatLiteralZeroPoint5_ProducesFloatToken()
    {
        Token token = LexSingle("0.5", TokenType.FloatLiteral);
        Assert.AreEqual("0.5", token.Value);
    }

    [TestMethod]
    public void Lexer_FloatLiteralOnePoint0_ProducesFloatToken()
    {
        Token token = LexSingle("1.0", TokenType.FloatLiteral);
        Assert.AreEqual("1.0", token.Value);
    }

    // ================================================================
    // String literals
    // ================================================================

    [TestMethod]
    public void Lexer_DoubleQuotedString_ProducesStringToken()
    {
        Token token = LexSingle("\"hello\"", TokenType.StringLiteral);
        Assert.AreEqual("hello", token.Value);
    }

    [TestMethod]
    public void Lexer_SingleQuotedString_ProducesStringToken()
    {
        Token token = LexSingle("'hello'", TokenType.StringLiteral);
        Assert.AreEqual("hello", token.Value);
    }

    [TestMethod]
    public void Lexer_StringWithNewlineEscape_ProducesNewlineInValue()
    {
        Token token = LexSingle("\"a\\nb\"", TokenType.StringLiteral);
        Assert.AreEqual("a\nb", token.Value);
    }

    [TestMethod]
    public void Lexer_StringWithTabEscape_ProducesTabInValue()
    {
        Token token = LexSingle("\"a\\tb\"", TokenType.StringLiteral);
        Assert.AreEqual("a\tb", token.Value);
    }

    [TestMethod]
    public void Lexer_StringWithBackslashEscape_ProducesBackslashInValue()
    {
        Token token = LexSingle("\"a\\\\b\"", TokenType.StringLiteral);
        Assert.AreEqual("a\\b", token.Value);
    }

    [TestMethod]
    public void Lexer_StringWithDoubleQuoteEscape_ProducesQuoteInValue()
    {
        Token token = LexSingle("\"a\\\"b\"", TokenType.StringLiteral);
        Assert.AreEqual("a\"b", token.Value);
    }

    [TestMethod]
    public void Lexer_StringWithSingleQuoteEscape_ProducesQuoteInValue()
    {
        Token token = LexSingle("'a\\'b'", TokenType.StringLiteral);
        Assert.AreEqual("a'b", token.Value);
    }

    // ================================================================
    // Identifiers
    // ================================================================

    [TestMethod]
    public void Lexer_SimpleIdentifier_ProducesIdentifierToken()
    {
        Token token = LexSingle("myVar", TokenType.Identifier);
        Assert.AreEqual("myVar", token.Value);
    }

    [TestMethod]
    public void Lexer_IdentifierWithUnderscore_ProducesIdentifierToken()
    {
        Token token = LexSingle("my_var", TokenType.Identifier);
        Assert.AreEqual("my_var", token.Value);
    }

    [TestMethod]
    public void Lexer_IdentifierWithDigits_ProducesIdentifierToken()
    {
        Token token = LexSingle("var1", TokenType.Identifier);
        Assert.AreEqual("var1", token.Value);
    }

    // ================================================================
    // Comments
    // ================================================================

    [TestMethod]
    public void Lexer_LineComment_SkipsCommentTextAndProducesOnlyEof()
    {
        List<Token> tokens = Lex("# this is a comment");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
    }

    [TestMethod]
    public void Lexer_LineCommentBeforeToken_SkipsCommentAndProducesToken()
    {
        List<Token> tokens = Lex("# comment\n42");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.AreEqual("42", tokens[0].Value);
        Assert.AreEqual(TokenType.EndOfFile, tokens[1].Type);
    }

    [TestMethod]
    public void Lexer_BlockComment_SkipsCommentAndProducesOnlyEof()
    {
        List<Token> tokens = Lex("/* this is a block comment */");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
    }

    [TestMethod]
    public void Lexer_BlockCommentMultiLine_SkipsCommentAndProducesNextToken()
    {
        List<Token> tokens = Lex("/* line1\nline2 */ 99");
        Assert.AreEqual(2, tokens.Count);
        Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.AreEqual("99", tokens[0].Value);
    }

    [TestMethod]
    public void Lexer_DoubleSlash_IsFloorDivisionNotComment()
    {
        // // must never be treated as a comment; it is floor-division.
        List<Token> tokens = Lex("7 // 2");
        Assert.AreEqual(3, tokens.Count);
        Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
        Assert.AreEqual(TokenType.DoubleSlash,    tokens[1].Type);
        Assert.AreEqual(TokenType.IntegerLiteral, tokens[2].Type);
        Assert.AreEqual("//", tokens[1].Value);
    }

    // ================================================================
    // Line numbers
    // ================================================================

    [TestMethod]
    public void Lexer_LineNumbers_FirstTokenIsOnLineOne()
    {
        List<Token> tokens = Lex("42");
        Assert.AreEqual(1, tokens[0].Line);
    }

    [TestMethod]
    public void Lexer_LineNumbers_IncrementsAcrossNewlines()
    {
        List<Token> tokens = Lex("a\nb\nc");
        Assert.AreEqual(1, tokens[0].Line, "a should be on line 1");
        Assert.AreEqual(2, tokens[1].Line, "b should be on line 2");
        Assert.AreEqual(3, tokens[2].Line, "c should be on line 3");
    }

    [TestMethod]
    public void Lexer_LineNumbers_TokenAfterCommentIsOnCorrectLine()
    {
        List<Token> tokens = Lex("# comment\n42");
        Assert.AreEqual(2, tokens[0].Line, "42 should be on line 2 after a comment line");
    }

    // ================================================================
    // Error cases
    // ================================================================

    [TestMethod]
    [ExpectedException(typeof(LexerException))]
    public void Lexer_UnterminatedString_ThrowsLexerException()
    {
        Lex("\"unterminated");
    }

    [TestMethod]
    [ExpectedException(typeof(LexerException))]
    public void Lexer_UnterminatedBlockComment_ThrowsLexerException()
    {
        Lex("/* unterminated");
    }

    // ================================================================
    // Edge cases
    // ================================================================

    [TestMethod]
    public void Lexer_EmptyInput_ProducesOnlyEndOfFileToken()
    {
        List<Token> tokens = Lex(string.Empty);
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
    }

    [TestMethod]
    public void Lexer_WhitespaceOnly_ProducesOnlyEndOfFileToken()
    {
        List<Token> tokens = Lex("   \t  \n  ");
        Assert.AreEqual(1, tokens.Count);
        Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
    }

    [TestMethod]
    public void Lexer_UnknownCharacter_ProducesUnknownToken()
    {
        Token token = LexSingle("$", TokenType.Unknown);
        Assert.AreEqual("$", token.Value);
    }

    [TestMethod]
    public void Lexer_MultiTokenSequence_ProducesCorrectTokensInOrder()
    {
        List<Token> tokens = Lex("let x := 5");
        Assert.AreEqual(5, tokens.Count); // let, x, :=, 5, EOF
        Assert.AreEqual(TokenType.Let,            tokens[0].Type);
        Assert.AreEqual(TokenType.Identifier,     tokens[1].Type);
        Assert.AreEqual(TokenType.Assign,         tokens[2].Type);
        Assert.AreEqual(TokenType.IntegerLiteral, tokens[3].Type);
        Assert.AreEqual(TokenType.EndOfFile,      tokens[4].Type);
    }
}
