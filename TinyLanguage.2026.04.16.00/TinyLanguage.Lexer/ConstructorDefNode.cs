using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a constructor definition inside a class body:
    // "Constructor" "(" [<param_list>] ")" <stmt_list> "end"
    // Note: "Constructor" uses UpperCamelCase as specified in the grammar.
    public class ConstructorDefNode : AstNode
    {
        // The formal parameter list for the constructor — may be empty.
        public readonly List<ParameterNode> Parameters;

        // The list of statements forming the constructor body.
        public readonly List<AstNode> Body;

        public ConstructorDefNode(List<ParameterNode> parameters, List<AstNode> body, int line) : base(line)
        {
            Parameters = parameters;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
