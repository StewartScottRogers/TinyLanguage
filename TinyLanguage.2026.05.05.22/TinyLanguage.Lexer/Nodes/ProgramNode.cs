using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // The root node of every parsed program. Contains the flat list of
    // top-level statements (definitions and executable statements may
    // appear in any order per Implementation Note 18).
    public class ProgramNode : AstNode
    {
        // The ordered list of top-level statements.
        public IReadOnlyList<AstNode> Statements { get; }

        public ProgramNode(int line, IReadOnlyList<AstNode> statements)
            : base(line)
        {
            Statements = statements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitProgramNode(this);
        }
    }
}
