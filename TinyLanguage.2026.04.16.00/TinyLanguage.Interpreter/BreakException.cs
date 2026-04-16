namespace TinyLanguage.Interpreter
{
    // Control-flow exception used to exit the innermost enclosing loop
    // when a "break" statement is executed. Caught by loop handlers.
    internal sealed class BreakException : System.Exception
    {
        public BreakException() : base()
        {
        }
    }
}
