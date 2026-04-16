using System;
using System.Collections.Generic;
using System.Globalization;

namespace TinyLanguage.Lexer
{
    // Recursive-descent parser that converts a token stream into an AST.
    // Entry point: Parser.Parse(tokens) returns a ProgramNode.
    //
    // Implements every BNF production from Build.Solution.md section 1.4.2
    // and all 23 disambiguation rules from section 1.4.1.
    public sealed class Parser
    {
        // The flat token list produced by the lexer.
        private readonly IReadOnlyList<Token> Tokens;

        // Current position in the token list.
        private int Position;

        private Parser(IReadOnlyList<Token> tokens)
        {
            Tokens = tokens;
            Position = 0;
        }

        // ================================================================
        // Public entry point
        // ================================================================

        public static ProgramNode Parse(IReadOnlyList<Token> tokens)
        {
            Parser parser = new Parser(tokens);
            List<AstNode> statements = parser.ParseStatementList(null);
            parser.Expect(TokenType.EndOfFile, "Expected end of file");
            int line = tokens.Count > 0 ? tokens[0].Line : 1;
            return new ProgramNode(statements, line);
        }

        // ================================================================
        // Token helpers
        // ================================================================

        // Returns the current token without consuming it.
        private Token Peek()
        {
            return Tokens[Position];
        }

        // Returns the token at the given offset from the current position.
        private Token PeekAt(int offset)
        {
            int index = Position + offset;
            if (index >= Tokens.Count)
            {
                return Tokens[Tokens.Count - 1]; // EOF
            }
            return Tokens[index];
        }

        // Consumes and returns the current token.
        private Token Advance()
        {
            Token token = Tokens[Position];
            Position++;
            return token;
        }

        // Returns true if the current token matches the given type.
        private bool Check(TokenType type)
        {
            return Peek().Type == type;
        }

        // Consumes the current token if it matches the given type; returns true if consumed.
        private bool Match(TokenType type)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
            return false;
        }

        // Consumes the current token and asserts it has the expected type.
        private Token Expect(TokenType type, string errorMessage)
        {
            if (Check(type))
            {
                return Advance();
            }
            throw new ParserException(errorMessage + " but got '" + Peek().Value + "' (" + Peek().Type + ")", Peek().Line);
        }

        // Returns true if the current token is one that can begin a statement.
        private bool CanStartStatement()
        {
            TokenType type = Peek().Type;
            switch (type)
            {
                case TokenType.Identifier:
                case TokenType.Let:
                case TokenType.Var:
                case TokenType.Const:
                case TokenType.Enum:
                case TokenType.If:
                case TokenType.While:
                case TokenType.For:
                case TokenType.Foreach:
                case TokenType.Do:
                case TokenType.Switch:
                case TokenType.Break:
                case TokenType.Continue:
                case TokenType.Print:
                case TokenType.Input:
                case TokenType.Function:
                case TokenType.Return:
                case TokenType.Class:
                case TokenType.Static:
                case TokenType.Module:
                case TokenType.Import:
                case TokenType.Export:
                case TokenType.Try:
                case TokenType.Throw:
                case TokenType.Match:
                case TokenType.At:
                    return true;
                default:
                    return false;
            }
        }

        // ================================================================
        // Statement list parsing (note 3, 21)
        // ================================================================

        // Parses a list of statements separated by semicolons.
        // Stops when the current token is in the stopTokens set, is EndOfFile,
        // or cannot begin a statement.
        // Note 21: trailing semicolons before stop tokens are errors.
        private List<AstNode> ParseStatementList(HashSet<TokenType> stopTokens)
        {
            List<AstNode> statements = new List<AstNode>();

            while (true)
            {
                // Check stop conditions
                if (Check(TokenType.EndOfFile))
                {
                    break;
                }
                if (stopTokens != null && stopTokens.Contains(Peek().Type))
                {
                    break;
                }
                if (!CanStartStatement())
                {
                    break;
                }

                AstNode statement = ParseStatement();
                statements.Add(statement);

                // Semicolons are separators (note 3, 21): consume one if present,
                // but it must not appear before a block terminator.
                if (Check(TokenType.Semicolon))
                {
                    // Check if the token after the semicolon is a stop token / block terminator.
                    // If so, the trailing semicolon is an error per note 21.
                    TokenType afterSemicolon = PeekAt(1).Type;
                    if (afterSemicolon == TokenType.End ||
                        afterSemicolon == TokenType.Else ||
                        afterSemicolon == TokenType.Catch ||
                        afterSemicolon == TokenType.Finally ||
                        afterSemicolon == TokenType.While ||
                        afterSemicolon == TokenType.RightBrace ||
                        afterSemicolon == TokenType.EndOfFile)
                    {
                        throw new ParserException(
                            "Trailing semicolon before block terminator is not allowed",
                            Peek().Line);
                    }
                    if (stopTokens != null && stopTokens.Contains(afterSemicolon))
                    {
                        throw new ParserException(
                            "Trailing semicolon before block terminator is not allowed",
                            Peek().Line);
                    }
                    Advance(); // consume semicolon
                }
            }

            return statements;
        }

        // ================================================================
        // Statement parsing
        // ================================================================

