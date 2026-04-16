namespace TinyLanguage.Interpreter
{
    // Control-flow exception used when TinyLanguage code executes a "throw"
    // statement. Carries the thrown value and source line so that a "catch"
    // clause in the same program can handle it.
    internal sealed class ThrownException : System.Exception
    {
        // The value that was thrown by the TinyLanguage program.
        public readonly object ThrownValue;

        // The source line where the throw occurred.
        public readonly int Line;

        public ThrownException(object thrownValue, int line)
            : base(thrownValue != null ? thrownValue.ToString() : "null")
        {
            ThrownValue = thrownValue;
            Line = line;
        }
    }
}
