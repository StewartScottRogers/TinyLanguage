namespace TinyLanguage.Lexer
{
    // Abstract base class for every node in the Abstract Syntax Tree.
    // Every node stores the source line number so that runtime errors
    // can report exactly where a fault occurred (per Build.Solution.md
    // "AST requirement").
    public abstract class AstNode
    {
        // The 1-based line number in the source file where this node begins.
        public int Line { get; }

        // Initializes the node with the line number from the source text.
        protected AstNode(int line)
        {
            Line = line;
        }

        // Visitor dispatch — each concrete subclass calls the matching
        // Visit overload on the visitor, enabling double dispatch without
        // a large if-else or switch chain.
        public abstract void Accept(INodeVisitor visitor);
    }
}
