using System.Collections.Generic;

namespace TinyLanguage.Interpreter;

// Runtime representation of a class instance. Holds a reference to its
// class definition and a Dictionary of field values. Field access reads
// and writes this dictionary directly.
public sealed class TinyLanguageObject
{
    public readonly TinyLanguageClass Class;
    public readonly Dictionary<string, object> Fields;

    public TinyLanguageObject(TinyLanguageClass classDefinition)
    {
        Class = classDefinition;
        Fields = new Dictionary<string, object>();
    }
}
