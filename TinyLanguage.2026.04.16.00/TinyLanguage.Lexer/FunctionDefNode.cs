using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a function definition statement:
    // "function" <id> "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end"
    // Functions are valid at any nesting level (spec note 18).
    // The optional "static" modifier is captured in IsStatic.
    public class FunctionDefNode : AstNode
    {
        // The function name.
        public readonly string FunctionName;

        // The formal parameter list — may be empty.
        public readonly List<ParameterNode> Parameters;

        // Optional declared return type — null when not specified.
        public readonly TypeNode ReturnType;

        // The list of statements that form the function body.
        public readonly List<AstNode> Body;

        // True when the "static" modifier preceded the "function" keyword.
        public readonly bool IsStatic;

        public FunctionDefNode(string functionName, List<ParameterNode> parameters, TypeNode returnType, List<AstNode> body, bool isStatic, int line) : base(line)
        {
            FunctionName = functionName;
            Parameters = parameters;
            ReturnType = returnType;
            Body = body;
            IsStatic = isStatic;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
