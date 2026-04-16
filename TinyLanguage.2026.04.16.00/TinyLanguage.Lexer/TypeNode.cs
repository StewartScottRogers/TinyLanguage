using System.Collections.Generic;

namespace TinyLanguage.Lexer
{
    // Represents a type expression in the AST.
    // Handles primitive types, user-defined types, generic types (e.g. map<int,string>),
    // array type suffixes (e.g. int[]), and the nullable suffix (e.g. int?).
    public class TypeNode : AstNode
    {
        // The base type name (e.g. "int", "string", "MyClass", "map").
        public readonly string TypeName;

        // Generic type arguments — empty list when not a generic type.
        public readonly List<TypeNode> TypeArguments;

        // Number of array dimensions applied as trailing [] suffixes.
        // Zero means not an array type; 2 means int[][] etc.
        public readonly int ArrayDimensions;

        // True when the type has a trailing nullable suffix "?".
        public readonly bool IsNullable;

        public TypeNode(string typeName, List<TypeNode> typeArguments, int arrayDimensions, bool isNullable, int line) : base(line)
        {
            TypeName = typeName;
            TypeArguments = typeArguments;
            ArrayDimensions = arrayDimensions;
            IsNullable = isNullable;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
