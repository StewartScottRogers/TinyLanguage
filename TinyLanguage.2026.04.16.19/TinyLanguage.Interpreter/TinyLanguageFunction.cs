using System.Collections.Generic;
using TinyLanguage.Lexer;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Interpreter;

// Runtime representation of a user-defined function or lambda.
// Holds the parameters, the body, an optional single-expression body flag
// for lambdas (when IsBlockBody is false the body list holds exactly one
// expression node), and an optional closure scope. Closure captures are
// limited per the language spec — regular functions bind only to the global
// scope; lambdas may additionally retain their defining scope when desired.
public sealed class TinyLanguageFunction
{
    public readonly string Name; // may be empty for lambdas
    public readonly List<ParameterNode> Parameters;
    public readonly List<AstNode> Body;
    public readonly bool IsBlockBody; // true for normal functions and block-body lambdas
    public readonly Scope Closure; // null for ordinary functions; captured scope for lambdas

    public TinyLanguageFunction(string name, List<ParameterNode> parameters, List<AstNode> body, bool isBlockBody, Scope closure)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
        IsBlockBody = isBlockBody;
        Closure = closure;
    }
}
