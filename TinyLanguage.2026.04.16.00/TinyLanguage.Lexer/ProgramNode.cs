using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Root node of the AST — contains the flat list of top-level statements.
    // A program is a <stmt_list> that may mix definitions and executable statements in any order.
    public class ProgramNode : AstNode
    {
        // The ordered list of top-level statements parsed from the source.
        public readonly List<AstNode> Statements;

        public ProgramNode(List<AstNode> statements, int line) : base(line)
        {
            Statements = statements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
