using System;

namespace TinyLanguage.Interpreter;

// Internal flow-control exception used to unwind a function call when a
// "return" statement executes. Carries the returned value.
internal sealed class ReturnSignal : Exception
{
    public readonly object Value;

    public ReturnSignal(object value)
    {
        Value = value;
    }
}
