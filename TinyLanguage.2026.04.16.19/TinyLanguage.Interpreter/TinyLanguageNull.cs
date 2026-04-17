namespace TinyLanguage.Interpreter;

// Singleton sentinel for the TinyLanguage null value.
// Using a dedicated type (rather than C# null) lets us distinguish
// "no value was set" from "the value is the language-level null" and
// keeps all runtime values as non-null C# references.
public sealed class TinyLanguageNull
{
    public static readonly TinyLanguageNull Instance = new TinyLanguageNull();

    private TinyLanguageNull()
    {
    }

    public override string ToString()
    {
        return "null";
    }
}
