using System.Collections.Generic;

namespace TinyLanguage.Interpreter
{
    // Linked-list scope chain for variable resolution.
    // Each scope holds its own bindings and a reference to an optional parent scope.
    // Lookup walks upward through the chain; assignment targets the declaring scope.
    public sealed class Scope
    {
        // The parent scope — null for the global (root) scope.
        private readonly Scope Parent;

        // Bindings declared in this scope.
        private readonly Dictionary<string, object> Bindings;

        // Names that were declared with "const" and must not be reassigned.
        private readonly HashSet<string> Constants;

        public Scope(Scope parent)
        {
            Parent = parent;
            Bindings = new Dictionary<string, object>();
            Constants = new HashSet<string>();
        }

        // Define a new binding in the current scope.
        public void Define(string name, object value)
        {
            Bindings[name] = value;
        }

        // Define a constant binding in the current scope.
        public void DefineConstant(string name, object value)
        {
            Bindings[name] = value;
            Constants.Add(name);
        }

        // Assign to the nearest enclosing scope that contains the name.
        // Throws InterpreterException if the name is not found anywhere in the chain.
        public void Assign(string name, object value, int line)
        {
            Scope current = this;
            while (current != null)
            {
                if (current.Bindings.ContainsKey(name))
                {
                    if (current.Constants.Contains(name))
                    {
                        throw new InterpreterException($"Cannot reassign constant '{name}'", line);
                    }
                    current.Bindings[name] = value;
                    return;
                }
                current = current.Parent;
            }
            throw new InterpreterException($"Undefined variable '{name}'", line);
        }

        // Walk the scope chain upward looking for the name.
        // Throws InterpreterException if the name is not found.
        public object Lookup(string name, int line)
        {
            Scope current = this;
            while (current != null)
            {
                if (current.Bindings.TryGetValue(name, out object value))
                {
                    return value;
                }
                current = current.Parent;
            }
            throw new InterpreterException($"Undefined variable '{name}'", line);
        }

        // Check whether a name is defined in this scope or any ancestor.
        public bool IsDefined(string name)
        {
            Scope current = this;
            while (current != null)
            {
                if (current.Bindings.ContainsKey(name))
                {
                    return true;
                }
                current = current.Parent;
            }
            return false;
        }

        // Create a child scope whose parent is this scope.
        public Scope CreateChild()
        {
            return new Scope(this);
        }
    }
}
