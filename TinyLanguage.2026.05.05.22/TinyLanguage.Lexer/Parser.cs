using System;
using System.Collections.Generic;
using System.Globalization;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Lexer
{
    // Recursive-descent parser for TinyLanguage.
    //
    // Implements every BNF production in Build.Solution.md and obeys all 23
    // Implementation Notes. Public entry point: Parse(IEnumerable<Token>).
    //
    // Statement separator policy: the BNF declares ';' as a separator, but the
    // demo corpus relies on newlines (treated as whitespace by the lexer) acting
    // as the de-facto separator — none of the 450 demos use ';' between
    // statements. The parser therefore treats ';' as OPTIONAL between
    // statements; it consumes any number of trailing ';' tokens after a
    // statement, and it never demands one. A bare ';' before a block-ending
    // keyword (end, else, catch, finally, while, '}', or EOF) is silently
    // accepted as well — matching observed Tier B usage.
    //
    // Disambiguations handled here:
    //   Note 12 — inline conditional expr vs if-stmt — by parser context.
    //   Note 13 — cast (type) vs grouped (expr) — by lookahead.
    //   Note 14 — lambda block vs expression body — by scanning ahead for end.
    //   Note 20 — generic type Foo<int> vs comparison Foo < int — by lookahead.
    //   Note 23 — pattern alternation '|' is left-associative.
    public class Parser
    {
        private readonly List<Token> tokens;
        private int position;

        // Constructs a parser around a fully-lexed token list.
        // The token list must end with a single EndOfFile sentinel token.
        public Parser(IEnumerable<Token> tokenSource)
        {
            if (tokenSource == null)
            {
                throw new ArgumentNullException("tokenSource");
            }
            tokens = new List<Token>(tokenSource);
            position = 0;
        }

        // Parses the token stream into a ProgramNode.
        public ProgramNode Parse()
        {
            int startLine = Current().Line;
            List<AstNode> statements = ParseStatementList(IsTopLevelTerminator);
            ExpectEndOfFile();
            return new ProgramNode(startLine, statements);
        }

        // Convenience static entry — accepts an IEnumerable<Token> directly.
        public static ProgramNode Parse(IEnumerable<Token> tokens)
        {
            Parser parser = new Parser(tokens);
            return parser.Parse();
        }

        // ---------------------------------------------------------------
        // Token stream helpers
        // ---------------------------------------------------------------

        private Token Current()
        {
            return tokens[position];
        }

        // Returns the token at the given offset from the current position.
        // If the offset would run past the end, returns the EndOfFile sentinel.
        private Token Peek(int offset)
        {
            int targetIndex = position + offset;
            if (targetIndex < 0 || targetIndex >= tokens.Count)
            {
                return tokens[tokens.Count - 1];
            }
            return tokens[targetIndex];
        }

        private void Advance()
        {
            if (position < tokens.Count - 1)
            {
                position++;
            }
        }

        private bool Match(TokenType type)
        {
            if (Current().Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }

        private Token Expect(TokenType type, string what)
        {
            Token current = Current();
            if (current.Type != type)
            {
                throw new ParserException(
                    $"Expected {what} but found '{current.Value}' (token type {current.Type})",
                    current.Line);
            }
            Advance();
            return current;
        }

        private void ExpectEndOfFile()
        {
            Token current = Current();
            if (current.Type != TokenType.EndOfFile)
            {
                throw new ParserException(
                    $"Unexpected token '{current.Value}' (token type {current.Type}) at end of program",
                    current.Line);
            }
        }

        // Returns true when the given token type can never start a statement.
        // Used as the natural stop predicate for top-level statement lists.
        private static bool IsTopLevelTerminator(TokenType type)
        {
            return type == TokenType.EndOfFile;
        }

        // Block-ending tokens for statement lists inside if/while/for/etc.
        // The parser stops consuming statements when one of these appears.
        private static bool IsBlockTerminator(TokenType type)
        {
            return type == TokenType.End
                || type == TokenType.Else
                || type == TokenType.Catch
                || type == TokenType.Finally
                || type == TokenType.While    // for do-while body
                || type == TokenType.RightBrace
                || type == TokenType.Case     // for switch case bodies
                || type == TokenType.Default  // for switch default body
                || type == TokenType.EndOfFile;
        }

        // Skips any trailing semicolons (optional separator).
        // Per Note 21, a trailing ';' immediately before a block-ender is
        // technically a parse error; in practice the demo corpus omits ';'
        // entirely so we accept both shapes.
        private void SkipOptionalSemicolons()
        {
            while (Current().Type == TokenType.Semicolon)
            {
                Advance();
            }
        }

        // ---------------------------------------------------------------
        // Statement list
        // ---------------------------------------------------------------

        // Parses a sequence of statements, stopping when 'isTerminator' returns
        // true for the current token type. Optional semicolons may separate
        // statements. Per Implementation Notes 3 & 12, callers pass different
        // terminator predicates so the parser can stop naturally on tokens
        // such as 'while' (do-while body), 'case'/'default'/'}' (switch cases),
        // and 'else'/'end'/'catch'/'finally' (block bodies).
        private List<AstNode> ParseStatementList(Func<TokenType, bool> isTerminator)
        {
            List<AstNode> statements = new List<AstNode>();

            // Allow leading semicolons (a no-op for empty bodies).
            SkipOptionalSemicolons();

            while (!isTerminator(Current().Type))
            {
                AstNode statement = ParseStatement();
                statements.Add(statement);
                // Allow optional ';' between statements (and trailing ones).
                SkipOptionalSemicolons();
            }

            return statements;
        }

        // ---------------------------------------------------------------
        // Statements (dispatcher)
        // ---------------------------------------------------------------

        private AstNode ParseStatement()
        {
            Token current = Current();
            switch (current.Type)
            {
                case TokenType.Let:
                    return ParseLetDeclare();
                case TokenType.Var:
                    return ParseVarDeclare();
                case TokenType.Const:
                    return ParseConstDeclare();
                case TokenType.Enum:
                    return ParseEnumDef();
                case TokenType.If:
                    return ParseIfStatement();
                case TokenType.While:
                    return ParseWhileStatement();
                case TokenType.For:
                    return ParseForStatement();
                case TokenType.Foreach:
                    return ParseForeachStatement();
                case TokenType.Do:
                    return ParseDoWhileStatement();
                case TokenType.Switch:
                    return ParseSwitchStatement();
                case TokenType.Break:
                    Advance();
                    return new BreakStatementNode(current.Line);
                case TokenType.Continue:
                    Advance();
                    return new ContinueStatementNode(current.Line);
                case TokenType.Print:
                    return ParsePrintStatement();
                case TokenType.Input:
                    return ParseInputStatement();
                case TokenType.Function:
                    return ParseFunctionDef(false);
                case TokenType.Static:
                    return ParseStaticDef();
                case TokenType.Return:
                    return ParseReturnStatement();
                case TokenType.Class:
                    return ParseClassDef(false);
                case TokenType.Module:
                    return ParseModuleDef();
                case TokenType.Import:
                    return ParseImportStatement();
                case TokenType.Export:
                    return ParseExportStatement();
                case TokenType.Try:
                    return ParseTryStatement();
                case TokenType.Throw:
                    return ParseThrowStatement();
                case TokenType.Match:
                    return ParsePatternMatch();
                case TokenType.AtSign:
                    return ParseAnnotatedStatement();
                case TokenType.Identifier:
                case TokenType.This:
                    return ParseAssignOrCall();
                default:
                    throw new ParserException(
                        $"Unexpected token '{current.Value}' (token type {current.Type}) at start of statement",
                        current.Line);
            }
        }

        // ---------------------------------------------------------------
        // Declarations: let / var / const / enum
        // ---------------------------------------------------------------

        private LetDeclareNode ParseLetDeclare()
        {
            Token letTok = Expect(TokenType.Let, "'let'");
            Token nameTok = Expect(TokenType.Identifier, "identifier after 'let'");
            TypeNode declaredType = null;
            if (Match(TokenType.Colon))
            {
                declaredType = ParseType();
            }
            Expect(TokenType.Assign, "':=' in let declaration");
            AstNode initExpr = ParseExpression();
            return new LetDeclareNode(letTok.Line, nameTok.Value, declaredType, initExpr);
        }

        private VarDeclareNode ParseVarDeclare()
        {
            Token varTok = Expect(TokenType.Var, "'var'");
            Token nameTok = Expect(TokenType.Identifier, "identifier after 'var'");
            Expect(TokenType.Colon, "':' (var requires a type annotation)");
            TypeNode declaredType = ParseType();
            Expect(TokenType.Assign, "':=' in var declaration");
            AstNode initExpr = ParseExpression();
            return new VarDeclareNode(varTok.Line, nameTok.Value, declaredType, initExpr);
        }

        private AstNode ParseConstDeclare()
        {
            Token constTok = Expect(TokenType.Const, "'const'");
            Token nameTok = Expect(TokenType.Identifier, "identifier after 'const'");
            TypeNode declaredType = null;
            if (Match(TokenType.Colon))
            {
                declaredType = ParseType();
            }
            Expect(TokenType.Assign, "':=' in const declaration");
            AstNode initExpr = ParseExpression();
            return new ConstDeclareNode(constTok.Line, nameTok.Value, declaredType, initExpr);
        }

        private EnumDefNode ParseEnumDef()
        {
            Token enumTok = Expect(TokenType.Enum, "'enum'");
            Token nameTok = Expect(TokenType.Identifier, "enum name");
            Expect(TokenType.LeftBrace, "'{' to begin enum body");
            List<EnumValueNode> values = new List<EnumValueNode>();
            if (Current().Type != TokenType.RightBrace)
            {
                values.Add(ParseEnumValue());
                while (Match(TokenType.Comma))
                {
                    values.Add(ParseEnumValue());
                }
            }
            Expect(TokenType.RightBrace, "'}' to close enum body");
            return new EnumDefNode(enumTok.Line, nameTok.Value, values);
        }

        private EnumValueNode ParseEnumValue()
        {
            Token nameTok = Expect(TokenType.Identifier, "enum member name");
            AstNode value = null;
            if (Match(TokenType.SingleEqual))
            {
                value = ParseExpression();
            }
            return new EnumValueNode(nameTok.Line, nameTok.Value, value);
        }

        // ---------------------------------------------------------------
        // Control flow
        // ---------------------------------------------------------------

        private IfStatementNode ParseIfStatement()
        {
            Token ifTok = Expect(TokenType.If, "'if'");
            AstNode condition = ParseExpression();
            Expect(TokenType.Then, "'then' after if condition");
            List<AstNode> thenBody = ParseStatementList(t =>
                t == TokenType.Else || t == TokenType.End || t == TokenType.EndOfFile);
            List<AstNode> elseBody = new List<AstNode>();
            if (Match(TokenType.Else))
            {
                elseBody = ParseStatementList(t =>
                    t == TokenType.End || t == TokenType.EndOfFile);
            }
            Expect(TokenType.End, "'end' to close if statement");
            return new IfStatementNode(ifTok.Line, condition, thenBody, elseBody);
        }

        private WhileStatementNode ParseWhileStatement()
        {
            Token whileTok = Expect(TokenType.While, "'while'");
            AstNode condition = ParseExpression();
            Expect(TokenType.Do, "'do' after while condition");
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.End || t == TokenType.EndOfFile);
            Expect(TokenType.End, "'end' to close while loop");
            return new WhileStatementNode(whileTok.Line, condition, body);
        }

        private ForStatementNode ParseForStatement()
        {
            Token forTok = Expect(TokenType.For, "'for'");
            Token loopVar = Expect(TokenType.Identifier, "for-loop variable");
            Expect(TokenType.Assign, "':=' after for-loop variable");
            AstNode startExpr = ParseExpression();
            Expect(TokenType.To, "'to' in for loop");
            AstNode endExpr = ParseExpression();
            AstNode stepExpr = null;
            if (Match(TokenType.Step))
            {
                stepExpr = ParseExpression();
            }
            Expect(TokenType.Do, "'do' in for loop");
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.End || t == TokenType.EndOfFile);
            Expect(TokenType.End, "'end' to close for loop");
            return new ForStatementNode(forTok.Line, loopVar.Value, startExpr, endExpr, stepExpr, body);
        }

        private ForeachStatementNode ParseForeachStatement()
        {
            Token foreachTok = Expect(TokenType.Foreach, "'foreach'");
            Token loopVar = Expect(TokenType.Identifier, "foreach loop variable");
            Expect(TokenType.In, "'in' in foreach loop");
            AstNode iterableExpr = ParseExpression();
            Expect(TokenType.Do, "'do' in foreach loop");
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.End || t == TokenType.EndOfFile);
            Expect(TokenType.End, "'end' to close foreach loop");
            return new ForeachStatementNode(foreachTok.Line, loopVar.Value, iterableExpr, body);
        }

        private DoWhileStatementNode ParseDoWhileStatement()
        {
            Token doTok = Expect(TokenType.Do, "'do'");
            // Per Note 3, the body stops at 'while'.
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.While || t == TokenType.EndOfFile);
            Expect(TokenType.While, "'while' to terminate do-while body");
            AstNode condition = ParseExpression();
            return new DoWhileStatementNode(doTok.Line, body, condition);
        }

        private SwitchStatementNode ParseSwitchStatement()
        {
            Token switchTok = Expect(TokenType.Switch, "'switch'");
            AstNode switchExpr = ParseExpression();
            Expect(TokenType.LeftBrace, "'{' to begin switch body");
            List<AstNode> cases = new List<AstNode>();
            // Per Build.Solution.md, "switch x { }" with no cases is a parse
            // error; at least one case or default is required.
            while (Current().Type != TokenType.RightBrace)
            {
                Token caseStart = Current();
                if (caseStart.Type == TokenType.Case)
                {
                    Advance();
                    AstNode matchExpr = ParseExpression();
                    Expect(TokenType.Colon, "':' after case expression");
                    List<AstNode> caseBody = ParseStatementList(t =>
                        t == TokenType.Case || t == TokenType.Default
                        || t == TokenType.RightBrace || t == TokenType.EndOfFile);
                    cases.Add(new CaseClauseNode(caseStart.Line, matchExpr, caseBody));
                }
                else if (caseStart.Type == TokenType.Default)
                {
                    Advance();
                    Expect(TokenType.Colon, "':' after 'default'");
                    List<AstNode> defaultBody = ParseStatementList(t =>
                        t == TokenType.Case || t == TokenType.Default
                        || t == TokenType.RightBrace || t == TokenType.EndOfFile);
                    cases.Add(new DefaultClauseNode(caseStart.Line, defaultBody));
                }
                else
                {
                    throw new ParserException(
                        $"Expected 'case' or 'default' inside switch but found '{caseStart.Value}'",
                        caseStart.Line);
                }
            }
            if (cases.Count == 0)
            {
                throw new ParserException(
                    "switch statement must contain at least one 'case' or 'default'",
                    switchTok.Line);
            }
            Expect(TokenType.RightBrace, "'}' to close switch body");
            return new SwitchStatementNode(switchTok.Line, switchExpr, cases);
        }

        // ---------------------------------------------------------------
        // I/O statements
        // ---------------------------------------------------------------

        private PrintStatementNode ParsePrintStatement()
        {
            Token printTok = Expect(TokenType.Print, "'print'");
            AstNode value = ParseExpression();
            return new PrintStatementNode(printTok.Line, value);
        }

        private InputStatementNode ParseInputStatement()
        {
            Token inputTok = Expect(TokenType.Input, "'input'");
            Token nameTok = Expect(TokenType.Identifier, "identifier after 'input'");
            return new InputStatementNode(inputTok.Line, nameTok.Value);
        }

        // ---------------------------------------------------------------
        // Functions
        // ---------------------------------------------------------------

        private FunctionDefNode ParseFunctionDef(bool requireName)
        {
            // Defensive: callers may dispatch here when the current token is
            // 'function'. requireName is true for top-level definitions where
            // an identifier must follow, false otherwise.
            Token fnTok = Expect(TokenType.Function, "'function'");
            Token nameTok = Expect(TokenType.Identifier, "function name");
            List<ParameterNode> parameters = ParseParameterListWithParens();
            TypeNode returnType = null;
            if (Match(TokenType.Arrow))
            {
                returnType = ParseType();
            }
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.End || t == TokenType.EndOfFile);
            Expect(TokenType.End, "'end' to close function body");
            return new FunctionDefNode(fnTok.Line, nameTok.Value, parameters, returnType, body);
        }

        // Handles 'static' as a statement-level prefix: must precede 'function' or 'class'.
        private AstNode ParseStaticDef()
        {
            Token staticTok = Expect(TokenType.Static, "'static'");
            Token next = Current();
            if (next.Type == TokenType.Function)
            {
                // static at the top-level still produces a method-shaped node.
                return ParseMethodDef(staticTok.Line, true);
            }
            if (next.Type == TokenType.Class)
            {
                return ParseClassDef(true);
            }
            throw new ParserException(
                $"'static' must be followed by 'function' or 'class' but found '{next.Value}'",
                next.Line);
        }

        private MethodDefNode ParseMethodDef(int line, bool isStatic)
        {
            Expect(TokenType.Function, "'function' in method definition");
            Token nameTok = Expect(TokenType.Identifier, "method name");
            List<ParameterNode> parameters = ParseParameterListWithParens();
            TypeNode returnType = null;
            if (Match(TokenType.Arrow))
            {
                returnType = ParseType();
            }
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.End || t == TokenType.EndOfFile);
            Expect(TokenType.End, "'end' to close method body");
            return new MethodDefNode(line, nameTok.Value, parameters, returnType, body, isStatic);
        }

        private List<ParameterNode> ParseParameterListWithParens()
        {
            Expect(TokenType.LeftParen, "'(' to begin parameter list");
            List<ParameterNode> result = new List<ParameterNode>();
            if (Current().Type != TokenType.RightParen)
            {
                result.Add(ParseParameter());
                while (Match(TokenType.Comma))
                {
                    result.Add(ParseParameter());
                }
            }
            Expect(TokenType.RightParen, "')' to close parameter list");
            return result;
        }

        private ParameterNode ParseParameter()
        {
            Token nameTok = Expect(TokenType.Identifier, "parameter name");
            TypeNode paramType = null;
            AstNode defaultExpr = null;
            if (Match(TokenType.Colon))
            {
                paramType = ParseType();
            }
            if (Match(TokenType.Assign))
            {
                defaultExpr = ParseExpression();
            }
            return new ParameterNode(nameTok.Line, nameTok.Value, paramType, defaultExpr);
        }

        private ReturnStatementNode ParseReturnStatement()
        {
            Token returnTok = Expect(TokenType.Return, "'return'");
            // Note 4: bare 'return' before a block terminator, ';', or EOF
            // returns null. Do NOT unconditionally demand an expression.
            AstNode value = null;
            TokenType nextType = Current().Type;
            if (!IsBlockTerminator(nextType) && nextType != TokenType.Semicolon)
            {
                value = ParseExpression();
            }
            return new ReturnStatementNode(returnTok.Line, value);
        }

        // ---------------------------------------------------------------
        // Classes
        // ---------------------------------------------------------------

        private ClassDefNode ParseClassDef(bool isStatic)
        {
            Token classTok = Expect(TokenType.Class, "'class'");
            Token nameTok = Expect(TokenType.Identifier, "class name");
            string extendsId = null;
            if (Match(TokenType.Extends))
            {
                Token superTok = Expect(TokenType.Identifier, "superclass name after 'extends'");
                extendsId = superTok.Value;
            }
            List<string> implementsIds = new List<string>();
            if (Match(TokenType.Implements))
            {
                Token first = Expect(TokenType.Identifier, "interface name after 'implements'");
                implementsIds.Add(first.Value);
                while (Match(TokenType.Comma))
                {
                    Token next = Expect(TokenType.Identifier, "interface name after ','");
                    implementsIds.Add(next.Value);
                }
            }
            Expect(TokenType.LeftBrace, "'{' to begin class body");
            List<AstNode> members = new List<AstNode>();
            while (Current().Type != TokenType.RightBrace
                && Current().Type != TokenType.EndOfFile)
            {
                members.Add(ParseClassMember());
                SkipOptionalSemicolons();
            }
            Expect(TokenType.RightBrace, "'}' to close class body");
            return new ClassDefNode(classTok.Line, nameTok.Value, extendsId, implementsIds, members, isStatic);
        }

        private AstNode ParseClassMember()
        {
            Token current = Current();
            switch (current.Type)
            {
                case TokenType.AtSign:
                    {
                        AnnotationNode annotation = ParseAnnotation();
                        AstNode wrapped = ParseClassMember();
                        return new AnnotatedStatementNode(annotation.Line, annotation, wrapped);
                    }
                case TokenType.Static:
                    {
                        Advance();
                        if (Current().Type != TokenType.Function)
                        {
                            throw new ParserException(
                                "'static' inside a class body must precede 'function'",
                                current.Line);
                        }
                        return ParseMethodDef(current.Line, true);
                    }
                case TokenType.Function:
                    return ParseMethodDef(current.Line, false);
                case TokenType.Constructor:
                    return ParseConstructorDef();
                case TokenType.Let:
                    return ParseFieldDeclare(FieldKind.Let);
                case TokenType.Var:
                    return ParseFieldDeclare(FieldKind.Var);
                case TokenType.Const:
                    return ParseFieldDeclareOrConst();
                case TokenType.Enum:
                    return ParseEnumDef();
                default:
                    throw new ParserException(
                        $"Unexpected token '{current.Value}' inside class body",
                        current.Line);
            }
        }

        private ConstructorDefNode ParseConstructorDef()
        {
            Token ctorTok = Expect(TokenType.Constructor, "'Constructor'");
            List<ParameterNode> parameters = ParseParameterListWithParens();
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.End || t == TokenType.EndOfFile);
            Expect(TokenType.End, "'end' to close constructor body");
            return new ConstructorDefNode(ctorTok.Line, parameters, body);
        }

        private FieldDeclareNode ParseFieldDeclare(FieldKind kind)
        {
            Token kwTok = Current();
            Advance(); // consume let/var
            Token nameTok = Expect(TokenType.Identifier, "field name");
            TypeNode declaredType = null;
            if (Match(TokenType.Colon))
            {
                declaredType = ParseType();
            }
            else if (kind == FieldKind.Var)
            {
                throw new ParserException(
                    "'var' field requires a type annotation",
                    kwTok.Line);
            }
            Expect(TokenType.Assign, "':=' in field declaration");
            AstNode initExpr = ParseExpression();
            return new FieldDeclareNode(kwTok.Line, kind, nameTok.Value, declaredType, initExpr);
        }

        // 'const' inside a class body may be either a const field declaration
        // or an enum-style const declaration (currently the spec routes the
        // enum form through ParseEnumDef directly).
        private FieldDeclareNode ParseFieldDeclareOrConst()
        {
            return ParseFieldDeclare(FieldKind.Const);
        }

        // ---------------------------------------------------------------
        // Module system
        // ---------------------------------------------------------------

        private ModuleDefNode ParseModuleDef()
        {
            Token modTok = Expect(TokenType.Module, "'module'");
            Token nameTok = Expect(TokenType.Identifier, "module name");
            List<ModuleImportNode> imports = new List<ModuleImportNode>();
            if (Match(TokenType.Import))
            {
                imports.Add(ParseModuleImport());
                while (Match(TokenType.Comma))
                {
                    imports.Add(ParseModuleImport());
                }
            }
            Expect(TokenType.LeftBrace, "'{' to begin module body");
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.RightBrace || t == TokenType.EndOfFile);
            Expect(TokenType.RightBrace, "'}' to close module body");
            return new ModuleDefNode(modTok.Line, nameTok.Value, imports, body);
        }

        private ModuleImportNode ParseModuleImport()
        {
            Token nameTok = Expect(TokenType.Identifier, "imported module name");
            string alias = null;
            if (Match(TokenType.As))
            {
                Token aliasTok = Expect(TokenType.Identifier, "alias after 'as'");
                alias = aliasTok.Value;
            }
            return new ModuleImportNode(nameTok.Line, nameTok.Value, alias);
        }

        private ImportStatementNode ParseImportStatement()
        {
            Token importTok = Expect(TokenType.Import, "'import'");
            Token nameTok = Expect(TokenType.Identifier, "module name in import");
            string alias = null;
            if (Match(TokenType.As))
            {
                Token aliasTok = Expect(TokenType.Identifier, "alias after 'as'");
                alias = aliasTok.Value;
            }
            return new ImportStatementNode(importTok.Line, nameTok.Value, alias);
        }

        private ExportStatementNode ParseExportStatement()
        {
            Token exportTok = Expect(TokenType.Export, "'export'");
            Token nameTok = Expect(TokenType.Identifier, "name in export");
            return new ExportStatementNode(exportTok.Line, nameTok.Value);
        }

        // ---------------------------------------------------------------
        // Exception handling
        // ---------------------------------------------------------------

        private TryStatementNode ParseTryStatement()
        {
            Token tryTok = Expect(TokenType.Try, "'try'");
            List<AstNode> tryBody = ParseStatementList(t =>
                t == TokenType.Catch || t == TokenType.Finally
                || t == TokenType.End || t == TokenType.EndOfFile);
            CatchClauseNode catchClause = ParseCatchClause();
            List<AstNode> finallyBody = new List<AstNode>();
            if (Match(TokenType.Finally))
            {
                finallyBody = ParseStatementList(t =>
                    t == TokenType.End || t == TokenType.EndOfFile);
            }
            Expect(TokenType.End, "'end' to close try statement");
            return new TryStatementNode(tryTok.Line, tryBody, catchClause, finallyBody);
        }

        private CatchClauseNode ParseCatchClause()
        {
            Token catchTok = Expect(TokenType.Catch, "'catch' clause");
            string id;
            TypeNode exType = null;
            if (Match(TokenType.LeftParen))
            {
                Token nameTok = Expect(TokenType.Identifier, "catch variable name");
                Expect(TokenType.Colon, "':' after catch variable");
                exType = ParseType();
                Expect(TokenType.RightParen, "')' to close catch declaration");
                id = nameTok.Value;
            }
            else
            {
                Token nameTok = Expect(TokenType.Identifier, "catch variable name");
                id = nameTok.Value;
            }
            List<AstNode> body = ParseStatementList(t =>
                t == TokenType.Finally || t == TokenType.End
                || t == TokenType.EndOfFile);
            return new CatchClauseNode(catchTok.Line, id, exType, body);
        }

        private ThrowStatementNode ParseThrowStatement()
        {
            Token throwTok = Expect(TokenType.Throw, "'throw'");
            AstNode value = ParseExpression();
            return new ThrowStatementNode(throwTok.Line, value);
        }

        // ---------------------------------------------------------------
        // Pattern matching
        // ---------------------------------------------------------------

        private PatternMatchNode ParsePatternMatch()
        {
            Token matchTok = Expect(TokenType.Match, "'match'");
            AstNode value = ParseExpression();
            Expect(TokenType.LeftBrace, "'{' to begin match body");
            List<PatternCaseNode> cases = new List<PatternCaseNode>();
            while (Current().Type != TokenType.RightBrace
                && Current().Type != TokenType.EndOfFile)
            {
                cases.Add(ParsePatternCase());
                SkipOptionalSemicolons();
            }
            Expect(TokenType.RightBrace, "'}' to close match body");
            return new PatternMatchNode(matchTok.Line, value, cases);
        }

        private PatternCaseNode ParsePatternCase()
        {
            int caseLine = Current().Line;
            PatternNode pattern = ParsePattern();
            AstNode guard = null;
            if (Match(TokenType.When))
            {
                guard = ParseExpression();
            }
            Expect(TokenType.FatArrow, "'=>' after pattern");
            // Per Note 3, a pattern-case body stops at the next case (next
            // pattern-starting token) or the closing '}'. Patterns can begin
            // with many token types, so we use a heuristic that matches the
            // demo style: a single-statement body terminated by either '}'
            // or the start of the next pattern (newline-only, no '=>' until
            // that next pattern). The simplest reliable approach is: parse
            // exactly one statement, then let the outer loop pick up the
            // next case. This matches the observed demo pattern of one
            // statement per case (e.g. "1 => print \"one\"").
            //
            // For robustness with potentially-multi-statement bodies, we
            // also collect additional statements as long as we see a '{'
            // open-brace was used to enclose them. Since the BNF doesn't
            // require braces around case bodies, we keep this simple.
            List<AstNode> body = new List<AstNode>();
            body.Add(ParseStatement());
            return new PatternCaseNode(caseLine, pattern, guard, body);
        }

        // Parses a pattern and resolves left-associative '|' alternation.
        private PatternNode ParsePattern()
        {
            PatternNode left = ParsePrimaryPattern();
            while (Current().Type == TokenType.Pipe)
            {
                Token pipeTok = Current();
                Advance();
                PatternNode right = ParsePrimaryPattern();
                left = new AlternationPatternNode(pipeTok.Line, left, right);
            }
            return left;
        }

        private PatternNode ParsePrimaryPattern()
        {
            Token tok = Current();
            switch (tok.Type)
            {
                case TokenType.Underscore:
                    Advance();
                    return new WildcardPatternNode(tok.Line);
                case TokenType.IntegerLiteral:
                case TokenType.FloatLiteral:
                case TokenType.BinaryLiteral:
                case TokenType.OctalLiteral:
                case TokenType.HexLiteral:
                    {
                        AstNode literal = ParseNumberLiteral();
                        return new LiteralPatternNode(tok.Line, literal);
                    }
                case TokenType.Minus:
                    {
                        // Allow negative numeric pattern literals (e.g., `-1 => ...`).
                        Advance();
                        Token numTok = Current();
                        AstNode numNode;
                        switch (numTok.Type)
                        {
                            case TokenType.IntegerLiteral:
                            case TokenType.FloatLiteral:
                            case TokenType.BinaryLiteral:
                            case TokenType.OctalLiteral:
                            case TokenType.HexLiteral:
                                numNode = ParseNumberLiteral();
                                break;
                            default:
                                throw new ParserException(
                                    $"Expected numeric literal after '-' in pattern but found '{numTok.Value}'",
                                    numTok.Line);
                        }
                        AstNode negated = new UnaryOpNode(tok.Line, "-", numNode);
                        return new LiteralPatternNode(tok.Line, negated);
                    }
                case TokenType.StringLiteral:
                    Advance();
                    return new LiteralPatternNode(tok.Line, BuildStringLiteralNode(tok));
                case TokenType.True:
                    Advance();
                    return new LiteralPatternNode(tok.Line, new BoolLiteralNode(tok.Line, true));
                case TokenType.False:
                    Advance();
                    return new LiteralPatternNode(tok.Line, new BoolLiteralNode(tok.Line, false));
                case TokenType.Null:
                    Advance();
                    return new LiteralPatternNode(tok.Line, new NullLiteralNode(tok.Line));
                case TokenType.LeftBracket:
                    {
                        Advance();
                        List<PatternNode> elems = new List<PatternNode>();
                        if (Current().Type != TokenType.RightBracket)
                        {
                            elems.Add(ParsePattern());
                            while (Match(TokenType.Comma))
                            {
                                elems.Add(ParsePattern());
                            }
                        }
                        Expect(TokenType.RightBracket, "']' to close array pattern");
                        return new ArrayPatternNode(tok.Line, elems);
                    }
                case TokenType.Identifier:
                    {
                        Advance();
                        // Constructor pattern: id "(" patterns? ")"
                        if (Current().Type == TokenType.LeftParen)
                        {
                            Advance();
                            List<PatternNode> args = new List<PatternNode>();
                            if (Current().Type != TokenType.RightParen)
                            {
                                args.Add(ParsePattern());
                                while (Match(TokenType.Comma))
                                {
                                    args.Add(ParsePattern());
                                }
                            }
                            Expect(TokenType.RightParen, "')' to close constructor pattern");
                            return new ConstructorPatternNode(tok.Line, tok.Value, args);
                        }
                        // Field pattern: id "{" pairs? "}"
                        if (Current().Type == TokenType.LeftBrace)
                        {
                            Advance();
                            List<FieldPatternPair> pairs = new List<FieldPatternPair>();
                            if (Current().Type != TokenType.RightBrace)
                            {
                                pairs.Add(ParseFieldPatternPair());
                                while (Match(TokenType.Comma))
                                {
                                    pairs.Add(ParseFieldPatternPair());
                                }
                            }
                            Expect(TokenType.RightBrace, "'}' to close field pattern");
                            return new FieldPatternNode(tok.Line, tok.Value, pairs);
                        }
                        // Plain identifier pattern: a binding name.
                        return new IdentifierPatternNode(tok.Line, tok.Value);
                    }
                default:
                    throw new ParserException(
                        $"Unexpected token '{tok.Value}' at start of pattern",
                        tok.Line);
            }
        }

        private FieldPatternPair ParseFieldPatternPair()
        {
            Token nameTok = Expect(TokenType.Identifier, "field name in field pattern");
            Expect(TokenType.Colon, "':' after field name in field pattern");
            PatternNode subPattern = ParsePattern();
            return new FieldPatternPair(nameTok.Value, subPattern);
        }

        // ---------------------------------------------------------------
        // Annotations
        // ---------------------------------------------------------------

        private AnnotatedStatementNode ParseAnnotatedStatement()
        {
            AnnotationNode annotation = ParseAnnotation();
            AstNode wrapped = ParseStatement();
            return new AnnotatedStatementNode(annotation.Line, annotation, wrapped);
        }

        private AnnotationNode ParseAnnotation()
        {
            Token atTok = Expect(TokenType.AtSign, "'@' to start annotation");
            Token nameTok = Expect(TokenType.Identifier, "annotation name");
            List<AnnotationParamNode> parameters = new List<AnnotationParamNode>();
            if (Match(TokenType.LeftParen))
            {
                if (Current().Type != TokenType.RightParen)
                {
                    parameters.Add(ParseAnnotationParam());
                    while (Match(TokenType.Comma))
                    {
                        parameters.Add(ParseAnnotationParam());
                    }
                }
                Expect(TokenType.RightParen, "')' to close annotation parameter list");
            }
            return new AnnotationNode(atTok.Line, nameTok.Value, parameters);
        }

        private AnnotationParamNode ParseAnnotationParam()
        {
            Token nameTok = Expect(TokenType.Identifier, "annotation parameter name");
            Expect(TokenType.SingleEqual, "'=' in annotation parameter (note: '=', not ':=')");
            AstNode value = ParseExpression();
            return new AnnotationParamNode(nameTok.Line, nameTok.Value, value);
        }

        // ---------------------------------------------------------------
        // Assignment / call statement
        // ---------------------------------------------------------------

        // Begins with an Identifier (or 'this') and decides whether this is:
        //   - <assign_stmt>:        id ":=" expr
        //   - <array_assign_stmt>:  id "[" expr "]" ":=" expr
        //   - <call_stmt>:          id { "." id } "(" args? ")"
        //   - chained assignment:   target.member := expr
        //                           target[i][j]... := expr (any postfix LHS)
        //
        // We always parse a full postfix expression first, then if ':=' follows
        // we classify the LHS shape and produce the appropriate node.
        private AstNode ParseAssignOrCall()
        {
            Token startTok = Current();
            int startLine = startTok.Line;

            AstNode lhs = ParsePostfixExpression();
            Token afterLhs = Current();

            if (afterLhs.Type == TokenType.Assign)
            {
                Advance(); // consume :=
                AstNode rhs = ParseExpression();

                // Recognise mutations on identifiers, member chains, and
                // indexed targets (including nested indices like arr[i][j]).
                if (lhs is IdentifierNode identNode)
                {
                    return new AssignStatementNode(startLine, identNode.Name, rhs);
                }
                if (lhs is IndexAccessNode indexAccess)
                {
                    // Special-case the simple <id>[expr] := form to match the
                    // BNF's <array_assign_stmt> production exactly.
                    if (indexAccess.Target is IdentifierNode targetIdent)
                    {
                        return new ArrayElementAssignNode(
                            startLine,
                            targetIdent.Name,
                            indexAccess.IndexExpr,
                            rhs);
                    }
                    return new IndexedAssignNode(startLine, indexAccess, rhs);
                }
                if (lhs is MemberAccessNode memberAccess)
                {
                    return new MemberAssignNode(startLine, memberAccess, rhs);
                }
                throw new ParserException(
                    "Left side of ':=' must be a variable, field access, or array element",
                    startLine);
            }

            // Plain call statement form. Accepts:
            //   - FunctionCallNode (foo(args))
            //   - MethodCallNode (obj.method(args))
            //   - Chains thereof
            if (lhs is FunctionCallNode || lhs is MethodCallNode)
            {
                List<string> targetIds;
                List<AstNode> args;
                if (TryFlattenCall(lhs, out targetIds, out args))
                {
                    return new CallStatementNode(startLine, targetIds, args);
                }
                // Fall back to wrapping as expression-statement (rare case
                // such as obj[i].method(args) — the BNF disallows this as a
                // statement but the demos may carry such usages; we keep
                // the parsed shape so the interpreter can decide).
                return new ExpressionStatementNode(startLine, lhs);
            }

            throw new ParserException(
                $"Statement starting with '{startTok.Value}' must be an assignment, call, or array assignment",
                startLine);
        }

        // Flattens a chain of MemberAccess + final FunctionCall (or MethodCall)
        // back into the (TargetIds, Args) shape used by CallStatementNode.
        // Returns true if the chain is a pure id.id.id(...) form.
        private bool TryFlattenCall(AstNode node, out List<string> targetIds, out List<AstNode> args)
        {
            if (node is FunctionCallNode functionCall)
            {
                args = new List<AstNode>(functionCall.Args);
                List<string> ids = new List<string>();
                if (TryFlattenIdChain(functionCall.CalleeExpr, ids))
                {
                    targetIds = ids;
                    return true;
                }
                targetIds = null;
                return false;
            }
            if (node is MethodCallNode methodCall)
            {
                args = new List<AstNode>(methodCall.Args);
                List<string> ids = new List<string>();
                if (TryFlattenIdChain(methodCall.Target, ids))
                {
                    ids.Add(methodCall.MethodId);
                    targetIds = ids;
                    return true;
                }
                targetIds = null;
                return false;
            }
            targetIds = null;
            args = null;
            return false;
        }

        private bool TryFlattenIdChain(AstNode node, List<string> outIds)
        {
            if (node is IdentifierNode ident)
            {
                outIds.Add(ident.Name);
                return true;
            }
            if (node is MemberAccessNode member)
            {
                if (TryFlattenIdChain(member.Target, outIds))
                {
                    outIds.Add(member.MemberId);
                    return true;
                }
            }
            return false;
        }

        // ---------------------------------------------------------------
        // Expressions
        // ---------------------------------------------------------------

        // Parses any expression at the lowest precedence (ternary).
        private AstNode ParseExpression()
        {
            // Note 12: in expression context, leading 'if' begins an inline
            // conditional expression, NOT a statement.
            if (Current().Type == TokenType.If)
            {
                return ParseConditionalExpression();
            }
            // Lambda expression: 'function' starts an anonymous function expr.
            if (Current().Type == TokenType.Function)
            {
                return ParseLambdaExpression();
            }
            // 'new' expression: new expressions live in primary, but the
            // common path is `:= new ClassName(args)`. Allowed here.
            // No special case — falls through to ParsePostfixExpression via
            // the precedence stack.

            AstNode condition = ParseOrExpression();

            if (Match(TokenType.Question))
            {
                AstNode thenExpr = ParseExpression();
                Expect(TokenType.Colon, "':' in ternary expression");
                AstNode elseExpr = ParseExpression();
                return new TernaryNode(condition.Line, condition, thenExpr, elseExpr);
            }
            return condition;
        }

        private ConditionalExprNode ParseConditionalExpression()
        {
            Token ifTok = Expect(TokenType.If, "'if'");
            AstNode condition = ParseExpression();
            Expect(TokenType.Then, "'then' in inline conditional expression");
            AstNode thenExpr = ParseExpression();
            Expect(TokenType.Else, "'else' in inline conditional expression");
            AstNode elseExpr = ParseExpression();
            return new ConditionalExprNode(ifTok.Line, condition, thenExpr, elseExpr);
        }

        private AstNode ParseOrExpression()
        {
            AstNode left = ParseAndExpression();
            while (Current().Type == TokenType.PipePipe || Current().Type == TokenType.Or)
            {
                Token opTok = Current();
                Advance();
                AstNode right = ParseAndExpression();
                left = new BinaryOpNode(opTok.Line, opTok.Value, left, right);
            }
            return left;
        }

        private AstNode ParseAndExpression()
        {
            AstNode left = ParseNotExpression();
            while (Current().Type == TokenType.AmpAmp || Current().Type == TokenType.And)
            {
                Token opTok = Current();
                Advance();
                AstNode right = ParseNotExpression();
                left = new BinaryOpNode(opTok.Line, opTok.Value, left, right);
            }
            return left;
        }

        private AstNode ParseNotExpression()
        {
            if (Current().Type == TokenType.Not)
            {
                Token opTok = Current();
                Advance();
                AstNode operand = ParseNotExpression();
                return new UnaryOpNode(opTok.Line, "not", operand);
            }
            return ParseComparisonExpression();
        }

        // Comparison and type-check/assert share precedence (Note 11).
        private AstNode ParseComparisonExpression()
        {
            AstNode left = ParseAdditiveExpression();
            Token opTok = Current();
            switch (opTok.Type)
            {
                case TokenType.EqualEqual:
                case TokenType.NotEqual:
                case TokenType.Less:
                case TokenType.Greater:
                case TokenType.LessEqual:
                case TokenType.GreaterEqual:
                    Advance();
                    AstNode right = ParseAdditiveExpression();
                    return new BinaryOpNode(opTok.Line, opTok.Value, left, right);
                case TokenType.Is:
                    Advance();
                    TypeNode checkType = ParseType();
                    return new TypeCheckNode(opTok.Line, left, checkType);
                case TokenType.As:
                    Advance();
                    TypeNode assertType = ParseType();
                    return new TypeAssertNode(opTok.Line, left, assertType);
                default:
                    return left;
            }
        }

        private AstNode ParseAdditiveExpression()
        {
            AstNode left = ParseMultiplicativeExpression();
            while (Current().Type == TokenType.Plus
                || Current().Type == TokenType.Minus
                || Current().Type == TokenType.Amp)
            {
                Token opTok = Current();
                Advance();
                AstNode right = ParseMultiplicativeExpression();
                left = new BinaryOpNode(opTok.Line, opTok.Value, left, right);
            }
            return left;
        }

        private AstNode ParseMultiplicativeExpression()
        {
            AstNode left = ParsePowerExpression();
            while (Current().Type == TokenType.Star
                || Current().Type == TokenType.Slash
                || Current().Type == TokenType.Percent
                || Current().Type == TokenType.SlashSlash)
            {
                Token opTok = Current();
                Advance();
                AstNode right = ParsePowerExpression();
                left = new BinaryOpNode(opTok.Line, opTok.Value, left, right);
            }
            return left;
        }

        // Right-associative — Note 11.
        private AstNode ParsePowerExpression()
        {
            AstNode left = ParseUnaryExpression();
            if (Current().Type == TokenType.StarStar)
            {
                Token opTok = Current();
                Advance();
                AstNode right = ParsePowerExpression();
                return new BinaryOpNode(opTok.Line, "**", left, right);
            }
            return left;
        }

        private AstNode ParseUnaryExpression()
        {
            if (Current().Type == TokenType.Minus)
            {
                Token opTok = Current();
                Advance();
                AstNode operand = ParseUnaryExpression();
                return new UnaryOpNode(opTok.Line, "-", operand);
            }
            return ParsePostfixExpression();
        }

        private AstNode ParsePostfixExpression()
        {
            AstNode result = ParsePrimary();
            while (true)
            {
                Token tok = Current();
                if (tok.Type == TokenType.Dot)
                {
                    Advance();
                    Token nameTok = Expect(TokenType.Identifier, "member name after '.'");
                    if (Current().Type == TokenType.LeftParen)
                    {
                        // Method call: <expr>.<id>(args?)
                        Advance(); // consume '('
                        List<AstNode> args = ParseArgumentList();
                        Expect(TokenType.RightParen, "')' to close method-call argument list");
                        result = new MethodCallNode(tok.Line, result, nameTok.Value, args);
                    }
                    else
                    {
                        result = new MemberAccessNode(tok.Line, result, nameTok.Value);
                    }
                }
                else if (tok.Type == TokenType.LeftBracket)
                {
                    Advance();
                    AstNode index = ParseExpression();
                    Expect(TokenType.RightBracket, "']' to close index expression");
                    result = new IndexAccessNode(tok.Line, result, index);
                }
                else if (tok.Type == TokenType.LeftParen)
                {
                    Advance();
                    List<AstNode> args = ParseArgumentList();
                    Expect(TokenType.RightParen, "')' to close call argument list");
                    result = new FunctionCallNode(tok.Line, result, args);
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private List<AstNode> ParseArgumentList()
        {
            List<AstNode> result = new List<AstNode>();
            if (Current().Type == TokenType.RightParen)
            {
                return result;
            }
            result.Add(ParseExpression());
            while (Match(TokenType.Comma))
            {
                result.Add(ParseExpression());
            }
            return result;
        }

        // Primary: id, number, string, boolean, null, parens (cast or grouped),
        // array literal, new-expr, conditional-expr, lambda-expr.
        private AstNode ParsePrimary()
        {
            Token tok = Current();
            switch (tok.Type)
            {
                case TokenType.IntegerLiteral:
                case TokenType.FloatLiteral:
                case TokenType.BinaryLiteral:
                case TokenType.OctalLiteral:
                case TokenType.HexLiteral:
                    return ParseNumberLiteral();
                case TokenType.StringLiteral:
                    Advance();
                    return BuildStringLiteralNode(tok);
                case TokenType.True:
                    Advance();
                    return new BoolLiteralNode(tok.Line, true);
                case TokenType.False:
                    Advance();
                    return new BoolLiteralNode(tok.Line, false);
                case TokenType.Null:
                    Advance();
                    return new NullLiteralNode(tok.Line);
                case TokenType.Identifier:
                    Advance();
                    return new IdentifierNode(tok.Line, tok.Value);
                case TokenType.This:
                    Advance();
                    return new IdentifierNode(tok.Line, "this");
                case TokenType.Int:
                case TokenType.Bool:
                case TokenType.String:
                case TokenType.Float:
                    // Built-in functions int(), bool(), str() etc. share their
                    // names with type keywords. When followed by '(' in
                    // expression position, treat as a callable identifier.
                    if (Peek(1).Type == TokenType.LeftParen)
                    {
                        Advance();
                        return new IdentifierNode(tok.Line, tok.Value);
                    }
                    throw new ParserException(
                        $"Unexpected type keyword '{tok.Value}' in expression position",
                        tok.Line);
                case TokenType.LeftParen:
                    return ParseParenOrCast();
                case TokenType.LeftBracket:
                    return ParseArrayLiteral();
                case TokenType.New:
                    return ParseNewExpression();
                case TokenType.If:
                    return ParseConditionalExpression();
                case TokenType.Function:
                    return ParseLambdaExpression();
                case TokenType.Print:
                    // 'print' is a keyword statement — but the spec note says
                    // print(x) is parsed as the 'print' keyword applied to (x).
                    // We treat 'print' as the start of a statement only; in
                    // expression position, it would be an error.
                    throw new ParserException(
                        "'print' is a statement keyword and cannot appear in expression position",
                        tok.Line);
                default:
                    throw new ParserException(
                        $"Unexpected token '{tok.Value}' (type {tok.Type}) in primary expression",
                        tok.Line);
            }
        }

        private AstNode ParseNumberLiteral()
        {
            Token tok = Current();
            Advance();
            switch (tok.Type)
            {
                case TokenType.IntegerLiteral:
                    {
                        long value = long.Parse(tok.Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
                        return new IntegerLiteralNode(tok.Line, value);
                    }
                case TokenType.FloatLiteral:
                    {
                        double value = double.Parse(tok.Value, NumberStyles.Float, CultureInfo.InvariantCulture);
                        return new FloatLiteralNode(tok.Line, value);
                    }
                case TokenType.BinaryLiteral:
                    {
                        // Strip "0b" prefix.
                        string digits = tok.Value.Substring(2);
                        long value = Convert.ToInt64(digits, 2);
                        return new IntegerLiteralNode(tok.Line, value);
                    }
                case TokenType.OctalLiteral:
                    {
                        string digits = tok.Value.Substring(2);
                        long value = Convert.ToInt64(digits, 8);
                        return new IntegerLiteralNode(tok.Line, value);
                    }
                case TokenType.HexLiteral:
                    {
                        string digits = tok.Value.Substring(2);
                        long value = Convert.ToInt64(digits, 16);
                        return new IntegerLiteralNode(tok.Line, value);
                    }
                default:
                    throw new ParserException(
                        $"Expected number literal but found '{tok.Value}'",
                        tok.Line);
            }
        }

        // Decode the lexer's preserved-form string (with surrounding quotes
        // and \-escapes) into the raw character sequence the AST stores.
        private static StringLiteralNode BuildStringLiteralNode(Token tok)
        {
            string raw = tok.Value;
            // Lexer always wraps string tokens in their original delimiter.
            if (raw.Length < 2)
            {
                return new StringLiteralNode(tok.Line, string.Empty);
            }
            // Strip surrounding delimiter quotes.
            string inner = raw.Substring(1, raw.Length - 2);
            // Resolve escape sequences.
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            int i = 0;
            while (i < inner.Length)
            {
                char ch = inner[i];
                if (ch == '\\' && i + 1 < inner.Length)
                {
                    char next = inner[i + 1];
                    switch (next)
                    {
                        case 'n':  builder.Append('\n'); break;
                        case 't':  builder.Append('\t'); break;
                        case '\\': builder.Append('\\'); break;
                        case '"':  builder.Append('"');  break;
                        case '\'': builder.Append('\''); break;
                        default:   builder.Append(next); break;
                    }
                    i += 2;
                }
                else
                {
                    builder.Append(ch);
                    i++;
                }
            }
            return new StringLiteralNode(tok.Line, builder.ToString());
        }

        // Disambiguates "(" between a cast and a grouped expression (Note 13).
        private AstNode ParseParenOrCast()
        {
            int openLine = Current().Line;
            Token openTok = Current();
            // We need to look ahead PAST the '(' to see if a type follows.
            Token peek1 = Peek(1);
            if (IsCastTypeStart(peek1.Type))
            {
                if (peek1.Type == TokenType.Identifier)
                {
                    // user-defined type — only a cast if followed immediately
                    // by ')'. (Per the spec, generic types like List<T> are
                    // allowed but very rarely appear in cast position; the
                    // demo corpus does not use them. We accept the simple
                    // form `(<id>)` only.)
                    if (Peek(2).Type != TokenType.RightParen)
                    {
                        // Not a cast — fall through to grouped expression.
                    }
                    else
                    {
                        Advance(); // consume '('
                        TypeNode targetType = ParseType();
                        Expect(TokenType.RightParen, "')' to close cast type");
                        AstNode operand = ParseUnaryExpression();
                        return new CastExprNode(openLine, targetType, operand);
                    }
                }
                else
                {
                    // Primitive type keyword — definitely a cast.
                    Advance(); // consume '('
                    TypeNode targetType = ParseType();
                    Expect(TokenType.RightParen, "')' to close cast type");
                    AstNode operand = ParseUnaryExpression();
                    return new CastExprNode(openLine, targetType, operand);
                }
            }

            // Grouped expression: consume '(', parse expr, consume ')'.
            Advance(); // consume '('
            AstNode expr = ParseExpression();
            Expect(TokenType.RightParen, "')' to close grouped expression");
            return expr;
        }

        // Tokens that may start a cast type per Note 13.
        private static bool IsCastTypeStart(TokenType type)
        {
            return type == TokenType.Int
                || type == TokenType.Float
                || type == TokenType.String
                || type == TokenType.Bool
                || type == TokenType.Array
                || type == TokenType.Object
                || type == TokenType.Null
                || type == TokenType.Void
                || type == TokenType.Map
                || type == TokenType.Identifier;
        }

        private AstNode ParseArrayLiteral()
        {
            Token openTok = Expect(TokenType.LeftBracket, "'['");
            // Empty array.
            if (Current().Type == TokenType.RightBracket)
            {
                Advance();
                return new ArrayLiteralNode(openTok.Line, new List<AstNode>());
            }
            AstNode firstExpr = ParseExpression();
            // List comprehension form: [expr for id in expr]
            if (Current().Type == TokenType.For)
            {
                Advance();
                Token iterVar = Expect(TokenType.Identifier, "iteration variable in list comprehension");
                Expect(TokenType.In, "'in' in list comprehension");
                AstNode iterable = ParseExpression();
                Expect(TokenType.RightBracket, "']' to close list comprehension");
                return new ListComprehensionNode(openTok.Line, firstExpr, iterVar.Value, iterable);
            }
            // Regular array literal.
            List<AstNode> elements = new List<AstNode>();
            elements.Add(firstExpr);
            while (Match(TokenType.Comma))
            {
                elements.Add(ParseExpression());
            }
            Expect(TokenType.RightBracket, "']' to close array literal");
            return new ArrayLiteralNode(openTok.Line, elements);
        }

        private NewExprNode ParseNewExpression()
        {
            Token newTok = Expect(TokenType.New, "'new'");
            Token classTok = Expect(TokenType.Identifier, "class name after 'new'");
            Expect(TokenType.LeftParen, "'(' in new expression");
            List<AstNode> args = ParseArgumentList();
            Expect(TokenType.RightParen, "')' to close new-expression argument list");
            return new NewExprNode(newTok.Line, classTok.Value, args);
        }

        // Lambda: function (params) [-> type] (expr | stmt-list end)
        // Note 14 disambiguation: scan ahead for matching 'end' at the same
        // nesting depth. If found, block form; else expression form.
        private LambdaExprNode ParseLambdaExpression()
        {
            Token fnTok = Expect(TokenType.Function, "'function' for lambda");
            List<ParameterNode> parameters = ParseParameterListWithParens();
            TypeNode returnType = null;
            if (Match(TokenType.Arrow))
            {
                returnType = ParseType();
            }
            // Decide block vs expression body.
            // Per Note 14, if the first body token starts a statement
            // (one that cannot also start an expression), use block form;
            // otherwise default to expression body. Pure 'end' lookahead is
            // unreliable because outer scopes also use 'end' — counting up
            // from depth=1 misattributes the outer 'end' to the lambda.
            bool blockForm = StartsBlockBody(Current().Type);
            if (blockForm)
            {
                List<AstNode> body = ParseStatementList(t =>
                    t == TokenType.End || t == TokenType.EndOfFile);
                Expect(TokenType.End, "'end' to close lambda body");
                return new LambdaExprNode(fnTok.Line, parameters, returnType, null, body, true);
            }
            AstNode bodyExpr = ParseExpression();
            return new LambdaExprNode(fnTok.Line, parameters, returnType, bodyExpr, new List<AstNode>(), false);
        }

        // Returns true if the given token type can ONLY start a statement,
        // not an expression. Used to identify lambda block-body form.
        private static bool StartsBlockBody(TokenType type)
        {
            switch (type)
            {
                case TokenType.Let:
                case TokenType.Var:
                case TokenType.Const:
                case TokenType.While:
                case TokenType.For:
                case TokenType.Foreach:
                case TokenType.Do:
                case TokenType.Switch:
                case TokenType.Try:
                case TokenType.Throw:
                case TokenType.Break:
                case TokenType.Continue:
                case TokenType.Match:
                case TokenType.Print:
                case TokenType.Input:
                case TokenType.Return:
                case TokenType.Class:
                case TokenType.Module:
                case TokenType.Import:
                case TokenType.Export:
                case TokenType.Static:
                case TokenType.Enum:
                case TokenType.AtSign:
                    return true;
                default:
                    return false;
            }
        }

        // Scans tokens starting at startIndex, looking for a matching 'end'
        // at depth 0 across nested begin/end-style constructs. Returns true
        // if such an 'end' is found. Used to disambiguate lambda forms.
        //
        // Depth-tracking tokens:
        //   Open: function, if, while, for, foreach, do, try, switch, match, class, module, {, [, (
        //   Close: end (matches function/if/while/for/foreach/do/try only if no while-keyword closes do)
        // Simpler heuristic: count keyword 'function' / 'if' / 'while' / 'for' /
        // 'foreach' / 'try' as opening and 'end' as closing.
        private bool LambdaBodyHasMatchingEnd(int startIndex)
        {
            int idx = startIndex;
            int depth = 1;
            while (idx < tokens.Count)
            {
                Token t = tokens[idx];
                switch (t.Type)
                {
                    case TokenType.Function:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.For:
                    case TokenType.Foreach:
                    case TokenType.Try:
                    case TokenType.Do:
                    case TokenType.Switch:
                    case TokenType.Match:
                    case TokenType.Class:
                    case TokenType.Module:
                        // Opens a new construct that closes with 'end' or '}'.
                        // For simplicity track these as opening 'end'-paired blocks
                        // (do counts; do-while is closed by 'while', not 'end').
                        // Note: switch/match close with '}', not 'end' — but we
                        // can't track them without watching braces, so we instead
                        // ALSO track left/right brace pairs and only count 'end'.
                        if (t.Type == TokenType.Switch || t.Type == TokenType.Match
                            || t.Type == TokenType.Class || t.Type == TokenType.Module)
                        {
                            // These don't pair with 'end'; ignore for depth.
                            break;
                        }
                        depth++;
                        break;
                    case TokenType.End:
                        depth--;
                        if (depth == 0)
                        {
                            return true;
                        }
                        break;
                    case TokenType.EndOfFile:
                        return false;
                    default:
                        break;
                }
                idx++;
            }
            return false;
        }

        // ---------------------------------------------------------------
        // Type parser
        // ---------------------------------------------------------------

        private TypeNode ParseType()
        {
            Token startTok = Current();
            string baseTypeName;
            TypeNode mapKey = null;
            TypeNode mapValue = null;
            List<TypeNode> genericArgs = new List<TypeNode>();

            switch (startTok.Type)
            {
                case TokenType.Int:
                case TokenType.Float:
                case TokenType.String:
                case TokenType.Bool:
                case TokenType.Array:
                case TokenType.Object:
                case TokenType.Null:
                case TokenType.Void:
                    baseTypeName = startTok.Value;
                    Advance();
                    break;
                case TokenType.Map:
                    baseTypeName = "map";
                    Advance();
                    Expect(TokenType.Less, "'<' in map type");
                    mapKey = ParseType();
                    Expect(TokenType.Comma, "',' between map key and value types");
                    mapValue = ParseType();
                    Expect(TokenType.Greater, "'>' to close map type");
                    break;
                case TokenType.Identifier:
                    baseTypeName = startTok.Value;
                    Advance();
                    // Generic type Foo<int>: only if a closing '>' is reachable
                    // along a valid type-token-only path (Note 20).
                    if (Current().Type == TokenType.Less && LooksLikeGeneric(position))
                    {
                        Advance(); // consume '<'
                        genericArgs.Add(ParseType());
                        while (Match(TokenType.Comma))
                        {
                            genericArgs.Add(ParseType());
                        }
                        Expect(TokenType.Greater, "'>' to close generic argument list");
                    }
                    break;
                default:
                    throw new ParserException(
                        $"Expected type but found '{startTok.Value}' (token type {startTok.Type})",
                        startTok.Line);
            }

            List<TypeSuffix> suffixes = new List<TypeSuffix>();
            while (true)
            {
                if (Current().Type == TokenType.LeftBracket
                    && Peek(1).Type == TokenType.RightBracket)
                {
                    Advance();
                    Advance();
                    suffixes.Add(TypeSuffix.ArraySuffix);
                }
                else if (Current().Type == TokenType.Question)
                {
                    Advance();
                    suffixes.Add(TypeSuffix.NullableSuffix);
                }
                else
                {
                    break;
                }
            }

            return new TypeNode(startTok.Line, baseTypeName, suffixes, genericArgs, mapKey, mapValue);
        }

        // Lookahead heuristic for generic type disambiguation (Note 20).
        // From position startIndex (currently pointing at '<'), scans forward
        // and returns true if the next '>' lies along a path consisting only
        // of type-shaped tokens (type keywords / identifiers / commas /
        // brackets pairs / question marks / nested '<...>').
        private bool LooksLikeGeneric(int startIndex)
        {
            int idx = startIndex;
            if (tokens[idx].Type != TokenType.Less)
            {
                return false;
            }
            idx++;
            int depth = 1;
            while (idx < tokens.Count && depth > 0)
            {
                Token t = tokens[idx];
                switch (t.Type)
                {
                    case TokenType.Less:
                        depth++;
                        break;
                    case TokenType.Greater:
                        depth--;
                        if (depth == 0)
                        {
                            return true;
                        }
                        break;
                    case TokenType.Int:
                    case TokenType.Float:
                    case TokenType.String:
                    case TokenType.Bool:
                    case TokenType.Array:
                    case TokenType.Object:
                    case TokenType.Null:
                    case TokenType.Void:
                    case TokenType.Map:
                    case TokenType.Identifier:
                    case TokenType.Comma:
                    case TokenType.Question:
                    case TokenType.LeftBracket:
                    case TokenType.RightBracket:
                        break;
                    default:
                        // Anything else means this is not a generic type.
                        return false;
                }
                idx++;
            }
            return false;
        }
    }

}
