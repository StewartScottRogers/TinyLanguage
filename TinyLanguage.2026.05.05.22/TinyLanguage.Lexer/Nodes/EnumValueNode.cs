namespace TinyLanguage.Lexer.Nodes
{
    // Represents one member inside an enum body.
    //   <id>              — bare identifier (auto-assigned value)
    //   <id> "=" <expr>   — identifier with explicit value (uses "=" not ":=")
    public class EnumValueNode : AstNode
    {
        // The name of the enum member.
        public string Id { get; }

        // Optional explicit value expression. Null for bare-identifier form.
        public AstNode ValueExpr { get; }

        public EnumValueNode(int line, string id, AstNode valueExpr)
            : base(line)
        {
            Id = id;
            ValueExpr = valueExpr;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitEnumValueNode(this);
        }
    }
}
