namespace TinyLanguage.Lexer
{
    // Visitor interface implemented by any pass that walks the AST without modifying nodes.
    // The interpreter, pretty-printer, and any future analysis tools all implement this.
    // Every concrete AstNode subclass has exactly one overload here.
    public interface INodeVisitor
    {
        // ----------------------------------------------------------------
        // Program root
        // ----------------------------------------------------------------
        void Visit(ProgramNode node);

        // ----------------------------------------------------------------
        // Statement nodes
        // ----------------------------------------------------------------
        void Visit(AssignStatementNode node);
        void Visit(LetDeclareNode node);
        void Visit(VarDeclareNode node);
        void Visit(ConstDeclareNode node);
        void Visit(EnumDefNode node);
        void Visit(EnumValueNode node);
        void Visit(IfStatementNode node);
        void Visit(WhileStatementNode node);
        void Visit(ForStatementNode node);
        void Visit(ForeachStatementNode node);
        void Visit(DoWhileStatementNode node);
        void Visit(SwitchStatementNode node);
        void Visit(SwitchCaseNode node);
        void Visit(BreakStatementNode node);
        void Visit(ContinueStatementNode node);
        void Visit(PrintStatementNode node);
        void Visit(InputStatementNode node);
        void Visit(FunctionDefNode node);
        void Visit(ReturnStatementNode node);
        void Visit(CallStatementNode node);
        void Visit(ClassDefNode node);
        void Visit(ConstructorDefNode node);
        void Visit(FieldDeclareNode node);
        void Visit(ModuleDefNode node);
        void Visit(ModuleImportNode node);
        void Visit(ImportStatementNode node);
        void Visit(ExportStatementNode node);
        void Visit(TryStatementNode node);
        void Visit(CatchClauseNode node);
        void Visit(ThrowStatementNode node);
        void Visit(PatternMatchNode node);
        void Visit(PatternCaseNode node);
        void Visit(AnnotatedStatementNode node);
        void Visit(AnnotationNode node);
        void Visit(AnnotationParamNode node);
        void Visit(ArrayElementAssignNode node);
        void Visit(MemberAssignNode node);

        // ----------------------------------------------------------------
        // Expression nodes
        // ----------------------------------------------------------------
        void Visit(BinaryOpNode node);
        void Visit(UnaryOpNode node);
        void Visit(ConditionalExprNode node);
        void Visit(IdentifierNode node);
        void Visit(IntegerLiteralNode node);
        void Visit(FloatLiteralNode node);
        void Visit(StringLiteralNode node);
        void Visit(BoolLiteralNode node);
        void Visit(NullLiteralNode node);
        void Visit(ArrayLiteralNode node);
        void Visit(IndexAccessNode node);
        void Visit(MemberAccessNode node);
        void Visit(MethodCallNode node);
        void Visit(FunctionCallNode node);
        void Visit(NewExprNode node);
        void Visit(CastExprNode node);
        void Visit(TypeCheckNode node);
        void Visit(TypeAssertNode node);
        void Visit(LambdaExprNode node);
        void Visit(ListComprehensionNode node);

        // ----------------------------------------------------------------
        // Pattern nodes
        // ----------------------------------------------------------------
        void Visit(WildcardPatternNode node);
        void Visit(ConstructorPatternNode node);
        void Visit(ArrayPatternNode node);
        void Visit(FieldPatternNode node);
        void Visit(FieldPatternEntryNode node);
        void Visit(AlternationPatternNode node);

        // ----------------------------------------------------------------
        // Support nodes
        // ----------------------------------------------------------------
        void Visit(ParameterNode node);
        void Visit(TypeNode node);
    }
}
