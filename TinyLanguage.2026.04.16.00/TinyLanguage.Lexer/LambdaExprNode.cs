using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a lambda (anonymous function) expression.
    // Two forms:
    //   "function" "(" [<param_list>] ")" ["->" <type>] <expr>        — expression body
    //   "function" "(" [<param_list>] ")" ["->" <type>] <stmt_list> "end" — block body
    // See spec note 14 for disambiguation strategy.
    // Lambdas capture no variables from the enclosing scope (spec Closures section).
    public class LambdaExprNode : AstNode
    {
        // The formal parameter list — may be empty.
        public readonly List<ParameterNode> Parameters;

        // Optional declared return type — null when not specified.
        public readonly TypeNode ReturnType;

        // For expression-body lambdas: a single expression node.
        // For block-body lambdas: a list of statement nodes.
        // Exactly one of ExpressionBody / StatementBody is non-null.
        public readonly AstNode ExpressionBody;

        // The statement list for block-body lambdas — null for expression-body lambdas.
        public readonly List<AstNode> StatementBody;

        // True when this lambda uses a block body (ends with "end"); false for expression body.
        public readonly bool IsBlockBody;

        public LambdaExprNode(List<ParameterNode> parameters, TypeNode returnType, AstNode expressionBody, int line) : base(line)
        {
            Parameters = parameters;
            ReturnType = returnType;
            ExpressionBody = expressionBody;
            StatementBody = null;
            IsBlockBody = false;
        }

        public LambdaExprNode(List<ParameterNode> parameters, TypeNode returnType, List<AstNode> statementBody, int line) : base(line)
        {
            Parameters = parameters;
            ReturnType = returnType;
            ExpressionBody = null;
            StatementBody = statementBody;
            IsBlockBody = true;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
