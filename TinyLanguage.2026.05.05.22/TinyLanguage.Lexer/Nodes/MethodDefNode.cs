using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a method inside a class body (Implementation Note 16):
    //   ["static"] "function" <id> "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"
    // Structurally the same as FunctionDefNode but also carries the IsStatic flag.
    public class MethodDefNode : AstNode
    {
        // The method name.
        public string Id { get; }

        // The ordered list of parameter declarations (may be empty).
        public IReadOnlyList<ParameterNode> Parameters { get; }

        // Optional return-type annotation. Null when "->" is absent.
        public TypeNode ReturnType { get; }

        // The statements that form the method body.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        // True when the "static" keyword preceded the "function" keyword.
        public bool IsStatic { get; }

        public MethodDefNode(
            int line,
            string id,
            IReadOnlyList<ParameterNode> parameters,
            TypeNode returnType,
            IReadOnlyList<AstNode> bodyStatements,
            bool isStatic)
            : base(line)
        {
            Id = id;
            Parameters = parameters;
            ReturnType = returnType;
            BodyStatements = bodyStatements;
            IsStatic = isStatic;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitMethodDefNode(this);
        }
    }
}
