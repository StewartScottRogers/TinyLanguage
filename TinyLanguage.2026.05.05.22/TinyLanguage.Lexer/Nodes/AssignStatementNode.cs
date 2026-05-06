namespace TinyLanguage.Lexer.Nodes
{
    // Represents a plain assignment statement: <id> ":=" <expr>
    // This node is for reassigning an existing variable. New variable
    // declarations use LetDeclareNode, VarDeclareNode, or ConstDeclareNode.
    public class AssignStatementNode : AstNode
    {
        // The name of the variable being assigned.
        public string Id { get; }

        // The expression whose value is stored in the variable.
        public AstNode Expr { get; }

        public AssignStatementNode(int line, string id, AstNode expr)
            : base(line)
        {
            Id = id;
            Expr = expr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitAssignStatementNode(this);
        }
    }
}
