using TinyLanguage.Lexer;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Interpreter;

// Public interface of the tree-walking interpreter. Execute runs a whole
// parsed program; Evaluate reduces a single expression node to a value.
public interface IInterpreter
{
    void Execute(ProgramNode program);
    object Evaluate(AstNode expression);
}
