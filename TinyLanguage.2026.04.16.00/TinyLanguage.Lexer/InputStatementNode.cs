namespace TinyLanguage.Lexer
{
    // Represents an input statement: "input" <id>
    // Reads a line from Console.In and stores it as a string in the named variable.
    public class InputStatementNode : AstNode
    {
        // The name of the variable that will receive the input string.
        public readonly string VariableName;

        public InputStatementNode(string variableName, int line) : base(line)
        {
            VariableName = variableName;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
