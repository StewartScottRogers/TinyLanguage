namespace TinyLanguage.Interpreter
{
    // Represents a runtime enum type definition.
    // Stored in scope so that enum members can be accessed via EnumName.MemberName.
    public sealed class EnumValue
    {
        // The name of the enum type.
        public readonly string Name;

        // The dictionary of member name to integer value.
        public readonly System.Collections.Generic.Dictionary<string, long> Members;

        public EnumValue(string name, System.Collections.Generic.Dictionary<string, long> members)
        {
            Name = name;
            Members = members;
        }

        public override string ToString()
        {
            return $"<enum {Name}>";
        }
    }
}
