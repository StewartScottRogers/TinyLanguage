using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Enumerates the kind of type represented by a TypeNode.
public enum TypeKind
{
    // Primitive / built-in type keywords
    Int,
    Float,
    String,
    Bool,
    Array,
    Object,
    Null,
    Void,

    // User-defined identifier type
    Named,

    // Composite forms
    MapType,        // map<K, V>
    GenericType,    // Id<T, ...>
    ArraySuffix,    // T[]
    NullableSuffix  // T?
}

// Represents a parsed type expression from the grammar:
//   <type> ::= <base_type> { <type_suffix> }
//
// For primitive types (Int, Float, etc.) only Kind and Name are set.
// For Named types, Name holds the identifier.
// For MapType, MapKeyType and MapValueType hold the key and value types.
// For GenericType, Name holds the base identifier and TypeArguments holds the arguments.
// For ArraySuffix and NullableSuffix, InnerType holds the wrapped type.
public sealed class TypeNode : AstNode
{
    // The kind discriminator — determines which other fields are meaningful.
    public readonly TypeKind Kind;

    // For Named and GenericType: the type identifier string.
    // For primitive types: a human-readable name (e.g. "int").
    public readonly string Name;

    // For MapType: key type
    public readonly TypeNode MapKeyType;   // null except for MapType

    // For MapType: value type
    public readonly TypeNode MapValueType; // null except for MapType

    // For GenericType: the type arguments list
    public readonly List<TypeNode> TypeArguments; // null except for GenericType

    // For ArraySuffix and NullableSuffix: the inner wrapped type
    public readonly TypeNode InnerType;    // null except for ArraySuffix / NullableSuffix

    // Constructor for simple named / primitive types
    public TypeNode(TypeKind kind, string name, int line) : base(line)
    {
        Kind = kind;
        Name = name;
        TypeArguments = new List<TypeNode>();
    }

    // Constructor for map<K, V>
    public TypeNode(TypeNode mapKeyType, TypeNode mapValueType, int line) : base(line)
    {
        Kind = TypeKind.MapType;
        Name = "map";
        MapKeyType = mapKeyType;
        MapValueType = mapValueType;
        TypeArguments = new List<TypeNode>();
    }

    // Constructor for generic types Id<T, ...>
    public TypeNode(string name, List<TypeNode> typeArguments, int line) : base(line)
    {
        Kind = TypeKind.GenericType;
        Name = name;
        TypeArguments = typeArguments;
    }

    // Constructor for array and nullable suffix types
    public TypeNode(TypeKind kind, TypeNode innerType, int line) : base(line)
    {
        Kind = kind;
        Name = string.Empty;
        InnerType = innerType;
        TypeArguments = new List<TypeNode>();
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}
