using System;
using System.Collections.Generic;
using System.Globalization;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Lexer;

/// <summary>
/// Recursive-descent parser that converts a flat token stream into a
/// <see cref="ProgramNode"/> AST. Implements every BNF production from
/// Build.Solution.md and honours all 23 disambiguation notes.
/// </summary>
public sealed class Parser
{
    // The flat token list produced by the lexer.
    private readonly IReadOnlyList<Token> Tokens;

    // Current position within the token list.
    private int Position;

    /// <summary>Initialises the parser with the given token stream.</summary>
    public Parser(IReadOnlyList<Token> tokens)
    {
        Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        Position = 0;
    }

    // ================================================================
    // Public entry point
    // ================================================================

    /// <summary>Parses the entire token stream and returns the program AST.</summary>
    public ProgramNode Parse()
    {
        int startLine = Tokens.Count > 0 ? Tokens[0].Line : 1;
        List<AstNode> statements = ParseStatementList(null);
        Expect(TokenType.EndOfFile, "Expected end of file");
        return new ProgramNode(statements, startLine);
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
    // If the offset exceeds the stream, returns the last (EndOfFile) token.
    private Token PeekAt(int offset)
    {
        int index = Position + offset;
        if (index >= Tokens.Count)
        {
            return Tokens[Tokens.Count - 1];
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
        Token current = Peek();
        throw new ParserException(
            errorMessage + " but got '" + current.Value + "' (" + current.Type + ")",
            current.Line);
    }

    // The wildcard pattern token "_" arrives as TokenType.Unknown with value "_"
    // because the lexer forbids identifiers starting with '_'. This helper
    // centralises the check.
    private bool IsWildcardToken(Token token)
    {
        return token.Type == TokenType.Unknown && token.Value == "_";
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
    // Statement list parsing (notes 3, 12, 21)
    // ================================================================

    // Parses a list of statements separated by semicolons. The list stops when
    // the current token is in stopTokens, is EndOfFile, or cannot begin a
    // statement. Trailing semicolons before block-ending keywords are errors.
    private List<AstNode> ParseStatementList(HashSet<TokenType> stopTokens)
    {
        List<AstNode> statements = new List<AstNode>();

        while (true)
        {
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

            // Semicolons are separators; note 21: a trailing semicolon before a
            // block terminator is a parse error.
            if (Check(TokenType.Semicolon))
            {
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
                Advance();
            }
        }

        return statements;
    }

    // ================================================================
    // Statement dispatch
    // ================================================================

    private AstNode ParseStatement()
    {
        TokenType type = Peek().Type;

        switch (type)
        {
            case TokenType.Let:        return ParseLetDeclare();
            case TokenType.Var:        return ParseVarDeclare();
            case TokenType.Const:      return ParseConstDeclare();
            case TokenType.Enum:       return ParseEnumDef();
            case TokenType.If:         return ParseIfStatement();
            case TokenType.While:      return ParseWhileStatement();
            case TokenType.For:        return ParseForStatement();
            case TokenType.Foreach:    return ParseForeachStatement();
            case TokenType.Do:         return ParseDoStatement();
            case TokenType.Switch:     return ParseSwitchStatement();
            case TokenType.Break:      return ParseBreakStatement();
            case TokenType.Continue:   return ParseContinueStatement();
            case TokenType.Print:      return ParsePrintStatement();
            case TokenType.Input:      return ParseInputStatement();
            case TokenType.Function:   return ParseFunctionDef();
            case TokenType.Static:     return ParseStaticDef();
            case TokenType.Return:     return ParseReturnStatement();
            case TokenType.Class:      return ParseClassDef(false);
            case TokenType.Module:     return ParseModuleDef();
            case TokenType.Import:     return ParseImportStatement();
            case TokenType.Export:     return ParseExportStatement();
            case TokenType.Try:        return ParseTryStatement();
            case TokenType.Throw:      return ParseThrowStatement();
            case TokenType.Match:      return ParsePatternMatch();
            case TokenType.At:         return ParseAnnotatedStatement();
            case TokenType.Identifier: return ParseIdentifierStatement();

            default:
                throw new ParserException(
                    "Unexpected token '" + Peek().Value + "' at start of statement",
                    Peek().Line);
        }
    }

    // ================================================================
    // Identifier-led statements (assign / array assign / call)
    // ================================================================

    // An identifier at the start of a statement may begin one of three forms:
    //   assign_stmt:       <id> ":=" <expr>
    //   array_assign_stmt: <id> "[" <expr> "]" ":=" <expr>
    //   call_stmt:         <id> { "." <id> } "(" <arg_list>? ")"
    private AstNode ParseIdentifierStatement()
    {
        Token idToken = Expect(TokenType.Identifier, "Expected identifier");
        int line = idToken.Line;
        string name = idToken.Value;

        // Array element assignment: id "[" expr "]" ":="
        if (Check(TokenType.LeftBracket))
        {
            Advance();
            AstNode indexExpression = ParseExpression();
            Expect(TokenType.RightBracket, "Expected ']'");
            Expect(TokenType.Assign, "Expected ':=' in array element assignment");
            AstNode valueExpression = ParseExpression();
            return new ArrayElementAssignNode(name, indexExpression, valueExpression, line);
        }

        // Simple assignment: id ":="
        if (Check(TokenType.Assign))
        {
            Advance();
            AstNode valueExpression = ParseExpression();
            return new AssignStatementNode(name, valueExpression, line);
        }

        // Call statement: id { "." id } "(" [arg_list] ")"   (note 22)
        List<string> memberPath = new List<string>();
        memberPath.Add(name);

        while (Check(TokenType.Dot))
        {
            Advance();
            Token memberToken = Expect(TokenType.Identifier, "Expected member name after '.'");
            memberPath.Add(memberToken.Value);
        }

        Expect(TokenType.LeftParen,
            "Expected '(' for call statement, ':=' for assignment, or '[' for array assignment");
        List<AstNode> arguments = ParseArgList();
        Expect(TokenType.RightParen, "Expected ')'");

        return new CallStatementNode(memberPath, arguments, line);
    }

    // ================================================================
    // Declarations
    // ================================================================

    // "let" <id> ":=" <expr>
    // "let" <id> ":" <type> ":=" <expr>
    private AstNode ParseLetDeclare()
    {
        Token letToken = Advance();
        int line = letToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected identifier after 'let'");
        string name = idToken.Value;

        TypeNode declaredType = null;
        if (Match(TokenType.Colon))
        {
            declaredType = ParseType();
        }

        Expect(TokenType.Assign, "Expected ':=' in let declaration");
        AstNode initialiser = ParseExpression();
        return new LetDeclareNode(name, declaredType, initialiser, line);
    }

    // "var" <id> ":" <type> ":=" <expr>
    private AstNode ParseVarDeclare()
    {
        Token varToken = Advance();
        int line = varToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected identifier after 'var'");
        string name = idToken.Value;
        Expect(TokenType.Colon, "Expected ':' with type annotation in var declaration");
        TypeNode declaredType = ParseType();
        Expect(TokenType.Assign, "Expected ':=' in var declaration");
        AstNode initialiser = ParseExpression();
        return new VarDeclareNode(name, declaredType, initialiser, line);
    }

    // "const" <id> ":=" <expr>
    // "const" <id> ":" <type> ":=" <expr>
    private AstNode ParseConstDeclare()
    {
        Token constToken = Advance();
        int line = constToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected identifier after 'const'");
        string name = idToken.Value;

        TypeNode declaredType = null;
        if (Match(TokenType.Colon))
        {
            declaredType = ParseType();
        }

        Expect(TokenType.Assign, "Expected ':=' in const declaration");
        AstNode initialiser = ParseExpression();
        return new ConstDeclareNode(name, declaredType, initialiser, line);
    }

    // "enum" <id> "{" <enum_value_list> "}"
    // Enum member bodies use "=" (SingleEqual), not ":=".
    private AstNode ParseEnumDef()
    {
        Token enumToken = Advance();
        int line = enumToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected enum name");
        string name = idToken.Value;
        Expect(TokenType.LeftBrace, "Expected '{' after enum name");

        List<EnumMember> members = new List<EnumMember>();
        while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
        {
            Token memberToken = Expect(TokenType.Identifier, "Expected enum member name");
            AstNode valueExpression = null;

            if (Match(TokenType.SingleEqual))
            {
                valueExpression = ParseExpression();
            }

            members.Add(new EnumMember(memberToken.Value, valueExpression));

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
        Token ifToken = Advance();
        int line = ifToken.Line;
        AstNode condition = ParseExpression();
        Expect(TokenType.Then, "Expected 'then' after if condition");

        HashSet<TokenType> thenStop = new HashSet<TokenType> { TokenType.Else, TokenType.End };
        List<AstNode> thenBody = ParseStatementList(thenStop);

        List<AstNode> elseBody = new List<AstNode>();
        if (Match(TokenType.Else))
        {
            // "else if" is a common chained conditional: parse the nested if
            // statement as the entire else-body. The nested if consumes its
            // own 'end', so we do NOT consume another one here.
            if (Check(TokenType.If))
            {
                AstNode nestedIf = ParseIfStatement();
                elseBody.Add(nestedIf);
                return new IfStatementNode(condition, thenBody, elseBody, line);
            }
            HashSet<TokenType> elseStop = new HashSet<TokenType> { TokenType.End };
            elseBody = ParseStatementList(elseStop);
        }

        Expect(TokenType.End, "Expected 'end' to close if statement");
        return new IfStatementNode(condition, thenBody, elseBody, line);
    }

    // "while" <expr> "do" <stmt_list> "end"
    private AstNode ParseWhileStatement()
    {
        Token whileToken = Advance();
        int line = whileToken.Line;
        AstNode condition = ParseExpression();
        Expect(TokenType.Do, "Expected 'do' after while condition");

        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.End, "Expected 'end' to close while loop");
        return new WhileStatementNode(condition, body, line);
    }

    // "for" <id> ":=" <expr> "to" <expr> ["step" <expr>] "do" <stmt_list> "end"
    private AstNode ParseForStatement()
    {
        Token forToken = Advance();
        int line = forToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected loop variable after 'for'");
        string loopVariable = idToken.Value;
        Expect(TokenType.Assign, "Expected ':=' after for loop variable");
        AstNode startExpression = ParseExpression();
        Expect(TokenType.To, "Expected 'to' in for loop");
        AstNode endExpression = ParseExpression();

        AstNode stepExpression = null;
        if (Match(TokenType.Step))
        {
            stepExpression = ParseExpression();
        }

        Expect(TokenType.Do, "Expected 'do' in for loop");
        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.End, "Expected 'end' to close for loop");
        return new ForStatementNode(loopVariable, startExpression, endExpression, stepExpression, body, line);
    }

    // "foreach" <id> "in" <expr> "do" <stmt_list> "end"
    private AstNode ParseForeachStatement()
    {
        Token foreachToken = Advance();
        int line = foreachToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected variable name after 'foreach'");
        string elementVariable = idToken.Value;
        Expect(TokenType.In, "Expected 'in' after foreach variable");
        AstNode iterableExpression = ParseExpression();
        Expect(TokenType.Do, "Expected 'do' in foreach loop");

        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.End, "Expected 'end' to close foreach loop");
        return new ForeachStatementNode(elementVariable, iterableExpression, body, line);
    }

    // "do" <stmt_list> "while" <expr>   — do-while (note 2)
    // or "do" <stmt_list> "end"          — plain do-block
    // The body's stmt_list stops at "while" or "end"; the keyword that actually
    // appears decides which form was used.
    private AstNode ParseDoStatement()
    {
        Token doToken = Advance();
        int line = doToken.Line;

        HashSet<TokenType> stop = new HashSet<TokenType>
        {
            TokenType.While,
            TokenType.End
        };

        List<AstNode> body = ParseStatementList(stop);

        if (Match(TokenType.While))
        {
            AstNode condition = ParseExpression();
            return new DoWhileStatementNode(body, condition, line);
        }

        Expect(TokenType.End, "Expected 'while' or 'end' after do block");
        return new DoBlockNode(body, line);
    }

    // "switch" <expr> "{" <case_list> "}"
    private AstNode ParseSwitchStatement()
    {
        Token switchToken = Advance();
        int line = switchToken.Line;
        AstNode subjectExpression = ParseExpression();
        Expect(TokenType.LeftBrace, "Expected '{' after switch expression");

        List<SwitchCaseNode> cases = new List<SwitchCaseNode>();

        // The stmt_list inside each case stops naturally at "case", "default", or "}".
        HashSet<TokenType> caseStop = new HashSet<TokenType>
        {
            TokenType.Case,
            TokenType.Default,
            TokenType.RightBrace
        };

        while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
        {
            if (Check(TokenType.Case))
            {
                Advance();
                AstNode caseValue = ParseExpression();
                Expect(TokenType.Colon, "Expected ':' after case value");
                List<AstNode> caseBody = ParseStatementList(caseStop);
                cases.Add(new SwitchCaseNode(false, caseValue, caseBody));
            }
            else if (Check(TokenType.Default))
            {
                Advance();
                Expect(TokenType.Colon, "Expected ':' after 'default'");
                List<AstNode> defaultBody = ParseStatementList(caseStop);
                cases.Add(new SwitchCaseNode(true, null, defaultBody));
            }
            else
            {
                throw new ParserException(
                    "Expected 'case' or 'default' in switch body",
                    Peek().Line);
            }
        }

        if (cases.Count == 0)
        {
            throw new ParserException(
                "Switch statement requires at least one case or default clause",
                line);
        }

        Expect(TokenType.RightBrace, "Expected '}' to close switch statement");
        return new SwitchStatementNode(subjectExpression, cases, line);
    }

    private AstNode ParseBreakStatement()
    {
        Token breakToken = Advance();
        return new BreakStatementNode(breakToken.Line);
    }

    private AstNode ParseContinueStatement()
    {
        Token continueToken = Advance();
        return new ContinueStatementNode(continueToken.Line);
    }

    // ================================================================
    // I/O statements
    // ================================================================

    // "print" <expr>
    private AstNode ParsePrintStatement()
    {
        Token printToken = Advance();
        int line = printToken.Line;
        AstNode expression = ParseExpression();
        return new PrintStatementNode(expression, line);
    }

    // "input" <id>
    private AstNode ParseInputStatement()
    {
        Token inputToken = Advance();
        int line = inputToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected variable name after 'input'");
        return new InputStatementNode(idToken.Value, line);
    }

    // ================================================================
    // Functions
    // ================================================================

    // "function" <id> "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"
    private AstNode ParseFunctionDef()
    {
        Token functionToken = Advance();
        int line = functionToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected function name");
        string functionName = idToken.Value;

        Expect(TokenType.LeftParen, "Expected '(' after function name");
        List<ParameterNode> parameters = ParseParamList();
        Expect(TokenType.RightParen, "Expected ')' after parameter list");

        TypeNode returnType = null;
        if (Match(TokenType.Arrow))
        {
            returnType = ParseType();
        }

        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.End, "Expected 'end' to close function definition");
        return new FunctionDefNode(functionName, parameters, returnType, body, line);
    }

