using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a single catch clause inside a try statement.
    // Two forms:
    //   "catch" <id> <stmt_list>               — bare identifier (no type)
    //   "catch" "(" <id> ":" <type> ")" <stmt_list> — typed with parentheses
    public class CatchClauseNode : AstNode
    {
        // The name of the variable that will hold the caught exception value.
        public readonly string ExceptionVariable;

        // Optional type constraint — null for the bare-identifier form.
        public readonly TypeNode ExceptionType;

        // The list of statements forming the catch handler body.
        public readonly List<AstNode> Body;

        public CatchClauseNode(string exceptionVariable, TypeNode exceptionType, List<AstNode> body, int line) : base(line)
        {
            ExceptionVariable = exceptionVariable;
            ExceptionType = exceptionType;
            Body = body;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
