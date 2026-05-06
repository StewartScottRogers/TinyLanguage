using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a top-level (non-method) function definition:
    //   "function" <id> "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"
    // Top-level functions are valid statements (Implementation Note 18).
    public class FunctionDefNode : AstNode
    {
        // The function name.
        public string Id { get; }

        // The ordered list of parameter declarations (may be empty).
        public IReadOnlyList<ParameterNode> Parameters { get; }

        // Optional return-type annotation. Null when "->" is absent.
        public TypeNode ReturnType { get; }

        // The statements that form the function body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public FunctionDefNode(
            int line,
            string id,
            IReadOnlyList<ParameterNode> parameters,
            TypeNode returnType,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Id = id;
            Parameters = parameters;
            ReturnType = returnType;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitFunctionDefNode(this);
        }
    }
}
