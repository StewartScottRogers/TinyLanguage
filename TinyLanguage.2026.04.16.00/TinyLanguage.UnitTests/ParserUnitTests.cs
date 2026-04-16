using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLanguage.Lexer;

namespace TinyLanguage.UnitTests
{
    [TestClass]
    public class ParserUnitTests
    {
        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private static ProgramNode Parse(string src)
        {
            List<Token> tokens = TinyLanguage.Lexer.Lexer.Tokenise(src);
            return Parser.Parse(tokens);
        }

        private static AstNode FirstStatement(string src)
        {
            return Parse(src).Statements[0];
        }

        // ----------------------------------------------------------------
        // Group 1: Declarations
        // ----------------------------------------------------------------

        [TestMethod]
        public void LetDeclare_NoTypeAnnotation_ProducesLetDeclareNode()
        {
            LetDeclareNode node = (LetDeclareNode)FirstStatement("let x := 42");
            Assert.AreEqual("x", node.VariableName);
            Assert.IsNull(node.DeclaredType);
            Assert.IsInstanceOfType(node.InitialiserExpression, typeof(IntegerLiteralNode));
        }

        [TestMethod]
        public void LetDeclare_WithTypeAnnotation_ProducesLetDeclareNode()
        {
            LetDeclareNode node = (LetDeclareNode)FirstStatement("let x : int := 42");
            Assert.AreEqual("x", node.VariableName);
            Assert.IsNotNull(node.DeclaredType);
            Assert.AreEqual("int", node.DeclaredType.TypeName);
        }

        [TestMethod]
        public void VarDeclare_WithMandatoryTypeAnnotation_ProducesVarDeclareNode()
        {
            VarDeclareNode node = (VarDeclareNode)FirstStatement("var counter : int := 0");
            Assert.AreEqual("counter", node.VariableName);
            Assert.IsNotNull(node.DeclaredType);
            Assert.AreEqual("int", node.DeclaredType.TypeName);
        }

        [TestMethod]
        public void ConstDeclare_NoTypeAnnotation_ProducesConstDeclareNode()
        {
            ConstDeclareNode node = (ConstDeclareNode)FirstStatement("const PI := 3");
            Assert.AreEqual("PI", node.ConstantName);
            Assert.IsNull(node.DeclaredType);
        }

        [TestMethod]
        public void ConstDeclare_WithTypeAnnotation_ProducesConstDeclareNode()
        {
            ConstDeclareNode node = (ConstDeclareNode)FirstStatement("const PI : float := 3");
            Assert.AreEqual("PI", node.ConstantName);
            Assert.IsNotNull(node.DeclaredType);
        }

        // ----------------------------------------------------------------
        // Group 2: Assignment
        // ----------------------------------------------------------------

        [TestMethod]
        public void Assignment_SimpleAssign_ProducesAssignStatementNode()
        {
            AssignStatementNode node = (AssignStatementNode)FirstStatement("x := 10");
            Assert.AreEqual("x", node.VariableName);
            Assert.IsInstanceOfType(node.ValueExpression, typeof(IntegerLiteralNode));
        }

        [TestMethod]
        public void Assignment_StringRhs_ProducesAssignStatementNode()
        {
            AssignStatementNode node = (AssignStatementNode)FirstStatement("name := \"alice\"");
            Assert.AreEqual("name", node.VariableName);
            Assert.IsInstanceOfType(node.ValueExpression, typeof(StringLiteralNode));
        }

        // ----------------------------------------------------------------
        // Group 3: If statement
        // ----------------------------------------------------------------

        [TestMethod]
        public void IfStatement_NoElse_ProducesIfStatementNode()
        {
            IfStatementNode node = (IfStatementNode)FirstStatement("if x then print x end");
            Assert.IsNotNull(node.Condition);
            Assert.AreEqual(1, node.ThenBody.Count);
            Assert.IsNull(node.ElseBody);
        }

        [TestMethod]
        public void IfStatement_WithElse_ProducesElseBody()
        {
            IfStatementNode node = (IfStatementNode)FirstStatement(
                "if x then print x else print y end");
            Assert.IsNotNull(node.ElseBody);
            Assert.AreEqual(1, node.ElseBody.Count);
        }