        private AstNode ParseStatement()
        {
            TokenType type = Peek().Type;

            switch (type)
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
                    return ParseBreakStatement();

                case TokenType.Continue:
                    return ParseContinueStatement();

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

                case TokenType.At:
                    return ParseAnnotatedStatement();

                case TokenType.Identifier:
                    return ParseIdentifierStatement();

                default:
                    throw new ParserException(
                        "Unexpected token '" + Peek().Value + "' at start of statement",
                        Peek().Line);
            }
        }

        // ================================================================
        // Identifier-led statements: assignment, array assign, call statement
        // ================================================================

        // An identifier can start:
        //   assign_stmt:       <id> ":=" <expr>
        //   array_assign_stmt: <id> "[" <expr> "]" ":=" <expr>
        //   call_stmt:         <id> { "." <id> } "(" [<arg_list>] ")"
        private AstNode ParseIdentifierStatement()
        {
            Token idToken = Expect(TokenType.Identifier, "Expected identifier");
            int line = idToken.Line;
            string name = idToken.Value;

            // Array element assignment: id "[" expr "]" ":="
            if (Check(TokenType.LeftBracket))
            {
                Advance(); // consume "["
                AstNode indexExpression = ParseExpression();
                Expect(TokenType.RightBracket, "Expected ']'");
                Expect(TokenType.ColonEquals, "Expected ':=' in array element assignment");
                AstNode valueExpression = ParseExpression();
                return new ArrayElementAssignNode(name, indexExpression, valueExpression, line);
            }

            // Simple assignment: id ":="
            if (Check(TokenType.ColonEquals))
            {
                Advance(); // consume ":="
                AstNode valueExpression = ParseExpression();
                return new AssignStatementNode(name, valueExpression, line);
            }

            // Call statement: id { "." id } "(" [arg_list] ")"
            // Per note 22, must be id chain ending with "()"
            List<string> receiverChain = new List<string>();
            receiverChain.Add(name);

            while (Check(TokenType.Dot))
            {
                Advance(); // consume "."
                Token memberToken = Expect(TokenType.Identifier, "Expected member name after '.'");
                receiverChain.Add(memberToken.Value);
            }

            Expect(TokenType.LeftParen, "Expected '(' for call statement, ':=' for assignment, or '[' for array assignment");
            List<AstNode> arguments = ParseArgList();
            Expect(TokenType.RightParen, "Expected ')'");

            return new CallStatementNode(receiverChain, arguments, line);
        }

        // ================================================================
        // Declarations
        // ================================================================

        // "let" <id> ":=" <expr>
        // "let" <id> ":" <type> ":=" <expr>
        private AstNode ParseLetDeclare()
        {
            Token letToken = Advance(); // consume "let"
            int line = letToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected identifier after 'let'");
            string name = idToken.Value;

            TypeNode declaredType = null;
            if (Check(TokenType.Colon))
            {
                Advance(); // consume ":"
                declaredType = ParseType();
            }

            Expect(TokenType.ColonEquals, "Expected ':=' in let declaration");
            AstNode initialiser = ParseExpression();
            return new LetDeclareNode(name, declaredType, initialiser, line);
        }

        // "var" <id> ":" <type> ":=" <expr>
        private AstNode ParseVarDeclare()
        {
            Token varToken = Advance(); // consume "var"
            int line = varToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected identifier after 'var'");
            string name = idToken.Value;
            Expect(TokenType.Colon, "Expected ':' with type annotation in var declaration");
            TypeNode declaredType = ParseType();
            Expect(TokenType.ColonEquals, "Expected ':=' in var declaration");
            AstNode initialiser = ParseExpression();
            return new VarDeclareNode(name, declaredType, initialiser, line);
        }

        // "const" <id> ":=" <expr>
        // "const" <id> ":" <type> ":=" <expr>
        private AstNode ParseConstDeclare()
        {
            Token constToken = Advance(); // consume "const"
            int line = constToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected identifier after 'const'");
            string name = idToken.Value;

            TypeNode declaredType = null;
            if (Check(TokenType.Colon))
            {
                Advance(); // consume ":"
                declaredType = ParseType();
            }

            Expect(TokenType.ColonEquals, "Expected ':=' in const declaration");
            AstNode initialiser = ParseExpression();
            return new ConstDeclareNode(name, declaredType, initialiser, line);
        }

        // "enum" <id> "{" <enum_value_list> "}"
        private AstNode ParseEnumDef()
        {
            Token enumToken = Advance(); // consume "enum"
            int line = enumToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected enum name");
            string name = idToken.Value;
            Expect(TokenType.LeftBrace, "Expected '{' after enum name");

            List<EnumValueNode> members = new List<EnumValueNode>();
            while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
            {
                Token memberToken = Expect(TokenType.Identifier, "Expected enum member name");
                AstNode valueExpression = null;

                // Enum members use "=" (SingleEqual), not ":="
                if (Check(TokenType.SingleEqual))
                {
                    Advance(); // consume "="
                    valueExpression = ParseExpression();
                }

                members.Add(new EnumValueNode(memberToken.Value, valueExpression, memberToken.Line));

                if (!Match(TokenType.Comma))
                {
                    break;
                }
            }

            Expect(TokenType.RightBrace, "Expected '}' at end of enum definition");
            return new EnumDefNode(name, members, line);
        }

        // ================================================================
        // Control flow
        // ================================================================

        // "if" <expr> "then" <stmt_list> ["else" <stmt_list>] "end"
        private AstNode ParseIfStatement()
        {
            Token ifToken = Advance(); // consume "if"
            int line = ifToken.Line;
            AstNode condition = ParseExpression();
            Expect(TokenType.Then, "Expected 'then' after if condition");

            HashSet<TokenType> ifStopTokens = new HashSet<TokenType>();
            ifStopTokens.Add(TokenType.Else);
            ifStopTokens.Add(TokenType.End);

            List<AstNode> thenBody = ParseStatementList(ifStopTokens);
            List<AstNode> elseBody = null;

            if (Match(TokenType.Else))
            {
                HashSet<TokenType> elseStopTokens = new HashSet<TokenType>();
                elseStopTokens.Add(TokenType.End);
                elseBody = ParseStatementList(elseStopTokens);
            }

            Expect(TokenType.End, "Expected 'end' to close if statement");
            return new IfStatementNode(condition, thenBody, elseBody, line);
        }

        // "while" <expr> "do" <stmt_list> "end"
        private AstNode ParseWhileStatement()
        {
            Token whileToken = Advance(); // consume "while"
            int line = whileToken.Line;
            AstNode condition = ParseExpression();
            Expect(TokenType.Do, "Expected 'do' after while condition");

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.End);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.End, "Expected 'end' to close while loop");
            return new WhileStatementNode(condition, body, line);
        }

        // "for" <id> ":=" <expr> "to" <expr> ["step" <expr>] "do" <stmt_list> "end"
        private AstNode ParseForStatement()
        {
            Token forToken = Advance(); // consume "for"
            int line = forToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected loop variable after 'for'");
            string loopVariable = idToken.Value;
            Expect(TokenType.ColonEquals, "Expected ':=' after for loop variable");
            AstNode startExpression = ParseExpression();
            Expect(TokenType.To, "Expected 'to' in for loop");
            AstNode endExpression = ParseExpression();

            AstNode stepExpression = null;
            if (Match(TokenType.Step))
            {
                stepExpression = ParseExpression();
            }

            Expect(TokenType.Do, "Expected 'do' in for loop");

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.End);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.End, "Expected 'end' to close for loop");
            return new ForStatementNode(loopVariable, startExpression, endExpression, stepExpression, body, line);
        }

        // "foreach" <id> "in" <expr> "do" <stmt_list> "end"
        private AstNode ParseForeachStatement()
        {
            Token foreachToken = Advance(); // consume "foreach"
            int line = foreachToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected variable name after 'foreach'");
            string elementVariable = idToken.Value;
            Expect(TokenType.In, "Expected 'in' after foreach variable");
            AstNode iterableExpression = ParseExpression();
            Expect(TokenType.Do, "Expected 'do' in foreach loop");

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.End);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.End, "Expected 'end' to close foreach loop");
            return new ForeachStatementNode(elementVariable, iterableExpression, body, line);
        }

        // "do" <stmt_list> "while" <expr>   (note 20)
        // The stmt_list stops at "while".
        private AstNode ParseDoWhileStatement()
        {
            Token doToken = Advance(); // consume "do"
            int line = doToken.Line;

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.While);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.While, "Expected 'while' after do block");
            AstNode condition = ParseExpression();
            return new DoWhileStatementNode(body, condition, line);
        }

        // "switch" <expr> "{" <case_list> "}"
        private AstNode ParseSwitchStatement()
        {
            Token switchToken = Advance(); // consume "switch"
            int line = switchToken.Line;
            AstNode subjectExpression = ParseExpression();
            Expect(TokenType.LeftBrace, "Expected '{' after switch expression");

            List<SwitchCaseNode> cases = new List<SwitchCaseNode>();

            HashSet<TokenType> caseStopTokens = new HashSet<TokenType>();
            caseStopTokens.Add(TokenType.Case);
            caseStopTokens.Add(TokenType.Default);
            caseStopTokens.Add(TokenType.RightBrace);

            while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
            {
                if (Check(TokenType.Case))
                {
                    Token caseToken = Advance(); // consume "case"
                    AstNode caseValue = ParseExpression();
                    Expect(TokenType.Colon, "Expected ':' after case value");
                    List<AstNode> caseBody = ParseStatementList(caseStopTokens);
                    cases.Add(new SwitchCaseNode(caseValue, caseBody, false, caseToken.Line));
                }
                else if (Check(TokenType.Default))
                {
                    Token defaultToken = Advance(); // consume "default"
                    Expect(TokenType.Colon, "Expected ':' after 'default'");
                    List<AstNode> defaultBody = ParseStatementList(caseStopTokens);
                    cases.Add(new SwitchCaseNode(null, defaultBody, true, defaultToken.Line));
                }
                else
                {
                    throw new ParserException(
                        "Expected 'case' or 'default' in switch body",
                        Peek().Line);
                }
            }

            Expect(TokenType.RightBrace, "Expected '}' to close switch statement");
            return new SwitchStatementNode(subjectExpression, cases, line);
        }

        private AstNode ParseBreakStatement()
        {
            Token breakToken = Advance(); // consume "break"
            return new BreakStatementNode(breakToken.Line);
        }

        private AstNode ParseContinueStatement()
        {
            Token continueToken = Advance(); // consume "continue"
            return new ContinueStatementNode(continueToken.Line);
        }

        // ================================================================
        // I/O statements
        // ================================================================

        // "print" <expr>
        private AstNode ParsePrintStatement()
        {
            Token printToken = Advance(); // consume "print"
            int line = printToken.Line;
            AstNode expression = ParseExpression();
            return new PrintStatementNode(expression, line);
        }

        // "input" <id>
        private AstNode ParseInputStatement()
        {
            Token inputToken = Advance(); // consume "input"
            int line = inputToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected variable name after 'input'");
            return new InputStatementNode(idToken.Value, line);
        }

        // ================================================================
        // Functions
        // ================================================================

        // "function" <id> "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"
        private AstNode ParseFunctionDef(bool isStatic)
        {
            Token functionToken = Advance(); // consume "function"
            int line = functionToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected function name");
            string functionName = idToken.Value;

            Expect(TokenType.LeftParen, "Expected '(' after function name");
            List<ParameterNode> parameters = ParseParamList();
            Expect(TokenType.RightParen, "Expected ')' after parameter list");

            TypeNode returnType = null;
            if (Check(TokenType.Arrow))
            {
                Advance(); // consume "->"
                returnType = ParseType();
            }

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.End);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.End, "Expected 'end' to close function definition");
            return new FunctionDefNode(functionName, parameters, returnType, body, isStatic, line);
        }

        // "static" can precede "function" or "class" (note 16)
        private AstNode ParseStaticDef()
        {
            Advance(); // consume "static"
            if (Check(TokenType.Function))
            {
                return ParseFunctionDef(true);
            }
            if (Check(TokenType.Class))
            {
                return ParseClassDef(true);
            }
            throw new ParserException(
                "Expected 'function' or 'class' after 'static'",
                Peek().Line);
        }

        // "return" [<expr>]   (note 4: bare return)
        private AstNode ParseReturnStatement()
        {
            Token returnToken = Advance(); // consume "return"
            int line = returnToken.Line;

            // Bare return: the next token is a block terminator, semicolon, or EOF
            if (Check(TokenType.End) ||
                Check(TokenType.Else) ||
                Check(TokenType.Catch) ||
                Check(TokenType.Finally) ||
                Check(TokenType.RightBrace) ||
                Check(TokenType.Semicolon) ||
                Check(TokenType.EndOfFile))
            {
                return new ReturnStatementNode(null, line);
            }

            AstNode expression = ParseExpression();
            return new ReturnStatementNode(expression, line);
        }

        // ================================================================
        // Parameter list parsing
        // ================================================================

        // <param_list> ::= <param> "," <param_list> | <param>
        private List<ParameterNode> ParseParamList()
        {
            List<ParameterNode> parameters = new List<ParameterNode>();

            if (Check(TokenType.RightParen))
            {
                return parameters; // empty parameter list
            }

            parameters.Add(ParseParam());
            while (Match(TokenType.Comma))
            {
                parameters.Add(ParseParam());
            }

            return parameters;
        }

        // <param> ::= <id> | <id> ":=" <expr> | <id> ":" <type> | <id> ":" <type> ":=" <expr>
        private ParameterNode ParseParam()
        {
            Token idToken = Expect(TokenType.Identifier, "Expected parameter name");
            int line = idToken.Line;
            string parameterName = idToken.Value;

            TypeNode declaredType = null;
            AstNode defaultExpression = null;

            if (Check(TokenType.Colon))
            {
                Advance(); // consume ":"
                declaredType = ParseType();
            }

            if (Check(TokenType.ColonEquals))
            {
                Advance(); // consume ":="
                defaultExpression = ParseExpression();
            }

            return new ParameterNode(parameterName, declaredType, defaultExpression, line);
        }

        // ================================================================
        // Argument list parsing
        // ================================================================

        // <arg_list> ::= <expr> "," <arg_list> | <expr>
        private List<AstNode> ParseArgList()
        {
            List<AstNode> arguments = new List<AstNode>();

            if (Check(TokenType.RightParen))
            {
                return arguments; // empty argument list
            }

            arguments.Add(ParseExpression());
            while (Match(TokenType.Comma))
            {
                arguments.Add(ParseExpression());
            }

            return arguments;
        }

        // ================================================================
        // Classes
        // ================================================================

        // ["static"] "class" <id> ["extends" <id>] ["implements" <id_list>] "{" <member_list> "}"
        private AstNode ParseClassDef(bool isStatic)
        {
            Token classToken = Advance(); // consume "class"
            int line = classToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected class name");
            string className = idToken.Value;

            string baseClassName = null;
            if (Match(TokenType.Extends))
            {
                Token baseToken = Expect(TokenType.Identifier, "Expected base class name after 'extends'");
                baseClassName = baseToken.Value;
            }

            List<string> implementedInterfaces = new List<string>();
            if (Match(TokenType.Implements))
            {
                Token ifaceToken = Expect(TokenType.Identifier, "Expected interface name after 'implements'");
                implementedInterfaces.Add(ifaceToken.Value);
                while (Match(TokenType.Comma))
                {
                    Token nextIfaceToken = Expect(TokenType.Identifier, "Expected interface name");
                    implementedInterfaces.Add(nextIfaceToken.Value);
                }
            }

            Expect(TokenType.LeftBrace, "Expected '{' to open class body");

            List<AstNode> members = new List<AstNode>();
            while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
            {
                AstNode member = ParseClassMember();
                members.Add(member);

                // Consume optional semicolons between members
                if (Check(TokenType.Semicolon))
                {
                    TokenType afterSemicolon = PeekAt(1).Type;
                    if (afterSemicolon == TokenType.RightBrace || afterSemicolon == TokenType.EndOfFile)
                    {
                        throw new ParserException(
                            "Trailing semicolon before '}' is not allowed",
                            Peek().Line);
                    }
                    Advance(); // consume semicolon
                }
            }

            Expect(TokenType.RightBrace, "Expected '}' to close class body");
            return new ClassDefNode(className, baseClassName, implementedInterfaces, members, isStatic, line);
        }

        // <member> ::= <method_def> | <field_declare_stmt> | <const_declare_stmt> | <constructor_def>
        private AstNode ParseClassMember()
        {
            if (Check(TokenType.Constructor))
            {
                return ParseConstructorDef();
            }

            if (Check(TokenType.Static))
            {
                // static function inside a class
                Token staticToken = Peek();
                if (PeekAt(1).Type == TokenType.Function)
                {
                    Advance(); // consume "static"
                    return ParseFunctionDef(true);
                }
                throw new ParserException("Expected 'function' after 'static' in class body", staticToken.Line);
            }

            if (Check(TokenType.Function))
            {
                return ParseFunctionDef(false);
            }

            if (Check(TokenType.Let) || Check(TokenType.Var) || Check(TokenType.Const))
            {
                // Check for const_declare_stmt (enum is not expected inside a class member list)
                return ParseFieldDeclare();
            }

            if (Check(TokenType.Enum))
            {
                return ParseEnumDef();
            }

            throw new ParserException(
                "Expected class member (field, method, constructor, or enum)",
                Peek().Line);
        }

        // "Constructor" "(" [<param_list>] ")" <stmt_list> "end"
        private AstNode ParseConstructorDef()
        {
            Token constructorToken = Advance(); // consume "Constructor"
            int line = constructorToken.Line;
            Expect(TokenType.LeftParen, "Expected '(' after 'Constructor'");
            List<ParameterNode> parameters = ParseParamList();
            Expect(TokenType.RightParen, "Expected ')' after constructor parameter list");

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.End);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.End, "Expected 'end' to close constructor definition");
            return new ConstructorDefNode(parameters, body, line);
        }

        // <field_declare_stmt> for class bodies
        private AstNode ParseFieldDeclare()
        {
            Token keywordToken = Advance(); // consume "let", "var", or "const"
            string keyword = keywordToken.Value;
            int line = keywordToken.Line;

            Token idToken = Expect(TokenType.Identifier, "Expected field name after '" + keyword + "'");
            string fieldName = idToken.Value;

            TypeNode declaredType = null;

            if (keyword == "var")
            {
                // var requires a type annotation
                Expect(TokenType.Colon, "Expected ':' with type annotation in var declaration");
                declaredType = ParseType();
            }
            else if (Check(TokenType.Colon))
            {
                Advance(); // consume ":"
                declaredType = ParseType();
            }

            Expect(TokenType.ColonEquals, "Expected ':=' in field declaration");
            AstNode initialiser = ParseExpression();
            return new FieldDeclareNode(keyword, fieldName, declaredType, initialiser, line);
        }

        // ================================================================
        // Modules
        // ================================================================

        // "module" <id> ["import" <module_import_list>] "{" <stmt_list> "}"
        private AstNode ParseModuleDef()
        {
            Token moduleToken = Advance(); // consume "module"
            int line = moduleToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected module name");
            string moduleName = idToken.Value;

            List<ModuleImportNode> headerImports = new List<ModuleImportNode>();
            if (Check(TokenType.Import))
            {
                Advance(); // consume "import"
                headerImports = ParseModuleImportList();
            }

            Expect(TokenType.LeftBrace, "Expected '{' to open module body");

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.RightBrace);

            List<AstNode> body = ParseStatementList(stopTokens);
            Expect(TokenType.RightBrace, "Expected '}' to close module body");
            return new ModuleDefNode(moduleName, headerImports, body, line);
        }

        // <module_import_list> ::= <module_import> "," <module_import_list> | <module_import>
        private List<ModuleImportNode> ParseModuleImportList()
        {
            List<ModuleImportNode> imports = new List<ModuleImportNode>();
            imports.Add(ParseModuleImport());
            while (Match(TokenType.Comma))
            {
                imports.Add(ParseModuleImport());
            }
            return imports;
        }

        // <module_import> ::= <id> | <id> "as" <id>
        private ModuleImportNode ParseModuleImport()
        {
            Token idToken = Expect(TokenType.Identifier, "Expected module name in import");
            string moduleName = idToken.Value;
            string alias = null;

            if (Match(TokenType.As))
            {
                Token aliasToken = Expect(TokenType.Identifier, "Expected alias after 'as'");
                alias = aliasToken.Value;
            }

            return new ModuleImportNode(moduleName, alias, idToken.Line);
        }

        // "import" <id> ["as" <id>]
        private AstNode ParseImportStatement()
        {
            Token importToken = Advance(); // consume "import"
            int line = importToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected module name after 'import'");
            string moduleName = idToken.Value;
            string alias = null;

            if (Match(TokenType.As))
            {
                Token aliasToken = Expect(TokenType.Identifier, "Expected alias after 'as'");
                alias = aliasToken.Value;
            }

            return new ImportStatementNode(moduleName, alias, line);
        }

        // "export" <id>
        private AstNode ParseExportStatement()
        {
            Token exportToken = Advance(); // consume "export"
            int line = exportToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected name after 'export'");
            return new ExportStatementNode(idToken.Value, line);
        }

        // ================================================================
        // Exception handling
        // ================================================================

        // "try" <stmt_list> <catch_clause> ["finally" <stmt_list>] "end"
        private AstNode ParseTryStatement()
        {
            Token tryToken = Advance(); // consume "try"
            int line = tryToken.Line;

            HashSet<TokenType> tryStopTokens = new HashSet<TokenType>();
            tryStopTokens.Add(TokenType.Catch);

            List<AstNode> tryBody = ParseStatementList(tryStopTokens);

            // Parse catch clause
            Expect(TokenType.Catch, "Expected 'catch' in try statement");
            CatchClauseNode catchClause = ParseCatchClause();

            // Parse optional finally
            List<AstNode> finallyBody = null;
            if (Check(TokenType.Finally))
            {
                Advance(); // consume "finally"
                HashSet<TokenType> finallyStopTokens = new HashSet<TokenType>();
                finallyStopTokens.Add(TokenType.End);
                finallyBody = ParseStatementList(finallyStopTokens);
            }

            Expect(TokenType.End, "Expected 'end' to close try statement");
            return new TryStatementNode(tryBody, catchClause, finallyBody, line);
        }

        // "catch" <id> <stmt_list>
        // "catch" "(" <id> ":" <type> ")" <stmt_list>
        private CatchClauseNode ParseCatchClause()
        {
            int line = Peek().Line;

            string exceptionVariable;
            TypeNode exceptionType = null;

            if (Check(TokenType.LeftParen))
            {
                // Typed form: "catch" "(" <id> ":" <type> ")"
                Advance(); // consume "("
                Token idToken = Expect(TokenType.Identifier, "Expected exception variable name");
                exceptionVariable = idToken.Value;
                Expect(TokenType.Colon, "Expected ':' after exception variable");
                exceptionType = ParseType();
                Expect(TokenType.RightParen, "Expected ')' after exception type");
            }
            else
            {
                // Bare form: "catch" <id>
                Token idToken = Expect(TokenType.Identifier, "Expected exception variable name after 'catch'");
                exceptionVariable = idToken.Value;
            }

            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.Finally);
            stopTokens.Add(TokenType.End);

            List<AstNode> body = ParseStatementList(stopTokens);
            return new CatchClauseNode(exceptionVariable, exceptionType, body, line);
        }

        // "throw" <expr>
        private AstNode ParseThrowStatement()
        {
            Token throwToken = Advance(); // consume "throw"
            int line = throwToken.Line;
            AstNode expression = ParseExpression();
            return new ThrowStatementNode(expression, line);
        }

        // ================================================================
        // Pattern matching
        // ================================================================

        // "match" <expr> "{" <pattern_case_list> "}"
        private AstNode ParsePatternMatch()
        {
            Token matchToken = Advance(); // consume "match"
            int line = matchToken.Line;
            AstNode subjectExpression = ParseExpression();
            Expect(TokenType.LeftBrace, "Expected '{' after match expression");

            List<PatternCaseNode> cases = new List<PatternCaseNode>();

            while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
            {
                AstNode pattern = ParsePattern();
                AstNode whenGuard = null;

                if (Check(TokenType.When))
                {
                    Advance(); // consume "when"
                    whenGuard = ParseExpression();
                }

                Expect(TokenType.FatArrow, "Expected '=>' after pattern");

                HashSet<TokenType> caseStopTokens = new HashSet<TokenType>();
                caseStopTokens.Add(TokenType.RightBrace);

                // The stmt_list for a pattern case stops at the start of a new pattern
                // or at '}'. We detect the start of a new pattern by checking if the
                // current token can start a pattern when we are not in a statement.
                List<AstNode> caseBody = ParsePatternCaseBody();

                cases.Add(new PatternCaseNode(pattern, whenGuard, caseBody, pattern.Line));
            }

            Expect(TokenType.RightBrace, "Expected '}' to close match expression");
            return new PatternMatchNode(subjectExpression, cases, line);
        }

        // Parse the body of a pattern case. Stops before the next pattern or '}'.
        private List<AstNode> ParsePatternCaseBody()
        {
            // We need to recognize the boundary between pattern cases.
            // A pattern can start with: Identifier, IntegerLiteral, FloatLiteral,
            // StringLiteral, True, False, Null, Underscore, LeftBracket.
            // But many of these can also start statements.
            // We parse as a normal statement list but use special stop tokens.

            // The simplest approach: the pattern case body is a statement list
            // that stops at '}'. When we detect that the next sequence looks like
            // a new pattern (not a statement), we stop.
            HashSet<TokenType> stopTokens = new HashSet<TokenType>();
            stopTokens.Add(TokenType.RightBrace);

            List<AstNode> statements = new List<AstNode>();

            while (true)
            {
                if (Check(TokenType.EndOfFile) || Check(TokenType.RightBrace))
                {
                    break;
                }

                // Check if this looks like the start of a new pattern case.
                // A pattern case always has some token(s) then "=>" or "when".
                // If the current position looks like a pattern (not a statement), stop.
                if (IsPatternCaseStart())
                {
                    break;
                }

                if (!CanStartStatement())
                {
                    break;
                }

                AstNode statement = ParseStatement();
                statements.Add(statement);

                // Handle semicolons between statements
                if (Check(TokenType.Semicolon))
                {
                    TokenType afterSemicolon = PeekAt(1).Type;
                    if (afterSemicolon == TokenType.RightBrace || afterSemicolon == TokenType.EndOfFile)
                    {
                        throw new ParserException(
                            "Trailing semicolon before block terminator is not allowed",
                            Peek().Line);
                    }
                    Advance(); // consume semicolon
                }
            }

            return statements;
        }

        // Heuristic to detect start of a new pattern case inside a match body.
        // We look for a token sequence that ends with "=>" or "when" ... "=>"
        // at a reasonable depth.
        private bool IsPatternCaseStart()
        {
            // Quick check: patterns that cannot possibly be statements
            TokenType current = Peek().Type;

            // Underscore "_" is always a pattern, never a statement
            if (current == TokenType.Underscore)
            {
                return true;
            }

            // A literal (int, float, string, true, false, null) followed eventually by "=>" or "when"
            // is a pattern, not a statement. But we need a quick heuristic.
            if (current == TokenType.IntegerLiteral ||
                current == TokenType.FloatLiteral ||
                current == TokenType.StringLiteral ||
                current == TokenType.True ||
                current == TokenType.False ||
                current == TokenType.Null)
            {
                // Check if followed by "=>" or "|" then "=>" or "when"
                TokenType next = PeekAt(1).Type;
                if (next == TokenType.FatArrow || next == TokenType.When || next == TokenType.Pipe)
                {
                    return true;
                }
                return false;
            }

            // An identifier can start both patterns and statements.
            // Pattern forms:
            //   id "=>"         — simple pattern
            //   id "when"       — guarded pattern
            //   id "|"          — alternation pattern
            //   id "(" ...      — constructor pattern  (but also call_stmt!)
            //   id "{" ...      — field pattern (but also... unlikely in stmt context)
            if (current == TokenType.Identifier)
            {
                TokenType next = PeekAt(1).Type;
                if (next == TokenType.FatArrow || next == TokenType.When)
                {
                    return true;
                }
                if (next == TokenType.Pipe)
                {
                    return true;
                }
                // id "(" could be a constructor pattern or a call statement.
                // We look for the matching ")" then "=>" or "when".
                if (next == TokenType.LeftParen)
                {
                    return LookAheadPatternParens(2);
                }
                // id "{" is a field pattern
                if (next == TokenType.LeftBrace)
                {
                    return true;
                }
                return false;
            }

            // "[" could be an array pattern
            if (current == TokenType.LeftBracket)
            {
                return LookAheadPatternBrackets(1);
            }

            return false;
        }

        // Look ahead from offset past matching parens to see if followed by "=>" or "when" or "|".
        private bool LookAheadPatternParens(int offset)
        {
            int depth = 1;
            int index = offset;
            while (depth > 0)
            {
                TokenType tokenType = PeekAt(index).Type;
                if (tokenType == TokenType.EndOfFile)
                {
                    return false;
                }
                if (tokenType == TokenType.LeftParen)
                {
                    depth++;
                }
                else if (tokenType == TokenType.RightParen)
                {
                    depth--;
                }
                index++;
            }
            TokenType afterParen = PeekAt(index).Type;
            return afterParen == TokenType.FatArrow || afterParen == TokenType.When || afterParen == TokenType.Pipe;
        }

        // Look ahead from offset past matching brackets to see if followed by "=>" or "when" or "|".
        private bool LookAheadPatternBrackets(int offset)
        {
            int depth = 1;
            int index = offset;
            while (depth > 0)
            {
                TokenType tokenType = PeekAt(index).Type;
                if (tokenType == TokenType.EndOfFile)
                {
                    return false;
                }
                if (tokenType == TokenType.LeftBracket)
                {
                    depth++;
                }
                else if (tokenType == TokenType.RightBracket)
                {
                    depth--;
                }
                index++;
            }
            TokenType afterBracket = PeekAt(index).Type;
            return afterBracket == TokenType.FatArrow || afterBracket == TokenType.When || afterBracket == TokenType.Pipe;
        }

        // ================================================================
        // Pattern parsing (note 23)
        // ================================================================

        // <pattern> ::= <primary_pattern> { "|" <primary_pattern> }
        // Left-associative alternation.
        private AstNode ParsePattern()
        {
            AstNode left = ParsePrimaryPattern();

            while (Check(TokenType.Pipe))
            {
                Advance(); // consume "|"
                AstNode right = ParsePrimaryPattern();
                left = new AlternationPatternNode(left, right, left.Line);
            }

            return left;
        }

        // <primary_pattern> — one of the non-alternation pattern forms
        private AstNode ParsePrimaryPattern()
        {
            TokenType type = Peek().Type;
            int line = Peek().Line;

            // Wildcard "_"
            if (type == TokenType.Underscore)
            {
                Advance();
                return new WildcardPatternNode(line);
            }

            // Null literal pattern
            if (type == TokenType.Null)
            {
                Advance();
                return new NullLiteralNode(line);
            }

            // Boolean literal pattern
            if (type == TokenType.True)
            {
                Advance();
                return new BoolLiteralNode(true, line);
            }
            if (type == TokenType.False)
            {
                Advance();
                return new BoolLiteralNode(false, line);
            }

            // String literal pattern
            if (type == TokenType.StringLiteral)
            {
                Token strToken = Advance();
                return new StringLiteralNode(strToken.Value, line);
            }

            // Numeric literal pattern (may have unary minus)
            if (type == TokenType.IntegerLiteral)
            {
                Token intToken = Advance();
                long value = ParseIntegerValue(intToken.Value);
                return new IntegerLiteralNode(value, line);
            }
            if (type == TokenType.FloatLiteral)
            {
                Token floatToken = Advance();
                double value = double.Parse(floatToken.Value, CultureInfo.InvariantCulture);
                return new FloatLiteralNode(value, line);
            }
            if (type == TokenType.Minus)
            {
                Advance(); // consume "-"
                if (Check(TokenType.IntegerLiteral))
                {
                    Token intToken = Advance();
                    long value = -ParseIntegerValue(intToken.Value);
                    return new IntegerLiteralNode(value, line);
                }
                if (Check(TokenType.FloatLiteral))
                {
                    Token floatToken = Advance();
                    double value = -double.Parse(floatToken.Value, CultureInfo.InvariantCulture);
                    return new FloatLiteralNode(value, line);
                }
                throw new ParserException("Expected number after '-' in pattern", Peek().Line);
            }

            // Array pattern: "[" [<pattern_list>] "]"
            if (type == TokenType.LeftBracket)
            {
                Advance(); // consume "["
                List<AstNode> elementPatterns = new List<AstNode>();

                if (!Check(TokenType.RightBracket))
                {
                    elementPatterns.Add(ParsePattern());
                    while (Match(TokenType.Comma))
                    {
                        elementPatterns.Add(ParsePattern());
                    }
                }

                Expect(TokenType.RightBracket, "Expected ']' to close array pattern");
                return new ArrayPatternNode(elementPatterns, line);
            }

            // Identifier-led patterns: plain id, constructor pattern, field pattern
            if (type == TokenType.Identifier)
            {
                Token idToken = Advance();
                string name = idToken.Value;

                // Constructor pattern: id "(" [<pattern_list>] ")"
                if (Check(TokenType.LeftParen))
                {
                    Advance(); // consume "("
                    List<AstNode> subPatterns = new List<AstNode>();
                    if (!Check(TokenType.RightParen))
                    {
                        subPatterns.Add(ParsePattern());
                        while (Match(TokenType.Comma))
                        {
                            subPatterns.Add(ParsePattern());
                        }
                    }
                    Expect(TokenType.RightParen, "Expected ')' to close constructor pattern");
                    return new ConstructorPatternNode(name, subPatterns, line);
                }

                // Field pattern: id "{" [<field_pattern_list>] "}"
                if (Check(TokenType.LeftBrace))
                {
                    Advance(); // consume "{"
                    List<FieldPatternEntryNode> fieldPatterns = new List<FieldPatternEntryNode>();
                    if (!Check(TokenType.RightBrace))
                    {
                        fieldPatterns.Add(ParseFieldPatternEntry());
                        while (Match(TokenType.Comma))
                        {
                            fieldPatterns.Add(ParseFieldPatternEntry());
                        }
                    }
                    Expect(TokenType.RightBrace, "Expected '}' to close field pattern");
                    return new FieldPatternNode(name, fieldPatterns, line);
                }

                // Plain identifier pattern (binds the matched value to the variable)
                return new IdentifierNode(name, line);
            }

            throw new ParserException("Unexpected token '" + Peek().Value + "' in pattern", Peek().Line);
        }

        // <field_pattern_entry> ::= <id> ":" <pattern>
        private FieldPatternEntryNode ParseFieldPatternEntry()
        {
            Token idToken = Expect(TokenType.Identifier, "Expected field name in field pattern");
            int line = idToken.Line;
            Expect(TokenType.Colon, "Expected ':' after field name in field pattern");
            AstNode pattern = ParsePattern();
            return new FieldPatternEntryNode(idToken.Value, pattern, line);
        }

        // ================================================================
        // Annotations
        // ================================================================

        // "@" <id> ["(" [<annotation_param_list>] ")"] <stmt>
        private AstNode ParseAnnotatedStatement()
        {
            Token atToken = Advance(); // consume "@"
            int line = atToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected annotation name after '@'");

            List<AnnotationParamNode> annotationParams = new List<AnnotationParamNode>();
            if (Check(TokenType.LeftParen))
            {
                Advance(); // consume "("
                if (!Check(TokenType.RightParen))
                {
                    annotationParams.Add(ParseAnnotationParam());
                    while (Match(TokenType.Comma))
                    {
                        annotationParams.Add(ParseAnnotationParam());
                    }
                }
                Expect(TokenType.RightParen, "Expected ')' after annotation parameters");
            }

            AnnotationNode annotation = new AnnotationNode(idToken.Value, annotationParams, line);
            AstNode innerStatement = ParseStatement();
            return new AnnotatedStatementNode(annotation, innerStatement, line);
        }

        // <annotation_param> ::= <id> "=" <expr>
        private AnnotationParamNode ParseAnnotationParam()
        {
            Token idToken = Expect(TokenType.Identifier, "Expected parameter name in annotation");
            int line = idToken.Line;
            Expect(TokenType.SingleEqual, "Expected '=' in annotation parameter");
            AstNode valueExpression = ParseExpression();
            return new AnnotationParamNode(idToken.Value, valueExpression, line);
        }

        // ================================================================
        // Expression parsing  (precedence climbing)
        // ================================================================

        // <expr> ::= <or_expr> "?" <expr> ":" <expr> | <or_expr>
        // Right-associative ternary.
        private AstNode ParseExpression()
        {
            AstNode left = ParseOrExpression();

            if (Check(TokenType.Question))
            {
                int line = Peek().Line;
                Advance(); // consume "?"
                AstNode thenExpression = ParseExpression();
                Expect(TokenType.Colon, "Expected ':' in ternary expression");
                AstNode elseExpression = ParseExpression();
                return new ConditionalExprNode(left, thenExpression, elseExpression, line);
            }

            return left;
        }

        // <or_expr> ::= <or_expr> ("||" | "or") <and_expr> | <and_expr>
        // Left-associative.
        private AstNode ParseOrExpression()
        {
            AstNode left = ParseAndExpression();

            while (Check(TokenType.PipePipe) || Check(TokenType.Or))
            {
                Token operatorToken = Advance();
                AstNode right = ParseAndExpression();
                left = new BinaryOpNode(left, operatorToken.Value, right, operatorToken.Line);
            }

            return left;
        }

        // <and_expr> ::= <and_expr> ("&&" | "and") <not_expr> | <not_expr>
        // Left-associative.
        private AstNode ParseAndExpression()
        {
            AstNode left = ParseNotExpression();

            while (Check(TokenType.AmpAmp) || Check(TokenType.And))
            {
                Token operatorToken = Advance();
                AstNode right = ParseNotExpression();
                left = new BinaryOpNode(left, operatorToken.Value, right, operatorToken.Line);
            }

            return left;
        }

        // <not_expr> ::= "not" <not_expr> | <comparison_expr>
        // Right-associative unary prefix.
        private AstNode ParseNotExpression()
        {
            if (Check(TokenType.Not))
            {
                Token notToken = Advance();
                AstNode operand = ParseNotExpression();
                return new UnaryOpNode(notToken.Value, operand, notToken.Line);
            }

            return ParseComparisonExpression();
        }

        // <comparison_expr> — non-associative comparison and type operations.
        // Handles ==, !=, <, >, <=, >=, is, as at the same precedence level.
        private AstNode ParseComparisonExpression()
        {
            AstNode left = ParseAdditiveExpression();

            // "is" <type>
            if (Check(TokenType.Is))
            {
                int line = Peek().Line;
                Advance(); // consume "is"
                TypeNode checkedType = ParseType();
                return new TypeCheckNode(left, checkedType, line);
            }

            // "as" <type>
            if (Check(TokenType.As))
            {
                int line = Peek().Line;
                Advance(); // consume "as"
                TypeNode assertedType = ParseType();
                return new TypeAssertNode(left, assertedType, line);
            }

            // Comparison operators (non-associative)
            if (Check(TokenType.EqualEqual) || Check(TokenType.BangEqual) ||
                Check(TokenType.Less) || Check(TokenType.Greater) ||
                Check(TokenType.LessEqual) || Check(TokenType.GreaterEqual))
            {
                Token operatorToken = Advance();
                AstNode right = ParseAdditiveExpression();
                return new BinaryOpNode(left, operatorToken.Value, right, operatorToken.Line);
            }

            return left;
        }

        // <additive_expr> ::= <additive_expr> ("+" | "-" | "&") <multiplicative_expr>
        // Left-associative.
        private AstNode ParseAdditiveExpression()
        {
            AstNode left = ParseMultiplicativeExpression();

            while (Check(TokenType.Plus) || Check(TokenType.Minus) || Check(TokenType.Ampersand))
            {
                Token operatorToken = Advance();
                AstNode right = ParseMultiplicativeExpression();
                left = new BinaryOpNode(left, operatorToken.Value, right, operatorToken.Line);
            }

            return left;
        }

        // <multiplicative_expr> ::= <multiplicative_expr> ("*" | "/" | "%" | "//") <power_expr>
        // Left-associative.
        private AstNode ParseMultiplicativeExpression()
        {
            AstNode left = ParsePowerExpression();

            while (Check(TokenType.Star) || Check(TokenType.Slash) ||
                   Check(TokenType.Percent) || Check(TokenType.SlashSlash))
            {
                Token operatorToken = Advance();
                AstNode right = ParsePowerExpression();
                left = new BinaryOpNode(left, operatorToken.Value, right, operatorToken.Line);
            }

            return left;
        }

        // <power_expr> ::= <unary_expr> "**" <power_expr> | <unary_expr>
        // Right-associative.
        private AstNode ParsePowerExpression()
        {
            AstNode left = ParseUnaryExpression();

            if (Check(TokenType.StarStar))
            {
                Token operatorToken = Advance();
                AstNode right = ParsePowerExpression(); // right-recursive for right-associativity
                return new BinaryOpNode(left, operatorToken.Value, right, operatorToken.Line);
            }

            return left;
        }

        // <unary_expr> ::= "-" <unary_expr> | <postfix_expr>
        private AstNode ParseUnaryExpression()
        {
            if (Check(TokenType.Minus))
            {
                Token minusToken = Advance();
                AstNode operand = ParseUnaryExpression();
                return new UnaryOpNode(minusToken.Value, operand, minusToken.Line);
            }

            return ParsePostfixExpression();
        }

        // <postfix_expr> — left-associative: method call, member access, index, call-via-stored.
        private AstNode ParsePostfixExpression()
        {
            AstNode left = ParsePrimary();

            while (true)
            {
                if (Check(TokenType.Dot))
                {
                    Advance(); // consume "."
                    Token memberToken = Expect(TokenType.Identifier, "Expected member name after '.'");

                    // Method call: "." <id> "(" [arg_list] ")"
                    if (Check(TokenType.LeftParen))
                    {
                        Advance(); // consume "("
                        List<AstNode> arguments = ParseArgList();
                        Expect(TokenType.RightParen, "Expected ')'");
                        left = new MethodCallNode(left, memberToken.Value, arguments, memberToken.Line);
                    }
                    else
                    {
                        // Member access: "." <id>
                        left = new MemberAccessNode(left, memberToken.Value, memberToken.Line);
                    }
                }
                else if (Check(TokenType.LeftBracket))
                {
                    int line = Peek().Line;
                    Advance(); // consume "["
                    AstNode indexExpression = ParseExpression();
                    Expect(TokenType.RightBracket, "Expected ']'");
                    left = new IndexAccessNode(left, indexExpression, line);
                }
                else if (Check(TokenType.LeftParen))
                {
                    // Call via stored value: expr "(" [arg_list] ")"
                    int line = Peek().Line;
                    Advance(); // consume "("
                    List<AstNode> arguments = ParseArgList();
                    Expect(TokenType.RightParen, "Expected ')'");
                    left = new FunctionCallNode(left, arguments, line);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        // ================================================================
        // Primary expressions
        // ================================================================

        private AstNode ParsePrimary()
        {
            TokenType type = Peek().Type;
            int line = Peek().Line;

            // Integer literal
            if (type == TokenType.IntegerLiteral)
            {
                Token token = Advance();
                long value = ParseIntegerValue(token.Value);
                return new IntegerLiteralNode(value, token.Line);
            }

            // Float literal
            if (type == TokenType.FloatLiteral)
            {
                Token token = Advance();
                double value = double.Parse(token.Value, CultureInfo.InvariantCulture);
                return new FloatLiteralNode(value, token.Line);
            }

            // String literal
            if (type == TokenType.StringLiteral)
            {
                Token token = Advance();
                return new StringLiteralNode(token.Value, token.Line);
            }

            // Boolean literals
            if (type == TokenType.True)
            {
                Token token = Advance();
                return new BoolLiteralNode(true, token.Line);
            }
            if (type == TokenType.False)
            {
                Token token = Advance();
                return new BoolLiteralNode(false, token.Line);
            }

            // Null literal
            if (type == TokenType.Null)
            {
                Token token = Advance();
                return new NullLiteralNode(token.Line);
            }

            // "new" <id> "(" [<arg_list>] ")"
            if (type == TokenType.New)
            {
                return ParseNewExpr();
            }

            // "if" <expr> "then" <expr> "else" <expr>  (conditional expression, note 12)
            if (type == TokenType.If)
            {
                return ParseConditionalExpression();
            }

            // "function" — lambda expression (note 14)
            if (type == TokenType.Function)
            {
                return ParseLambdaExpression();
            }

            // Parenthesised expression or cast expression (note 13)
            if (type == TokenType.LeftParen)
            {
                return ParseParenOrCast();
            }

            // Array literal or list comprehension: "[" ... "]"
            if (type == TokenType.LeftBracket)
            {
                return ParseArrayLiteralOrComprehension();
            }

            // Identifier
            if (type == TokenType.Identifier)
            {
                Token token = Advance();
                return new IdentifierNode(token.Value, token.Line);
            }

            throw new ParserException(
                "Expected expression but got '" + Peek().Value + "' (" + Peek().Type + ")",
                Peek().Line);
        }

        // ================================================================
        // Specific primary sub-parsers
        // ================================================================

        // "new" <id> "(" [<arg_list>] ")"
        private AstNode ParseNewExpr()
        {
            Token newToken = Advance(); // consume "new"
            int line = newToken.Line;
            Token idToken = Expect(TokenType.Identifier, "Expected class name after 'new'");
            Expect(TokenType.LeftParen, "Expected '(' after class name in new expression");
            List<AstNode> arguments = ParseArgList();
            Expect(TokenType.RightParen, "Expected ')' after constructor arguments");
            return new NewExprNode(idToken.Value, arguments, line);
        }

        // Conditional expression: "if" <expr> "then" <expr> "else" <expr>  (note 12)
        // This is the inline expression form, not the statement form.
        private AstNode ParseConditionalExpression()
        {
            Token ifToken = Advance(); // consume "if"
            int line = ifToken.Line;
            AstNode condition = ParseExpression();
            Expect(TokenType.Then, "Expected 'then' in conditional expression");
            AstNode thenExpression = ParseExpression();
            Expect(TokenType.Else, "Expected 'else' in conditional expression");
            AstNode elseExpression = ParseExpression();
            return new ConditionalExprNode(condition, thenExpression, elseExpression, line);
        }

        // Lambda expression (note 14):
        //   "function" "(" [<param_list>] ")" ["->" <type>] <expr>             — expression body
        //   "function" "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"  — block body
        private AstNode ParseLambdaExpression()
        {
            Token functionToken = Advance(); // consume "function"
            int line = functionToken.Line;
            Expect(TokenType.LeftParen, "Expected '(' after 'function' in lambda");
            List<ParameterNode> parameters = ParseParamList();
            Expect(TokenType.RightParen, "Expected ')' after lambda parameters");

            TypeNode returnType = null;
            if (Check(TokenType.Arrow))
            {
                Advance(); // consume "->"
                returnType = ParseType();
            }

            // Disambiguation (note 14): if the next token can start a statement,
            // parse as block body (stmt_list ... end); otherwise parse as expression body.
            if (CanStartStatement())
            {
                HashSet<TokenType> stopTokens = new HashSet<TokenType>();
                stopTokens.Add(TokenType.End);
                List<AstNode> statementBody = ParseStatementList(stopTokens);
                Expect(TokenType.End, "Expected 'end' to close block-body lambda");
                return new LambdaExprNode(parameters, returnType, statementBody, line);
            }
            else
            {
                AstNode expressionBody = ParseExpression();
                return new LambdaExprNode(parameters, returnType, expressionBody, line);
            }
        }

        // Parenthesised expression or cast expression (note 13):
        // "(type) expr" is a cast when the first token inside parens is a primitive type
        // keyword or a user-defined type identifier followed by ")".
        private AstNode ParseParenOrCast()
        {
            int line = Peek().Line;

            // Look ahead to determine if this is a cast
            if (IsCastExpression())
            {
                Advance(); // consume "("
                TypeNode targetType = ParseType();
                Expect(TokenType.RightParen, "Expected ')' after cast type");
                AstNode operand = ParseUnaryExpression();
                return new CastExprNode(targetType, operand, line);
            }

            // Grouped expression: "(" <expr> ")"
            Advance(); // consume "("
            AstNode expression = ParseExpression();
            Expect(TokenType.RightParen, "Expected ')'");
            return expression;
        }

        // Determines if the current "(" starts a cast expression.
        // Note 13: A cast is detected when the token inside parens is a primitive type keyword
        // (int, float, string, bool, array, object, null, void) or a user-defined type
        // identifier followed immediately by ")".
        private bool IsCastExpression()
        {
            TokenType insideToken = PeekAt(1).Type;

            // Primitive type keywords in a cast
            if (insideToken == TokenType.Int ||
                insideToken == TokenType.Float ||
                insideToken == TokenType.String ||
                insideToken == TokenType.Bool ||
                insideToken == TokenType.Array ||
                insideToken == TokenType.Object ||
                insideToken == TokenType.Null ||
                insideToken == TokenType.Void ||
                insideToken == TokenType.Map)
            {
                // This is a type keyword inside parens — it's a cast.
                // We need to scan ahead to find the closing paren to be sure.
                // For simple cases like (int), the next token after the type is ")".
                // For complex types like (int[]) or (map<int,string>), we need to
                // find the matching ")".
                return LookAheadTypeClosingParen(1);
            }

            // User-defined type: identifier followed by ")"
            if (insideToken == TokenType.Identifier)
            {
                TokenType afterId = PeekAt(2).Type;
                if (afterId == TokenType.RightParen)
                {
                    // Check that the token after ")" is not a binary operator — if it is,
                    // this is likely a grouped expression, not a cast.
                    // Actually per spec note 13: if identifier followed by ")" then it IS a cast.
                    return true;
                }
                // Could be a generic type in a cast: (List<int>) expr
                if (afterId == TokenType.Less)
                {
                    return LookAheadTypeClosingParen(1);
                }
                // Could be identifier with array suffix: (MyType[]) expr
                if (afterId == TokenType.LeftBracket)
                {
                    return LookAheadTypeClosingParen(1);
                }
                // Could be identifier with nullable suffix: (MyType?) expr
                if (afterId == TokenType.Question)
                {
                    return LookAheadTypeClosingParen(1);
                }
                return false;
            }

            return false;
        }

        // Scan ahead to verify that a type inside parens is followed by ")".
        // Starts scanning from the token at (Position + offset), which should be the first
        // token of the type. Returns true if the type ends with ")".
        private bool LookAheadTypeClosingParen(int offset)
        {
            int index = Position + offset;
            // Skip the base type
            TokenType baseType = Tokens[index].Type;
            index++;

            // If it's "map", skip "<" type "," type ">"
            if (baseType == TokenType.Map)
            {
                if (index < Tokens.Count && Tokens[index].Type == TokenType.Less)
                {
                    index++; // skip "<"
                    index = SkipTypeAhead(index);
                    if (index < Tokens.Count && Tokens[index].Type == TokenType.Comma)
                    {
                        index++; // skip ","
                        index = SkipTypeAhead(index);
                    }
                    if (index < Tokens.Count && Tokens[index].Type == TokenType.Greater)
                    {
                        index++; // skip ">"
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            // If identifier with generic: skip "<" type_list ">"
            else if (baseType == TokenType.Identifier)
            {
                if (index < Tokens.Count && Tokens[index].Type == TokenType.Less)
                {
                    // Try to match generic angle brackets
                    int saved = index;
                    index++; // skip "<"
                    index = SkipTypeAhead(index);
                    while (index < Tokens.Count && Tokens[index].Type == TokenType.Comma)
                    {
                        index++; // skip ","
                        index = SkipTypeAhead(index);
                    }
                    if (index < Tokens.Count && Tokens[index].Type == TokenType.Greater)
                    {
                        index++; // skip ">"
                    }
                    else
                    {
                        // Not a valid generic type — revert
                        index = saved;
                    }
                }
            }

            // Skip type suffixes: "[]" and "?"
            while (index < Tokens.Count)
            {
                if (Tokens[index].Type == TokenType.LeftBracket &&
                    index + 1 < Tokens.Count && Tokens[index + 1].Type == TokenType.RightBracket)
                {
                    index += 2;
                }
                else if (Tokens[index].Type == TokenType.Question)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            // Must end with ")"
            return index < Tokens.Count && Tokens[index].Type == TokenType.RightParen;
        }

        // Skip a single type expression in the token stream for lookahead purposes.
        // Returns the index after the type.
        private int SkipTypeAhead(int index)
        {
            if (index >= Tokens.Count)
            {
                return index;
            }

            TokenType baseType = Tokens[index].Type;

            // Primitive types
            if (baseType == TokenType.Int || baseType == TokenType.Float ||
                baseType == TokenType.String || baseType == TokenType.Bool ||
                baseType == TokenType.Array || baseType == TokenType.Object ||
                baseType == TokenType.Null || baseType == TokenType.Void)
            {
                index++;
            }
            else if (baseType == TokenType.Map)
            {
                index++; // skip "map"
                if (index < Tokens.Count && Tokens[index].Type == TokenType.Less)
                {
                    index++; // skip "<"
                    index = SkipTypeAhead(index);
                    if (index < Tokens.Count && Tokens[index].Type == TokenType.Comma)
                    {
                        index++;
                        index = SkipTypeAhead(index);
                    }
                    if (index < Tokens.Count && Tokens[index].Type == TokenType.Greater)
                    {
                        index++;
                    }
                }
            }
            else if (baseType == TokenType.Identifier)
            {
                index++; // skip identifier
                if (index < Tokens.Count && Tokens[index].Type == TokenType.Less)
                {
                    // Generic type — scan for matching >
                    int depth = 1;
                    index++;
                    while (index < Tokens.Count && depth > 0)
                    {
                        if (Tokens[index].Type == TokenType.Less)
                        {
                            depth++;
                        }
                        else if (Tokens[index].Type == TokenType.Greater)
                        {
                            depth--;
                        }
                        index++;
                    }
                }
            }
            else
            {
                return index; // not a recognized type token
            }

            // Skip type suffixes: "[]" and "?"
            while (index < Tokens.Count)
            {
                if (Tokens[index].Type == TokenType.LeftBracket &&
                    index + 1 < Tokens.Count && Tokens[index + 1].Type == TokenType.RightBracket)
                {
                    index += 2;
                }
                else if (Tokens[index].Type == TokenType.Question)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }

            return index;
        }

        // Array literal or list comprehension
        private AstNode ParseArrayLiteralOrComprehension()
        {
            Token bracketToken = Advance(); // consume "["
            int line = bracketToken.Line;

            // Empty array
            if (Check(TokenType.RightBracket))
            {
                Advance(); // consume "]"
                return new ArrayLiteralNode(new List<AstNode>(), line);
            }

            // Parse the first expression
            AstNode firstExpression = ParseExpression();

            // Check for list comprehension: "[" <expr> "for" <id> "in" <expr> "]"
            if (Check(TokenType.For))
            {
                Advance(); // consume "for"
                Token loopVarToken = Expect(TokenType.Identifier, "Expected variable in list comprehension");
                Expect(TokenType.In, "Expected 'in' in list comprehension");
                AstNode sourceExpression = ParseExpression();
                Expect(TokenType.RightBracket, "Expected ']' to close list comprehension");
                return new ListComprehensionNode(firstExpression, loopVarToken.Value, sourceExpression, line);
            }

            // Regular array literal
            List<AstNode> elements = new List<AstNode>();
            elements.Add(firstExpression);

            while (Match(TokenType.Comma))
            {
                elements.Add(ParseExpression());
            }

            Expect(TokenType.RightBracket, "Expected ']' to close array literal");
            return new ArrayLiteralNode(elements, line);
        }

        // ================================================================
        // Type parsing
        // ================================================================

        // <type> ::= <base_type> { <type_suffix> }
        private TypeNode ParseType()
        {
            int line = Peek().Line;
            string typeName;
            List<TypeNode> typeArguments = new List<TypeNode>();

            TokenType tokenType = Peek().Type;

            // Primitive type keywords
            if (tokenType == TokenType.Int || tokenType == TokenType.Float ||
                tokenType == TokenType.String || tokenType == TokenType.Bool ||
                tokenType == TokenType.Array || tokenType == TokenType.Object ||
                tokenType == TokenType.Null || tokenType == TokenType.Void)
            {
                Token typeToken = Advance();
                typeName = typeToken.Value;
            }
            // Map type: "map" "<" <type> "," <type> ">"
            else if (tokenType == TokenType.Map)
            {
                Token mapToken = Advance(); // consume "map"
                typeName = mapToken.Value;
                Expect(TokenType.Less, "Expected '<' after 'map'");
                TypeNode keyType = ParseType();
                Expect(TokenType.Comma, "Expected ',' between map key and value types");
                TypeNode valueType = ParseType();
                Expect(TokenType.Greater, "Expected '>' to close map type");
                typeArguments.Add(keyType);
                typeArguments.Add(valueType);
            }
            // User-defined type or generic type (note 20)
            else if (tokenType == TokenType.Identifier)
            {
                Token idToken = Advance();
                typeName = idToken.Value;

                // Generic type: <id> "<" <type_list> ">"
                if (Check(TokenType.Less))
                {
                    // Look ahead to see if this is actually a generic type
                    if (IsGenericTypeArgList())
                    {
                        Advance(); // consume "<"
                        typeArguments.Add(ParseType());
                        while (Match(TokenType.Comma))
                        {
                            typeArguments.Add(ParseType());
                        }
                        Expect(TokenType.Greater, "Expected '>' to close generic type");
                    }
                }
            }
            else
            {
                throw new ParserException(
                    "Expected type but got '" + Peek().Value + "'",
                    Peek().Line);
            }

            // Type suffixes: "[]" for array and "?" for nullable
            int arrayDimensions = 0;
            bool isNullable = false;

            while (true)
            {
                if (Check(TokenType.LeftBracket) && PeekAt(1).Type == TokenType.RightBracket)
                {
                    Advance(); // consume "["
                    Advance(); // consume "]"
                    arrayDimensions++;
                }
                else if (Check(TokenType.Question))
                {
                    Advance(); // consume "?"
                    isNullable = true;
                }
                else
                {
                    break;
                }
            }

            return new TypeNode(typeName, typeArguments, arrayDimensions, isNullable, line);
        }

        // Disambiguation (note 20): determines if "<" after an identifier in type context
        // is the start of generic type arguments vs a less-than comparison.
        // Strategy: scan ahead for valid type tokens followed by ">".
        private bool IsGenericTypeArgList()
        {
            int index = Position + 1; // one past the "<"
            int depth = 1;

            while (index < Tokens.Count && depth > 0)
            {
                TokenType tokenType = Tokens[index].Type;

                if (tokenType == TokenType.Less)
                {
                    depth++;
                }
                else if (tokenType == TokenType.Greater)
                {
                    depth--;
                    if (depth == 0)
                    {
                        return true; // Found matching ">"
                    }
                }
                else if (tokenType == TokenType.Comma ||
                         tokenType == TokenType.LeftBracket ||
                         tokenType == TokenType.RightBracket ||
                         tokenType == TokenType.Question ||
                         IsTypeToken(tokenType))
                {
                    // These are valid inside a generic type argument list
                }
                else
                {
                    // Found a token that cannot appear in a type argument list
                    return false;
                }

                index++;
            }

            return false;
        }

        // Returns true if the token type can appear as the start of a type.
        private bool IsTypeToken(TokenType tokenType)
        {
            switch (tokenType)
            {
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
                    return true;
                default:
                    return false;
            }
        }

        // ================================================================
        // Integer parsing helper
        // ================================================================

        // Parses an integer literal value from its string representation.
        // Handles decimal, binary (0b), octal (0o), and hex (0x) forms.
        private static long ParseIntegerValue(string text)
        {
            if (text.Length > 2)
            {
                string prefix = text.Substring(0, 2).ToLowerInvariant();
                if (prefix == "0b")
                {
                    return Convert.ToInt64(text.Substring(2), 2);
                }
                if (prefix == "0o")
                {
                    return Convert.ToInt64(text.Substring(2), 8);
                }
                if (prefix == "0x")
                {
                    return Convert.ToInt64(text.Substring(2), 16);
                }
            }
            return long.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}
