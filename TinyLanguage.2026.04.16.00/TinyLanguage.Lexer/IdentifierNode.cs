namespace TinyLanguage.Lexer
{
    // Represents a variable or function reference by name.
    // At runtime the interpreter looks up the name in the scope chain.
    public class IdentifierNode : AstNode
    {
        // The identifier string as it appeared in the source.
        public readonly string Name;

        public IdentifierNode(string name, int line) : base(line)
        {
            Name = name;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
