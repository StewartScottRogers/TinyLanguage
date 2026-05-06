using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Lexer
{
    // Visitor interface for the AST. Each method corresponds to exactly one
    // concrete node type. Implementing classes (Interpreter, AstPrettyPrinter,
    // etc.) provide the logic for each node without touching the node classes.
    public interface INodeVisitor
    {
        // -----------------------------------------------------------------
        // Program
        // -----------------------------------------------------------------
        void VisitProgramNode(ProgramNode node);

        // -----------------------------------------------------------------
        // Statements
        // -----------------------------------------------------------------
        void VisitAssignStatementNode(AssignStatementNode node);
        void VisitLetDeclareNode(LetDeclareNode node);
        void VisitVarDeclareNode(VarDeclareNode node);
        void VisitConstDeclareNode(ConstDeclareNode node);
        void VisitEnumDefNode(EnumDefNode node);
        void VisitEnumValueNode(EnumValueNode node);
        void VisitArrayElementAssignNode(ArrayElementAssignNode node);
        void VisitMemberAssignNode(MemberAssignNode node);
        void VisitIndexedAssignNode(IndexedAssignNode node);
        void VisitExpressionStatementNode(ExpressionStatementNode node);
        void VisitIfStatementNode(IfStatementNode node);
        void VisitWhileStatementNode(WhileStatementNode node);
        void VisitForStatementNode(ForStatementNode node);
        void VisitForeachStatementNode(ForeachStatementNode node);
        void VisitDoWhileStatementNode(DoWhileStatementNode node);
        void VisitSwitchStatementNode(SwitchStatementNode node);
        void VisitCaseClauseNode(CaseClauseNode node);
        void VisitDefaultClauseNode(DefaultClauseNode node);
        void VisitBreakStatementNode(BreakStatementNode node);
        void VisitContinueStatementNode(ContinueStatementNode node);
        void VisitPrintStatementNode(PrintStatementNode node);
        void VisitInputStatementNode(InputStatementNode node);
        void VisitFunctionDefNode(FunctionDefNode node);
        void VisitMethodDefNode(MethodDefNode node);
        void VisitCallStatementNode(CallStatementNode node);
        void VisitReturnStatementNode(ReturnStatementNode node);
        void VisitClassDefNode(ClassDefNode node);
        void VisitConstructorDefNode(ConstructorDefNode node);
        void VisitFieldDeclareNode(FieldDeclareNode node);
        void VisitModuleDefNode(ModuleDefNode node);
        void VisitModuleImportNode(ModuleImportNode node);
        void VisitImportStatementNode(ImportStatementNode node);
        void VisitExportStatementNode(ExportStatementNode node);
        void VisitTryStatementNode(TryStatementNode node);
        void VisitCatchClauseNode(CatchClauseNode node);
        void VisitThrowStatementNode(ThrowStatementNode node);
        void VisitPatternMatchNode(PatternMatchNode node);
        void VisitPatternCaseNode(PatternCaseNode node);
        void VisitAnnotatedStatementNode(AnnotatedStatementNode node);
        void VisitAnnotationNode(AnnotationNode node);
        void VisitAnnotationParamNode(AnnotationParamNode node);

        // -----------------------------------------------------------------
        // Patterns
        // -----------------------------------------------------------------
        void VisitIdentifierPatternNode(IdentifierPatternNode node);
        void VisitLiteralPatternNode(LiteralPatternNode node);
        void VisitWildcardPatternNode(WildcardPatternNode node);
        void VisitConstructorPatternNode(ConstructorPatternNode node);
        void VisitArrayPatternNode(ArrayPatternNode node);
        void VisitFieldPatternNode(FieldPatternNode node);
        void VisitAlternationPatternNode(AlternationPatternNode node);

        // -----------------------------------------------------------------
        // Expressions
        // -----------------------------------------------------------------
        void VisitBinaryOpNode(BinaryOpNode node);
        void VisitUnaryOpNode(UnaryOpNode node);
        void VisitTernaryNode(TernaryNode node);
        void VisitIdentifierNode(IdentifierNode node);
        void VisitIntegerLiteralNode(IntegerLiteralNode node);
        void VisitFloatLiteralNode(FloatLiteralNode node);
        void VisitStringLiteralNode(StringLiteralNode node);
        void VisitBoolLiteralNode(BoolLiteralNode node);
        void VisitNullLiteralNode(NullLiteralNode node);
        void VisitArrayLiteralNode(ArrayLiteralNode node);
        void VisitIndexAccessNode(IndexAccessNode node);
        void VisitMemberAccessNode(MemberAccessNode node);
        void VisitMethodCallNode(MethodCallNode node);
        void VisitFunctionCallNode(FunctionCallNode node);
        void VisitNewExprNode(NewExprNode node);
        void VisitCastExprNode(CastExprNode node);
        void VisitTypeCheckNode(TypeCheckNode node);
        void VisitTypeAssertNode(TypeAssertNode node);
        void VisitConditionalExprNode(ConditionalExprNode node);
        void VisitLambdaExprNode(LambdaExprNode node);
        void VisitListComprehensionNode(ListComprehensionNode node);

        // -----------------------------------------------------------------
        // Shared / auxiliary
        // -----------------------------------------------------------------
        void VisitParameterNode(ParameterNode node);
        void VisitTypeNode(TypeNode node);
    }
}