        [TestMethod]
        public void IfStatement_ElifChain_UsesNestedIfInElseBody()
        {
            // "elif" is encoded as an if in the else body
            IfStatementNode node = (IfStatementNode)FirstStatement(
                "if a then print a else if b then print b end end");
            Assert.IsNotNull(node.ElseBody);
            Assert.IsInstanceOfType(node.ElseBody[0], typeof(IfStatementNode));
        }

        [TestMethod]
        public void IfStatement_Nested_ParsesCorrectly()
        {
            IfStatementNode outer = (IfStatementNode)FirstStatement(
                "if a then if b then print b end end");
            Assert.AreEqual(1, outer.ThenBody.Count);
            Assert.IsInstanceOfType(outer.ThenBody[0], typeof(IfStatementNode));
        }

        // ----------------------------------------------------------------
        // Group 4: While loop
        // ----------------------------------------------------------------

        [TestMethod]
        public void WhileStatement_Basic_ProducesWhileStatementNode()
        {
            WhileStatementNode node = (WhileStatementNode)FirstStatement(
                "while x do print x end");
            Assert.IsNotNull(node.Condition);
            Assert.AreEqual(1, node.Body.Count);
        }

        [TestMethod]
        public void WhileStatement_WithBreak_BreakIsInBody()
        {
            WhileStatementNode node = (WhileStatementNode)FirstStatement(
                "while true do break end");
            Assert.IsInstanceOfType(node.Body[0], typeof(BreakStatementNode));
        }

        [TestMethod]
        public void WhileStatement_WithContinue_ContinueIsInBody()
        {
            WhileStatementNode node = (WhileStatementNode)FirstStatement(
                "while true do continue end");
            Assert.IsInstanceOfType(node.Body[0], typeof(ContinueStatementNode));
        }

        // ----------------------------------------------------------------
        // Group 5: For loop
        // ----------------------------------------------------------------

        [TestMethod]
        public void ForStatement_Basic_ProducesForStatementNode()
        {
            ForStatementNode node = (ForStatementNode)FirstStatement(
                "for i := 1 to 10 do print i end");
            Assert.AreEqual("i", node.LoopVariable);
            Assert.IsNotNull(node.StartExpression);
            Assert.IsNotNull(node.EndExpression);
            Assert.IsNull(node.StepExpression);
        }

        [TestMethod]
        public void ForStatement_WithStep_StepExpressionIsSet()
        {
            ForStatementNode node = (ForStatementNode)FirstStatement(
                "for i := 0 to 100 step 2 do print i end");
            Assert.IsNotNull(node.StepExpression);
        }

        // ----------------------------------------------------------------
        // Group 6: Foreach loop
        // ----------------------------------------------------------------

        [TestMethod]
        public void ForeachStatement_OverArray_ProducesForeachStatementNode()
        {
            ForeachStatementNode node = (ForeachStatementNode)FirstStatement(
                "foreach item in myList do print item end");
            Assert.AreEqual("item", node.ElementVariable);
            Assert.IsInstanceOfType(node.IterableExpression, typeof(IdentifierNode));
        }

        [TestMethod]
        public void ForeachStatement_OverStringLiteral_ParsesCorrectly()
        {
            ForeachStatementNode node = (ForeachStatementNode)FirstStatement(
                "foreach ch in \"hello\" do print ch end");
            Assert.AreEqual("ch", node.ElementVariable);
            Assert.IsInstanceOfType(node.IterableExpression, typeof(StringLiteralNode));
        }

        // ----------------------------------------------------------------
        // Group 7: Do-while
        // ----------------------------------------------------------------

        [TestMethod]
        public void DoWhileStatement_Basic_ProducesDoWhileStatementNode()
        {
            DoWhileStatementNode node = (DoWhileStatementNode)FirstStatement(
                "do print x while x");
            Assert.IsNotNull(node.Condition);
            Assert.AreEqual(1, node.Body.Count);
        }

        [TestMethod]
        public void DoWhileStatement_EmptyBody_ParsesCorrectly()
        {
            DoWhileStatementNode node = (DoWhileStatementNode)FirstStatement(
                "do while false");
            Assert.AreEqual(0, node.Body.Count);
            Assert.IsInstanceOfType(node.Condition, typeof(BoolLiteralNode));
        }

