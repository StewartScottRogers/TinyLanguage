using System.Collections.Generic;

namespace TinyLanguage.Interpreter;

// Linked-list variable scope chain.
// Each Scope holds its own bindings and delegates lookups to Parent when
// a name is not found locally. Assignment walks up the chain and updates
// the variable in the scope where it was originally declared.
// Declaration (Define) always creates the binding in the current scope,
// allowing inner scopes to shadow outer ones.
public sealed class Scope
{
    public readonly Scope Parent; // null for the global scope
    public readonly Dictionary<string, object> Bindings;
    public readonly HashSet<string> Constants; // names declared with const — assignment forbidden

    public Scope()
    {
        Parent = null;
        Bindings = new Dictionary<string, object>();
        Constants = new HashSet<string>();
    }

    public Scope(Scope parent)
    {
        Parent = parent;
        Bindings = new Dictionary<string, object>();
        Constants = new HashSet<string>();
    }

    // Declares a new binding in the current scope. Overwrites any binding
    // of the same name already present in this scope (shadowing is handled
    // at outer scope level; identical-scope redeclaration just replaces).
    public void Define(string name, object value)
    {
        Bindings[name] = value;
    }

    // Declares an immutable binding in the current scope.
    public void DefineConst(string name, object value)
    {
        Bindings[name] = value;
        Constants.Add(name);
    }

    // Returns true when the name is bound in this scope only (does not walk up).
    public bool IsDefined(string name)
    {
        return Bindings.ContainsKey(name);
    }

    // Returns true when the name is bound anywhere in the chain.
    public bool IsDefinedAnywhere(string name)
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

    // Walks the chain looking up a name; throws when not found.
    public object Lookup(string name, int line)
    {
        Scope current = this;
        while (current != null)
        {
            object existingValue;
            if (current.Bindings.TryGetValue(name, out existingValue))
            {
                return existingValue;
            }
            current = current.Parent;
        }
        throw new InterpreterException("Undefined variable '" + name + "'", line);
    }

    // Walks the chain and updates the existing binding in the scope where it
    // was originally declared. Throws when the name is not found.
    public void Assign(string name, object value, int line)
    {
        Scope current = this;
        while (current != null)
        {
            if (current.Bindings.ContainsKey(name))
            {
                if (current.Constants.Contains(name))
                {
                    throw new InterpreterException("Cannot assign to constant '" + name + "'", line);
                }
                current.Bindings[name] = value;
                return;
            }
            current = current.Parent;
        }
        throw new InterpreterException("Undefined variable '" + name + "'", line);
    }

    // Creates a child scope whose parent is the current scope.
    public Scope CreateChild()
    {
        return new Scope(this);
    }

    // Walks to the root (global) scope.
    public Scope Global()
    {
        Scope current = this;
        while (current.Parent != null)
        {
            current = current.Parent;
        }
        return current;
    }
}
