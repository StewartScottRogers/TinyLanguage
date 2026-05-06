using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents an enum declaration:
    //   enum <id> "{" <enum_value_list> "}"
    // Enum members are collected as EnumValueNode children.
    public class EnumDefNode : AstNode
    {
        // The name of the enumeration type.
        public string Id { get; }

        // The ordered list of enum member declarations.
        public IReadOnlyList<EnumValueNode> EnumValues { get; }

        public EnumDefNode(int line, string id, IReadOnlyList<EnumValueNode> enumValues)
            : base(line)
        {
            Id = id;
            EnumValues = enumValues;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitEnumDefNode(this);
        }
    }
}
