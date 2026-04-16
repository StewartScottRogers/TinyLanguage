using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a single case clause in a switch statement:
    // "case" <expr> ":" <stmt_list>
    // The IsDefault flag distinguishes a "default" clause from a "case" clause.
    public class SwitchCaseNode : AstNode
    {
        // The value expression to compare against the switch subject.
        // Null when this is the default clause.
        public readonly AstNode ValueExpression;

        // The list of statements to execute when this case matches.
        public readonly List<AstNode> Body;

        // True when this represents the "default:" clause; false for "case <expr>:".
        public readonly bool IsDefault;

        public SwitchCaseNode(AstNode valueExpression, List<AstNode> body, bool isDefault, int line) : base(line)
        {
            ValueExpression = valueExpression;
            Body = body;
            IsDefault = isDefault;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
