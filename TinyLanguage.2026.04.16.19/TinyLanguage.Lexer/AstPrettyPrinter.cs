using System;
using System.Collections.Generic;
using System.IO;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Lexer;

// Implements INodeVisitor to produce a human-readable, indented representation
// of the Abstract Syntax Tree. Each node type prints its name and key fields,
// then visits its children indented by two additional spaces.
//
// Usage:
//   AstPrettyPrinter printer = new AstPrettyPrinter(Console.Out);
//   programNode.Accept(printer);
public sealed class AstPrettyPrinter : INodeVisitor
{
    private readonly TextWriter Writer;
    private int IndentLevel;
    private const int IndentWidth = 2;

    public AstPrettyPrinter(TextWriter writer)
    {
        Writer = writer;
        IndentLevel = 0;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void Write(string text)
    {
        Writer.WriteLine(new string(' ', IndentLevel * IndentWidth) + text);
    }

    private void Indent()
    {
        IndentLevel++;
    }

    private void Dedent()
    {
        IndentLevel--;
    }

    private void VisitChildren(List<AstNode> children)
    {
        Indent();
        foreach (AstNode child in children)
        {
            child.Accept(this);
        }
        Dedent();
    }

    private void VisitChild(AstNode child)
    {
        if (child == null)
        {
            return;
        }
        Indent();
        child.Accept(this);
        Dedent();
    }

    // ── Program structure ─────────────────────────────────────────────────────

    public void Visit(ProgramNode node)
    {
        Write("ProgramNode");
        VisitChildren(node.Statements);
    }

    // ── Declarations ──────────────────────────────────────────────────────────

    public void Visit(AssignStatementNode node)
    {
        Write("AssignStatementNode Name=" + node.Name);
        VisitChild(node.Value);
    }

    public void Visit(LetDeclareNode node)
    {
        string typeInfo = node.TypeAnnotation != null ? " Type=<annotated>" : "";
        Write("LetDeclareNode Name=" + node.Name + typeInfo);
        VisitChild(node.Value);
    }

    public void Visit(VarDeclareNode node)
    {
        Write("VarDeclareNode Name=" + node.Name);
        VisitChild(node.Value);
    }

    public void Visit(ConstDeclareNode node)
    {
        string typeInfo = node.TypeAnnotation != null ? " Type=<annotated>" : "";
        Write("ConstDeclareNode Name=" + node.Name + typeInfo);
        VisitChild(node.Value);
    }

    public void Visit(EnumDefNode node)
    {
        Write("EnumDefNode Name=" + node.Name + " Members=" + node.Members.Count);
        Indent();
        foreach (EnumMember member in node.Members)
        {
            if (member.Value != null)
            {
                Write("EnumMember Name=" + member.Name + " HasValue=true");
                VisitChild(member.Value);
            }
            else
            {
                Write("EnumMember Name=" + member.Name);
            }
        }
        Dedent();
    }

    public void Visit(FieldDeclareNode node)
    {
        string typeInfo = node.TypeAnnotation != null ? " Type=<annotated>" : "";
        Write("FieldDeclareNode Keyword=" + node.Keyword + " Name=" + node.Name + typeInfo);
        VisitChild(node.Value);
    }

    // ── Control flow ──────────────────────────────────────────────────────────

    public void Visit(IfStatementNode node)
    {
        Write("IfStatementNode HasElse=" + (node.ElseBody.Count > 0));
        Write("  Condition:");
        VisitChild(node.Condition);
        Write("  Then:");
        VisitChildren(node.ThenBody);
        if (node.ElseBody.Count > 0)
        {
            Write("  Else:");
            VisitChildren(node.ElseBody);
        }
    }

    public void Visit(WhileStatementNode node)
    {
        Write("WhileStatementNode");
        Write("  Condition:");
        VisitChild(node.Condition);
        Write("  Body:");
        VisitChildren(node.Body);
    }

    public void Visit(ForStatementNode node)
    {
        string stepInfo = node.StepExpr != null ? " HasStep=true" : "";
        Write("ForStatementNode Variable=" + node.VariableName + stepInfo);
        Write("  Start:");
        VisitChild(node.StartExpr);
        Write("  End:");
        VisitChild(node.EndExpr);
        if (node.StepExpr != null)
        {
            Write("  Step:");
            VisitChild(node.StepExpr);
        }
        Write("  Body:");
        VisitChildren(node.Body);
    }

    public void Visit(ForeachStatementNode node)
    {
        Write("ForeachStatementNode Variable=" + node.VariableName);
        Write("  Collection:");
        VisitChild(node.Collection);
        Write("  Body:");
        VisitChildren(node.Body);
    }

    public void Visit(DoWhileStatementNode node)
    {
        Write("DoWhileStatementNode");
        Write("  Body:");
        VisitChildren(node.Body);
        Write("  Condition:");
        VisitChild(node.Condition);
    }

    public void Visit(DoBlockNode node)
    {
        Write("DoBlockNode");
        VisitChildren(node.Body);
    }

    public void Visit(SwitchStatementNode node)
    {
        Write("SwitchStatementNode Cases=" + node.Cases.Count);
        Write("  Subject:");
        VisitChild(node.Subject);
        Indent();
        foreach (SwitchCaseNode switchCase in node.Cases)
        {
            if (switchCase.IsDefault)
            {
                Write("DefaultClause");
            }
            else
            {
                Write("CaseClause");
                VisitChild(switchCase.CaseExpr);
            }
            VisitChildren(switchCase.Body);
        }
        Dedent();
    }

    public void Visit(BreakStatementNode node)
    {
        Write("BreakStatementNode");
    }

    public void Visit(ContinueStatementNode node)
    {
        Write("ContinueStatementNode");
    }

    // ── I/O ───────────────────────────────────────────────────────────────────

    public void Visit(PrintStatementNode node)
    {
        Write("PrintStatementNode");
        VisitChild(node.Expression);
    }

    public void Visit(InputStatementNode node)
    {
        Write("InputStatementNode Variable=" + node.VariableName);
    }

    // ── Functions ─────────────────────────────────────────────────────────────

    public void Visit(FunctionDefNode node)
    {
        string retInfo = node.ReturnType != null ? " Returns=<annotated>" : "";
        Write("FunctionDefNode Name=" + node.Name + " Params=" + node.Parameters.Count + retInfo);
        Indent();
        foreach (ParameterNode param in node.Parameters)
        {
            param.Accept(this);
        }
        Dedent();
        VisitChildren(node.Body);
    }

    public void Visit(MethodDefNode node)
    {
        string staticInfo = node.IsStatic ? " Static=true" : "";
        string retInfo = node.ReturnType != null ? " Returns=<annotated>" : "";
        Write("MethodDefNode Name=" + node.Name + staticInfo + " Params=" + node.Parameters.Count + retInfo);
        Indent();
        foreach (ParameterNode param in node.Parameters)
        {
            param.Accept(this);
        }
        Dedent();
        VisitChildren(node.Body);
    }

    public void Visit(CallStatementNode node)
    {
        Write("CallStatementNode Path=" + string.Join(".", node.MemberPath) + " Args=" + node.Arguments.Count);
        VisitChildren(node.Arguments);
    }

    public void Visit(ReturnStatementNode node)
    {
        Write("ReturnStatementNode HasValue=" + (node.Value != null));
        VisitChild(node.Value);
    }

    public void Visit(ParameterNode node)
    {
        string typeInfo = node.TypeAnnotation != null ? " Type=<annotated>" : "";
        string defInfo = node.DefaultValue != null ? " HasDefault=true" : "";
        Write("ParameterNode Name=" + node.Name + typeInfo + defInfo);
        VisitChild(node.DefaultValue);
    }

    // ── Classes ───────────────────────────────────────────────────────────────

    public void Visit(ClassDefNode node)
    {
        string staticInfo = node.IsStatic ? " Static=true" : "";
        string extendsInfo = node.ExtendsName != null ? " Extends=" + node.ExtendsName : "";
        string implementsInfo = node.ImplementsNames.Count > 0
            ? " Implements=" + string.Join(",", node.ImplementsNames)
            : "";
        Write("ClassDefNode Name=" + node.Name + staticInfo + extendsInfo + implementsInfo + " Members=" + node.Members.Count);
        VisitChildren(node.Members);
    }

    public void Visit(ConstructorDefNode node)
    {
        Write("ConstructorDefNode Params=" + node.Parameters.Count);
        Indent();
        foreach (ParameterNode param in node.Parameters)
        {
            param.Accept(this);
        }
        Dedent();
        VisitChildren(node.Body);
    }

    // ── Modules ───────────────────────────────────────────────────────────────

    public void Visit(ModuleDefNode node)
    {
        Write("ModuleDefNode Name=" + node.Name + " Imports=" + node.Imports.Count);
        VisitChildren(node.Body);
    }

    public void Visit(ImportStatementNode node)
    {
        string aliasInfo = node.Alias != null ? " As=" + node.Alias : "";
        Write("ImportStatementNode Module=" + node.ModuleName + aliasInfo);
    }

    public void Visit(ExportStatementNode node)
    {
        Write("ExportStatementNode Name=" + node.Name);
    }

    // ── Exceptions ────────────────────────────────────────────────────────────

    public void Visit(TryStatementNode node)
    {
        string typeInfo = node.CatchVariableType != null ? " CatchType=<annotated>" : "";
        string finallyInfo = node.FinallyBody.Count > 0 ? " HasFinally=true" : "";
        Write("TryStatementNode CatchVar=" + node.CatchVariableName + typeInfo + finallyInfo);
        Write("  Try:");
        VisitChildren(node.TryBody);
        Write("  Catch:");
        VisitChildren(node.CatchBody);
        if (node.FinallyBody.Count > 0)
        {
            Write("  Finally:");
            VisitChildren(node.FinallyBody);
        }
    }

    public void Visit(ThrowStatementNode node)
    {
        Write("ThrowStatementNode");
        VisitChild(node.Value);
    }

    // ── Pattern matching ──────────────────────────────────────────────────────

    public void Visit(PatternMatchNode node)
    {
        Write("PatternMatchNode Cases=" + node.Cases.Count);
        Write("  Subject:");
        VisitChild(node.Subject);
        Indent();
        foreach (PatternCaseNode patternCase in node.Cases)
        {
            patternCase.Accept(this);
        }
        Dedent();
    }

    public void Visit(PatternCaseNode node)
    {
        string guardInfo = node.Guard != null ? " HasGuard=true" : "";
        Write("PatternCaseNode" + guardInfo);
        Indent();
        node.Pattern.Accept(this);
        Dedent();
        if (node.Guard != null)
        {
            Write("  Guard:");
            VisitChild(node.Guard);
        }
        Write("  Body:");
        VisitChildren(node.Body);
    }

    public void Visit(PatternNode node)
    {
        switch (node.Kind)
        {
            case PatternKind.Identifier:
                Write("PatternNode Kind=Identifier Name=" + node.Name);
                break;
            case PatternKind.IntegerLiteral:
                Write("PatternNode Kind=Integer Value=" + node.IntegerValue);
                break;
            case PatternKind.FloatLiteral:
                Write("PatternNode Kind=Float Value=" + node.FloatValue);
                break;
            case PatternKind.StringLiteral:
                Write("PatternNode Kind=String Value=\"" + node.StringValue + "\"");
                break;
            case PatternKind.BoolLiteral:
                Write("PatternNode Kind=Bool Value=" + node.BoolValue);
                break;
            case PatternKind.NullLiteral:
                Write("PatternNode Kind=Null");
                break;
            case PatternKind.Wildcard:
                Write("PatternNode Kind=Wildcard");
                break;
            case PatternKind.Constructor:
                Write("PatternNode Kind=Constructor Name=" + node.Name + " SubPatterns=" + node.SubPatterns.Count);
                Indent();
                foreach (PatternNode sub in node.SubPatterns)
                {
                    sub.Accept(this);
                }
                Dedent();
                break;
            case PatternKind.ArrayPattern:
                Write("PatternNode Kind=Array SubPatterns=" + node.SubPatterns.Count);
                Indent();
                foreach (PatternNode sub in node.SubPatterns)
                {
                    sub.Accept(this);
                }
                Dedent();
                break;
            case PatternKind.FieldPattern:
                Write("PatternNode Kind=FieldPattern Name=" + node.Name + " Fields=" + node.FieldPatterns.Count);
                Indent();
                foreach (FieldPatternEntry entry in node.FieldPatterns)
                {
                    Write("FieldPattern FieldName=" + entry.FieldName);
                    VisitChild(entry.SubPattern);
                }
                Dedent();
                break;
            case PatternKind.Alternation:
                Write("PatternNode Kind=Alternation");
                Indent();
                node.Left.Accept(this);
                node.Right.Accept(this);
                Dedent();
                break;
            default:
                Write("PatternNode Kind=Unknown");
                break;
        }
    }

    // ── Annotations ───────────────────────────────────────────────────────────

    public void Visit(AnnotatedStatementNode node)
    {
        Write("AnnotatedStatementNode");
        Indent();
        node.Annotation.Accept(this);
        node.Statement.Accept(this);
        Dedent();
    }

    public void Visit(AnnotationNode node)
    {
        Write("AnnotationNode Name=" + node.Name + " Params=" + node.Parameters.Count);
        Indent();
        foreach (AnnotationParam param in node.Parameters)
        {
            Write("AnnotationParam Name=" + param.Name);
            VisitChild(param.Value);
        }
        Dedent();
    }

    // ── Array assignment ──────────────────────────────────────────────────────

    public void Visit(ArrayElementAssignNode node)
    {
        Write("ArrayElementAssignNode Array=" + node.ArrayName);
        Write("  Index:");
        VisitChild(node.IndexExpr);
        Write("  Value:");
        VisitChild(node.Value);
    }

    // ── Binary / unary / ternary expressions ─────────────────────────────────

    public void Visit(BinaryOpNode node)
    {
        Write("BinaryOpNode Op=" + node.Operator);
        VisitChild(node.Left);
        VisitChild(node.Right);
    }

    public void Visit(UnaryOpNode node)
    {
        Write("UnaryOpNode Op=" + node.Operator);
        VisitChild(node.Operand);
    }

    public void Visit(TernaryNode node)
    {
        Write("TernaryNode");
        Write("  Condition:");
        VisitChild(node.Condition);
        Write("  Then:");
        VisitChild(node.ThenExpr);
        Write("  Else:");
        VisitChild(node.ElseExpr);
    }

    // ── Literals ─────────────────────────────────────────────────────────────

    public void Visit(IdentifierNode node)
    {
        Write("IdentifierNode Name=" + node.Name);
    }

    public void Visit(IntegerLiteralNode node)
    {
        Write("IntegerLiteralNode Value=" + node.Value);
    }

    public void Visit(FloatLiteralNode node)
    {
        Write("FloatLiteralNode Value=" + node.Value);
    }

    public void Visit(StringLiteralNode node)
    {
        Write("StringLiteralNode Value=\"" + node.Value + "\"");
    }

    public void Visit(BoolLiteralNode node)
    {
        Write("BoolLiteralNode Value=" + node.Value);
    }

    public void Visit(NullLiteralNode node)
    {
        Write("NullLiteralNode");
    }

    public void Visit(ArrayLiteralNode node)
    {
        Write("ArrayLiteralNode Elements=" + node.Elements.Count);
        VisitChildren(node.Elements);
    }

    // ── Postfix expressions ───────────────────────────────────────────────────

    public void Visit(IndexAccessNode node)
    {
        Write("IndexAccessNode");
        Write("  Target:");
        VisitChild(node.Target);
        Write("  Index:");
        VisitChild(node.IndexExpr);
    }

    public void Visit(MemberAccessNode node)
    {
        Write("MemberAccessNode Member=" + node.MemberName);
        VisitChild(node.Target);
    }

    public void Visit(MethodCallNode node)
    {
        Write("MethodCallNode Method=" + node.MethodName + " Args=" + node.Arguments.Count);
        Write("  Target:");
        VisitChild(node.Target);
        if (node.Arguments.Count > 0)
        {
            Write("  Args:");
            VisitChildren(node.Arguments);
        }
    }

    public void Visit(FunctionCallNode node)
    {
        Write("FunctionCallNode Name=" + node.Name + " Args=" + node.Arguments.Count);
        VisitChildren(node.Arguments);
    }

    // ── Object / type expressions ─────────────────────────────────────────────

    public void Visit(NewExprNode node)
    {
        Write("NewExprNode Class=" + node.ClassName + " Args=" + node.Arguments.Count);
        VisitChildren(node.Arguments);
    }

    public void Visit(CastExprNode node)
    {
        Write("CastExprNode");
        Write("  Type:");
        VisitChild(node.TargetType);
        Write("  Operand:");
        VisitChild(node.Operand);
    }

    public void Visit(TypeCheckNode node)
    {
        Write("TypeCheckNode");
        Write("  Operand:");
        VisitChild(node.Operand);
        Write("  Type:");
        VisitChild(node.CheckType);
    }

    public void Visit(TypeAssertNode node)
    {
        Write("TypeAssertNode");
        Write("  Operand:");
        VisitChild(node.Operand);
        Write("  Type:");
        VisitChild(node.AssertType);
    }

    // ── Special expressions ───────────────────────────────────────────────────

    public void Visit(ConditionalExprNode node)
    {
        Write("ConditionalExprNode");
        Write("  Condition:");
        VisitChild(node.Condition);
        Write("  Then:");
        VisitChild(node.ThenExpr);
        Write("  Else:");
        VisitChild(node.ElseExpr);
    }

    public void Visit(LambdaExprNode node)
    {
        string bodyKind = node.IsBlockBody ? "Block" : "Expression";
        string retInfo = node.ReturnType != null ? " Returns=<annotated>" : "";
        Write("LambdaExprNode BodyKind=" + bodyKind + " Params=" + node.Parameters.Count + retInfo);
        Indent();
        foreach (ParameterNode param in node.Parameters)
        {
            param.Accept(this);
        }
        Dedent();
        VisitChildren(node.Body);
    }

    public void Visit(ListComprehensionNode node)
    {
        Write("ListComprehensionNode Variable=" + node.VariableName);
        Write("  Projection:");
        VisitChild(node.ProjectionExpr);
        Write("  Source:");
        VisitChild(node.SourceExpr);
    }

    // ── Types ─────────────────────────────────────────────────────────────────

    public void Visit(TypeNode node)
    {
        switch (node.Kind)
        {
            case TypeKind.MapType:
                Write("TypeNode Kind=Map");
                Indent();
                node.MapKeyType.Accept(this);
                node.MapValueType.Accept(this);
                Dedent();
                break;
            case TypeKind.GenericType:
                Write("TypeNode Kind=Generic Name=" + node.Name + " Args=" + node.TypeArguments.Count);
                Indent();
                foreach (TypeNode arg in node.TypeArguments)
                {
                    arg.Accept(this);
                }
                Dedent();
                break;
            case TypeKind.ArraySuffix:
                Write("TypeNode Kind=ArraySuffix");
                VisitChild(node.InnerType);
                break;
            case TypeKind.NullableSuffix:
                Write("TypeNode Kind=NullableSuffix");
                VisitChild(node.InnerType);
                break;
            default:
                Write("TypeNode Kind=" + node.Kind + " Name=" + node.Name);
                break;
        }
    }
}
