using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLanguage.Lexer;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.UnitTests;

/// <summary>
/// Unit tests for the TinyLanguage Parser.
/// Each test parses a minimal source string and asserts on the resulting AST node types
/// and properties. Covers every BNF production rule and disambiguation rule.
/// </summary>
[TestClass]
public sealed class ParserUnitTests
{
    // ── Helper ────────────────────────────────────────────────────────────

    /// <summary>Tokenises and parses <paramref name="source"/>, returning the root ProgramNode.</summary>
    private static ProgramNode Parse(string source)
    {
        List<Token> tokens = new TinyLanguage.Lexer.Lexer(source).Tokenize();
        return new Parser(tokens).Parse();
    }

    // ================================================================
    // Program structure
    // ================================================================

    [TestMethod]
    public void Parser_EmptyProgram_ProducesEmptyStatementList()
    {
        ProgramNode program = Parse(string.Empty);
        Assert.AreEqual(0, program.Statements.Count);
    }

    [TestMethod]
    public void Parser_SingleStatement_ProducesOneStatement()
    {
        ProgramNode program = Parse("let x := 1");
        Assert.AreEqual(1, program.Statements.Count);
    }

    [TestMethod]
    public void Parser_MultipleStatements_SeparatedBySemicolon()
    {
        ProgramNode program = Parse("let x := 1; let y := 2");
        Assert.AreEqual(2, program.Statements.Count);
    }

    [TestMethod]
    public void Parser_MultipleFunctionDefs_AllParsedWithoutError()
    {
        ProgramNode program = Parse(
            "function foo() print 1 end; " +
            "function bar() print 2 end");
        Assert.AreEqual(2, program.Statements.Count);
        Assert.IsInstanceOfType(program.Statements[0], typeof(FunctionDefNode));
        Assert.IsInstanceOfType(program.Statements[1], typeof(FunctionDefNode));
    }

    // ================================================================
    // Assignment and declaration
    // ================================================================

    [TestMethod]
    public void Parser_AssignStatement_ProducesAssignStatementNode()
    {
        ProgramNode program = Parse("x := 42");
        Assert.AreEqual(1, program.Statements.Count);
        Assert.IsInstanceOfType(program.Statements[0], typeof(AssignStatementNode));
        AssignStatementNode node = (AssignStatementNode)program.Statements[0];
        Assert.AreEqual("x", node.Name);
        Assert.IsInstanceOfType(node.Value, typeof(IntegerLiteralNode));
    }

    [TestMethod]
    public void Parser_LetDeclareWithoutType_ProducesLetDeclareNode()
    {
        ProgramNode program = Parse("let x := 5");
        Assert.AreEqual(1, program.Statements.Count);
        Assert.IsInstanceOfType(program.Statements[0], typeof(LetDeclareNode));
        LetDeclareNode node = (LetDeclareNode)program.Statements[0];
        Assert.AreEqual("x", node.Name);
        Assert.IsNull(node.TypeAnnotation);
        Assert.IsInstanceOfType(node.Value, typeof(IntegerLiteralNode));
    }

    [TestMethod]
    public void Parser_LetDeclareWithType_ProducesLetDeclareNodeWithAnnotation()
    {
        ProgramNode program = Parse("let x : int := 5");
        LetDeclareNode node = (LetDeclareNode)program.Statements[0];
        Assert.AreEqual("x", node.Name);
        Assert.IsNotNull(node.TypeAnnotation);
        Assert.AreEqual(TypeKind.Int, node.TypeAnnotation.Kind);
    }

    [TestMethod]
    public void Parser_VarDeclare_ProducesVarDeclareNode()
    {
        ProgramNode program = Parse("var x : int := 7");
        Assert.AreEqual(1, program.Statements.Count);
        Assert.IsInstanceOfType(program.Statements[0], typeof(VarDeclareNode));
        VarDeclareNode node = (VarDeclareNode)program.Statements[0];
        Assert.AreEqual("x", node.Name);
        Assert.IsNotNull(node.TypeAnnotation);
    }

    [TestMethod]
    public void Parser_ConstDeclareWithoutType_ProducesConstDeclareNode()
    {
        ProgramNode program = Parse("const PI := 3");
        ConstDeclareNode node = (ConstDeclareNode)program.Statements[0];
        Assert.AreEqual("PI", node.Name);
        Assert.IsNull(node.TypeAnnotation);
    }

    [TestMethod]
    public void Parser_ConstDeclareWithType_ProducesConstDeclareNodeWithAnnotation()
    {
        ProgramNode program = Parse("const PI : float := 3");
        ConstDeclareNode node = (ConstDeclareNode)program.Statements[0];
        Assert.AreEqual("PI", node.Name);
        Assert.IsNotNull(node.TypeAnnotation);
        Assert.AreEqual(TypeKind.Float, node.TypeAnnotation.Kind);
    }

    [TestMethod]
    public void Parser_EnumDef_ProducesEnumDefNode()
    {
        ProgramNode program = Parse("enum Color { Red, Green, Blue }");
        Assert.IsInstanceOfType(program.Statements[0], typeof(EnumDefNode));
        EnumDefNode node = (EnumDefNode)program.Statements[0];
        Assert.AreEqual("Color", node.Name);
        Assert.AreEqual(3, node.Members.Count);
        Assert.AreEqual("Red", node.Members[0].Name);
        Assert.IsNull(node.Members[0].Value);
    }

    [TestMethod]
    public void Parser_EnumDefWithValues_ProducesEnumMembersWithExpressions()
    {
        ProgramNode program = Parse("enum Status { Ok = 0, Fail = 1 }");
        EnumDefNode node = (EnumDefNode)program.Statements[0];
        Assert.AreEqual(2, node.Members.Count);
        Assert.IsNotNull(node.Members[0].Value);
        Assert.IsNotNull(node.Members[1].Value);
    }

    // ================================================================
    // Control flow
    // ================================================================

