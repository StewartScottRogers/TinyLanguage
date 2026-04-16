namespace TinyLanguage.Interpreter
{
    // Control-flow exception used to skip the remainder of the current loop
    // iteration when a "continue" statement is executed. Caught by loop handlers.
    internal sealed class ContinueException : System.Exception
    {
        public ContinueException() : base()
        {
        }
    }
}
