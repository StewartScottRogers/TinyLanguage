namespace TinyLanguage.Interpreter
{
    // Control-flow exception used to unwind the call stack when a "return"
    // statement is executed inside a function body. This is not a runtime
    // error — it is caught by the function-call handler.
    internal sealed class ReturnException : System.Exception
    {
        // The value being returned — null for a bare "return".
        public readonly object Value;

        public ReturnException(object value) : base()
        {
            Value = value;
        }
    }
}
