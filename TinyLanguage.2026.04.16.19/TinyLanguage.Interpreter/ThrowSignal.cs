using System;

namespace TinyLanguage.Interpreter;

// Internal flow-control exception carrying a user-thrown value.
// Distinct from InterpreterException so that try/catch in TinyLanguage
// code can catch them without also catching interpreter-internal errors.
internal sealed class ThrowSignal : Exception
{
    public readonly object Value;
    public readonly int ThrowLine;

    public ThrowSignal(object value, int throwLine)
    {
        Value = value;
        ThrowLine = throwLine;
    }
}
