using System;

namespace TinyLanguage.Interpreter;

// Internal flow-control exception used to unwind to the nearest enclosing
// loop when a "continue" statement executes. Never propagates past a loop.
internal sealed class ContinueSignal : Exception
{
    public static readonly ContinueSignal Instance = new ContinueSignal();
    private ContinueSignal() { }
}
