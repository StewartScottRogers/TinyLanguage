namespace TinyLanguage.Lexer
{
    // Represents one field binding in a field pattern: <id> ":" <pattern>
    // Part of the <field_pattern_list> inside a FieldPatternNode.
    public class FieldPatternEntryNode : AstNode
    {
        // The name of the field to match.
        public readonly string FieldName;

        // The pattern to test the field value against.
        public readonly AstNode FieldPattern;

        public FieldPatternEntryNode(string fieldName, AstNode fieldPattern, int line) : base(line)
        {
            FieldName = fieldName;
            FieldPattern = fieldPattern;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
