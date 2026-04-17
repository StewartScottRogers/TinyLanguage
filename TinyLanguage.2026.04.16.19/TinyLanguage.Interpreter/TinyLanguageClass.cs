using System.Collections.Generic;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Interpreter;

// Runtime representation of a class definition. Holds the declared members
// (fields, constructor, methods) and the parent class (if any) so that
// method lookup can walk the inheritance chain. Static members are kept in
// their own static-scope dictionary rather than copied onto each instance.
public sealed class TinyLanguageClass
{
    public readonly string Name;
    public readonly TinyLanguageClass Parent; // null when there is no extends clause
    public readonly List<FieldDeclareNode> Fields;
    public readonly Dictionary<string, MethodDefNode> InstanceMethods;
    public readonly Dictionary<string, MethodDefNode> StaticMethods;
    public readonly Dictionary<string, object> StaticFields;
    public readonly ConstructorDefNode Constructor; // may be null
    public readonly bool IsStatic;

    public TinyLanguageClass(string name, TinyLanguageClass parent, List<FieldDeclareNode> fields,
        Dictionary<string, MethodDefNode> instanceMethods,
        Dictionary<string, MethodDefNode> staticMethods,
        Dictionary<string, object> staticFields,
        ConstructorDefNode constructor,
        bool isStatic)
    {
        Name = name;
        Parent = parent;
        Fields = fields;
        InstanceMethods = instanceMethods;
        StaticMethods = staticMethods;
        StaticFields = staticFields;
        Constructor = constructor;
        IsStatic = isStatic;
    }

    // Walks the inheritance chain looking up a method by name.
    public MethodDefNode FindInstanceMethod(string methodName)
    {
        TinyLanguageClass current = this;
        while (current != null)
        {
            MethodDefNode found;
            if (current.InstanceMethods.TryGetValue(methodName, out found))
            {
                return found;
            }
            current = current.Parent;
        }
        return null;
    }
}