        // ----------------------------------------------------------------
        // Group 8: Switch statement
        // ----------------------------------------------------------------

        [TestMethod]
        public void SwitchStatement_WithCases_ProducesSwitchStatementNode()
        {
            SwitchStatementNode node = (SwitchStatementNode)FirstStatement(
                "switch x { case 1: print x }");
            Assert.IsNotNull(node.SubjectExpression);
            Assert.AreEqual(1, node.Cases.Count);
            Assert.IsFalse(node.Cases[0].IsDefault);
        }

        [TestMethod]
        public void SwitchStatement_WithDefault_DefaultCaseFlagIsSet()
        {
            SwitchStatementNode node = (SwitchStatementNode)FirstStatement(
                "switch x { case 1: print x default: print y }");
            Assert.AreEqual(2, node.Cases.Count);
            Assert.IsTrue(node.Cases[1].IsDefault);
            Assert.IsNull(node.Cases[1].ValueExpression);
        }

        // ----------------------------------------------------------------
        // Group 9: Function definition
        // ----------------------------------------------------------------

        [TestMethod]
        public void FunctionDef_NoParams_ProducesFunctionDefNode()
        {
            FunctionDefNode node = (FunctionDefNode)FirstStatement(
                "function greet() print 1 end");
            Assert.AreEqual("greet", node.FunctionName);
            Assert.AreEqual(0, node.Parameters.Count);
            Assert.IsNull(node.ReturnType);
            Assert.IsFalse(node.IsStatic);
        }

        [TestMethod]
        public void FunctionDef_WithParams_ParametersAreCaptured()
        {
            FunctionDefNode node = (FunctionDefNode)FirstStatement(
                "function add(a, b) return a end");
            Assert.AreEqual(2, node.Parameters.Count);
            Assert.AreEqual("a", node.Parameters[0].ParameterName);
            Assert.AreEqual("b", node.Parameters[1].ParameterName);
        }

        [TestMethod]
        public void FunctionDef_WithReturnType_ReturnTypeIsSet()
        {
            FunctionDefNode node = (FunctionDefNode)FirstStatement(
                "function getVal() -> int return 1 end");
            Assert.IsNotNull(node.ReturnType);
            Assert.AreEqual("int", node.ReturnType.TypeName);
        }

        [TestMethod]
        public void FunctionDef_Static_IsStaticFlagIsTrue()
        {
            FunctionDefNode node = (FunctionDefNode)FirstStatement(
                "static function helper() print 1 end");
            Assert.IsTrue(node.IsStatic);
        }

        // ----------------------------------------------------------------
        // Group 10: Return statement
        // ----------------------------------------------------------------

        [TestMethod]
        public void ReturnStatement_WithExpression_ProducesReturnStatementNode()
        {
            // Return inside a function
            ProgramNode prog = Parse("function f() return 42 end");
            FunctionDefNode fn = (FunctionDefNode)prog.Statements[0];
            ReturnStatementNode ret = (ReturnStatementNode)fn.Body[0];
            Assert.IsNotNull(ret.ReturnExpression);
            Assert.IsInstanceOfType(ret.ReturnExpression, typeof(IntegerLiteralNode));
        }

        [TestMethod]
        public void ReturnStatement_Bare_ReturnExpressionIsNull()
        {
            ProgramNode prog = Parse("function f() return end");
            FunctionDefNode fn = (FunctionDefNode)prog.Statements[0];
            ReturnStatementNode ret = (ReturnStatementNode)fn.Body[0];
            Assert.IsNull(ret.ReturnExpression);
        }

        // ----------------------------------------------------------------
        // Group 11: Function call (as statement and expression)
        // ----------------------------------------------------------------

        [TestMethod]
        public void CallStatement_NoArgs_ProducesCallStatementNode()
        {
            CallStatementNode node = (CallStatementNode)FirstStatement("foo()");
            Assert.AreEqual(1, node.ReceiverChain.Count);
            Assert.AreEqual("foo", node.ReceiverChain[0]);
            Assert.AreEqual(0, node.Arguments.Count);
        }

        [TestMethod]
        public void CallStatement_WithArgs_ArgumentsAreCaptured()
        {
            CallStatementNode node = (CallStatementNode)FirstStatement("add(1, 2)");
            Assert.AreEqual(2, node.Arguments.Count);
        }

