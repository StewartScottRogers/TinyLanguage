namespace TinyLanguage.Lexer;

// Abstract base class for every node in the Abstract Syntax Tree.
// All concrete node types inherit from this class and implement Accept
// to dispatch themselves to the INodeVisitor (visitor pattern).
// The Line property stores the 1-based source line number for error reporting.
public abstract class AstNode
{
    public readonly int Line;

    protected AstNode(int line)
    {
        Line = line;
    }

    // Each concrete node calls visitor.Visit(this) so the visitor
    // receives the exact derived type without any casting.
    public abstract void Accept(INodeVisitor visitor);
}
