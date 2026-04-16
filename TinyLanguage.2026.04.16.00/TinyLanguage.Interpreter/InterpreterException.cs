namespace TinyLanguage.Interpreter
{
    // Runtime exception thrown by the interpreter when a semantic error is
    // detected during program execution. Carries the 1-based source line
    // number so callers can produce meaningful diagnostics.
    public sealed class InterpreterException : System.Exception
    {
        // The 1-based source line number where the error occurred.
        public readonly int Line;

        public InterpreterException(string message, int line)
            : base($"Line {line}: {message}")
        {
            Line = line;
        }
    }
}
