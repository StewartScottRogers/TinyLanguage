// stub
namespace TinyLanguage.Lexer
{
    // Represents a type annotation in the AST (e.g., int, bool, string).
    public class TypeNode : AstNode
    {
        // The name of the type as it appears in source code.
        public readonly string TypeName;

        public TypeNode(string typeName, int line) : base(line)
        {
            TypeName = typeName;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
