using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents the "default" arm inside a switch statement:
    //   "default" ":" <stmt_list>
    // At most one default clause is allowed per switch.
    public class DefaultClauseNode : AstNode
    {
        // The statements executed when no case matches the switch expression.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public DefaultClauseNode(int line, IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitDefaultClauseNode(this);
        }
    }
}
