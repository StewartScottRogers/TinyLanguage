using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents one case inside a match statement:
    //   <pattern> "=>" <stmt_list>
    //   <pattern> "when" <expr> "=>" <stmt_list>
    public class PatternCaseNode : AstNode
    {
        // The pattern to match against the match expression.
        public PatternNode Pattern { get; }

        // An optional guard condition. Null when "when" is absent.
        public AstNode GuardExpr { get; }

        // The statements executed when this pattern matches (and the guard passes).
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public PatternCaseNode(
            int line,
            PatternNode pattern,
            AstNode guardExpr,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Pattern = pattern;
            GuardExpr = guardExpr;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitPatternCaseNode(this);
        }
    }
}
