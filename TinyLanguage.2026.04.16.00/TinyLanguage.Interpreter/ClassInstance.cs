using System.Collections.Generic;

namespace TinyLanguage.Interpreter
{
    // Represents a runtime instance of a TinyLanguage class.
    // Holds a dictionary of field values and a reference to the class definition.
    public sealed class ClassInstance
    {
        // The class definition that this instance was created from.
        public readonly ClassValue ClassDefinition;

        // The instance field values, keyed by field name.
        public readonly Dictionary<string, object> Fields;

        public ClassInstance(ClassValue classDefinition)
        {
            ClassDefinition = classDefinition;
            Fields = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            return $"<instance of {ClassDefinition.Name}>";
        }
    }
}