    [TestMethod]
    public void Parser_IfStatementWithoutElse_ProducesIfStatementNodeWithEmptyElse()
    {
        ProgramNode program = Parse("if true then print 1 end");
        Assert.IsInstanceOfType(program.Statements[0], typeof(IfStatementNode));
        IfStatementNode node = (IfStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.ThenBody.Count);
        Assert.AreEqual(0, node.ElseBody.Count);
    }

    [TestMethod]
    public void Parser_IfStatementWithElse_ProducesIfStatementNodeWithElseBody()
    {
        ProgramNode program = Parse("if true then print 1 else print 2 end");
        IfStatementNode node = (IfStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.ThenBody.Count);
        Assert.AreEqual(1, node.ElseBody.Count);
    }

    [TestMethod]
    public void Parser_WhileStatement_ProducesWhileStatementNode()
    {
        ProgramNode program = Parse("while x do print x end");
        Assert.IsInstanceOfType(program.Statements[0], typeof(WhileStatementNode));
        WhileStatementNode node = (WhileStatementNode)program.Statements[0];
        Assert.IsInstanceOfType(node.Condition, typeof(IdentifierNode));
        Assert.AreEqual(1, node.Body.Count);
    }

    [TestMethod]
    public void Parser_ForStatementWithoutStep_ProducesForStatementNodeWithNullStep()
    {
        ProgramNode program = Parse("for i := 1 to 10 do print i end");
        ForStatementNode node = (ForStatementNode)program.Statements[0];
        Assert.AreEqual("i", node.VariableName);
        Assert.IsNull(node.StepExpr);
        Assert.AreEqual(1, node.Body.Count);
    }

    [TestMethod]
    public void Parser_ForStatementWithStep_ProducesForStatementNodeWithStep()
    {
        ProgramNode program = Parse("for i := 1 to 10 step 2 do print i end");
        ForStatementNode node = (ForStatementNode)program.Statements[0];
        Assert.AreEqual("i", node.VariableName);
        Assert.IsNotNull(node.StepExpr);
        Assert.IsInstanceOfType(node.StepExpr, typeof(IntegerLiteralNode));
    }

