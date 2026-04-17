using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Lexer;

/// <summary>
/// Visitor interface for the Abstract Syntax Tree.
/// Every concrete AST node calls <c>visitor.Visit(this)</c> in its
/// <c>Accept</c> implementation, dispatching to the appropriate overload below.
/// Both the pretty-printer and the interpreter implement this interface.
/// </summary>
public interface INodeVisitor
{
    // ── Statements ────────────────────────────────────────────────────────
    void Visit(AnnotatedStatementNode node);
    void Visit(ArrayElementAssignNode node);
    void Visit(AssignStatementNode node);
    void Visit(BreakStatementNode node);
    void Visit(CallStatementNode node);
    void Visit(ConstDeclareNode node);
    void Visit(ContinueStatementNode node);
    void Visit(DoBlockNode node);
    void Visit(DoWhileStatementNode node);
    void Visit(EnumDefNode node);
    void Visit(ExportStatementNode node);
    void Visit(FieldDeclareNode node);
    void Visit(ForeachStatementNode node);
    void Visit(ForStatementNode node);
    void Visit(FunctionDefNode node);
    void Visit(IfStatementNode node);
    void Visit(ImportStatementNode node);
    void Visit(InputStatementNode node);
    void Visit(LetDeclareNode node);
    void Visit(PatternMatchNode node);
    void Visit(PrintStatementNode node);
    void Visit(ReturnStatementNode node);
    void Visit(SwitchStatementNode node);
    void Visit(ThrowStatementNode node);
    void Visit(TryStatementNode node);
    void Visit(VarDeclareNode node);
    void Visit(WhileStatementNode node);

    // ── Definitions ───────────────────────────────────────────────────────
    void Visit(ClassDefNode node);
    void Visit(ConstructorDefNode node);
    void Visit(MethodDefNode node);
    void Visit(ModuleDefNode node);

    // ── Expressions ───────────────────────────────────────────────────────
    void Visit(AnnotationNode node);
    void Visit(ArrayLiteralNode node);
    void Visit(BinaryOpNode node);
    void Visit(BoolLiteralNode node);
    void Visit(CastExprNode node);
    void Visit(ConditionalExprNode node);
    void Visit(FloatLiteralNode node);
    void Visit(FunctionCallNode node);
    void Visit(IdentifierNode node);
    void Visit(IndexAccessNode node);
    void Visit(IntegerLiteralNode node);
    void Visit(LambdaExprNode node);
    void Visit(ListComprehensionNode node);
    void Visit(MemberAccessNode node);
    void Visit(MethodCallNode node);
    void Visit(NewExprNode node);
    void Visit(NullLiteralNode node);
    void Visit(StringLiteralNode node);
    void Visit(TernaryNode node);
    void Visit(TypeAssertNode node);
    void Visit(TypeCheckNode node);
    void Visit(UnaryOpNode node);

    // ── Structural / support nodes ────────────────────────────────────────
    void Visit(ParameterNode node);
    void Visit(PatternCaseNode node);
    void Visit(PatternNode node);
    void Visit(ProgramNode node);
    void Visit(TypeNode node);
}
