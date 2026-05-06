using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a constructor inside a class body:
    //   "Constructor" "(" [<param_list>] ")" <stmt_list> "end"
    // Note: the keyword is "Constructor" (capital C) per the BNF.
    public class ConstructorDefNode : AstNode
    {
        // The ordered list of parameter declarations (may be empty).
        public IReadOnlyList<ParameterNode> Parameters { get; }

        // The statements that form the constructor body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public ConstructorDefNode(
            int line,
            IReadOnlyList<ParameterNode> parameters,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Parameters = parameters;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitConstructorDefNode(this);
        }
    }
}
