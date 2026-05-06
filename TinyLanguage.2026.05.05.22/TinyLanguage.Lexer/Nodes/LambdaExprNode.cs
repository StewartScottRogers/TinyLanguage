using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a lambda (anonymous function) expression (Implementation Note 14).
    // Two forms:
    //   Expression body: function([params]) [-> type] <expr>
    //   Block body:      function([params]) [-> type] <stmt_list> end
    //
    // IsBlockForm distinguishes the two at the AST level. For the expression
    // body form, BodyExpr is set and BodyStatements is empty. For the block body
    // form, BodyStatements is set and BodyExpr is null.
    public class LambdaExprNode : AstNode
    {
        // The ordered list of parameter declarations (may be empty).
        public IReadOnlyList<ParameterNode> Parameters { get; }

        // Optional return-type annotation. Null when "->" is absent.
        public TypeNode ReturnType { get; }

        // The single body expression (expression-body form). Null for block form.
        public AstNode BodyExpr { get; }

        // The body statement list (block-body form). Empty for expression form.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        // True when the block-body form (with "end") was parsed.
        public bool IsBlockForm { get; }

        public LambdaExprNode(
            int line,
            IReadOnlyList<ParameterNode> parameters,
            TypeNode returnType,
            AstNode bodyExpr,
            IReadOnlyList<AstNode> bodyStatements,
            bool isBlockForm)
            : base(line)
        {
            Parameters = parameters;
            ReturnType = returnType;
            BodyExpr = bodyExpr;
            BodyStatements = bodyStatements;
            IsBlockForm = isBlockForm;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitLambdaExprNode(this);
        }
    }
}
