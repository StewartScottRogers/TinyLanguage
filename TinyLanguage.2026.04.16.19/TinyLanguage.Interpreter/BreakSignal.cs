using System;

namespace TinyLanguage.Interpreter;

// Internal flow-control exception used to unwind to the nearest enclosing
// loop when a "break" statement executes. Never propagates past a loop.
internal sealed class BreakSignal : Exception
{
    public static readonly BreakSignal Instance = new BreakSignal();
    private BreakSignal() { }
}
