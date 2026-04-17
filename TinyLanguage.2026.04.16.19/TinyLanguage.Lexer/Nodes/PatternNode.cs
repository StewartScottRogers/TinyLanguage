using System.Collections.Generic;
using TinyLanguage.Lexer;

namespace TinyLanguage.Lexer.Nodes;

// Enumerates every distinct pattern kind in the grammar.
public enum PatternKind
{
    Identifier,         // id — binds the matched value to a name
    IntegerLiteral,     // 42
    FloatLiteral,       // 3.14
    StringLiteral,      // "hello"
    BoolLiteral,        // true / false
    NullLiteral,        // null
    Wildcard,           // _ — matches anything without binding
    Constructor,        // id ( pattern_list? ) — destructuring constructor
    ArrayPattern,       // [ pattern_list? ] — destructuring array
    FieldPattern,       // id { field: pattern, ... } — field destructuring
    Alternation         // pattern | pattern — left-associative
}

// Represents a single pattern inside a match arm.
// The active fields depend on PatternKind:
//   Identifier       — Name
//   IntegerLiteral   — IntegerValue
//   FloatLiteral     — FloatValue
//   StringLiteral    — StringValue
//   BoolLiteral      — BoolValue
//   NullLiteral      — (no extra fields)
//   Wildcard         — (no extra fields)
//   Constructor      — Name, SubPatterns
//   ArrayPattern     — SubPatterns
//   FieldPattern     — Name, FieldPatterns
//   Alternation      — Left, Right
public sealed class PatternNode : AstNode
{
    public readonly PatternKind Kind;

    // Used by Identifier, Constructor, FieldPattern
    public readonly string Name;

    // Literal values
    public readonly long IntegerValue;
    public readonly double FloatValue;
    public readonly string StringValue;
    public readonly bool BoolValue;

    // Used by Constructor and ArrayPattern
    public readonly List<PatternNode> SubPatterns;

    // Used by FieldPattern: maps field name -> sub-pattern
    public readonly List<FieldPatternEntry> FieldPatterns;

    // Used by Alternation
    public readonly PatternNode Left;
    public readonly PatternNode Right;

    // Constructor for simple patterns: Wildcard, NullLiteral
    public PatternNode(PatternKind kind, int line) : base(line)
    {
        Kind = kind;
        Name = string.Empty;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for Identifier
    public PatternNode(PatternKind kind, string name, int line) : base(line)
    {
        Kind = kind;
        Name = name;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for IntegerLiteral
    public PatternNode(long integerValue, int line) : base(line)
    {
        Kind = PatternKind.IntegerLiteral;
        IntegerValue = integerValue;
        Name = string.Empty;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for FloatLiteral
    public PatternNode(double floatValue, int line) : base(line)
    {
        Kind = PatternKind.FloatLiteral;
        FloatValue = floatValue;
        Name = string.Empty;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for StringLiteral
    public PatternNode(string stringValue, bool isString, int line) : base(line)
    {
        Kind = PatternKind.StringLiteral;
        StringValue = stringValue;
        Name = string.Empty;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for BoolLiteral
    public PatternNode(bool boolValue, int line) : base(line)
    {
        Kind = PatternKind.BoolLiteral;
        BoolValue = boolValue;
        Name = string.Empty;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for Constructor and ArrayPattern patterns
    public PatternNode(PatternKind kind, string name, List<PatternNode> subPatterns, int line) : base(line)
    {
        Kind = kind;
        Name = name;
        SubPatterns = subPatterns;
        FieldPatterns = new List<FieldPatternEntry>();
    }

    // Constructor for FieldPattern
    public PatternNode(string name, List<FieldPatternEntry> fieldPatterns, int line) : base(line)
    {
        Kind = PatternKind.FieldPattern;
        Name = name;
        FieldPatterns = fieldPatterns;
        SubPatterns = new List<PatternNode>();
    }

    // Constructor for Alternation
    public PatternNode(PatternNode left, PatternNode right, int line) : base(line)
    {
        Kind = PatternKind.Alternation;
        Left = left;
        Right = right;
        Name = string.Empty;
        SubPatterns = new List<PatternNode>();
        FieldPatterns = new List<FieldPatternEntry>();
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// Represents one entry in a field pattern: fieldName: subPattern
public sealed class FieldPatternEntry
{
    public readonly string FieldName;
    public readonly PatternNode SubPattern;

    public FieldPatternEntry(string fieldName, PatternNode subPattern)
    {
        FieldName = fieldName;
        SubPattern = subPattern;
    }
}
