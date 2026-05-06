using System.Collections.Generic;

namespace TinyLanguage.Lexer.Nodes
{
    // Represents a type annotation in the source, e.g. int, string[], int?
    // map<string, int>, or a user-defined generic type like List<T>.
    //
    // Suffixes are stored as a list of TypeSuffix enum values applied left to
    // right after the base type, so "int[]?" is BaseType="int" with suffixes
    // [ArraySuffix, NullableSuffix].
    public class TypeNode : AstNode
    {
        // The core type name: a primitive keyword or a user-defined identifier.
        public string BaseType { get; }

        // Optional key and value types when BaseType == "map".
        // Null when not a map type.
        public TypeNode MapKeyType { get; }
        public TypeNode MapValueType { get; }

        // Zero or more suffix modifiers ([] or ?) applied after the base type.
        public IReadOnlyList<TypeSuffix> Suffixes { get; }

        // Generic type arguments, e.g. the T in List<T>. Empty when none.
        public IReadOnlyList<TypeNode> GenericArgs { get; }

        public TypeNode(
            int line,
            string baseType,
            IReadOnlyList<TypeSuffix> suffixes,
            IReadOnlyList<TypeNode> genericArgs,
            TypeNode mapKeyType,
            TypeNode mapValueType)
            : base(line)
        {
            BaseType = baseType;
            Suffixes = suffixes;
            GenericArgs = genericArgs;
            MapKeyType = mapKeyType;
            MapValueType = mapValueType;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitTypeNode(this);
        }
    }

    // The two suffix forms that can follow a base type.
    public enum TypeSuffix
    {
        // "[]" — turns the type into an array type.
        ArraySuffix,

        // "?" — marks the type as nullable.
        NullableSuffix
    }
}
