using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLanguage.Lexer;

namespace TinyLanguage.UnitTests
{
    [TestClass]
    public class LexerUnitTests
    {
        // ----------------------------------------------------------------
        // Helper
        // ----------------------------------------------------------------

        private static List<Token> Lex(string src)
        {
            return TinyLanguage.Lexer.Lexer.Tokenise(src);
        }

        // ----------------------------------------------------------------
        // Group 1: Keywords — one test per keyword
        // ----------------------------------------------------------------

        [TestMethod]
        public void Keyword_If_ProducesIfToken()
        {
            List<Token> tokens = Lex("if");
            Assert.AreEqual(TokenType.If, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Then_ProducesThenToken()
        {
            List<Token> tokens = Lex("then");
            Assert.AreEqual(TokenType.Then, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Else_ProducesElseToken()
        {
            List<Token> tokens = Lex("else");
            Assert.AreEqual(TokenType.Else, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_End_ProducesEndToken()
        {
            List<Token> tokens = Lex("end");
            Assert.AreEqual(TokenType.End, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_While_ProducesWhileToken()
        {
            List<Token> tokens = Lex("while");
            Assert.AreEqual(TokenType.While, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Do_ProducesDoToken()
        {
            List<Token> tokens = Lex("do");
            Assert.AreEqual(TokenType.Do, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_For_ProducesForToken()
        {
            List<Token> tokens = Lex("for");
            Assert.AreEqual(TokenType.For, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_To_ProducesToToken()
        {
            List<Token> tokens = Lex("to");
            Assert.AreEqual(TokenType.To, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Step_ProducesStepToken()
        {
            List<Token> tokens = Lex("step");
            Assert.AreEqual(TokenType.Step, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Foreach_ProducesForeachToken()
        {
            List<Token> tokens = Lex("foreach");
            Assert.AreEqual(TokenType.Foreach, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_In_ProducesInToken()
        {
            List<Token> tokens = Lex("in");
            Assert.AreEqual(TokenType.In, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Break_ProducesBreakToken()
        {
            List<Token> tokens = Lex("break");
            Assert.AreEqual(TokenType.Break, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Continue_ProducesContinueToken()
        {
            List<Token> tokens = Lex("continue");
            Assert.AreEqual(TokenType.Continue, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Switch_ProducesSwitchToken()
        {
            List<Token> tokens = Lex("switch");
            Assert.AreEqual(TokenType.Switch, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Case_ProducesCaseToken()
        {
            List<Token> tokens = Lex("case");
            Assert.AreEqual(TokenType.Case, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Default_ProducesDefaultToken()
        {
            List<Token> tokens = Lex("default");
            Assert.AreEqual(TokenType.Default, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Let_ProducesLetToken()
        {
            List<Token> tokens = Lex("let");
            Assert.AreEqual(TokenType.Let, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Var_ProducesVarToken()
        {
            List<Token> tokens = Lex("var");
            Assert.AreEqual(TokenType.Var, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Const_ProducesConstToken()
        {
            List<Token> tokens = Lex("const");
            Assert.AreEqual(TokenType.Const, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Enum_ProducesEnumToken()
        {
            List<Token> tokens = Lex("enum");
            Assert.AreEqual(TokenType.Enum, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Function_ProducesFunctionToken()
        {
            List<Token> tokens = Lex("function");
            Assert.AreEqual(TokenType.Function, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Return_ProducesReturnToken()
        {
            List<Token> tokens = Lex("return");
            Assert.AreEqual(TokenType.Return, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Class_ProducesClassToken()
        {
            List<Token> tokens = Lex("class");
            Assert.AreEqual(TokenType.Class, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Extends_ProducesExtendsToken()
        {
            List<Token> tokens = Lex("extends");
            Assert.AreEqual(TokenType.Extends, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Implements_ProducesImplementsToken()
        {
            List<Token> tokens = Lex("implements");
            Assert.AreEqual(TokenType.Implements, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Constructor_ProducesConstructorToken()
        {
            List<Token> tokens = Lex("Constructor");
            Assert.AreEqual(TokenType.Constructor, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_New_ProducesNewToken()
        {
            List<Token> tokens = Lex("new");
            Assert.AreEqual(TokenType.New, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Static_ProducesStaticToken()
        {
            List<Token> tokens = Lex("static");
            Assert.AreEqual(TokenType.Static, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Module_ProducesModuleToken()
        {
            List<Token> tokens = Lex("module");
            Assert.AreEqual(TokenType.Module, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Import_ProducesImportToken()
        {
            List<Token> tokens = Lex("import");
            Assert.AreEqual(TokenType.Import, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_As_ProducesAsToken()
        {
            List<Token> tokens = Lex("as");
            Assert.AreEqual(TokenType.As, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Export_ProducesExportToken()
        {
            List<Token> tokens = Lex("export");
            Assert.AreEqual(TokenType.Export, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Try_ProducesTryToken()
        {
            List<Token> tokens = Lex("try");
            Assert.AreEqual(TokenType.Try, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Catch_ProducesCatchToken()
        {
            List<Token> tokens = Lex("catch");
            Assert.AreEqual(TokenType.Catch, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Finally_ProducesFinallyToken()
        {
            List<Token> tokens = Lex("finally");
            Assert.AreEqual(TokenType.Finally, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Throw_ProducesThrowToken()
        {
            List<Token> tokens = Lex("throw");
            Assert.AreEqual(TokenType.Throw, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Match_ProducesMatchToken()
        {
            List<Token> tokens = Lex("match");
            Assert.AreEqual(TokenType.Match, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_When_ProducesWhenToken()
        {
            List<Token> tokens = Lex("when");
            Assert.AreEqual(TokenType.When, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Print_ProducesPrintToken()
        {
            List<Token> tokens = Lex("print");
            Assert.AreEqual(TokenType.Print, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Input_ProducesInputToken()
        {
            List<Token> tokens = Lex("input");
            Assert.AreEqual(TokenType.Input, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Not_ProducesNotToken()
        {
            List<Token> tokens = Lex("not");
            Assert.AreEqual(TokenType.Not, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_And_ProducesAndToken()
        {
            List<Token> tokens = Lex("and");
            Assert.AreEqual(TokenType.And, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Or_ProducesOrToken()
        {
            List<Token> tokens = Lex("or");
            Assert.AreEqual(TokenType.Or, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Is_ProducesIsToken()
        {
            List<Token> tokens = Lex("is");
            Assert.AreEqual(TokenType.Is, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Int_ProducesIntToken()
        {
            List<Token> tokens = Lex("int");
            Assert.AreEqual(TokenType.Int, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Float_ProducesFloatToken()
        {
            List<Token> tokens = Lex("float");
            Assert.AreEqual(TokenType.Float, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_String_ProducesStringToken()
        {
            List<Token> tokens = Lex("string");
            Assert.AreEqual(TokenType.String, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Bool_ProducesBoolToken()
        {
            List<Token> tokens = Lex("bool");
            Assert.AreEqual(TokenType.Bool, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Array_ProducesArrayToken()
        {
            List<Token> tokens = Lex("array");
            Assert.AreEqual(TokenType.Array, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Object_ProducesObjectToken()
        {
            List<Token> tokens = Lex("object");
            Assert.AreEqual(TokenType.Object, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Void_ProducesVoidToken()
        {
            List<Token> tokens = Lex("void");
            Assert.AreEqual(TokenType.Void, tokens[0].Type);
        }

        [TestMethod]
        public void Keyword_Map_ProducesMapToken()
        {
            List<Token> tokens = Lex("map");
            Assert.AreEqual(TokenType.Map, tokens[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 2: Identifiers
        // ----------------------------------------------------------------

        [TestMethod]
        public void Identifier_SimpleWord_ProducesIdentifierToken()
        {
            List<Token> tokens = Lex("myVar");
            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual("myVar", tokens[0].Value);
        }

        [TestMethod]
        public void Identifier_WithDigitsAndUnderscores_ProducesIdentifierToken()
        {
            List<Token> tokens = Lex("foo_bar123");
            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual("foo_bar123", tokens[0].Value);
        }

        [TestMethod]
        public void Identifier_KeywordPrefixWithExtraChars_ProducesIdentifierToken()
        {
            // "letter" starts with "let" but is a full identifier
            List<Token> tokens = Lex("letter");
            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual("letter", tokens[0].Value);
        }

        [TestMethod]
        public void Identifier_KeywordSuffixedWithDigit_ProducesIdentifierToken()
        {
            // "if2" starts with "if" but is an identifier
            List<Token> tokens = Lex("if2");
            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual("if2", tokens[0].Value);
        }

        [TestMethod]
        public void Identifier_SingleLetter_ProducesIdentifierToken()
        {
            List<Token> tokens = Lex("x");
            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual("x", tokens[0].Value);
        }

        // ----------------------------------------------------------------
        // Group 3: Integer literals
        // ----------------------------------------------------------------

        [TestMethod]
        public void IntegerLiteral_Decimal_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("42");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("42", tokens[0].Value);
        }

        [TestMethod]
        public void IntegerLiteral_Zero_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("0");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("0", tokens[0].Value);
        }

        [TestMethod]
        public void IntegerLiteral_Hex_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("0xFF");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("0xFF", tokens[0].Value);
        }

        [TestMethod]
        public void IntegerLiteral_HexLowercase_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("0xdeadbeef");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("0xdeadbeef", tokens[0].Value);
        }

        [TestMethod]
        public void IntegerLiteral_Binary_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("0b1010");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("0b1010", tokens[0].Value);
        }

        [TestMethod]
        public void IntegerLiteral_Octal_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("0o755");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("0o755", tokens[0].Value);
        }

        [TestMethod]
        public void IntegerLiteral_LargeNumber_ProducesIntegerLiteralToken()
        {
            List<Token> tokens = Lex("9999999999");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual("9999999999", tokens[0].Value);
        }

        // ----------------------------------------------------------------
        // Group 4: Float literals
        // ----------------------------------------------------------------

        [TestMethod]
        public void FloatLiteral_Basic_ProducesFloatLiteralToken()
        {
            List<Token> tokens = Lex("3.14");
            Assert.AreEqual(TokenType.FloatLiteral, tokens[0].Type);
            Assert.AreEqual("3.14", tokens[0].Value);
        }

        [TestMethod]
        public void FloatLiteral_TrailingZeros_ProducesFloatLiteralToken()
        {
            List<Token> tokens = Lex("1.00");
            Assert.AreEqual(TokenType.FloatLiteral, tokens[0].Type);
            Assert.AreEqual("1.00", tokens[0].Value);
        }

        [TestMethod]
        public void FloatLiteral_NoDotBeforeDecimal_LexesAsIntThenDot()
        {
            // ".5" is not a float literal — the lexer requires a digit before the dot
            List<Token> tokens = Lex(".5");
            Assert.AreEqual(TokenType.Dot, tokens[0].Type);
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[1].Type);
        }

        [TestMethod]
        public void FloatLiteral_TrailingDot_LexesAsIntThenDot()
        {
            // "1." is integer then dot, not a float
            List<Token> tokens = Lex("1.");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual(TokenType.Dot, tokens[1].Type);
        }

        // ----------------------------------------------------------------
        // Group 5: String literals
        // ----------------------------------------------------------------

        [TestMethod]
        public void StringLiteral_DoubleQuoted_ProducesStringLiteralToken()
        {
            List<Token> tokens = Lex("\"hello\"");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("hello", tokens[0].Value);
        }

        [TestMethod]
        public void StringLiteral_SingleQuoted_ProducesStringLiteralToken()
        {
            List<Token> tokens = Lex("'world'");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("world", tokens[0].Value);
        }

        [TestMethod]
        public void StringLiteral_EscapeNewline_DecodesEscape()
        {
            List<Token> tokens = Lex("\"line1\\nline2\"");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("line1\nline2", tokens[0].Value);
        }

        [TestMethod]
        public void StringLiteral_EscapeTab_DecodesEscape()
        {
            List<Token> tokens = Lex("\"col1\\tcol2\"");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("col1\tcol2", tokens[0].Value);
        }

        [TestMethod]
        public void StringLiteral_EscapeBackslash_DecodesEscape()
        {
            List<Token> tokens = Lex("\"a\\\\b\"");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("a\\b", tokens[0].Value);
        }

        [TestMethod]
        public void StringLiteral_EscapeDoubleQuote_DecodesEscape()
        {
            List<Token> tokens = Lex("\"say \\\"hi\\\"\"");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("say \"hi\"", tokens[0].Value);
        }

        [TestMethod]
        public void StringLiteral_EmptyString_ProducesEmptyValue()
        {
            List<Token> tokens = Lex("\"\"");
            Assert.AreEqual(TokenType.StringLiteral, tokens[0].Type);
            Assert.AreEqual("", tokens[0].Value);
        }

        // ----------------------------------------------------------------
        // Group 6: Boolean literals
        // ----------------------------------------------------------------

        [TestMethod]
        public void BoolLiteral_True_ProducesTrueToken()
        {
            List<Token> tokens = Lex("true");
            Assert.AreEqual(TokenType.True, tokens[0].Type);
        }

        [TestMethod]
        public void BoolLiteral_False_ProducesFalseToken()
        {
            List<Token> tokens = Lex("false");
            Assert.AreEqual(TokenType.False, tokens[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 7: Null literal
        // ----------------------------------------------------------------

        [TestMethod]
        public void NullLiteral_Null_ProducesNullToken()
        {
            List<Token> tokens = Lex("null");
            Assert.AreEqual(TokenType.Null, tokens[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 8: Operators
        // ----------------------------------------------------------------

        [TestMethod]
        public void Operator_Plus_ProducesPlusToken()
        {
            Assert.AreEqual(TokenType.Plus, Lex("+")[0].Type);
        }

        [TestMethod]
        public void Operator_Minus_ProducesMinusToken()
        {
            Assert.AreEqual(TokenType.Minus, Lex("-")[0].Type);
        }

        [TestMethod]
        public void Operator_Star_ProducesStarToken()
        {
            Assert.AreEqual(TokenType.Star, Lex("*")[0].Type);
        }

        [TestMethod]
        public void Operator_Slash_ProducesSlashToken()
        {
            Assert.AreEqual(TokenType.Slash, Lex("/")[0].Type);
        }

        [TestMethod]
        public void Operator_SlashSlash_ProducesFloorDivisionToken()
        {
            // Note 1: '//' is ALWAYS floor-division, never a comment
            List<Token> tokens = Lex("//");
            Assert.AreEqual(TokenType.SlashSlash, tokens[0].Type);
            Assert.AreEqual("//", tokens[0].Value);
        }

        [TestMethod]
        public void Operator_StarStar_ProducesExponentiationToken()
        {
            Assert.AreEqual(TokenType.StarStar, Lex("**")[0].Type);
        }

        [TestMethod]
        public void Operator_Percent_ProducesPercentToken()
        {
            Assert.AreEqual(TokenType.Percent, Lex("%")[0].Type);
        }

        [TestMethod]
        public void Operator_ColonEquals_ProducesAssignmentToken()
        {
            Assert.AreEqual(TokenType.ColonEquals, Lex(":=")[0].Type);
        }

        [TestMethod]
        public void Operator_SingleEqual_ProducesSingleEqualToken()
        {
            Assert.AreEqual(TokenType.SingleEqual, Lex("=")[0].Type);
        }

        [TestMethod]
        public void Operator_EqualEqual_ProducesEqualEqualToken()
        {
            Assert.AreEqual(TokenType.EqualEqual, Lex("==")[0].Type);
        }

        [TestMethod]
        public void Operator_BangEqual_ProducesBangEqualToken()
        {
            Assert.AreEqual(TokenType.BangEqual, Lex("!=")[0].Type);
        }

        [TestMethod]
        public void Operator_Less_ProducesLessToken()
        {
            Assert.AreEqual(TokenType.Less, Lex("<")[0].Type);
        }

        [TestMethod]
        public void Operator_LessEqual_ProducesLessEqualToken()
        {
            Assert.AreEqual(TokenType.LessEqual, Lex("<=")[0].Type);
        }

        [TestMethod]
        public void Operator_Greater_ProducesGreaterToken()
        {
            Assert.AreEqual(TokenType.Greater, Lex(">")[0].Type);
        }

        [TestMethod]
        public void Operator_GreaterEqual_ProducesGreaterEqualToken()
        {
            Assert.AreEqual(TokenType.GreaterEqual, Lex(">=")[0].Type);
        }

        [TestMethod]
        public void Operator_AmpAmp_ProducesLogicalAndToken()
        {
            Assert.AreEqual(TokenType.AmpAmp, Lex("&&")[0].Type);
        }

        [TestMethod]
        public void Operator_PipePipe_ProducesLogicalOrToken()
        {
            Assert.AreEqual(TokenType.PipePipe, Lex("||")[0].Type);
        }

        [TestMethod]
        public void Operator_Ampersand_ProducesStringConcatToken()
        {
            Assert.AreEqual(TokenType.Ampersand, Lex("&")[0].Type);
        }

        [TestMethod]
        public void Operator_Pipe_ProducesPatternAlternationToken()
        {
            Assert.AreEqual(TokenType.Pipe, Lex("|")[0].Type);
        }

        [TestMethod]
        public void Operator_Arrow_ProducesArrowToken()
        {
            Assert.AreEqual(TokenType.Arrow, Lex("->")[0].Type);
        }

        [TestMethod]
        public void Operator_FatArrow_ProducesFatArrowToken()
        {
            Assert.AreEqual(TokenType.FatArrow, Lex("=>")[0].Type);
        }

        [TestMethod]
        public void Operator_Question_ProducesQuestionToken()
        {
            Assert.AreEqual(TokenType.Question, Lex("?")[0].Type);
        }

        [TestMethod]
        public void Operator_Colon_ProducesColonToken()
        {
            Assert.AreEqual(TokenType.Colon, Lex(":")[0].Type);
        }

        [TestMethod]
        public void Operator_Dot_ProducesDotToken()
        {
            Assert.AreEqual(TokenType.Dot, Lex(".")[0].Type);
        }

        [TestMethod]
        public void Operator_At_ProducesAtToken()
        {
            Assert.AreEqual(TokenType.At, Lex("@")[0].Type);
        }

        [TestMethod]
        public void Operator_Underscore_ProducesUnderscoreToken()
        {
            Assert.AreEqual(TokenType.Underscore, Lex("_")[0].Type);
        }

        [TestMethod]
        public void Operator_UnderscoreWithLetters_ProducesUnknownToken()
        {
            // Identifiers must not start with underscore — lexer emits Unknown
            Assert.AreEqual(TokenType.Unknown, Lex("_foo")[0].Type);
        }

        [TestMethod]
        public void Operator_Bang_Standalone_ProducesUnknownToken()
        {
            // '!' alone (not '!=') is not a valid token
            Assert.AreEqual(TokenType.Unknown, Lex("!")[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 9: Punctuation
        // ----------------------------------------------------------------

        [TestMethod]
        public void Punctuation_LeftParen_ProducesLeftParenToken()
        {
            Assert.AreEqual(TokenType.LeftParen, Lex("(")[0].Type);
        }

        [TestMethod]
        public void Punctuation_RightParen_ProducesRightParenToken()
        {
            Assert.AreEqual(TokenType.RightParen, Lex(")")[0].Type);
        }

        [TestMethod]
        public void Punctuation_LeftBracket_ProducesLeftBracketToken()
        {
            Assert.AreEqual(TokenType.LeftBracket, Lex("[")[0].Type);
        }

        [TestMethod]
        public void Punctuation_RightBracket_ProducesRightBracketToken()
        {
            Assert.AreEqual(TokenType.RightBracket, Lex("]")[0].Type);
        }

        [TestMethod]
        public void Punctuation_LeftBrace_ProducesLeftBraceToken()
        {
            Assert.AreEqual(TokenType.LeftBrace, Lex("{")[0].Type);
        }

        [TestMethod]
        public void Punctuation_RightBrace_ProducesRightBraceToken()
        {
            Assert.AreEqual(TokenType.RightBrace, Lex("}")[0].Type);
        }

        [TestMethod]
        public void Punctuation_Comma_ProducesCommaToken()
        {
            Assert.AreEqual(TokenType.Comma, Lex(",")[0].Type);
        }

        [TestMethod]
        public void Punctuation_Semicolon_ProducesSemicolonToken()
        {
            Assert.AreEqual(TokenType.Semicolon, Lex(";")[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 10: Comments — note 1: '#' is the comment character; '//' is floor-division
        // ----------------------------------------------------------------

        [TestMethod]
        public void Comment_HashLineComment_ProducesNoTokensBeforeEof()
        {
            // '#' is the line comment character per spec note 1
            List<Token> tokens = Lex("# this is a comment");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
        }

        [TestMethod]
        public void Comment_HashCommentThenToken_SkipsCommentText()
        {
            List<Token> tokens = Lex("42 # ignore me");
            Assert.AreEqual(2, tokens.Count);  // integer + EOF
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual(TokenType.EndOfFile, tokens[1].Type);
        }

        [TestMethod]
        public void Comment_SlashSlash_IsFloorDivisionNotComment()
        {
            // Per spec note 1: '//' is ALWAYS floor-division, never a comment
            List<Token> tokens = Lex("10 // 3");
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
            Assert.AreEqual(TokenType.SlashSlash, tokens[1].Type);
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[2].Type);
        }

        // ----------------------------------------------------------------
        // Group 11: Whitespace skipping
        // ----------------------------------------------------------------

        [TestMethod]
        public void Whitespace_SpacesTabsNewlines_AreSkipped()
        {
            List<Token> tokens = Lex("  \t  \n  42  \n  ");
            Assert.AreEqual(2, tokens.Count);  // integer + EOF
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[0].Type);
        }

        [TestMethod]
        public void Whitespace_EmptySource_ProducesOnlyEof()
        {
            List<Token> tokens = Lex("");
            Assert.AreEqual(1, tokens.Count);
            Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 12: Line numbers
        // ----------------------------------------------------------------

        [TestMethod]
        public void LineNumber_FirstToken_IsOnLine1()
        {
            List<Token> tokens = Lex("hello");
            Assert.AreEqual(1, tokens[0].Line);
        }

        [TestMethod]
        public void LineNumber_TokenAfterNewline_IsOnLine2()
        {
            List<Token> tokens = Lex("a\nb");
            Assert.AreEqual(1, tokens[0].Line);
            Assert.AreEqual(2, tokens[1].Line);
        }

        [TestMethod]
        public void LineNumber_TokenOnLine3_ReportsLine3()
        {
            List<Token> tokens = Lex("a\nb\nc");
            Assert.AreEqual(3, tokens[2].Line);
        }

        // ----------------------------------------------------------------
        // Group 13: Multiple tokens in sequence
        // ----------------------------------------------------------------

        [TestMethod]
        public void MultipleTokens_LetDeclaration_ProducesCorrectSequence()
        {
            List<Token> tokens = Lex("let x := 42");
            Assert.AreEqual(5, tokens.Count); // let, x, :=, 42, EOF
            Assert.AreEqual(TokenType.Let, tokens[0].Type);
            Assert.AreEqual(TokenType.Identifier, tokens[1].Type);
            Assert.AreEqual("x", tokens[1].Value);
            Assert.AreEqual(TokenType.ColonEquals, tokens[2].Type);
            Assert.AreEqual(TokenType.IntegerLiteral, tokens[3].Type);
            Assert.AreEqual("42", tokens[3].Value);
            Assert.AreEqual(TokenType.EndOfFile, tokens[4].Type);
        }

        [TestMethod]
        public void MultipleTokens_BinaryExpression_ProducesCorrectSequence()
        {
            List<Token> tokens = Lex("x + y * z");
            Assert.AreEqual(6, tokens.Count); // x, +, y, *, z, EOF
            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual(TokenType.Plus, tokens[1].Type);
            Assert.AreEqual(TokenType.Identifier, tokens[2].Type);
            Assert.AreEqual(TokenType.Star, tokens[3].Type);
            Assert.AreEqual(TokenType.Identifier, tokens[4].Type);
        }

        // ----------------------------------------------------------------
        // Group 14: Error cases
        // ----------------------------------------------------------------

        [TestMethod]
        public void Error_UnterminatedDoubleQuoteString_ThrowsLexerException()
        {
            Assert.ThrowsException<LexerException>(() => Lex("\"unterminated"));
        }

        [TestMethod]
        public void Error_UnterminatedSingleQuoteString_ThrowsLexerException()
        {
            Assert.ThrowsException<LexerException>(() => Lex("'unterminated"));
        }

        [TestMethod]
        public void Error_UnknownCharacter_ProducesUnknownToken()
        {
            // Unknown characters produce Unknown tokens (no throw per spec)
            List<Token> tokens = Lex("$");
            Assert.AreEqual(TokenType.Unknown, tokens[0].Type);
        }

        // ----------------------------------------------------------------
        // Group 15: EOF
        // ----------------------------------------------------------------

        [TestMethod]
        public void EndOfFile_LastToken_IsEof()
        {
            List<Token> tokens = Lex("hello world");
            Token lastToken = tokens[tokens.Count - 1];
            Assert.AreEqual(TokenType.EndOfFile, lastToken.Type);
        }

        [TestMethod]
        public void EndOfFile_EmptySource_EofOnLine1()
        {
            List<Token> tokens = Lex("");
            Assert.AreEqual(TokenType.EndOfFile, tokens[0].Type);
            Assert.AreEqual(1, tokens[0].Line);
        }
    }
}