        [TestMethod]
        public void CallStatement_ChainedMemberCall_ReceiverChainHasAllParts()
        {
            CallStatementNode node = (CallStatementNode)FirstStatement("obj.method(x)");
            Assert.AreEqual(2, node.ReceiverChain.Count);
            Assert.AreEqual("obj", node.ReceiverChain[0]);
            Assert.AreEqual("method", node.ReceiverChain[1]);
        }

        [TestMethod]
        public void FunctionCallExpr_AsRhsOfAssignment_ProducesFunctionCallNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("y := foo()");
            Assert.IsInstanceOfType(assign.ValueExpression, typeof(FunctionCallNode));
        }

        // ----------------------------------------------------------------
        // Group 12: Class definition
        // ----------------------------------------------------------------

        [TestMethod]
        public void ClassDef_WithFields_ProducesClassDefNode()
        {
            ClassDefNode node = (ClassDefNode)FirstStatement(
                "class Point { let x := 0; let y := 0 }");
            Assert.AreEqual("Point", node.ClassName);
            Assert.AreEqual(2, node.Members.Count);
        }

        [TestMethod]
        public void ClassDef_WithConstructor_ConstructorInMembers()
        {
            ClassDefNode node = (ClassDefNode)FirstStatement(
                "class Foo { Constructor() print 1 end }");
            Assert.AreEqual(1, node.Members.Count);
            Assert.IsInstanceOfType(node.Members[0], typeof(ConstructorDefNode));
        }

        [TestMethod]
        public void ClassDef_WithExtends_BaseClassNameIsSet()
        {
            ClassDefNode node = (ClassDefNode)FirstStatement(
                "class Dog extends Animal { }");
            Assert.AreEqual("Animal", node.BaseClassName);
        }

        // ----------------------------------------------------------------
        // Group 13: Lambda expression
        // ----------------------------------------------------------------