    // "static" can precede "function" or "class" (note 16). At the top level
    // "static function" is uncommon but we parse it as an ordinary
    // FunctionDefNode (static has no effect outside of a class body).
    private AstNode ParseStaticDef()
    {
        Advance(); // consume "static"
        if (Check(TokenType.Function))
        {
            return ParseFunctionDef();
        }
        if (Check(TokenType.Class))
        {
            return ParseClassDef(true);
        }
        throw new ParserException(
            "Expected 'function' or 'class' after 'static'",
            Peek().Line);
    }

    // "return" [<expr>]   (note 4: bare return is legal)
    private AstNode ParseReturnStatement()
    {
        Token returnToken = Advance();
        int line = returnToken.Line;

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

    // <param_list> ::= <param> "," <param_list> | <param>
    private List<ParameterNode> ParseParamList()
    {
        List<ParameterNode> parameters = new List<ParameterNode>();

        if (Check(TokenType.RightParen))
        {
            return parameters;
        }

        parameters.Add(ParseParam());
        while (Match(TokenType.Comma))
        {
            parameters.Add(ParseParam());
        }

        return parameters;
    }

    // <param> ::= <id>
    //           | <id> ":=" <expr>
    //           | <id> ":" <type>
    //           | <id> ":" <type> ":=" <expr>
    private ParameterNode ParseParam()
    {
        Token idToken = Expect(TokenType.Identifier, "Expected parameter name");
        int line = idToken.Line;

        TypeNode declaredType = null;
        AstNode defaultExpression = null;

        if (Match(TokenType.Colon))
        {
            declaredType = ParseType();
        }

        if (Match(TokenType.Assign))
        {
            defaultExpression = ParseExpression();
        }

        return new ParameterNode(idToken.Value, declaredType, defaultExpression, line);
    }

    // <arg_list> ::= <expr> "," <arg_list> | <expr>
    private List<AstNode> ParseArgList()
    {
        List<AstNode> arguments = new List<AstNode>();
        if (Check(TokenType.RightParen))
        {
            return arguments;
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
            Token firstInterfaceToken = Expect(TokenType.Identifier, "Expected interface name after 'implements'");
            implementedInterfaces.Add(firstInterfaceToken.Value);
            while (Match(TokenType.Comma))
            {
                Token nextInterfaceToken = Expect(TokenType.Identifier, "Expected interface name");
                implementedInterfaces.Add(nextInterfaceToken.Value);
            }
        }

        Expect(TokenType.LeftBrace, "Expected '{' to open class body");

        List<AstNode> members = new List<AstNode>();
        while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
        {
            AstNode member = ParseClassMember();
            members.Add(member);

            if (Check(TokenType.Semicolon))
            {
                TokenType afterSemicolon = PeekAt(1).Type;
                if (afterSemicolon == TokenType.RightBrace || afterSemicolon == TokenType.EndOfFile)
                {
                    throw new ParserException(
                        "Trailing semicolon before '}' is not allowed",
                        Peek().Line);
                }
                Advance();
            }
        }

        Expect(TokenType.RightBrace, "Expected '}' to close class body");
        return new ClassDefNode(isStatic, className, baseClassName, implementedInterfaces, members, line);
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
            Token staticToken = Peek();
            if (PeekAt(1).Type == TokenType.Function)
            {
                Advance(); // consume "static"
                return ParseMethodDef(true);
            }
            throw new ParserException(
                "Expected 'function' after 'static' in class body",
                staticToken.Line);
        }

        if (Check(TokenType.Function))
        {
            return ParseMethodDef(false);
        }

        if (Check(TokenType.Let) || Check(TokenType.Var) || Check(TokenType.Const))
        {
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

    // [static] "function" <id> "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"
    // Parses the method body form used inside a class body.
    private AstNode ParseMethodDef(bool isStatic)
    {
        Token functionToken = Advance(); // consume "function"
        int line = functionToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected method name");

        Expect(TokenType.LeftParen, "Expected '(' after method name");
        List<ParameterNode> parameters = ParseParamList();
        Expect(TokenType.RightParen, "Expected ')' after parameter list");

        TypeNode returnType = null;
        if (Match(TokenType.Arrow))
        {
            returnType = ParseType();
        }

        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.End, "Expected 'end' to close method definition");
        return new MethodDefNode(isStatic, idToken.Value, parameters, returnType, body, line);
    }

    // "Constructor" "(" [<param_list>] ")" <stmt_list> "end"
    private AstNode ParseConstructorDef()
    {
        Token constructorToken = Advance(); // consume "Constructor"
        int line = constructorToken.Line;
        Expect(TokenType.LeftParen, "Expected '(' after 'Constructor'");
        List<ParameterNode> parameters = ParseParamList();
        Expect(TokenType.RightParen, "Expected ')' after constructor parameter list");

        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.End, "Expected 'end' to close constructor definition");
        return new ConstructorDefNode(parameters, body, line);
    }

