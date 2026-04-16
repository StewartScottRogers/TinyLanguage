// stub
namespace TinyLanguage.Lexer
{
    // Visitor interface — implemented by the interpreter, pretty-printer, and any
    // other pass that needs to walk the AST without modifying node classes.
    // Concrete implementations dispatch on the runtime type of the node.
    public interface INodeVisitor
    {
        void Visit(AstNode node);
    }
}
