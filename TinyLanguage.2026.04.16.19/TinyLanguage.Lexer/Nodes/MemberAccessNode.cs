using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Represents postfix member access: expr . id
// Does not call the member — for method calls see MethodCallNode.
public sealed class MemberAccessNode : AstNode
{
    public readonly AstNode Target;
    public readonly string MemberName;

    public MemberAccessNode(AstNode target, string memberName, int line) : base(line)
    {
        Target = target;
        MemberName = memberName;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
