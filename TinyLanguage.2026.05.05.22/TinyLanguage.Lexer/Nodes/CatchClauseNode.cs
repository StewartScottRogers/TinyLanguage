using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a catch clause inside a try statement. Two forms:
    //   "catch" <id> <stmt_list>                    — bare identifier form
    //   "catch" "(" <id> ":" <type> ")" <stmt_list> — typed form
    public class CatchClauseNode : AstNode
    {
        // The name of the variable that receives the thrown exception.
        public string Id { get; }

        // Optional type annotation for the caught exception. Null for bare form.
        public TypeNode ExceptionType { get; }

        // The statements that form the catch handler.
        public IReadOnlyList<AstNode> BodyStatements { get; }

        public CatchClauseNode(
            int line,
            string id,
            TypeNode exceptionType,
            IReadOnlyList<AstNode> bodyStatements)
            : base(line)
        {
            Id = id;
            ExceptionType = exceptionType;
            BodyStatements = bodyStatements;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitCatchClauseNode(this);
        }
    }
}