    [TestMethod]
    public void Parser_ForeachStatement_ProducesForeachStatementNode()
    {
        ProgramNode program = Parse("foreach item in myList do print item end");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ForeachStatementNode));
        ForeachStatementNode node = (ForeachStatementNode)program.Statements[0];
        Assert.AreEqual("item", node.VariableName);
        Assert.AreEqual(1, node.Body.Count);
    }

    [TestMethod]
    public void Parser_DoWhileStatement_ProducesDoWhileStatementNode()
    {
        ProgramNode program = Parse("do print 1 while false");
        Assert.IsInstanceOfType(program.Statements[0], typeof(DoWhileStatementNode));
    }

    [TestMethod]
    public void Parser_DoWhileMultiStatement_ProducesDoWhileWithMultipleBodyStatements()
    {
        ProgramNode program = Parse("do print 1; print 2 while false");
        DoWhileStatementNode node = (DoWhileStatementNode)program.Statements[0];
        Assert.AreEqual(2, node.Body.Count);
    }

    [TestMethod]
    public void Parser_DoBlock_ProducesDoBlockNode()
    {
        ProgramNode program = Parse("do print 1 end");
        Assert.IsInstanceOfType(program.Statements[0], typeof(DoBlockNode));
    }

    [TestMethod]
    public void Parser_SwitchStatementWithOneCaseNoDefault_ParsesWithoutError()
    {
        ProgramNode program = Parse("switch x { case 1: print 1 }");
        Assert.IsInstanceOfType(program.Statements[0], typeof(SwitchStatementNode));
        SwitchStatementNode node = (SwitchStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.Cases.Count);
        Assert.IsFalse(node.Cases[0].IsDefault);
    }

    [TestMethod]
    public void Parser_SwitchStatementWithMultipleCasesAndDefault_ParsesAllClauses()
    {
        ProgramNode program = Parse(
            "switch x { case 1: print 1; case 2: print 2; default: print 0 }");
        SwitchStatementNode node = (SwitchStatementNode)program.Statements[0];
        Assert.AreEqual(3, node.Cases.Count);
        Assert.IsTrue(node.Cases[2].IsDefault);
        Assert.IsNull(node.Cases[2].CaseExpr);
    }

    [TestMethod]
    public void Parser_SwitchStatementDefaultOnly_ParsesWithoutError()
    {
        ProgramNode program = Parse("switch x { default: print 0 }");
        SwitchStatementNode node = (SwitchStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.Cases.Count);
        Assert.IsTrue(node.Cases[0].IsDefault);
    }

    [TestMethod]
    public void Parser_BreakStatement_ProducesBreakStatementNode()
    {
        ProgramNode program = Parse("while true do break end");
        WhileStatementNode whileNode = (WhileStatementNode)program.Statements[0];
        Assert.IsInstanceOfType(whileNode.Body[0], typeof(BreakStatementNode));
    }

    [TestMethod]
    public void Parser_ContinueStatement_ProducesContinueStatementNode()
    {
        ProgramNode program = Parse("while true do continue end");
        WhileStatementNode whileNode = (WhileStatementNode)program.Statements[0];
        Assert.IsInstanceOfType(whileNode.Body[0], typeof(ContinueStatementNode));
    }

    // ================================================================
    // I/O statements
    // ================================================================

    [TestMethod]
    public void Parser_PrintStatement_ProducesPrintStatementNode()
    {
        ProgramNode program = Parse("print 42");
        Assert.IsInstanceOfType(program.Statements[0], typeof(PrintStatementNode));
    }

    [TestMethod]
    public void Parser_InputStatement_ProducesInputStatementNode()
    {
        ProgramNode program = Parse("input x");
        Assert.IsInstanceOfType(program.Statements[0], typeof(InputStatementNode));
        InputStatementNode node = (InputStatementNode)program.Statements[0];
        Assert.AreEqual("x", node.VariableName);
    }

    // ================================================================
    // Functions
    // ================================================================

    [TestMethod]
    public void Parser_FunctionDefNoParamsNoReturnType_ProducesFunctionDefNode()
    {
        ProgramNode program = Parse("function greet() print 1 end");
        Assert.IsInstanceOfType(program.Statements[0], typeof(FunctionDefNode));
        FunctionDefNode node = (FunctionDefNode)program.Statements[0];
        Assert.AreEqual("greet", node.Name);
        Assert.AreEqual(0, node.Parameters.Count);
        Assert.IsNull(node.ReturnType);
    }

    [TestMethod]
    public void Parser_FunctionDefWithParamsAndReturnType_ProducesFunctionDefNodeWithAll()
    {
        ProgramNode program = Parse("function add(a, b) -> int return a end");
        FunctionDefNode node = (FunctionDefNode)program.Statements[0];
        Assert.AreEqual("add", node.Name);
        Assert.AreEqual(2, node.Parameters.Count);
        Assert.IsNotNull(node.ReturnType);
        Assert.AreEqual(TypeKind.Int, node.ReturnType.Kind);
    }

    [TestMethod]
    public void Parser_EmptyFunctionBody_ParsesWithoutError()
    {
        ProgramNode program = Parse("function foo() end");
        FunctionDefNode node = (FunctionDefNode)program.Statements[0];
        Assert.AreEqual(0, node.Body.Count);
    }

    [TestMethod]
    public void Parser_ParamBareIdentifier_ProducesParameterNodeWithNoTypeAndNoDefault()
    {
        ProgramNode program = Parse("function f(x) end");
        FunctionDefNode node = (FunctionDefNode)program.Statements[0];
        ParameterNode param = node.Parameters[0];
        Assert.AreEqual("x", param.Name);
        Assert.IsNull(param.TypeAnnotation);
        Assert.IsNull(param.DefaultValue);
    }

    [TestMethod]
    public void Parser_ParamWithTypeAnnotation_ProducesParameterNodeWithType()
    {
        ProgramNode program = Parse("function f(x : int) end");
        ParameterNode param = ((FunctionDefNode)program.Statements[0]).Parameters[0];
        Assert.IsNotNull(param.TypeAnnotation);
        Assert.AreEqual(TypeKind.Int, param.TypeAnnotation.Kind);
    }

    [TestMethod]
    public void Parser_ParamWithDefaultValue_ProducesParameterNodeWithDefault()
    {
        ProgramNode program = Parse("function f(x := 0) end");
        ParameterNode param = ((FunctionDefNode)program.Statements[0]).Parameters[0];
        Assert.IsNotNull(param.DefaultValue);
        Assert.IsNull(param.TypeAnnotation);
    }

    [TestMethod]
    public void Parser_ParamWithTypeAndDefault_ProducesFullParameterNode()
    {
        ProgramNode program = Parse("function f(x : int := 0) end");
        ParameterNode param = ((FunctionDefNode)program.Statements[0]).Parameters[0];
        Assert.IsNotNull(param.TypeAnnotation);
        Assert.IsNotNull(param.DefaultValue);
    }

    [TestMethod]
    public void Parser_CallStatementPlainNoArgs_ProducesCallStatementNode()
    {
        ProgramNode program = Parse("foo()");
        Assert.IsInstanceOfType(program.Statements[0], typeof(CallStatementNode));
        CallStatementNode node = (CallStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.MemberPath.Count);
        Assert.AreEqual("foo", node.MemberPath[0]);
        Assert.AreEqual(0, node.Arguments.Count);
    }

    [TestMethod]
    public void Parser_CallStatementWithMultipleArgs_ProducesCallStatementNodeWithArgs()
    {
        ProgramNode program = Parse("foo(1, 2, 3)");
        CallStatementNode node = (CallStatementNode)program.Statements[0];
        Assert.AreEqual(3, node.Arguments.Count);
    }

    [TestMethod]
    public void Parser_CallStatementMethodOneLevel_ProducesCallStatementNodeWithTwoPartPath()
    {
        ProgramNode program = Parse("obj.doSomething()");
        CallStatementNode node = (CallStatementNode)program.Statements[0];
        Assert.AreEqual(2, node.MemberPath.Count);
        Assert.AreEqual("obj", node.MemberPath[0]);
        Assert.AreEqual("doSomething", node.MemberPath[1]);
    }

    [TestMethod]
    public void Parser_CallStatementChainedMethod_ProducesCallStatementNodeWithThreePartPath()
    {
        ProgramNode program = Parse("a.b.c(1)");
        CallStatementNode node = (CallStatementNode)program.Statements[0];
        Assert.AreEqual(3, node.MemberPath.Count);
        Assert.AreEqual("a", node.MemberPath[0]);
        Assert.AreEqual("b", node.MemberPath[1]);
        Assert.AreEqual("c", node.MemberPath[2]);
    }

    [TestMethod]
    public void Parser_ReturnStatementWithExpression_ProducesReturnStatementNodeWithValue()
    {
        ProgramNode program = Parse("function f() return 5 end");
        FunctionDefNode func = (FunctionDefNode)program.Statements[0];
        Assert.IsInstanceOfType(func.Body[0], typeof(ReturnStatementNode));
        ReturnStatementNode ret = (ReturnStatementNode)func.Body[0];
        Assert.IsNotNull(ret.Value);
    }

    [TestMethod]
    public void Parser_ReturnStatementBare_ProducesReturnStatementNodeWithNullValue()
    {
        ProgramNode program = Parse("function f() return end");
        FunctionDefNode func = (FunctionDefNode)program.Statements[0];
        ReturnStatementNode ret = (ReturnStatementNode)func.Body[0];
        Assert.IsNull(ret.Value);
    }

    // ================================================================
    // OOP — Classes and Objects
    // ================================================================

    [TestMethod]
    public void Parser_ClassDefEmpty_ParsesWithoutError()
    {
        ProgramNode program = Parse("class Foo { }");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ClassDefNode));
        ClassDefNode node = (ClassDefNode)program.Statements[0];
        Assert.AreEqual("Foo", node.Name);
        Assert.AreEqual(0, node.Members.Count);
    }

    [TestMethod]
    public void Parser_ClassDefWithExtends_ProducesClassDefNodeWithBase()
    {
        ProgramNode program = Parse("class Dog extends Animal { }");
        ClassDefNode node = (ClassDefNode)program.Statements[0];
        Assert.AreEqual("Dog", node.Name);
        Assert.AreEqual("Animal", node.ExtendsName);
    }

    [TestMethod]
    public void Parser_ClassDefWithImplements_ProducesClassDefNodeWithInterfaces()
    {
        ProgramNode program = Parse("class Circle implements Shape { }");
        ClassDefNode node = (ClassDefNode)program.Statements[0];
        Assert.AreEqual(1, node.ImplementsNames.Count);
        Assert.AreEqual("Shape", node.ImplementsNames[0]);
    }

    [TestMethod]
    public void Parser_StaticClassDef_ProducesClassDefNodeWithIsStatic()
    {
        ProgramNode program = Parse("static class Utils { }");
        ClassDefNode node = (ClassDefNode)program.Statements[0];
        Assert.IsTrue(node.IsStatic);
    }

    [TestMethod]
    public void Parser_ClassDefWithConstructorAndMethod_ProducesCorrectMemberList()
    {
        ProgramNode program = Parse(
            "class Pt { Constructor(x, y) end; function getX() return x end }");
        ClassDefNode node = (ClassDefNode)program.Statements[0];
        Assert.AreEqual(2, node.Members.Count);
        Assert.IsInstanceOfType(node.Members[0], typeof(ConstructorDefNode));
        Assert.IsInstanceOfType(node.Members[1], typeof(MethodDefNode));
    }

    [TestMethod]
    public void Parser_ObjectInstantiationViaAssignAndNew_ProducesAssignWithNewExprNode()
    {
        ProgramNode program = Parse("myObj := new Foo(1, 2)");
        AssignStatementNode assign = (AssignStatementNode)program.Statements[0];
        Assert.IsInstanceOfType(assign.Value, typeof(NewExprNode));
        NewExprNode newExpr = (NewExprNode)assign.Value;
        Assert.AreEqual("Foo", newExpr.ClassName);
        Assert.AreEqual(2, newExpr.Arguments.Count);
    }

    [TestMethod]
    public void Parser_FieldDeclareLetUntyped_ProducesFieldDeclareNode()
    {
        ProgramNode program = Parse("class A { let x := 1 }");
        ClassDefNode cls = (ClassDefNode)program.Statements[0];
        Assert.IsInstanceOfType(cls.Members[0], typeof(FieldDeclareNode));
        FieldDeclareNode field = (FieldDeclareNode)cls.Members[0];
        Assert.AreEqual("let", field.Keyword);
        Assert.IsNull(field.TypeAnnotation);
    }

    [TestMethod]
    public void Parser_FieldDeclareLetTyped_ProducesFieldDeclareNodeWithType()
    {
        ProgramNode program = Parse("class A { let x : int := 0 }");
        ClassDefNode cls = (ClassDefNode)program.Statements[0];
        FieldDeclareNode field = (FieldDeclareNode)cls.Members[0];
        Assert.IsNotNull(field.TypeAnnotation);
    }

    [TestMethod]
    public void Parser_FieldDeclareVar_ProducesFieldDeclareNodeWithVarKeyword()
    {
        ProgramNode program = Parse("class A { var x : int := 0 }");
        ClassDefNode cls = (ClassDefNode)program.Statements[0];
        FieldDeclareNode field = (FieldDeclareNode)cls.Members[0];
        Assert.AreEqual("var", field.Keyword);
        Assert.IsNotNull(field.TypeAnnotation);
    }

    [TestMethod]
    public void Parser_FieldDeclareConstUntyped_ProducesFieldDeclareNodeWithConst()
    {
        ProgramNode program = Parse("class A { const MAX := 100 }");
        ClassDefNode cls = (ClassDefNode)program.Statements[0];
        FieldDeclareNode field = (FieldDeclareNode)cls.Members[0];
        Assert.AreEqual("const", field.Keyword);
    }

    [TestMethod]
    public void Parser_FieldDeclareConstTyped_ProducesFieldDeclareNodeWithConstAndType()
    {
        ProgramNode program = Parse("class A { const MAX : int := 100 }");
        ClassDefNode cls = (ClassDefNode)program.Statements[0];
        FieldDeclareNode field = (FieldDeclareNode)cls.Members[0];
        Assert.AreEqual("const", field.Keyword);
        Assert.IsNotNull(field.TypeAnnotation);
    }

    // ================================================================
    // Module system
    // ================================================================

    [TestMethod]
    public void Parser_ModuleDefWithoutImports_ProducesModuleDefNode()
    {
        ProgramNode program = Parse("module MathLib { print 1 }");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ModuleDefNode));
        ModuleDefNode node = (ModuleDefNode)program.Statements[0];
        Assert.AreEqual("MathLib", node.Name);
        Assert.AreEqual(0, node.Imports.Count);
    }

    [TestMethod]
    public void Parser_ModuleDefWithImportList_ProducesModuleDefNodeWithImports()
    {
        // Spec note: no double "import import" — the module header uses "import" once.
        ProgramNode program = Parse("module A import B, C { print 1 }");
        ModuleDefNode node = (ModuleDefNode)program.Statements[0];
        Assert.AreEqual(2, node.Imports.Count);
        Assert.AreEqual("B", node.Imports[0].ModuleName);
        Assert.IsNull(node.Imports[0].Alias);
        Assert.AreEqual("C", node.Imports[1].ModuleName);
    }

    [TestMethod]
    public void Parser_ModuleImportWithAlias_ProducesModuleImportWithAlias()
    {
        ProgramNode program = Parse("module A import B as BeeLib { print 1 }");
        ModuleDefNode node = (ModuleDefNode)program.Statements[0];
        Assert.AreEqual("B", node.Imports[0].ModuleName);
        Assert.AreEqual("BeeLib", node.Imports[0].Alias);
    }

    [TestMethod]
    public void Parser_ImportStatement_ProducesImportStatementNode()
    {
        ProgramNode program = Parse("import Foo");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ImportStatementNode));
        ImportStatementNode node = (ImportStatementNode)program.Statements[0];
        Assert.AreEqual("Foo", node.ModuleName);
        Assert.IsNull(node.Alias);
    }

    [TestMethod]
    public void Parser_ImportStatementWithAlias_ProducesImportStatementNodeWithAlias()
    {
        ProgramNode program = Parse("import Foo as F");
        ImportStatementNode node = (ImportStatementNode)program.Statements[0];
        Assert.AreEqual("Foo", node.ModuleName);
        Assert.AreEqual("F", node.Alias);
    }

    [TestMethod]
    public void Parser_ExportStatement_ProducesExportStatementNode()
    {
        ProgramNode program = Parse("export myFunc");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ExportStatementNode));
        ExportStatementNode node = (ExportStatementNode)program.Statements[0];
        Assert.AreEqual("myFunc", node.Name);
    }

    // ================================================================
    // Exception handling
    // ================================================================

    [TestMethod]
    public void Parser_TryStatementWithCatchNoCatch_ParsesCatchBody()
    {
        ProgramNode program = Parse("try print 1 catch err print 2 end");
        Assert.IsInstanceOfType(program.Statements[0], typeof(TryStatementNode));
        TryStatementNode node = (TryStatementNode)program.Statements[0];
        Assert.AreEqual("err", node.CatchVariableName);
        Assert.IsNull(node.CatchVariableType);
        Assert.AreEqual(0, node.FinallyBody.Count);
    }

    [TestMethod]
    public void Parser_TryStatementWithCatchAndFinally_ParsesFinallyBody()
    {
        ProgramNode program = Parse("try print 1 catch err print 2 finally print 3 end");
        TryStatementNode node = (TryStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.FinallyBody.Count);
    }

    [TestMethod]
    public void Parser_CatchClauseTypedForm_ProducesCatchWithType()
    {
        ProgramNode program = Parse("try print 1 catch (err : string) print 2 end");
        TryStatementNode node = (TryStatementNode)program.Statements[0];
        Assert.AreEqual("err", node.CatchVariableName);
        Assert.IsNotNull(node.CatchVariableType);
        Assert.AreEqual(TypeKind.String, node.CatchVariableType.Kind);
    }

    [TestMethod]
    public void Parser_ThrowStatement_ProducesThrowStatementNode()
    {
        ProgramNode program = Parse("throw \"error\"");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ThrowStatementNode));
    }

    // ================================================================
    // Pattern matching
    // ================================================================

    [TestMethod]
    public void Parser_PatternMatchWithLiteralCase_ProducesPatternMatchNode()
    {
        ProgramNode program = Parse("match x { 1 => print 1 }");
        Assert.IsInstanceOfType(program.Statements[0], typeof(PatternMatchNode));
        PatternMatchNode node = (PatternMatchNode)program.Statements[0];
        Assert.AreEqual(1, node.Cases.Count);
        Assert.AreEqual(PatternKind.IntegerLiteral, node.Cases[0].Pattern.Kind);
    }

    [TestMethod]
    public void Parser_PatternMatchWithWhenGuard_ProducesPatternCaseNodeWithGuard()
    {
        ProgramNode program = Parse("match x { n when n > 0 => print n }");
        PatternMatchNode node = (PatternMatchNode)program.Statements[0];
        Assert.IsNotNull(node.Cases[0].Guard);
    }

    [TestMethod]
    public void Parser_PatternMatchWithWildcard_ProducesWildcardPattern()
    {
        ProgramNode program = Parse("match x { _ => print 0 }");
        PatternMatchNode node = (PatternMatchNode)program.Statements[0];
        Assert.AreEqual(PatternKind.Wildcard, node.Cases[0].Pattern.Kind);
    }

    [TestMethod]
    public void Parser_PatternConstructorWithNoArgs_ParsesWithoutError()
    {
        ProgramNode program = Parse("match x { Foo() => print 1 }");
        PatternMatchNode node = (PatternMatchNode)program.Statements[0];
        Assert.AreEqual(PatternKind.Constructor, node.Cases[0].Pattern.Kind);
    }

    [TestMethod]
    public void Parser_PatternEmptyFieldPattern_ParsesWithoutError()
    {
        ProgramNode program = Parse("match x { Foo {} => print 1 }");
        PatternMatchNode node = (PatternMatchNode)program.Statements[0];
        Assert.AreEqual(PatternKind.FieldPattern, node.Cases[0].Pattern.Kind);
    }

    [TestMethod]
    public void Parser_PatternAlternation_ProducesLeftAssociativeAlternationTree()
    {
        // A | B | C should parse as (A | B) | C — left associative per note 23.
        ProgramNode program = Parse("match x { 1 | 2 | 3 => print 0 }");
        PatternMatchNode node = (PatternMatchNode)program.Statements[0];
        PatternNode top = node.Cases[0].Pattern;
        Assert.AreEqual(PatternKind.Alternation, top.Kind);
        // The top alternation's left child should also be an Alternation (1 | 2).
        Assert.AreEqual(PatternKind.Alternation, top.Left.Kind);
    }

    // ================================================================
    // Annotations
    // ================================================================

    [TestMethod]
    public void Parser_AnnotationWithNoParens_ProducesAnnotatedStatementNode()
    {
        ProgramNode program = Parse("@deprecated print 1");
        Assert.IsInstanceOfType(program.Statements[0], typeof(AnnotatedStatementNode));
        AnnotatedStatementNode node = (AnnotatedStatementNode)program.Statements[0];
        Assert.AreEqual("deprecated", node.Annotation.Name);
        Assert.AreEqual(0, node.Annotation.Parameters.Count);
    }

    [TestMethod]
    public void Parser_AnnotationWithEmptyParens_ParsesWithoutError()
    {
        ProgramNode program = Parse("@deprecated() print 1");
        AnnotatedStatementNode node = (AnnotatedStatementNode)program.Statements[0];
        Assert.AreEqual(0, node.Annotation.Parameters.Count);
    }

    [TestMethod]
    public void Parser_AnnotationWithOneParam_ProducesAnnotatedStatementNodeWithParam()
    {
        ProgramNode program = Parse("@ann(value = 1) print 1");
        AnnotatedStatementNode node = (AnnotatedStatementNode)program.Statements[0];
        Assert.AreEqual(1, node.Annotation.Parameters.Count);
        Assert.AreEqual("value", node.Annotation.Parameters[0].Name);
    }

    [TestMethod]
    public void Parser_AnnotationWithMultipleParams_ProducesAllAnnotationParams()
    {
        ProgramNode program = Parse("@ann(a = 1, b = 2) print 1");
        AnnotatedStatementNode node = (AnnotatedStatementNode)program.Statements[0];
        Assert.AreEqual(2, node.Annotation.Parameters.Count);
    }

    // ================================================================
    // Expressions — operator precedence and associativity
    // ================================================================

    [TestMethod]
    public void Parser_TernaryExpression_ProducesTernaryNode()
    {
        ProgramNode program = Parse("let r := x ? 1 : 0");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(TernaryNode));
    }

    [TestMethod]
    public void Parser_OrExpressionDoublePipe_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a || b");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode node = (BinaryOpNode)let.Value;
        Assert.AreEqual("||", node.Operator);
    }

    [TestMethod]
    public void Parser_OrExpressionOrKeyword_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a or b");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode node = (BinaryOpNode)let.Value;
        Assert.AreEqual("or", node.Operator);
    }

    [TestMethod]
    public void Parser_AndExpressionDoubleAmpersand_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a && b");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode node = (BinaryOpNode)let.Value;
        Assert.AreEqual("&&", node.Operator);
    }

    [TestMethod]
    public void Parser_AndExpressionAndKeyword_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a and b");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode node = (BinaryOpNode)let.Value;
        Assert.AreEqual("and", node.Operator);
    }

    [TestMethod]
    public void Parser_NotExpression_ProducesUnaryOpNodeWithNotOperator()
    {
        ProgramNode program = Parse("let r := not x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(UnaryOpNode));
        UnaryOpNode node = (UnaryOpNode)let.Value;
        Assert.AreEqual("not", node.Operator);
    }

    [TestMethod]
    public void Parser_EqualEqualComparison_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a == b");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode node = (BinaryOpNode)let.Value;
        Assert.AreEqual("==", node.Operator);
    }

    [TestMethod]
    public void Parser_NotEqualComparison_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a != b");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("!=", node.Operator);
    }

    [TestMethod]
    public void Parser_LessComparison_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a < b");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("<", node.Operator);
    }

    [TestMethod]
    public void Parser_GreaterComparison_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a > b");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual(">", node.Operator);
    }

    [TestMethod]
    public void Parser_LessEqualComparison_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a <= b");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("<=", node.Operator);
    }

    [TestMethod]
    public void Parser_GreaterEqualComparison_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a >= b");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual(">=", node.Operator);
    }

    [TestMethod]
    public void Parser_AdditionExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 1 + 2");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("+", node.Operator);
    }

    [TestMethod]
    public void Parser_SubtractionExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 5 - 3");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("-", node.Operator);
    }

    [TestMethod]
    public void Parser_StringConcatAmpersand_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := a & b");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("&", node.Operator);
    }

    [TestMethod]
    public void Parser_MultiplicationExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 3 * 4");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("*", node.Operator);
    }

    [TestMethod]
    public void Parser_DivisionExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 10 / 2");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("/", node.Operator);
    }

    [TestMethod]
    public void Parser_ModuloExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 7 % 3");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("%", node.Operator);
    }

    [TestMethod]
    public void Parser_FloorDivisionExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 7 // 2");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("//", node.Operator);
    }

    [TestMethod]
    public void Parser_PowerExpression_ProducesBinaryOpNode()
    {
        ProgramNode program = Parse("let r := 2 ** 10");
        BinaryOpNode node = (BinaryOpNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual("**", node.Operator);
    }

    [TestMethod]
    public void Parser_PowerExpressionRightAssociative_ParsesAs2Power3Power2()
    {
        // 2 ** 3 ** 2  should parse as  2 ** (3 ** 2)
        ProgramNode program = Parse("let r := 2 ** 3 ** 2");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode outer = (BinaryOpNode)let.Value;
        Assert.AreEqual("**", outer.Operator);
        // The right child should also be ** (3 ** 2), not an integer.
        Assert.IsInstanceOfType(outer.Right, typeof(BinaryOpNode));
        Assert.AreEqual("**", ((BinaryOpNode)outer.Right).Operator);
    }

    [TestMethod]
    public void Parser_UnaryNegation_ProducesUnaryOpNode()
    {
        ProgramNode program = Parse("let r := -5");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(UnaryOpNode));
        UnaryOpNode node = (UnaryOpNode)let.Value;
        Assert.AreEqual("-", node.Operator);
    }

    [TestMethod]
    public void Parser_OperatorPrecedenceMultiplyBindsTighterThanAdd_ProducesCorrectTree()
    {
        // 2 + 3 * 4 should parse as 2 + (3 * 4)
        ProgramNode program = Parse("let r := 2 + 3 * 4");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode addition = (BinaryOpNode)let.Value;
        Assert.AreEqual("+", addition.Operator);
        // Right child of + should be * (not an integer literal)
        Assert.IsInstanceOfType(addition.Right, typeof(BinaryOpNode));
        Assert.AreEqual("*", ((BinaryOpNode)addition.Right).Operator);
    }

    [TestMethod]
    public void Parser_LogicalPrecedenceAndBindsTighterThanOr_ProducesCorrectTree()
    {
        // a || b && c should parse as a || (b && c)
        ProgramNode program = Parse("let r := a || b && c");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode orNode = (BinaryOpNode)let.Value;
        Assert.AreEqual("||", orNode.Operator);
        Assert.IsInstanceOfType(orNode.Right, typeof(BinaryOpNode));
        Assert.AreEqual("&&", ((BinaryOpNode)orNode.Right).Operator);
    }

    [TestMethod]
    public void Parser_ParenthesisedExpressionOverridesPrecedence_ParsesGroupedFirst()
    {
        // (2 + 3) * 4 should parse as (2 + 3) * 4 — outer node is *
        ProgramNode program = Parse("let r := (2 + 3) * 4");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        BinaryOpNode multiply = (BinaryOpNode)let.Value;
        Assert.AreEqual("*", multiply.Operator);
        // Left child is the grouped (2 + 3) — the parser returns an addition node
        Assert.IsInstanceOfType(multiply.Left, typeof(BinaryOpNode));
    }

    // ================================================================
    // Postfix expressions
    // ================================================================

    [TestMethod]
    public void Parser_MemberAccess_ProducesMemberAccessNode()
    {
        ProgramNode program = Parse("let r := obj.name");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(MemberAccessNode));
        MemberAccessNode node = (MemberAccessNode)let.Value;
        Assert.AreEqual("name", node.MemberName);
    }

    [TestMethod]
    public void Parser_MethodCallExpression_ProducesMethodCallNode()
    {
        ProgramNode program = Parse("let r := obj.doIt(1)");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(MethodCallNode));
        MethodCallNode node = (MethodCallNode)let.Value;
        Assert.AreEqual("doIt", node.MethodName);
        Assert.AreEqual(1, node.Arguments.Count);
    }

    [TestMethod]
    public void Parser_IndexAccess_ProducesIndexAccessNode()
    {
        ProgramNode program = Parse("let r := arr[0]");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(IndexAccessNode));
    }

    [TestMethod]
    public void Parser_FunctionCallExpression_ProducesFunctionCallNode()
    {
        ProgramNode program = Parse("let r := foo(1, 2)");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(FunctionCallNode));
        FunctionCallNode node = (FunctionCallNode)let.Value;
        Assert.AreEqual("foo", node.Name);
        Assert.AreEqual(2, node.Arguments.Count);
    }

    // ================================================================
    // Type check and assert at comparison precedence
    // ================================================================

    [TestMethod]
    public void Parser_TypeCheckExpression_ProducesTypeCheckNode()
    {
        ProgramNode program = Parse("let r := x is int");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(TypeCheckNode));
        TypeCheckNode node = (TypeCheckNode)let.Value;
        Assert.AreEqual(TypeKind.Int, node.CheckType.Kind);
    }

    [TestMethod]
    public void Parser_TypeAssertExpression_ProducesTypeAssertNode()
    {
        ProgramNode program = Parse("let r := x as string");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(TypeAssertNode));
    }

    [TestMethod]
    public void Parser_IsOperatorPrecedence_LeftOperandIsAdditiveNotPostfix()
    {
        // a + b is int should parse as (a + b) is int
        ProgramNode program = Parse("let r := a + b is int");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        TypeCheckNode typeCheck = (TypeCheckNode)let.Value;
        // The operand of "is" should be the binary + node, not just b.
        Assert.IsInstanceOfType(typeCheck.Operand, typeof(BinaryOpNode));
    }

    [TestMethod]
    public void Parser_AsOperatorPrecedence_LeftOperandIsAdditive()
    {
        // a + b as int should parse as (a + b) as int
        ProgramNode program = Parse("let r := a + b as int");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        TypeAssertNode typeAssert = (TypeAssertNode)let.Value;
        Assert.IsInstanceOfType(typeAssert.Operand, typeof(BinaryOpNode));
    }

    // ================================================================
    // Arrays
    // ================================================================

    [TestMethod]
    public void Parser_ArrayElementAssign_ProducesArrayElementAssignNode()
    {
        ProgramNode program = Parse("arr[0] := 5");
        Assert.IsInstanceOfType(program.Statements[0], typeof(ArrayElementAssignNode));
        ArrayElementAssignNode node = (ArrayElementAssignNode)program.Statements[0];
        Assert.AreEqual("arr", node.ArrayName);
        Assert.IsInstanceOfType(node.IndexExpr, typeof(IntegerLiteralNode));
        Assert.IsInstanceOfType(node.Value, typeof(IntegerLiteralNode));
    }

    [TestMethod]
    public void Parser_ArrayLiteralNonEmpty_ProducesArrayLiteralNode()
    {
        ProgramNode program = Parse("let arr := [1, 2, 3]");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(ArrayLiteralNode));
        ArrayLiteralNode node = (ArrayLiteralNode)let.Value;
        Assert.AreEqual(3, node.Elements.Count);
    }

    [TestMethod]
    public void Parser_ArrayLiteralEmpty_ParsesWithoutError()
    {
        ProgramNode program = Parse("let arr := []");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(ArrayLiteralNode));
        ArrayLiteralNode node = (ArrayLiteralNode)let.Value;
        Assert.AreEqual(0, node.Elements.Count);
    }

    // ================================================================
    // Lambda
    // ================================================================

    [TestMethod]
    public void Parser_LambdaExpressionBody_ProducesLambdaExprNodeWithIsBlockBodyFalse()
    {
        ProgramNode program = Parse("let f := function(x) x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(LambdaExprNode));
        LambdaExprNode node = (LambdaExprNode)let.Value;
        Assert.IsFalse(node.IsBlockBody);
    }

    [TestMethod]
    public void Parser_LambdaStatementBody_ProducesLambdaExprNodeWithIsBlockBodyTrue()
    {
        ProgramNode program = Parse("let f := function(x) print x end");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        LambdaExprNode node = (LambdaExprNode)let.Value;
        Assert.IsTrue(node.IsBlockBody);
    }

    // ================================================================
    // Literals
    // ================================================================

    [TestMethod]
    public void Parser_IntegerLiteral_ProducesIntegerLiteralNode()
    {
        ProgramNode program = Parse("let r := 42");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(IntegerLiteralNode));
        IntegerLiteralNode node = (IntegerLiteralNode)let.Value;
        Assert.AreEqual(42L, node.Value);
    }

    [TestMethod]
    public void Parser_BinaryLiteral_ProducesIntegerLiteralNodeWithCorrectValue()
    {
        ProgramNode program = Parse("let r := 0b1010");
        IntegerLiteralNode node = (IntegerLiteralNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual(10L, node.Value);
    }

    [TestMethod]
    public void Parser_OctalLiteral_ProducesIntegerLiteralNodeWithCorrectValue()
    {
        ProgramNode program = Parse("let r := 0o17");
        IntegerLiteralNode node = (IntegerLiteralNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual(15L, node.Value);
    }

    [TestMethod]
    public void Parser_HexLiteral_ProducesIntegerLiteralNodeWithCorrectValue()
    {
        ProgramNode program = Parse("let r := 0xFF");
        IntegerLiteralNode node = (IntegerLiteralNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.AreEqual(255L, node.Value);
    }

    [TestMethod]
    public void Parser_FloatLiteral_ProducesFloatLiteralNode()
    {
        ProgramNode program = Parse("let r := 3.14");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(FloatLiteralNode));
        FloatLiteralNode node = (FloatLiteralNode)let.Value;
        Assert.AreEqual(3.14, node.Value, 0.0001);
    }

    [TestMethod]
    public void Parser_StringLiteral_ProducesStringLiteralNode()
    {
        ProgramNode program = Parse("let r := \"hello\"");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(StringLiteralNode));
        StringLiteralNode node = (StringLiteralNode)let.Value;
        Assert.AreEqual("hello", node.Value);
    }

    [TestMethod]
    public void Parser_TrueLiteral_ProducesBoolLiteralNodeWithTrue()
    {
        ProgramNode program = Parse("let r := true");
        BoolLiteralNode node = (BoolLiteralNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.IsTrue(node.Value);
    }

    [TestMethod]
    public void Parser_FalseLiteral_ProducesBoolLiteralNodeWithFalse()
    {
        ProgramNode program = Parse("let r := false");
        BoolLiteralNode node = (BoolLiteralNode)((LetDeclareNode)program.Statements[0]).Value;
        Assert.IsFalse(node.Value);
    }

    [TestMethod]
    public void Parser_NullLiteral_ProducesNullLiteralNode()
    {
        ProgramNode program = Parse("let r := null");
        Assert.IsInstanceOfType(((LetDeclareNode)program.Statements[0]).Value, typeof(NullLiteralNode));
    }

    // ================================================================
    // Cast expression disambiguation (note 13)
    // ================================================================

    [TestMethod]
    public void Parser_CastExpression_ProducesCastExprNode()
    {
        ProgramNode program = Parse("let r := (int) x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(CastExprNode));
        CastExprNode node = (CastExprNode)let.Value;
        Assert.AreEqual(TypeKind.Int, node.TargetType.Kind);
    }

    [TestMethod]
    public void Parser_GroupedExpression_IsNotCastNode()
    {
        // (x + 1) should NOT be a cast — it's a parenthesised expression.
        ProgramNode program = Parse("let r := (x + 1)");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(BinaryOpNode));
    }

    // ================================================================
    // Conditional expression (inline if — note 12)
    // ================================================================

    [TestMethod]
    public void Parser_ConditionalExpression_ProducesConditionalExprNode()
    {
        ProgramNode program = Parse("let r := if x then 1 else 0");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsInstanceOfType(let.Value, typeof(ConditionalExprNode));
    }

    // ================================================================
    // Type annotations
    // ================================================================

    [TestMethod]
    public void Parser_TypeAnnotationArraySuffix_ProducesArraySuffixTypeNode()
    {
        ProgramNode program = Parse("let r : int[] := x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.IsNotNull(let.TypeAnnotation);
        Assert.AreEqual(TypeKind.ArraySuffix, let.TypeAnnotation.Kind);
    }

    [TestMethod]
    public void Parser_TypeAnnotationMultiDimensionalArray_ProducesNestedArraySuffix()
    {
        ProgramNode program = Parse("let r : int[][] := x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.AreEqual(TypeKind.ArraySuffix, let.TypeAnnotation.Kind);
        Assert.AreEqual(TypeKind.ArraySuffix, let.TypeAnnotation.InnerType.Kind);
    }

    [TestMethod]
    public void Parser_TypeAnnotationNullable_ProducesNullableSuffixTypeNode()
    {
        ProgramNode program = Parse("let r : int? := x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.AreEqual(TypeKind.NullableSuffix, let.TypeAnnotation.Kind);
    }

    [TestMethod]
    public void Parser_TypeAnnotationMapType_ProducesMapTypeNode()
    {
        ProgramNode program = Parse("let r : map<string, int> := x");
        LetDeclareNode let = (LetDeclareNode)program.Statements[0];
        Assert.AreEqual(TypeKind.MapType, let.TypeAnnotation.Kind);
    }

    // ================================================================
    // Parser error cases
    // ================================================================

    [TestMethod]
    [ExpectedException(typeof(ParserException))]
    public void Parser_IfStatementMissingEnd_ThrowsParserException()
    {
        Parse("if true then print 1");
    }

    [TestMethod]
    [ExpectedException(typeof(ParserException))]
    public void Parser_WhileStatementMissingDo_ThrowsParserException()
    {
        Parse("while true print 1 end");
    }

    [TestMethod]
    [ExpectedException(typeof(ParserException))]
    public void Parser_AssignmentWithSingleEqual_ThrowsParserException()
    {
        // Bare = is not a valid assignment operator (spec note 2).
        Parse("x = 5");
    }

    [TestMethod]
    [ExpectedException(typeof(ParserException))]
    public void Parser_TrailingSemicolonBeforeEnd_ThrowsParserException()
    {
        Parse("function foo() x := 1; end");
    }

    [TestMethod]
    [ExpectedException(typeof(ParserException))]
    public void Parser_SwitchWithNoCasesAndNoDefault_ThrowsParserException()
    {
        Parse("switch x { }");
    }
}