        [TestMethod]
        public void LambdaExpr_ExpressionBody_ProducesLambdaExprNode()
        {
            // Use a literal as the body — a literal cannot start a statement,
            // so the parser disambiguates as an expression-body lambda.
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "fn := function(x) 42");
            LambdaExprNode lambda = (LambdaExprNode)assign.ValueExpression;
            Assert.IsFalse(lambda.IsBlockBody);
            Assert.IsNotNull(lambda.ExpressionBody);
            Assert.IsNull(lambda.StatementBody);
        }

        [TestMethod]
        public void LambdaExpr_BlockBody_IsBlockBodyTrue()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "fn := function(x) print x end");
            LambdaExprNode lambda = (LambdaExprNode)assign.ValueExpression;
            Assert.IsTrue(lambda.IsBlockBody);
            Assert.IsNull(lambda.ExpressionBody);
            Assert.IsNotNull(lambda.StatementBody);
        }

        [TestMethod]
        public void LambdaExpr_ZeroParams_EmptyParameterList()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "fn := function() 42");
            LambdaExprNode lambda = (LambdaExprNode)assign.ValueExpression;
            Assert.AreEqual(0, lambda.Parameters.Count);
        }

        [TestMethod]
        public void LambdaExpr_MultipleParams_AllParamsCaptured()
        {
            // Use a literal body so the parser disambiguates as expression-body.
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "fn := function(a, b, c) 0");
            LambdaExprNode lambda = (LambdaExprNode)assign.ValueExpression;
            Assert.AreEqual(3, lambda.Parameters.Count);
        }

        // ----------------------------------------------------------------
        // Group 14: Binary operator precedence
        // ----------------------------------------------------------------

        [TestMethod]
        public void BinaryOp_MultiplicationBeforeAddition_ParsesAsCorrectTree()
        {
            // "2 + 3 * 4" should parse as "2 + (3 * 4)"
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := 2 + 3 * 4");
            BinaryOpNode addition = (BinaryOpNode)assign.ValueExpression;
            Assert.AreEqual("+", addition.Operator);
            Assert.IsInstanceOfType(addition.Left, typeof(IntegerLiteralNode));
            BinaryOpNode multiplication = (BinaryOpNode)addition.Right;
            Assert.AreEqual("*", multiplication.Operator);
        }

        [TestMethod]
        public void BinaryOp_PowerBeforeMultiplication_ParsesAsCorrectTree()
        {
            // "2 * 3 ** 2" should parse as "2 * (3 ** 2)"
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := 2 * 3 ** 2");
            BinaryOpNode multiply = (BinaryOpNode)assign.ValueExpression;
            Assert.AreEqual("*", multiply.Operator);
            BinaryOpNode power = (BinaryOpNode)multiply.Right;
            Assert.AreEqual("**", power.Operator);
        }

        [TestMethod]
        public void BinaryOp_AdditionLeftAssociative_LeftTreeGroups()
        {
            // "1 + 2 + 3" parses as "(1 + 2) + 3" (left-associative)
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := 1 + 2 + 3");
            BinaryOpNode outerAdd = (BinaryOpNode)assign.ValueExpression;
            Assert.AreEqual("+", outerAdd.Operator);
            Assert.IsInstanceOfType(outerAdd.Left, typeof(BinaryOpNode));
        }

        [TestMethod]
        public void BinaryOp_LogicalAndBeforeOr_ParsesAsCorrectTree()
        {
            // "a || b && c" parses as "a || (b && c)"
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := a || b && c");
            BinaryOpNode orNode = (BinaryOpNode)assign.ValueExpression;
            Assert.AreEqual("||", orNode.Operator);
            BinaryOpNode andNode = (BinaryOpNode)orNode.Right;
            Assert.AreEqual("&&", andNode.Operator);
        }

        [TestMethod]
        public void BinaryOp_FloorDivision_ParsesAsCorrectToken()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := 10 // 3");
            BinaryOpNode binOp = (BinaryOpNode)assign.ValueExpression;
            Assert.AreEqual("//", binOp.Operator);
        }

        [TestMethod]
        public void BinaryOp_StringConcatenation_ParsesWithAmpersand()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := a & b");
            BinaryOpNode binOp = (BinaryOpNode)assign.ValueExpression;
            Assert.AreEqual("&", binOp.Operator);
        }

        // ----------------------------------------------------------------
        // Group 15: Unary operators
        // ----------------------------------------------------------------

        [TestMethod]
        public void UnaryOp_Minus_ProducesUnaryOpNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := -x");
            UnaryOpNode unary = (UnaryOpNode)assign.ValueExpression;
            Assert.AreEqual("-", unary.Operator);
        }

        [TestMethod]
        public void UnaryOp_Not_ProducesUnaryOpNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := not x");
            UnaryOpNode unary = (UnaryOpNode)assign.ValueExpression;
            Assert.AreEqual("not", unary.Operator);
        }

        [TestMethod]
        public void UnaryOp_DoubleNegation_WrapsCorrectly()
        {
            // "--x" would be two minus tokens, not a decrement; "not not x" nests
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := not not x");
            UnaryOpNode outer = (UnaryOpNode)assign.ValueExpression;
            Assert.AreEqual("not", outer.Operator);
            Assert.IsInstanceOfType(outer.Operand, typeof(UnaryOpNode));
        }

        // ----------------------------------------------------------------
        // Group 16: Ternary expression
        // ----------------------------------------------------------------

        [TestMethod]
        public void TernaryExpr_Basic_ProducesConditionalExprNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("r := a ? b : c");
            ConditionalExprNode ternary = (ConditionalExprNode)assign.ValueExpression;
            Assert.IsInstanceOfType(ternary.Condition, typeof(IdentifierNode));
            Assert.IsInstanceOfType(ternary.ThenExpression, typeof(IdentifierNode));
            Assert.IsInstanceOfType(ternary.ElseExpression, typeof(IdentifierNode));
        }

        // ----------------------------------------------------------------
        // Group 17: Array literal
        // ----------------------------------------------------------------

        [TestMethod]
        public void ArrayLiteral_Empty_ProducesArrayLiteralNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("arr := []");
            ArrayLiteralNode array = (ArrayLiteralNode)assign.ValueExpression;
            Assert.AreEqual(0, array.Elements.Count);
        }

        [TestMethod]
        public void ArrayLiteral_WithElements_ElementsAreCaptured()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "arr := [1, 2, 3]");
            ArrayLiteralNode array = (ArrayLiteralNode)assign.ValueExpression;
            Assert.AreEqual(3, array.Elements.Count);
        }

        // ----------------------------------------------------------------
        // Group 18: Index access
        // ----------------------------------------------------------------

        [TestMethod]
        public void IndexAccess_SimpleIndex_ProducesIndexAccessNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := arr[0]");
            IndexAccessNode index = (IndexAccessNode)assign.ValueExpression;
            Assert.IsInstanceOfType(index.TargetExpression, typeof(IdentifierNode));
            Assert.IsInstanceOfType(index.IndexExpression, typeof(IntegerLiteralNode));
        }

        [TestMethod]
        public void IndexAccess_ExpressionIndex_ParsesCorrectly()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := arr[i + 1]");
            IndexAccessNode index = (IndexAccessNode)assign.ValueExpression;
            Assert.IsInstanceOfType(index.IndexExpression, typeof(BinaryOpNode));
        }

        // ----------------------------------------------------------------
        // Group 19: Member access
        // ----------------------------------------------------------------

        [TestMethod]
        public void MemberAccess_DotField_ProducesMemberAccessNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := obj.field");
            MemberAccessNode member = (MemberAccessNode)assign.ValueExpression;
            Assert.AreEqual("field", member.MemberName);
        }

        // ----------------------------------------------------------------
        // Group 20: Method call (as expression)
        // ----------------------------------------------------------------

        [TestMethod]
        public void MethodCall_NoArgs_ProducesMethodCallNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := obj.method()");
            MethodCallNode method = (MethodCallNode)assign.ValueExpression;
            Assert.AreEqual("method", method.MethodName);
            Assert.AreEqual(0, method.Arguments.Count);
        }

        [TestMethod]
        public void MethodCall_Chained_NestedMethodCallNodes()
        {
            // "a.b().c()" — outer is MethodCallNode("c"), inner is MethodCallNode("b")
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := a.b().c()");
            MethodCallNode outer = (MethodCallNode)assign.ValueExpression;
            Assert.AreEqual("c", outer.MethodName);
            Assert.IsInstanceOfType(outer.TargetExpression, typeof(MethodCallNode));
        }

        // ----------------------------------------------------------------
        // Group 21: New expression
        // ----------------------------------------------------------------

        [TestMethod]
        public void NewExpr_NoArgs_ProducesNewExprNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("obj := new Foo()");
            NewExprNode newExpr = (NewExprNode)assign.ValueExpression;
            Assert.AreEqual("Foo", newExpr.ClassName);
            Assert.AreEqual(0, newExpr.Arguments.Count);
        }

        [TestMethod]
        public void NewExpr_WithArgs_ArgumentsAreCaptured()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "obj := new Foo(a, b)");
            NewExprNode newExpr = (NewExprNode)assign.ValueExpression;
            Assert.AreEqual(2, newExpr.Arguments.Count);
        }

        // ----------------------------------------------------------------
        // Group 22: Cast expression
        // ----------------------------------------------------------------

        [TestMethod]
        public void CastExpr_PrimitiveTypeCast_ProducesCastExprNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := (int) x");
            CastExprNode cast = (CastExprNode)assign.ValueExpression;
            Assert.AreEqual("int", cast.TargetType.TypeName);
            Assert.IsInstanceOfType(cast.Operand, typeof(IdentifierNode));
        }

        // ----------------------------------------------------------------
        // Group 23: Type check
        // ----------------------------------------------------------------

        [TestMethod]
        public void TypeCheck_IsExpression_ProducesTypeCheckNode()
        {
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := x is int");
            TypeCheckNode typeCheck = (TypeCheckNode)assign.ValueExpression;
            Assert.AreEqual("int", typeCheck.CheckedType.TypeName);
        }

        [TestMethod]
        public void TypeCheck_IsAtComparisonPrecedence_BindsToAdditiveExpr()
        {
            // "a + b is int" should parse as "(a + b) is int"
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := a + b is int");
            TypeCheckNode typeCheck = (TypeCheckNode)assign.ValueExpression;
            Assert.IsInstanceOfType(typeCheck.SubjectExpression, typeof(BinaryOpNode));
        }

        // ----------------------------------------------------------------
        // Group 24: Try-catch-finally
        // ----------------------------------------------------------------

        [TestMethod]
        public void TryStatement_WithCatch_ProducesTryStatementNode()
        {
            TryStatementNode node = (TryStatementNode)FirstStatement(
                "try print x catch e print e end");
            Assert.AreEqual(1, node.TryBody.Count);
            Assert.IsNotNull(node.CatchClause);
            Assert.AreEqual("e", node.CatchClause.ExceptionVariable);
            Assert.IsNull(node.FinallyBody);
        }

        [TestMethod]
        public void TryStatement_WithFinally_FinallyBodyIsSet()
        {
            TryStatementNode node = (TryStatementNode)FirstStatement(
                "try print x catch e print e finally print done end");
            Assert.IsNotNull(node.FinallyBody);
            Assert.AreEqual(1, node.FinallyBody.Count);
        }

        [TestMethod]
        public void TryStatement_WithTypedCatch_ExceptionTypeIsSet()
        {
            TryStatementNode node = (TryStatementNode)FirstStatement(
                "try print x catch (e : string) print e end");
            Assert.IsNotNull(node.CatchClause.ExceptionType);
            Assert.AreEqual("string", node.CatchClause.ExceptionType.TypeName);
        }

        // ----------------------------------------------------------------
        // Group 25: Throw
        // ----------------------------------------------------------------

        [TestMethod]
        public void ThrowStatement_WithExpression_ProducesThrowStatementNode()
        {
            ThrowStatementNode node = (ThrowStatementNode)FirstStatement(
                "throw \"error message\"");
            Assert.IsInstanceOfType(node.ThrownExpression, typeof(StringLiteralNode));
        }

        // ----------------------------------------------------------------
        // Group 26: Pattern match
        // ----------------------------------------------------------------

        [TestMethod]
        public void PatternMatch_WildcardPattern_ProducesWildcardPatternNode()
        {
            PatternMatchNode node = (PatternMatchNode)FirstStatement(
                "match x { _ => print x }");
            Assert.AreEqual(1, node.Cases.Count);
            Assert.IsInstanceOfType(node.Cases[0].Pattern, typeof(WildcardPatternNode));
        }

        [TestMethod]
        public void PatternMatch_LiteralPattern_ProducesLiteralNode()
        {
            PatternMatchNode node = (PatternMatchNode)FirstStatement(
                "match x { 1 => print x }");
            Assert.AreEqual(1, node.Cases.Count);
            Assert.IsInstanceOfType(node.Cases[0].Pattern, typeof(IntegerLiteralNode));
        }

        [TestMethod]
        public void PatternMatch_AlternationPattern_ProducesAlternationPatternNode()
        {
            PatternMatchNode node = (PatternMatchNode)FirstStatement(
                "match x { 1 | 2 => print x }");
            Assert.IsInstanceOfType(node.Cases[0].Pattern, typeof(AlternationPatternNode));
        }

        // ----------------------------------------------------------------
        // Group 27: Module definition
        // ----------------------------------------------------------------

        [TestMethod]
        public void ModuleDef_EmptyBody_ProducesModuleDefNode()
        {
            ModuleDefNode node = (ModuleDefNode)FirstStatement("module MyMod { }");
            Assert.AreEqual("MyMod", node.ModuleName);
            Assert.AreEqual(0, node.Body.Count);
        }

        [TestMethod]
        public void ModuleDef_WithBody_BodyStatementsCaptured()
        {
            ModuleDefNode node = (ModuleDefNode)FirstStatement(
                "module Utils { function helper() print 1 end }");
            Assert.AreEqual(1, node.Body.Count);
        }

        // ----------------------------------------------------------------
        // Group 28: Import / Export
        // ----------------------------------------------------------------

        [TestMethod]
        public void ImportStatement_Simple_ProducesImportStatementNode()
        {
            ImportStatementNode node = (ImportStatementNode)FirstStatement("import MyLib");
            Assert.AreEqual("MyLib", node.ModuleName);
            Assert.IsNull(node.Alias);
        }

        [TestMethod]
        public void ImportStatement_WithAlias_AliasIsSet()
        {
            ImportStatementNode node = (ImportStatementNode)FirstStatement(
                "import MyLib as ML");
            Assert.AreEqual("MyLib", node.ModuleName);
            Assert.AreEqual("ML", node.Alias);
        }

        [TestMethod]
        public void ExportStatement_Simple_ProducesExportStatementNode()
        {
            ExportStatementNode node = (ExportStatementNode)FirstStatement("export myFunc");
            Assert.AreEqual("myFunc", node.ExportedName);
        }

        // ----------------------------------------------------------------
        // Group 29: Disambiguation — lambda vs grouped expression
        // ----------------------------------------------------------------

        [TestMethod]
        public void Disambiguation_LambdaVsGrouped_FunctionKeywordMeansLambda()
        {
            // "function(x) 42" — starts with integer literal which cannot start a statement
            // so parsed as expression-body lambda
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "f := function(x) 42");
            Assert.IsInstanceOfType(assign.ValueExpression, typeof(LambdaExprNode));
            LambdaExprNode lambda = (LambdaExprNode)assign.ValueExpression;
            Assert.IsFalse(lambda.IsBlockBody);
        }

        [TestMethod]
        public void Disambiguation_GroupedExpr_ParenAroundExprIsGrouped()
        {
            // "(1 + 2)" is a grouped expression producing an integer addition
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "v := (1 + 2)");
            Assert.IsInstanceOfType(assign.ValueExpression, typeof(BinaryOpNode));
        }

        // ----------------------------------------------------------------
        // Group 30: Disambiguation — cast vs grouped
        // ----------------------------------------------------------------

        [TestMethod]
        public void Disambiguation_CastExpr_PrimitiveTypeInParensIsCast()
        {
            // "(int) x" is a cast
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := (int) x");
            Assert.IsInstanceOfType(assign.ValueExpression, typeof(CastExprNode));
        }

        [TestMethod]
        public void Disambiguation_GroupedExpr_ExpressionInParensIsNotCast()
        {
            // "(x + 1)" is a grouped expression
            AssignStatementNode assign = (AssignStatementNode)FirstStatement("v := (x + 1)");
            Assert.IsInstanceOfType(assign.ValueExpression, typeof(BinaryOpNode));
        }

        // ----------------------------------------------------------------
        // Group 31: Disambiguation — if as expression vs statement
        // ----------------------------------------------------------------

        [TestMethod]
        public void Disambiguation_IfAsStatement_TopLevelIfIsStatement()
        {
            // "if x then print x end" is an if statement
            ProgramNode prog = Parse("if x then print x end");
            Assert.IsInstanceOfType(prog.Statements[0], typeof(IfStatementNode));
        }

        [TestMethod]
        public void Disambiguation_IfAsExpression_RhsIfIsConditionalExpr()
        {
            // "v := if a then b else c" — inline conditional expression form
            AssignStatementNode assign = (AssignStatementNode)FirstStatement(
                "v := if a then b else c");
            Assert.IsInstanceOfType(assign.ValueExpression, typeof(ConditionalExprNode));
        }

        // ----------------------------------------------------------------
        // Group 32: Trailing semicolons are parse errors (note 21)
        // ----------------------------------------------------------------

        [TestMethod]
        public void SemicolonSeparator_TrailingSemicolonBeforeEnd_ThrowsParserException()
        {
            Assert.ThrowsException<ParserException>(() =>
                Parse("function f() print 1; end"));
        }

        [TestMethod]
        public void SemicolonSeparator_BetweenStatements_IsAllowed()
        {
            // Two statements separated by semicolon — valid
            ProgramNode prog = Parse("let x := 1; let y := 2");
            Assert.AreEqual(2, prog.Statements.Count);
        }

        // ----------------------------------------------------------------
        // Group 33: Multiple top-level statements
        // ----------------------------------------------------------------

        [TestMethod]
        public void Program_MultipleTopLevelStatements_AllParsed()
        {
            ProgramNode prog = Parse(
                "let x := 1; let y := 2; let z := 3");
            Assert.AreEqual(3, prog.Statements.Count);
        }

        [TestMethod]
        public void Program_FunctionAndStatement_BothParsedInOrder()
        {
            ProgramNode prog = Parse(
                "function foo() print 1 end; let x := 42");
            Assert.AreEqual(2, prog.Statements.Count);
            Assert.IsInstanceOfType(prog.Statements[0], typeof(FunctionDefNode));
            Assert.IsInstanceOfType(prog.Statements[1], typeof(LetDeclareNode));
        }
    }
}
