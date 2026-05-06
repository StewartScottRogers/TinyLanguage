namespace TinyLanguage.Lexer.Nodes
{
    // Represents the "input" keyword statement:
    //   "input" <id>
    // Reads a line from Console.In and stores it in the named variable.
    public class InputStatementNode : AstNode
    {
        // The variable that receives the user-typed line.
        public string Id { get; }

        public InputStatementNode(int line, string id)
            : base(line)
        {
            Id = id;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitInputStatementNode(this);
        }
    }
}