    // Field declaration inside a class body — one of the five forms documented
    // in the BNF <field_declare_stmt>.
    private AstNode ParseFieldDeclare()
    {
        Token keywordToken = Advance(); // "let", "var", or "const"
        string keyword = keywordToken.Value;
        int line = keywordToken.Line;

        Token idToken = Expect(TokenType.Identifier,
            "Expected field name after '" + keyword + "'");
        string fieldName = idToken.Value;

        TypeNode declaredType = null;

        if (keyword == "var")
        {
            Expect(TokenType.Colon, "Expected ':' with type annotation in var declaration");
            declaredType = ParseType();
        }
        else if (Match(TokenType.Colon))
        {
            declaredType = ParseType();
        }

        Expect(TokenType.Assign, "Expected ':=' in field declaration");
        AstNode initialiser = ParseExpression();
        return new FieldDeclareNode(keyword, fieldName, declaredType, initialiser, line);
    }

    // ================================================================
    // Modules
    // ================================================================

    // "module" <id> ["import" <module_import_list>] "{" <stmt_list> "}"
    private AstNode ParseModuleDef()
    {
        Token moduleToken = Advance();
        int line = moduleToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected module name");
        string moduleName = idToken.Value;

        List<ModuleImport> headerImports = new List<ModuleImport>();
        if (Match(TokenType.Import))
        {
            headerImports = ParseModuleImportList();
        }

        Expect(TokenType.LeftBrace, "Expected '{' to open module body");
        HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.RightBrace };
        List<AstNode> body = ParseStatementList(stop);
        Expect(TokenType.RightBrace, "Expected '}' to close module body");
        return new ModuleDefNode(moduleName, headerImports, body, line);
    }

    // <module_import_list> ::= <module_import> "," <module_import_list> | <module_import>
    private List<ModuleImport> ParseModuleImportList()
    {
        List<ModuleImport> imports = new List<ModuleImport>();
        imports.Add(ParseModuleImport());
        while (Match(TokenType.Comma))
        {
            imports.Add(ParseModuleImport());
        }
        return imports;
    }

    // <module_import> ::= <id> | <id> "as" <id>
    private ModuleImport ParseModuleImport()
    {
        Token idToken = Expect(TokenType.Identifier, "Expected module name in import");
        string alias = null;
        if (Match(TokenType.As))
        {
            Token aliasToken = Expect(TokenType.Identifier, "Expected alias after 'as'");
            alias = aliasToken.Value;
        }
        return new ModuleImport(idToken.Value, alias);
    }

    // "import" <id> ["as" <id>]
    private AstNode ParseImportStatement()
    {
        Token importToken = Advance();
        int line = importToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected module name after 'import'");
        string alias = null;
        if (Match(TokenType.As))
        {
            Token aliasToken = Expect(TokenType.Identifier, "Expected alias after 'as'");
            alias = aliasToken.Value;
        }
        return new ImportStatementNode(idToken.Value, alias, line);
    }

    // "export" <id>
    private AstNode ParseExportStatement()
    {
        Token exportToken = Advance();
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
        Token tryToken = Advance();
        int line = tryToken.Line;

        HashSet<TokenType> tryStop = new HashSet<TokenType> { TokenType.Catch };
        List<AstNode> tryBody = ParseStatementList(tryStop);

        Expect(TokenType.Catch, "Expected 'catch' in try statement");

        string catchVariableName;
        TypeNode catchVariableType = null;

        if (Match(TokenType.LeftParen))
        {
            Token idToken = Expect(TokenType.Identifier, "Expected exception variable name");
            catchVariableName = idToken.Value;
            Expect(TokenType.Colon, "Expected ':' after exception variable");
            catchVariableType = ParseType();
            Expect(TokenType.RightParen, "Expected ')' after exception type");
        }
        else
        {
            Token idToken = Expect(TokenType.Identifier, "Expected exception variable name after 'catch'");
            catchVariableName = idToken.Value;
        }

        HashSet<TokenType> catchStop = new HashSet<TokenType> { TokenType.Finally, TokenType.End };
        List<AstNode> catchBody = ParseStatementList(catchStop);

        List<AstNode> finallyBody = new List<AstNode>();
        if (Match(TokenType.Finally))
        {
            HashSet<TokenType> finallyStop = new HashSet<TokenType> { TokenType.End };
            finallyBody = ParseStatementList(finallyStop);
        }

        Expect(TokenType.End, "Expected 'end' to close try statement");
        return new TryStatementNode(tryBody, catchVariableName, catchVariableType, catchBody, finallyBody, line);
    }

    // "throw" <expr>
    private AstNode ParseThrowStatement()
    {
        Token throwToken = Advance();
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
        Token matchToken = Advance();
        int line = matchToken.Line;
        AstNode subjectExpression = ParseExpression();
        Expect(TokenType.LeftBrace, "Expected '{' after match expression");

        List<PatternCaseNode> cases = new List<PatternCaseNode>();

        while (!Check(TokenType.RightBrace) && !Check(TokenType.EndOfFile))
        {
            PatternNode pattern = ParsePattern();
            AstNode whenGuard = null;

            if (Match(TokenType.When))
            {
                whenGuard = ParseExpression();
            }

            Expect(TokenType.FatArrow, "Expected '=>' after pattern");
            List<AstNode> caseBody = ParsePatternCaseBody();
            cases.Add(new PatternCaseNode(pattern, whenGuard, caseBody, pattern.Line));
        }

        Expect(TokenType.RightBrace, "Expected '}' to close match expression");
        return new PatternMatchNode(subjectExpression, cases, line);
    }

    // Parse the body of a pattern case. Stops before the next pattern or '}'.
    private List<AstNode> ParsePatternCaseBody()
    {
        List<AstNode> statements = new List<AstNode>();

        while (true)
        {
            if (Check(TokenType.EndOfFile) || Check(TokenType.RightBrace))
            {
                break;
            }

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

            if (Check(TokenType.Semicolon))
            {
                TokenType afterSemicolon = PeekAt(1).Type;
                if (afterSemicolon == TokenType.RightBrace || afterSemicolon == TokenType.EndOfFile)
                {
                    throw new ParserException(
                        "Trailing semicolon before block terminator is not allowed",
                        Peek().Line);
                }
                Advance();
            }
        }

        return statements;
    }

    // Heuristic: is the current token the start of a new pattern case arm?
    // Patterns arrive before "=>" or "when ... =>"; this look-ahead avoids
    // conflating patterns with statement starts inside a match body.
    private bool IsPatternCaseStart()
    {
        Token current = Peek();

        if (IsWildcardToken(current))
        {
            return true;
        }

        TokenType currentType = current.Type;

        if (currentType == TokenType.IntegerLiteral ||
            currentType == TokenType.FloatLiteral ||
            currentType == TokenType.StringLiteral ||
            currentType == TokenType.True ||
            currentType == TokenType.False ||
            currentType == TokenType.Null)
        {
            TokenType next = PeekAt(1).Type;
            return next == TokenType.FatArrow || next == TokenType.When || next == TokenType.Pipe;
        }

        if (currentType == TokenType.Identifier)
        {
            TokenType next = PeekAt(1).Type;
            if (next == TokenType.FatArrow || next == TokenType.When || next == TokenType.Pipe)
            {
                return true;
            }
            if (next == TokenType.LeftParen)
            {
                return LookAheadBalancedToArm(2, TokenType.LeftParen, TokenType.RightParen);
            }
            if (next == TokenType.LeftBrace)
            {
                return LookAheadBalancedToArm(2, TokenType.LeftBrace, TokenType.RightBrace);
            }
            return false;
        }

        if (currentType == TokenType.LeftBracket)
        {
            return LookAheadBalancedToArm(1, TokenType.LeftBracket, TokenType.RightBracket);
        }

        return false;
    }

    // Scan forward from offset, tracking open/close of the given bracket kind,
    // and report whether the token following the match is "=>", "when", or "|".
    private bool LookAheadBalancedToArm(int offset, TokenType open, TokenType close)
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
            if (tokenType == open)
            {
                depth++;
            }
            else if (tokenType == close)
            {
                depth--;
            }
            index++;
        }
        TokenType after = PeekAt(index).Type;
        return after == TokenType.FatArrow || after == TokenType.When || after == TokenType.Pipe;
    }

    // ================================================================
    // Pattern parsing (note 23 — left-associative alternation)
    // ================================================================

    // <pattern> ::= <primary_pattern> { "|" <primary_pattern> }
    private PatternNode ParsePattern()
    {
        PatternNode left = ParsePrimaryPattern();
        while (Match(TokenType.Pipe))
        {
            PatternNode right = ParsePrimaryPattern();
            left = new PatternNode(left, right, left.Line);
        }
        return left;
    }

    private PatternNode ParsePrimaryPattern()
    {
        Token current = Peek();
        int line = current.Line;

        if (IsWildcardToken(current))
        {
            Advance();
            return new PatternNode(PatternKind.Wildcard, line);
        }

        TokenType type = current.Type;

        if (type == TokenType.Null)
        {
            Advance();
            return new PatternNode(PatternKind.NullLiteral, line);
        }
        if (type == TokenType.True)
        {
            Advance();
            return new PatternNode(true, line);
        }
        if (type == TokenType.False)
        {
            Advance();
            return new PatternNode(false, line);
        }
        if (type == TokenType.StringLiteral)
        {
            Token token = Advance();
            return new PatternNode(token.Value, true, line);
        }
        if (type == TokenType.IntegerLiteral)
        {
            Token token = Advance();
            return new PatternNode(ParseIntegerValue(token.Value), line);
        }
        if (type == TokenType.FloatLiteral)
        {
            Token token = Advance();
            return new PatternNode(double.Parse(token.Value, CultureInfo.InvariantCulture), line);
        }
        if (type == TokenType.Minus)
        {
            Advance();
            if (Check(TokenType.IntegerLiteral))
            {
                Token token = Advance();
                return new PatternNode(-ParseIntegerValue(token.Value), line);
            }
            if (Check(TokenType.FloatLiteral))
            {
                Token token = Advance();
                return new PatternNode(-double.Parse(token.Value, CultureInfo.InvariantCulture), line);
            }
            throw new ParserException("Expected number after '-' in pattern", Peek().Line);
        }

        // Array pattern: "[" [<pattern_list>] "]"
        if (type == TokenType.LeftBracket)
        {
            Advance();
            List<PatternNode> elements = new List<PatternNode>();
            if (!Check(TokenType.RightBracket))
            {
                elements.Add(ParsePattern());
                while (Match(TokenType.Comma))
                {
                    elements.Add(ParsePattern());
                }
            }
            Expect(TokenType.RightBracket, "Expected ']' to close array pattern");
            return new PatternNode(PatternKind.ArrayPattern, string.Empty, elements, line);
        }

        // Identifier-led patterns: identifier, constructor, field pattern
        if (type == TokenType.Identifier)
        {
            Token idToken = Advance();
            string name = idToken.Value;

            // Constructor pattern: id "(" [<pattern_list>] ")"
            if (Match(TokenType.LeftParen))
            {
                List<PatternNode> subPatterns = new List<PatternNode>();
                if (!Check(TokenType.RightParen))
                {
                    subPatterns.Add(ParsePattern());
                    while (Match(TokenType.Comma))
                    {
                        subPatterns.Add(ParsePattern());
                    }
                }
                Expect(TokenType.RightParen, "Expected ')' to close constructor pattern");
                return new PatternNode(PatternKind.Constructor, name, subPatterns, line);
            }

            // Field pattern: id "{" [<field_pattern_list>] "}"
            if (Match(TokenType.LeftBrace))
            {
                List<FieldPatternEntry> fieldPatterns = new List<FieldPatternEntry>();
                if (!Check(TokenType.RightBrace))
                {
                    fieldPatterns.Add(ParseFieldPatternEntry());
                    while (Match(TokenType.Comma))
                    {
                        fieldPatterns.Add(ParseFieldPatternEntry());
                    }
                }
                Expect(TokenType.RightBrace, "Expected '}' to close field pattern");
                return new PatternNode(name, fieldPatterns, line);
            }

            // Plain identifier binding pattern
            return new PatternNode(PatternKind.Identifier, name, line);
        }

        throw new ParserException(
            "Unexpected token '" + current.Value + "' in pattern",
            current.Line);
    }

    // <field_pattern_entry> ::= <id> ":" <pattern>
    private FieldPatternEntry ParseFieldPatternEntry()
    {
        Token idToken = Expect(TokenType.Identifier, "Expected field name in field pattern");
        Expect(TokenType.Colon, "Expected ':' after field name in field pattern");
        PatternNode pattern = ParsePattern();
        return new FieldPatternEntry(idToken.Value, pattern);
    }

    // ================================================================
    // Annotations
    // ================================================================

    // "@" <id> ["(" [<annotation_param_list>] ")"] <stmt>
    private AstNode ParseAnnotatedStatement()
    {
        Token atToken = Advance();
        int line = atToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected annotation name after '@'");

        List<AnnotationParam> annotationParams = new List<AnnotationParam>();
        if (Match(TokenType.LeftParen))
        {
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
    private AnnotationParam ParseAnnotationParam()
    {
        Token idToken = Expect(TokenType.Identifier, "Expected parameter name in annotation");
        Expect(TokenType.SingleEqual, "Expected '=' in annotation parameter");
        AstNode valueExpression = ParseExpression();
        return new AnnotationParam(idToken.Value, valueExpression);
    }

    // ================================================================
    // Expression parsing (precedence climbing, note 11)
    // ================================================================

    // <expr> ::= <or_expr> "?" <expr> ":" <expr> | <or_expr>
    // Right-associative ternary.
    private AstNode ParseExpression()
    {
        AstNode left = ParseOrExpression();

        if (Check(TokenType.Question))
        {
            int line = Peek().Line;
            Advance();
            AstNode thenExpression = ParseExpression();
            Expect(TokenType.Colon, "Expected ':' in ternary expression");
            AstNode elseExpression = ParseExpression();
            return new TernaryNode(left, thenExpression, elseExpression, line);
        }

        return left;
    }

    // <or_expr> ::= <or_expr> ("||" | "or") <and_expr> | <and_expr>
    private AstNode ParseOrExpression()
    {
        AstNode left = ParseAndExpression();
        while (Check(TokenType.DoublePipe) || Check(TokenType.Or))
        {
            Token op = Advance();
            AstNode right = ParseAndExpression();
            left = new BinaryOpNode(left, op.Value, right, op.Line);
        }
        return left;
    }

    // <and_expr> ::= <and_expr> ("&&" | "and") <not_expr> | <not_expr>
    private AstNode ParseAndExpression()
    {
        AstNode left = ParseNotExpression();
        while (Check(TokenType.DoubleAmpersand) || Check(TokenType.And))
        {
            Token op = Advance();
            AstNode right = ParseNotExpression();
            left = new BinaryOpNode(left, op.Value, right, op.Line);
        }
        return left;
    }

    // <not_expr> ::= "not" <not_expr> | <comparison_expr>
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

    // <comparison_expr> — non-associative comparison plus type check/assert.
    // Per note 11, "is" and "as" sit at comparison precedence.
    private AstNode ParseComparisonExpression()
    {
        AstNode left = ParseAdditiveExpression();

        if (Check(TokenType.Is))
        {
            int line = Peek().Line;
            Advance();
            TypeNode checkedType = ParseType();
            return new TypeCheckNode(left, checkedType, line);
        }

        if (Check(TokenType.As))
        {
            int line = Peek().Line;
            Advance();
            TypeNode assertedType = ParseType();
            return new TypeAssertNode(left, assertedType, line);
        }

        if (Check(TokenType.EqualEqual) || Check(TokenType.NotEqual) ||
            Check(TokenType.Less) || Check(TokenType.Greater) ||
            Check(TokenType.LessEqual) || Check(TokenType.GreaterEqual))
        {
            Token op = Advance();
            AstNode right = ParseAdditiveExpression();
            return new BinaryOpNode(left, op.Value, right, op.Line);
        }

        return left;
    }

    // <additive_expr> ::= <additive_expr> ("+" | "-" | "&") <multiplicative_expr>
    private AstNode ParseAdditiveExpression()
    {
        AstNode left = ParseMultiplicativeExpression();
        while (Check(TokenType.Plus) || Check(TokenType.Minus) || Check(TokenType.Ampersand))
        {
            Token op = Advance();
            AstNode right = ParseMultiplicativeExpression();
            left = new BinaryOpNode(left, op.Value, right, op.Line);
        }
        return left;
    }

    // <multiplicative_expr> ::= <multiplicative_expr> ("*" | "/" | "%" | "//") <power_expr>
    private AstNode ParseMultiplicativeExpression()
    {
        AstNode left = ParsePowerExpression();
        while (Check(TokenType.Star) || Check(TokenType.Slash) ||
               Check(TokenType.Percent) || Check(TokenType.DoubleSlash))
        {
            Token op = Advance();
            AstNode right = ParsePowerExpression();
            left = new BinaryOpNode(left, op.Value, right, op.Line);
        }
        return left;
    }

    // <power_expr> ::= <unary_expr> "**" <power_expr> | <unary_expr>
    // Right-associative.
    private AstNode ParsePowerExpression()
    {
        AstNode left = ParseUnaryExpression();
        if (Check(TokenType.DoubleStar))
        {
            Token op = Advance();
            AstNode right = ParsePowerExpression();
            return new BinaryOpNode(left, op.Value, right, op.Line);
        }
        return left;
    }

    // <unary_expr> ::= "-" <unary_expr> | <postfix_expr>
    private AstNode ParseUnaryExpression()
    {
        if (Check(TokenType.Minus))
        {
            Token minus = Advance();
            AstNode operand = ParseUnaryExpression();
            return new UnaryOpNode(minus.Value, operand, minus.Line);
        }
        return ParsePostfixExpression();
    }

    // <postfix_expr> — left-associative: "." id [ "(" args ")" ], "[" expr "]", "(" args ")".
    private AstNode ParsePostfixExpression()
    {
        AstNode left = ParsePrimary();

        while (true)
        {
            if (Check(TokenType.Dot))
            {
                Advance();
                Token memberToken = Expect(TokenType.Identifier, "Expected member name after '.'");

                if (Check(TokenType.LeftParen))
                {
                    Advance();
                    List<AstNode> arguments = ParseArgList();
                    Expect(TokenType.RightParen, "Expected ')'");
                    left = new MethodCallNode(left, memberToken.Value, arguments, memberToken.Line);
                }
                else
                {
                    left = new MemberAccessNode(left, memberToken.Value, memberToken.Line);
                }
            }
            else if (Check(TokenType.LeftBracket))
            {
                int line = Peek().Line;
                Advance();
                AstNode indexExpression = ParseExpression();
                Expect(TokenType.RightBracket, "Expected ']'");
                left = new IndexAccessNode(left, indexExpression, line);
            }
            else if (Check(TokenType.LeftParen))
            {
                int line = Peek().Line;

                // If the primary we parsed is a plain identifier, fold the call
                // into a FunctionCallNode for convenient interpreter dispatch.
                // Otherwise this is a call on a stored value — handled below as
                // a MethodCallNode on a synthetic "<call>" target? No: the AST
                // offers FunctionCallNode(name, args), so we only fold when left
                // is an IdentifierNode. For value calls we build a MethodCallNode
                // with an empty method name? The AST does not currently have a
                // dedicated ValueCallNode, so we route through FunctionCallNode
                // for identifier calls and a MethodCallNode with an empty name
                // is not valid. For unfoldable cases we therefore fall back to a
                // FunctionCallNode by name only when possible; an unusual
                // value-call remains represented as a MethodCallNode with the
                // member name synthesised from the preceding postfix.
                //
                // In practice every value call arrives through a prior "." step
                // that already produced a MemberAccessNode or the expression is
                // not call-ready. The simplest safe model: only named calls fold
                // into FunctionCallNode; unsupported value-calls throw.

                if (left is IdentifierNode identifierNode)
                {
                    Advance();
                    List<AstNode> arguments = ParseArgList();
                    Expect(TokenType.RightParen, "Expected ')'");
                    left = new FunctionCallNode(identifierNode.Name, arguments, line);
                }
                else if (left is MemberAccessNode memberAccessNode)
                {
                    // A "." id followed by "(" which we initially parsed as a
                    // MemberAccessNode: rewrite as a method call.
                    Advance();
                    List<AstNode> arguments = ParseArgList();
                    Expect(TokenType.RightParen, "Expected ')'");
                    left = new MethodCallNode(memberAccessNode.Target, memberAccessNode.MemberName,
                        arguments, memberAccessNode.Line);
                }
                else
                {
                    throw new ParserException(
                        "Unsupported call target — store the value in an identifier first",
                        line);
                }
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
        Token current = Peek();
        TokenType type = current.Type;
        int line = current.Line;

        if (type == TokenType.IntegerLiteral)
        {
            Token token = Advance();
            return new IntegerLiteralNode(ParseIntegerValue(token.Value), token.Line);
        }
        if (type == TokenType.FloatLiteral)
        {
            Token token = Advance();
            return new FloatLiteralNode(double.Parse(token.Value, CultureInfo.InvariantCulture), token.Line);
        }
        if (type == TokenType.StringLiteral)
        {
            Token token = Advance();
            return new StringLiteralNode(token.Value, token.Line);
        }
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
        if (type == TokenType.Null)
        {
            Advance();
            return new NullLiteralNode(line);
        }
        if (type == TokenType.New)
        {
            return ParseNewExpression();
        }
        if (type == TokenType.If)
        {
            return ParseConditionalExpression();
        }
        if (type == TokenType.Function)
        {
            return ParseLambdaExpression();
        }
        if (type == TokenType.LeftParen)
        {
            return ParseParenOrCast();
        }
        if (type == TokenType.LeftBracket)
        {
            return ParseArrayLiteralOrComprehension();
        }
        if (type == TokenType.Identifier)
        {
            Token token = Advance();
            return new IdentifierNode(token.Value, token.Line);
        }

        // Type keywords used as built-in function names: int(...), float(...),
        // bool(...), string(...). These are lexed as type tokens but are also
        // valid built-in function call names when followed immediately by '('.
        if ((type == TokenType.TypeInt || type == TokenType.TypeFloat ||
             type == TokenType.TypeBool || type == TokenType.TypeString) &&
            PeekAt(1).Type == TokenType.LeftParen)
        {
            Token typeToken = Advance(); // consume type keyword
            Advance();                   // consume '('
            List<AstNode> arguments = ParseArgList();
            Expect(TokenType.RightParen, "Expected ')' after built-in type-cast call");
            return new FunctionCallNode(typeToken.Value, arguments, typeToken.Line);
        }

        throw new ParserException(
            "Expected expression but got '" + current.Value + "' (" + current.Type + ")",
            current.Line);
    }

    // "new" <id> "(" [<arg_list>] ")"   (note 15)
    private AstNode ParseNewExpression()
    {
        Token newToken = Advance();
        int line = newToken.Line;
        Token idToken = Expect(TokenType.Identifier, "Expected class name after 'new'");
        Expect(TokenType.LeftParen, "Expected '(' after class name in new expression");
        List<AstNode> arguments = ParseArgList();
        Expect(TokenType.RightParen, "Expected ')' after constructor arguments");
        return new NewExprNode(idToken.Value, arguments, line);
    }

    // Conditional expression (note 12): "if" <expr> "then" <expr> "else" <expr>
    private AstNode ParseConditionalExpression()
    {
        Token ifToken = Advance();
        int line = ifToken.Line;
        AstNode condition = ParseExpression();
        Expect(TokenType.Then, "Expected 'then' in conditional expression");
        AstNode thenExpression = ParseExpression();
        Expect(TokenType.Else, "Expected 'else' in conditional expression");
        AstNode elseExpression = ParseExpression();
        return new ConditionalExprNode(condition, thenExpression, elseExpression, line);
    }

    // Lambda expression (note 14):
    //   "function" "(" [<param_list>] ")" ["->" <type>] <expr>            — expression body
    //   "function" "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end" — block body
    // Disambiguation: if the token after the parameter list (and optional
    // "-> type") can start a statement, parse as block body; otherwise parse
    // as expression body.
    private AstNode ParseLambdaExpression()
    {
        Token functionToken = Advance();
        int line = functionToken.Line;
        Expect(TokenType.LeftParen, "Expected '(' after 'function' in lambda");
        List<ParameterNode> parameters = ParseParamList();
        Expect(TokenType.RightParen, "Expected ')' after lambda parameters");

        TypeNode returnType = null;
        if (Match(TokenType.Arrow))
        {
            returnType = ParseType();
        }

        // Per note 14: scan ahead at the same nesting depth. If a matching
        // 'end' is found before EOF, this is a block-body lambda; otherwise
        // the body is a single expression.
        if (CanStartStatement() && HasEndAtCurrentDepth())
        {
            HashSet<TokenType> stop = new HashSet<TokenType> { TokenType.End };
            List<AstNode> blockBody = ParseStatementList(stop);
            Expect(TokenType.End, "Expected 'end' to close block-body lambda");
            return new LambdaExprNode(parameters, returnType, blockBody, true, line);
        }

        AstNode expressionBody = ParseExpression();
        List<AstNode> bodyList = new List<AstNode>();
        bodyList.Add(expressionBody);
        return new LambdaExprNode(parameters, returnType, bodyList, false, line);
    }

    // Scans forward from the current position looking for an 'end' token at
    // the same nesting depth as the current position. Nesting is tracked by
    // counting constructs that introduce 'end' delimiters (if/while/for/
    // foreach/do-block/function/class/try). Returns true when a matching 'end'
    // is found before EOF.
    private bool HasEndAtCurrentDepth()
    {
        int depth = 0;
        int index = Position;
        while (index < Tokens.Count)
        {
            TokenType type = Tokens[index].Type;
            if (type == TokenType.EndOfFile)
            {
                return false;
            }
            // These keywords open a nested block that is closed by 'end'.
            if (type == TokenType.If || type == TokenType.While ||
                type == TokenType.For || type == TokenType.Foreach ||
                type == TokenType.Function || type == TokenType.Class ||
                type == TokenType.Try)
            {
                depth = depth + 1;
            }
            else if (type == TokenType.Do)
            {
                // 'do' opens a block only when not followed by a statement list
                // and then 'while' (do-while). We conservatively count it as
                // a depth opener because ParseStatementList stops at 'while'
                // for do-while anyway.
                depth = depth + 1;
            }
            else if (type == TokenType.End)
            {
                if (depth == 0)
                {
                    return true;
                }
                depth = depth - 1;
            }
            index = index + 1;
        }
        return false;
    }

    // Parenthesised expression or cast expression (note 13).
    private AstNode ParseParenOrCast()
    {
        int line = Peek().Line;

        if (IsCastExpression())
        {
            Advance(); // "("
            TypeNode targetType = ParseType();
            Expect(TokenType.RightParen, "Expected ')' after cast type");
            AstNode operand = ParseUnaryExpression();
            return new CastExprNode(targetType, operand, line);
        }

        Advance(); // "("
        AstNode expression = ParseExpression();
        Expect(TokenType.RightParen, "Expected ')'");
        return expression;
    }

    // True if the current "(" begins a cast (type name followed by ")").
    private bool IsCastExpression()
    {
        TokenType inside = PeekAt(1).Type;

        if (IsPrimitiveTypeToken(inside))
        {
            return LookAheadTypeToClosingParen(1);
        }

        if (inside == TokenType.Identifier)
        {
            TokenType afterId = PeekAt(2).Type;
            if (afterId == TokenType.RightParen)
            {
                return true;
            }
            if (afterId == TokenType.Less ||
                afterId == TokenType.LeftBracket ||
                afterId == TokenType.Question)
            {
                return LookAheadTypeToClosingParen(1);
            }
        }

        return false;
    }

    // Returns true if the token type is a primitive type keyword usable in
    // either a cast or a type annotation.
    private static bool IsPrimitiveTypeToken(TokenType type)
    {
        switch (type)
        {
            case TokenType.TypeInt:
            case TokenType.TypeFloat:
            case TokenType.TypeString:
            case TokenType.TypeBool:
            case TokenType.TypeArray:
            case TokenType.TypeObject:
            case TokenType.TypeVoid:
            case TokenType.TypeMap:
            case TokenType.Null:
                return true;
            default:
                return false;
        }
    }

    // Walk a type starting at (Position + offset) and report whether a closing
    // ')' immediately follows. Used to disambiguate cast vs grouped expression.
    private bool LookAheadTypeToClosingParen(int offset)
    {
        int index = Position + offset;
        index = SkipTypeAhead(index);
        return index < Tokens.Count && Tokens[index].Type == TokenType.RightParen;
    }

    // Skip a single type expression in the token stream for lookahead.
    // Returns the index of the first token past the type.
    private int SkipTypeAhead(int index)
    {
        if (index >= Tokens.Count)
        {
            return index;
        }

        TokenType baseType = Tokens[index].Type;

        if (IsPrimitiveTypeToken(baseType) && baseType != TokenType.TypeMap)
        {
            index++;
        }
        else if (baseType == TokenType.TypeMap)
        {
            index++;
            if (index < Tokens.Count && Tokens[index].Type == TokenType.Less)
            {
                index++;
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
            index++;
            if (index < Tokens.Count && Tokens[index].Type == TokenType.Less)
            {
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
            return index;
        }

        // Skip trailing type suffixes "[]" and "?".
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

    // Array literal or list comprehension.
    private AstNode ParseArrayLiteralOrComprehension()
    {
        Token bracketToken = Advance();
        int line = bracketToken.Line;

        if (Check(TokenType.RightBracket))
        {
            Advance();
            return new ArrayLiteralNode(new List<AstNode>(), line);
        }

        AstNode firstExpression = ParseExpression();

        // List comprehension: "[" <expr> "for" <id> "in" <expr> "]"
        if (Match(TokenType.For))
        {
            Token loopVar = Expect(TokenType.Identifier, "Expected variable in list comprehension");
            Expect(TokenType.In, "Expected 'in' in list comprehension");
            AstNode sourceExpression = ParseExpression();
            Expect(TokenType.RightBracket, "Expected ']' to close list comprehension");
            return new ListComprehensionNode(firstExpression, loopVar.Value, sourceExpression, line);
        }

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
        TypeNode baseType = ParseBaseType(line);

        // Apply suffixes "[]" and "?" left-to-right.
        while (true)
        {
            if (Check(TokenType.LeftBracket) && PeekAt(1).Type == TokenType.RightBracket)
            {
                Advance();
                Advance();
                baseType = new TypeNode(TypeKind.ArraySuffix, baseType, line);
            }
            else if (Check(TokenType.Question))
            {
                Advance();
                baseType = new TypeNode(TypeKind.NullableSuffix, baseType, line);
            }
            else
            {
                break;
            }
        }

        return baseType;
    }

    // <base_type> — primitive keyword, map<K,V>, generic <id>< ... >, or plain <id>.
    private TypeNode ParseBaseType(int line)
    {
        Token current = Peek();
        TokenType type = current.Type;

        switch (type)
        {
            case TokenType.TypeInt:
                Advance();
                return new TypeNode(TypeKind.Int, "int", line);
            case TokenType.TypeFloat:
                Advance();
                return new TypeNode(TypeKind.Float, "float", line);
            case TokenType.TypeString:
                Advance();
                return new TypeNode(TypeKind.String, "string", line);
            case TokenType.TypeBool:
                Advance();
                return new TypeNode(TypeKind.Bool, "bool", line);
            case TokenType.TypeArray:
                Advance();
                return new TypeNode(TypeKind.Array, "array", line);
            case TokenType.TypeObject:
                Advance();
                return new TypeNode(TypeKind.Object, "object", line);
            case TokenType.TypeVoid:
                Advance();
                return new TypeNode(TypeKind.Void, "void", line);
            case TokenType.Null:
                Advance();
                return new TypeNode(TypeKind.Null, "null", line);

            case TokenType.TypeMap:
                {
                    Advance();
                    Expect(TokenType.Less, "Expected '<' after 'map'");
                    TypeNode keyType = ParseType();
                    Expect(TokenType.Comma, "Expected ',' between map key and value types");
                    TypeNode valueType = ParseType();
                    Expect(TokenType.Greater, "Expected '>' to close map type");
                    return new TypeNode(keyType, valueType, line);
                }

            case TokenType.Identifier:
                {
                    Token idToken = Advance();
                    string typeName = idToken.Value;

                    if (Check(TokenType.Less) && IsGenericTypeArgList())
                    {
                        Advance(); // consume "<"
                        List<TypeNode> typeArguments = new List<TypeNode>();
                        typeArguments.Add(ParseType());
                        while (Match(TokenType.Comma))
                        {
                            typeArguments.Add(ParseType());
                        }
                        Expect(TokenType.Greater, "Expected '>' to close generic type");
                        return new TypeNode(typeName, typeArguments, line);
                    }

                    return new TypeNode(TypeKind.Named, typeName, line);
                }

            default:
                throw new ParserException(
                    "Expected type but got '" + current.Value + "'",
                    current.Line);
        }
    }

    // Note 20 disambiguation: is the "<" after an identifier the start of a
    // generic type argument list (rather than a comparison)?
    private bool IsGenericTypeArgList()
    {
        int index = Position + 1; // first token after "<"
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
                    return true;
                }
            }
            else if (tokenType == TokenType.Comma ||
                     tokenType == TokenType.LeftBracket ||
                     tokenType == TokenType.RightBracket ||
                     tokenType == TokenType.Question ||
                     IsTypeStartToken(tokenType))
            {
                // Permitted inside a generic type argument list.
            }
            else
            {
                return false;
            }

            index++;
        }

        return false;
    }

    // Returns true if the token type can begin a <type>.
    private static bool IsTypeStartToken(TokenType type)
    {
        switch (type)
        {
            case TokenType.TypeInt:
            case TokenType.TypeFloat:
            case TokenType.TypeString:
            case TokenType.TypeBool:
            case TokenType.TypeArray:
            case TokenType.TypeObject:
            case TokenType.TypeVoid:
            case TokenType.TypeMap:
            case TokenType.Null:
            case TokenType.Identifier:
                return true;
            default:
                return false;
        }
    }

    // ================================================================
    // Integer parsing helper
    // ================================================================

    // Parses an integer literal from its lexeme, supporting decimal, binary,
    // octal, and hex forms.
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
