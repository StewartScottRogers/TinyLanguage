using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // A pattern that matches an object by field values:
    //   <id> "{" [<field_pattern_list>] "}"
    // Example: Point { x: 0, y: 0 } matches a Point where x and y are both 0.
    // An empty field list "Foo {}" is valid and matches any Foo instance.
    public class FieldPatternNode : PatternNode
    {
        // The type name to match against.
        public string TypeId { get; }

        // The list of field-name / sub-pattern pairs (may be empty).
        public IReadOnlyList<FieldPatternPair> FieldPatterns { get; }

        public FieldPatternNode(int line, string typeId, IReadOnlyList<FieldPatternPair> fieldPatterns)
            : base(line)
        {
            TypeId = typeId;
            FieldPatterns = fieldPatterns;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitFieldPatternNode(this);
        }
    }

    // One entry in a field pattern list: the field name and the pattern it must satisfy.
    public sealed class FieldPatternPair
    {
        public string FieldId { get; }
        public PatternNode Pattern { get; }

        public FieldPatternPair(string fieldId, PatternNode pattern)
        {
            FieldId = fieldId;
            Pattern = pattern;
        }
    }
}
