namespace TinyLanguage.Interpreter
{
    // Public contract for running a parsed TinyLanguage program.
    // Accepts the root ProgramNode and returns the final result value
    // (or null when the program produces no explicit value).
    public interface IInterpreter
    {
        object Execute(TinyLanguage.Lexer.ProgramNode program);
    }
}
