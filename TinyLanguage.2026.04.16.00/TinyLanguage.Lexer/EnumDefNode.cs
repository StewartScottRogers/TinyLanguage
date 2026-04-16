using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents an enum definition: "enum" <id> "{" <enum_value_list> "}"
    // Enum members are declared with "=" (SingleEqual), not ":=".
    public class EnumDefNode : AstNode
    {
        // The name of the enum type.
        public readonly string EnumName;

        // The ordered list of enum member declarations.
        public readonly List<EnumValueNode> Members;

        public EnumDefNode(string enumName, List<EnumValueNode> members, int line) : base(line)
        {
            EnumName = enumName;
            Members = members;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
