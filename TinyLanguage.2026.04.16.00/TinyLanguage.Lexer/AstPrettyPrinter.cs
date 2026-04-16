using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLanguage.Lexer
{
    // Walks the AST using the visitor pattern and produces a human-readable
    // indented tree representation. Each level of nesting is indented by two spaces.
    // Useful for debugging and for the Layer 3 pretty-printer tests.
    public class AstPrettyPrinter : INodeVisitor
    {
        private readonly StringBuilder Builder;
        private int IndentLevel;

        public AstPrettyPrinter()
        {
            Builder = new StringBuilder();
            IndentLevel = 0;
        }

        // Entry point — visits the given node and returns the formatted string.
        public string Print(AstNode node)
        {
            Builder.Clear();
            IndentLevel = 0;
            node.Accept(this);
            return Builder.ToString();
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private void WriteLine(string text)
        {
            Builder.Append(new string(' ', IndentLevel * 2));
            Builder.AppendLine(text);
        }

        private void Indent()
        {
            IndentLevel++;
        }

        private void Dedent()
        {
            IndentLevel--;
        }

        private void VisitList(List<AstNode> nodes)
        {
            foreach (AstNode node in nodes)
            {
                node.Accept(this);
            }
        }

        // ----------------------------------------------------------------
        // Program root
        // ----------------------------------------------------------------

        public void Visit(ProgramNode node)
        {
            WriteLine("ProgramNode");
            Indent();
            VisitList(node.Statements);
            Dedent();
        }

        // ----------------------------------------------------------------
        // Statement nodes
        // ----------------------------------------------------------------

        public void Visit(AssignStatementNode node)
        {
            WriteLine($"AssignStatementNode Variable={node.VariableName}");
            Indent();
            node.ValueExpression.Accept(this);
            Dedent();
        }

        public void Visit(LetDeclareNode node)
        {
            string typeInfo = node.DeclaredType != null ? $" Type={node.DeclaredType.TypeName}" : string.Empty;
            WriteLine($"LetDeclareNode Variable={node.VariableName}{typeInfo}");
            Indent();
            node.InitialiserExpression.Accept(this);
            Dedent();
        }

        public void Visit(VarDeclareNode node)
        {
            string typeInfo = node.DeclaredType != null ? $" Type={node.DeclaredType.TypeName}" : string.Empty;
            WriteLine($"VarDeclareNode Variable={node.VariableName}{typeInfo}");
            Indent();
            node.InitialiserExpression.Accept(this);
            Dedent();
        }

        public void Visit(ConstDeclareNode node)
        {
            string typeInfo = node.DeclaredType != null ? $" Type={node.DeclaredType.TypeName}" : string.Empty;
            WriteLine($"ConstDeclareNode Constant={node.ConstantName}{typeInfo}");
            Indent();
            node.InitialiserExpression.Accept(this);
            Dedent();
        }

        public void Visit(EnumDefNode node)
        {
            WriteLine($"EnumDefNode Name={node.EnumName}");
            Indent();
            foreach (EnumValueNode member in node.Members)
            {
                member.Accept(this);
            }
            Dedent();
        }

        public void Visit(EnumValueNode node)
        {
            WriteLine($"EnumValueNode Member={node.MemberName}");
            if (node.ValueExpression != null)
            {
                Indent();
                node.ValueExpression.Accept(this);
                Dedent();
            }
        }

        public void Visit(IfStatementNode node)
        {
            WriteLine("IfStatementNode");
            Indent();
            WriteLine("Condition:");
            Indent();
            node.Condition.Accept(this);
            Dedent();
            WriteLine("ThenBody:");
            Indent();
            VisitList(node.ThenBody);
            Dedent();
            if (node.ElseBody != null)
            {
                WriteLine("ElseBody:");
                Indent();
                VisitList(node.ElseBody);
                Dedent();
            }
            Dedent();
        }

        public void Visit(WhileStatementNode node)
        {
            WriteLine("WhileStatementNode");
            Indent();
            WriteLine("Condition:");
            Indent();
            node.Condition.Accept(this);
            Dedent();
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(ForStatementNode node)
        {
            WriteLine($"ForStatementNode LoopVariable={node.LoopVariable}");
            Indent();
            WriteLine("Start:");
            Indent();
            node.StartExpression.Accept(this);
            Dedent();
            WriteLine("End:");
            Indent();
            node.EndExpression.Accept(this);
            Dedent();
            if (node.StepExpression != null)
            {
                WriteLine("Step:");
                Indent();
                node.StepExpression.Accept(this);
                Dedent();
            }
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(ForeachStatementNode node)
        {
            WriteLine($"ForeachStatementNode ElementVariable={node.ElementVariable}");
            Indent();
            WriteLine("Iterable:");
            Indent();
            node.IterableExpression.Accept(this);
            Dedent();
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(DoWhileStatementNode node)
        {
            WriteLine("DoWhileStatementNode");
            Indent();
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            WriteLine("Condition:");
            Indent();
            node.Condition.Accept(this);
            Dedent();
            Dedent();
        }

        public void Visit(SwitchStatementNode node)
        {
            WriteLine("SwitchStatementNode");
            Indent();
            WriteLine("Subject:");
            Indent();
            node.SubjectExpression.Accept(this);
            Dedent();
            foreach (SwitchCaseNode switchCase in node.Cases)
            {
                switchCase.Accept(this);
            }
            Dedent();
        }

        public void Visit(SwitchCaseNode node)
        {
            string label = node.IsDefault ? "default" : "case";
            WriteLine($"SwitchCaseNode [{label}]");
            Indent();
            if (!node.IsDefault && node.ValueExpression != null)
            {
                WriteLine("Value:");
                Indent();
                node.ValueExpression.Accept(this);
                Dedent();
            }
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(BreakStatementNode node)
        {
            WriteLine("BreakStatementNode");
        }

        public void Visit(ContinueStatementNode node)
        {
            WriteLine("ContinueStatementNode");
        }

        public void Visit(PrintStatementNode node)
        {
            WriteLine("PrintStatementNode");
            Indent();
            node.Expression.Accept(this);
            Dedent();
        }

        public void Visit(InputStatementNode node)
        {
            WriteLine($"InputStatementNode Variable={node.VariableName}");
        }

        public void Visit(FunctionDefNode node)
        {
            string staticMark = node.IsStatic ? " [static]" : string.Empty;
            string returnInfo = node.ReturnType != null ? $" -> {node.ReturnType.TypeName}" : string.Empty;
            WriteLine($"FunctionDefNode Name={node.FunctionName}{staticMark}{returnInfo}");
            Indent();
            foreach (ParameterNode param in node.Parameters)
            {
                param.Accept(this);
            }
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(ReturnStatementNode node)
        {
            WriteLine("ReturnStatementNode");
            if (node.ReturnExpression != null)
            {
                Indent();
                node.ReturnExpression.Accept(this);
                Dedent();
            }
        }

        public void Visit(CallStatementNode node)
        {
            string chain = string.Join(".", node.ReceiverChain);
            WriteLine($"CallStatementNode Target={chain}");
            if (node.Arguments.Count > 0)
            {
                Indent();
                VisitList(node.Arguments);
                Dedent();
            }
        }

        public void Visit(ClassDefNode node)
        {
            string staticMark = node.IsStatic ? " [static]" : string.Empty;
            string baseInfo = node.BaseClassName != null ? $" extends {node.BaseClassName}" : string.Empty;
            string implInfo = node.ImplementedInterfaces.Count > 0
                ? $" implements {string.Join(", ", node.ImplementedInterfaces)}"
                : string.Empty;
            WriteLine($"ClassDefNode Name={node.ClassName}{staticMark}{baseInfo}{implInfo}");
            Indent();
            VisitList(node.Members);
            Dedent();
        }

        public void Visit(ConstructorDefNode node)
        {
            WriteLine("ConstructorDefNode");
            Indent();
            foreach (ParameterNode param in node.Parameters)
            {
                param.Accept(this);
            }
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(FieldDeclareNode node)
        {
            string typeInfo = node.DeclaredType != null ? $" Type={node.DeclaredType.TypeName}" : string.Empty;
            WriteLine($"FieldDeclareNode [{node.Keyword}] Name={node.FieldName}{typeInfo}");
            Indent();
            node.InitialiserExpression.Accept(this);
            Dedent();
        }

        public void Visit(ModuleDefNode node)
        {
            WriteLine($"ModuleDefNode Name={node.ModuleName}");
            Indent();
            if (node.HeaderImports.Count > 0)
            {
                WriteLine("HeaderImports:");
                Indent();
                foreach (ModuleImportNode importNode in node.HeaderImports)
                {
                    importNode.Accept(this);
                }
                Dedent();
            }
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(ModuleImportNode node)
        {
            string aliasInfo = node.Alias != null ? $" as {node.Alias}" : string.Empty;
            WriteLine($"ModuleImportNode Module={node.ModuleName}{aliasInfo}");
        }

        public void Visit(ImportStatementNode node)
        {
            string aliasInfo = node.Alias != null ? $" as {node.Alias}" : string.Empty;
            WriteLine($"ImportStatementNode Module={node.ModuleName}{aliasInfo}");
        }

        public void Visit(ExportStatementNode node)
        {
            WriteLine($"ExportStatementNode Name={node.ExportedName}");
        }

        public void Visit(TryStatementNode node)
        {
            WriteLine("TryStatementNode");
            Indent();
            WriteLine("TryBody:");
            Indent();
            VisitList(node.TryBody);
            Dedent();
            node.CatchClause.Accept(this);
            if (node.FinallyBody != null)
            {
                WriteLine("FinallyBody:");
                Indent();
                VisitList(node.FinallyBody);
                Dedent();
            }
            Dedent();
        }

        public void Visit(CatchClauseNode node)
        {
            string typeInfo = node.ExceptionType != null ? $" Type={node.ExceptionType.TypeName}" : string.Empty;
            WriteLine($"CatchClauseNode Variable={node.ExceptionVariable}{typeInfo}");
            Indent();
            VisitList(node.Body);
            Dedent();
        }

        public void Visit(ThrowStatementNode node)
        {
            WriteLine("ThrowStatementNode");
            Indent();
            node.ThrownExpression.Accept(this);
            Dedent();
        }

        public void Visit(PatternMatchNode node)
        {
            WriteLine("PatternMatchNode");
            Indent();
            WriteLine("Subject:");
            Indent();
            node.SubjectExpression.Accept(this);
            Dedent();
            foreach (PatternCaseNode patternCase in node.Cases)
            {
                patternCase.Accept(this);
            }
            Dedent();
        }

        public void Visit(PatternCaseNode node)
        {
            WriteLine("PatternCaseNode");
            Indent();
            WriteLine("Pattern:");
            Indent();
            node.Pattern.Accept(this);
            Dedent();
            if (node.WhenGuard != null)
            {
                WriteLine("Guard:");
                Indent();
                node.WhenGuard.Accept(this);
                Dedent();
            }
            WriteLine("Body:");
            Indent();
            VisitList(node.Body);
            Dedent();
            Dedent();
        }

        public void Visit(AnnotatedStatementNode node)
        {
            WriteLine("AnnotatedStatementNode");
            Indent();
            node.Annotation.Accept(this);
            node.InnerStatement.Accept(this);
            Dedent();
        }

        public void Visit(AnnotationNode node)
        {
            WriteLine($"AnnotationNode Name={node.AnnotationName}");
            if (node.Parameters.Count > 0)
            {
                Indent();
                foreach (AnnotationParamNode param in node.Parameters)
                {
                    param.Accept(this);
                }
                Dedent();
            }
        }

        public void Visit(AnnotationParamNode node)
        {
            WriteLine($"AnnotationParamNode Key={node.KeyName}");
            Indent();
            node.ValueExpression.Accept(this);
            Dedent();
        }

        public void Visit(ArrayElementAssignNode node)
        {
            WriteLine($"ArrayElementAssignNode Array={node.ArrayName}");
            Indent();
            WriteLine("Index:");
            Indent();
            node.IndexExpression.Accept(this);
            Dedent();
            WriteLine("Value:");
            Indent();
            node.ValueExpression.Accept(this);
            Dedent();
            Dedent();
        }

        public void Visit(MemberAssignNode node)
        {
            WriteLine($"MemberAssignNode Member={node.MemberName}");
            Indent();
            WriteLine("Target:");
            Indent();
            node.Target.Accept(this);
            Dedent();
            WriteLine("Value:");
            Indent();
            node.ValueExpression.Accept(this);
            Dedent();
            Dedent();
        }

        // ----------------------------------------------------------------
        // Expression nodes
        // ----------------------------------------------------------------

        public void Visit(BinaryOpNode node)
        {
            WriteLine($"BinaryOpNode Operator={node.Operator}");
            Indent();
            node.Left.Accept(this);
            node.Right.Accept(this);
            Dedent();
        }

        public void Visit(UnaryOpNode node)
        {
            WriteLine($"UnaryOpNode Operator={node.Operator}");
            Indent();
            node.Operand.Accept(this);
            Dedent();
        }

        public void Visit(ConditionalExprNode node)
        {
            WriteLine("ConditionalExprNode");
            Indent();
            WriteLine("Condition:");
            Indent();
            node.Condition.Accept(this);
            Dedent();
            WriteLine("Then:");
            Indent();
            node.ThenExpression.Accept(this);
            Dedent();
            WriteLine("Else:");
            Indent();
            node.ElseExpression.Accept(this);
            Dedent();
            Dedent();
        }

        public void Visit(IdentifierNode node)
        {
            WriteLine($"IdentifierNode Name={node.Name}");
        }

        public void Visit(IntegerLiteralNode node)
        {
            WriteLine($"IntegerLiteralNode Value={node.Value}");
        }

        public void Visit(FloatLiteralNode node)
        {
            WriteLine($"FloatLiteralNode Value={node.Value}");
        }

        public void Visit(StringLiteralNode node)
        {
            WriteLine($"StringLiteralNode Value=\"{node.Value}\"");
        }

        public void Visit(BoolLiteralNode node)
        {
            WriteLine($"BoolLiteralNode Value={node.Value}");
        }

        public void Visit(NullLiteralNode node)
        {
            WriteLine("NullLiteralNode");
        }

        public void Visit(ArrayLiteralNode node)
        {
            WriteLine($"ArrayLiteralNode Count={node.Elements.Count}");
            Indent();
            VisitList(node.Elements);
            Dedent();
        }

        public void Visit(IndexAccessNode node)
        {
            WriteLine("IndexAccessNode");
            Indent();
            WriteLine("Target:");
            Indent();
            node.TargetExpression.Accept(this);
            Dedent();
            WriteLine("Index:");
            Indent();
            node.IndexExpression.Accept(this);
            Dedent();
            Dedent();
        }

        public void Visit(MemberAccessNode node)
        {
            WriteLine($"MemberAccessNode Member={node.MemberName}");
            Indent();
            node.TargetExpression.Accept(this);
            Dedent();
        }

        public void Visit(MethodCallNode node)
        {
            WriteLine($"MethodCallNode Method={node.MethodName}");
            Indent();
            WriteLine("Target:");
            Indent();
            node.TargetExpression.Accept(this);
            Dedent();
            if (node.Arguments.Count > 0)
            {
                WriteLine("Arguments:");
                Indent();
                VisitList(node.Arguments);
                Dedent();
            }
            Dedent();
        }

        public void Visit(FunctionCallNode node)
        {
            WriteLine("FunctionCallNode");
            Indent();
            WriteLine("Callee:");
            Indent();
            node.CalleeExpression.Accept(this);
            Dedent();
            if (node.Arguments.Count > 0)
            {
                WriteLine("Arguments:");
                Indent();
                VisitList(node.Arguments);
                Dedent();
            }
            Dedent();
        }

        public void Visit(NewExprNode node)
        {
            WriteLine($"NewExprNode Class={node.ClassName}");
            if (node.Arguments.Count > 0)
            {
                Indent();
                VisitList(node.Arguments);
                Dedent();
            }
        }

        public void Visit(CastExprNode node)
        {
            WriteLine($"CastExprNode Type={node.TargetType.TypeName}");
            Indent();
            node.Operand.Accept(this);
            Dedent();
        }

        public void Visit(TypeCheckNode node)
        {
            WriteLine($"TypeCheckNode Type={node.CheckedType.TypeName}");
            Indent();
            node.SubjectExpression.Accept(this);
            Dedent();
        }

        public void Visit(TypeAssertNode node)
        {
            WriteLine($"TypeAssertNode Type={node.AssertedType.TypeName}");
            Indent();
            node.SubjectExpression.Accept(this);
            Dedent();
        }

        public void Visit(LambdaExprNode node)
        {
            string bodyKind = node.IsBlockBody ? "block" : "expression";
            string returnInfo = node.ReturnType != null ? $" -> {node.ReturnType.TypeName}" : string.Empty;
            WriteLine($"LambdaExprNode [{bodyKind}]{returnInfo}");
            Indent();
            foreach (ParameterNode param in node.Parameters)
            {
                param.Accept(this);
            }
            if (node.IsBlockBody)
            {
                WriteLine("Body:");
                Indent();
                VisitList(node.StatementBody);
                Dedent();
            }
            else
            {
                WriteLine("Body:");
                Indent();
                node.ExpressionBody.Accept(this);
                Dedent();
            }
            Dedent();
        }

        public void Visit(ListComprehensionNode node)
        {
            WriteLine($"ListComprehensionNode LoopVariable={node.LoopVariable}");
            Indent();
            WriteLine("Element:");
            Indent();
            node.ElementExpression.Accept(this);
            Dedent();
            WriteLine("Source:");
            Indent();
            node.SourceExpression.Accept(this);
            Dedent();
            Dedent();
        }

        // ----------------------------------------------------------------
        // Pattern nodes
        // ----------------------------------------------------------------

        public void Visit(WildcardPatternNode node)
        {
            WriteLine("WildcardPatternNode");
        }

        public void Visit(ConstructorPatternNode node)
        {
            WriteLine($"ConstructorPatternNode Constructor={node.ConstructorName}");
            if (node.SubPatterns.Count > 0)
            {
                Indent();
                VisitList(node.SubPatterns);
                Dedent();
            }
        }

        public void Visit(ArrayPatternNode node)
        {
            WriteLine($"ArrayPatternNode Count={node.ElementPatterns.Count}");
            if (node.ElementPatterns.Count > 0)
            {
                Indent();
                VisitList(node.ElementPatterns);
                Dedent();
            }
        }

        public void Visit(FieldPatternNode node)
        {
            WriteLine($"FieldPatternNode Type={node.TypeName}");
            if (node.FieldPatterns.Count > 0)
            {
                Indent();
                foreach (FieldPatternEntryNode entry in node.FieldPatterns)
                {
                    entry.Accept(this);
                }
                Dedent();
            }
        }

        public void Visit(FieldPatternEntryNode node)
        {
            WriteLine($"FieldPatternEntryNode Field={node.FieldName}");
            Indent();
            node.FieldPattern.Accept(this);
            Dedent();
        }

        public void Visit(AlternationPatternNode node)
        {
            WriteLine("AlternationPatternNode");
            Indent();
            node.LeftPattern.Accept(this);
            node.RightPattern.Accept(this);
            Dedent();
        }

        // ----------------------------------------------------------------
        // Support nodes
        // ----------------------------------------------------------------

        public void Visit(ParameterNode node)
        {
            string typeInfo = node.DeclaredType != null ? $" Type={node.DeclaredType.TypeName}" : string.Empty;
            string defaultInfo = node.DefaultExpression != null ? " [has default]" : string.Empty;
            WriteLine($"ParameterNode Name={node.ParameterName}{typeInfo}{defaultInfo}");
            if (node.DefaultExpression != null)
            {
                Indent();
                node.DefaultExpression.Accept(this);
                Dedent();
            }
        }

        public void Visit(TypeNode node)
        {
            string arrayInfo = node.ArrayDimensions > 0
                ? new string('[', node.ArrayDimensions) + new string(']', node.ArrayDimensions)
                : string.Empty;
            string nullableInfo = node.IsNullable ? "?" : string.Empty;
            string genericInfo = node.TypeArguments.Count > 0
                ? $"<{node.TypeArguments.Count} args>"
                : string.Empty;
            WriteLine($"TypeNode Name={node.TypeName}{genericInfo}{arrayInfo}{nullableInfo}");
        }
    }
}
