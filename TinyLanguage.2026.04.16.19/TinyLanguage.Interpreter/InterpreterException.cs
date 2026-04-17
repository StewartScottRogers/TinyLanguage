using System;

namespace TinyLanguage.Interpreter;

// Runtime exception raised by the tree-walking interpreter.
// Always includes the source line number so errors report exactly where
// in the input program the fault occurred.
public sealed class InterpreterException : Exception
{
    public readonly int Line;

    public InterpreterException(string message, int line) : base("Line " + line + ": " + message)
    {
        Line = line;
    }
}
