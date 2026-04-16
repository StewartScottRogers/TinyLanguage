namespace TinyLanguage.Lexer
{
    // Abstract base class for every node in the Abstract Syntax Tree.
    // Every concrete node stores the source line number for runtime error reporting.
    // All concrete nodes must implement Accept so that visitors can dispatch on them.
    public abstract class AstNode
    {
        // 1-based source line number where this node begins.
        public readonly int Line;

        protected AstNode(int line)
        {
            Line = line;
        }

        // Visitor dispatch — each concrete subclass calls visitor.Visit(this).
        public abstract void Accept(INodeVisitor visitor);
    }
}
