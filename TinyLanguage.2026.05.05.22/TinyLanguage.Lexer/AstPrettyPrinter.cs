using System;
using System.Collections.Generic;
using System.Text;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Lexer
{
    // Produces a deterministic, indented text representation of any AST.
    // Each node type prints its name and key fields on one line. Child nodes
    // are indented two spaces deeper than their parent. This class implements
    // INodeVisitor so the interpreter and this printer share the same dispatch
    // mechanism without duplicating logic.
    public class AstPrettyPrinter : INodeVisitor
    {
        // Accumulates the output text.
        private readonly StringBuilder outputBuilder;

        // Current indentation depth measured in levels. Each level is two spaces.
        private int indentLevel;

        // The indent string for the current level, recomputed whenever indentLevel changes.
        private string CurrentIndent
        {
            get { return new string(' ', indentLevel * 2); }
        }

        public AstPrettyPrinter()
        {
            outputBuilder = new StringBuilder();
            indentLevel = 0;
        }

        // Visits the root node and returns the complete pretty-printed string.
        public string Print(AstNode rootNode)
        {
            outputBuilder.Clear();
            indentLevel = 0;
            rootNode.Accept(this);
            return outputBuilder.ToString();
        }

        // Appends a line at the current indent level.
        private void WriteLine(string text)
        {
            outputBuilder.AppendLine(CurrentIndent + text);
        }

        // Visits a list of child nodes at one indent level deeper than the caller.
        private void VisitChildren(IReadOnlyList<AstNode> children)
        {
            indentLevel++;
            foreach (AstNode child in children)
            {
                child.Accept(this);
            }
            indentLevel--;
        }

        // Visits a single optional child node (null is a no-op).
        private void VisitChild(AstNode child)
        {
            if (child != null)
            {
                indentLevel++;
                child.Accept(this);
                indentLevel--;
            }
        }

        // -----------------------------------------------------------------
        // Program
        // -----------------------------------------------------------------

        public void VisitProgramNode(ProgramNode node)
        {
            WriteLine($"ProgramNode [line={node.Line}]");
            VisitChildren(node.Statements);
        }

        // -----------------------------------------------------------------
        // Statements
        // -----------------------------------------------------------------

        public void VisitAssignStatementNode(AssignStatementNode node)
        {
            WriteLine($"AssignStatementNode [line={node.Line}] id={node.Id}");
            VisitChild(node.Expr);
        }

        public void VisitLetDeclareNode(LetDeclareNode node)
        {
            string typeLabel = node.DeclaredType != null ? " typed" : " untyped";
            WriteLine($"LetDeclareNode [line={node.Line}] id={node.Id}{typeLabel}");
            VisitChild(node.DeclaredType);
            VisitChild(node.Expr);
        }

        public void VisitVarDeclareNode(VarDeclareNode node)
        {
            WriteLine($"VarDeclareNode [line={node.Line}] id={node.Id}");
            VisitChild(node.DeclaredType);
            VisitChild(node.Expr);
        }

        public void VisitConstDeclareNode(ConstDeclareNode node)
        {
            string typeLabel = node.DeclaredType != null ? " typed" : " untyped";
            WriteLine($"ConstDeclareNode [line={node.Line}] id={node.Id}{typeLabel}");
            VisitChild(node.DeclaredType);
            VisitChild(node.Expr);
        }

        public void VisitEnumDefNode(EnumDefNode node)
        {
            WriteLine($"EnumDefNode [line={node.Line}] id={node.Id}");
            indentLevel++;
            foreach (EnumValueNode enumValue in node.EnumValues)
            {
                enumValue.Accept(this);
            }
            indentLevel--;
        }

        public void VisitEnumValueNode(EnumValueNode node)
        {
            string hasValue = node.ValueExpr != null ? " withValue" : "";
            WriteLine($"EnumValueNode [line={node.Line}] id={node.Id}{hasValue}");
            VisitChild(node.ValueExpr);
        }

        public void VisitArrayElementAssignNode(ArrayElementAssignNode node)
        {
            WriteLine($"ArrayElementAssignNode [line={node.Line}] id={node.Id}");
            indentLevel++;
            WriteLine("index:");
            VisitChild(node.IndexExpr);
            WriteLine("value:");
            VisitChild(node.ValueExpr);
            indentLevel--;
        }

        public void VisitMemberAssignNode(MemberAssignNode node)
        {
            WriteLine($"MemberAssignNode [line={node.Line}]");
            indentLevel++;
            WriteLine("target:");
            VisitChild(node.Target);
            WriteLine("value:");
            VisitChild(node.ValueExpr);
            indentLevel--;
        }

        public void VisitIndexedAssignNode(IndexedAssignNode node)
        {
            WriteLine($"IndexedAssignNode [line={node.Line}]");
            indentLevel++;
            WriteLine("target:");
            VisitChild(node.Target);
            WriteLine("value:");
            VisitChild(node.ValueExpr);
            indentLevel--;
        }

        public void VisitExpressionStatementNode(ExpressionStatementNode node)
        {
            WriteLine($"ExpressionStatementNode [line={node.Line}]");
            indentLevel++;
            VisitChild(node.Expression);
            indentLevel--;
        }

        public void VisitIfStatementNode(IfStatementNode node)
        {
            bool hasElse = node.ElseStatements.Count > 0;
            WriteLine($"IfStatementNode [line={node.Line}] hasElse={hasElse}");
            indentLevel++;
            WriteLine("condition:");
            VisitChild(node.Condition);
            WriteLine("then:");
            VisitChildren(node.ThenStatements);
            if (hasElse)
            {
                WriteLine("else:");
                VisitChildren(node.ElseStatements);
            }
            indentLevel--;
        }

        public void VisitWhileStatementNode(WhileStatementNode node)
        {
            WriteLine($"WhileStatementNode [line={node.Line}]");
            indentLevel++;
            WriteLine("condition:");
            VisitChild(node.Condition);
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitForStatementNode(ForStatementNode node)
        {
            bool hasStep = node.StepExpr != null;
            WriteLine($"ForStatementNode [line={node.Line}] id={node.Id} hasStep={hasStep}");
            indentLevel++;
            WriteLine("start:");
            VisitChild(node.StartExpr);
            WriteLine("end:");
            VisitChild(node.EndExpr);
            if (hasStep)
            {
                WriteLine("step:");
                VisitChild(node.StepExpr);
            }
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitForeachStatementNode(ForeachStatementNode node)
        {
            WriteLine($"ForeachStatementNode [line={node.Line}] id={node.Id}");
            indentLevel++;
            WriteLine("iterable:");
            VisitChild(node.IterableExpr);
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitDoWhileStatementNode(DoWhileStatementNode node)
        {
            WriteLine($"DoWhileStatementNode [line={node.Line}]");
            indentLevel++;
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            WriteLine("condition:");
            VisitChild(node.Condition);
            indentLevel--;
        }

        public void VisitSwitchStatementNode(SwitchStatementNode node)
        {
            WriteLine($"SwitchStatementNode [line={node.Line}]");
            indentLevel++;
            WriteLine("expr:");
            VisitChild(node.SwitchExpr);
            WriteLine("cases:");
            VisitChildren(node.Cases);
            indentLevel--;
        }

        public void VisitCaseClauseNode(CaseClauseNode node)
        {
            WriteLine($"CaseClauseNode [line={node.Line}]");
            indentLevel++;
            WriteLine("match:");
            VisitChild(node.MatchExpr);
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitDefaultClauseNode(DefaultClauseNode node)
        {
            WriteLine($"DefaultClauseNode [line={node.Line}]");
            indentLevel++;
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitBreakStatementNode(BreakStatementNode node)
        {
            WriteLine($"BreakStatementNode [line={node.Line}]");
        }

        public void VisitContinueStatementNode(ContinueStatementNode node)
        {
            WriteLine($"ContinueStatementNode [line={node.Line}]");
        }

        public void VisitPrintStatementNode(PrintStatementNode node)
        {
            WriteLine($"PrintStatementNode [line={node.Line}]");
            VisitChild(node.Expr);
        }

        public void VisitInputStatementNode(InputStatementNode node)
        {
            WriteLine($"InputStatementNode [line={node.Line}] id={node.Id}");
        }

        public void VisitFunctionDefNode(FunctionDefNode node)
        {
            string returnLabel = node.ReturnType != null ? " hasReturn" : "";
            WriteLine($"FunctionDefNode [line={node.Line}] id={node.Id} params={node.Parameters.Count}{returnLabel}");
            indentLevel++;
            foreach (ParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            if (node.ReturnType != null)
            {
                WriteLine("returnType:");
                VisitChild(node.ReturnType);
            }
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitMethodDefNode(MethodDefNode node)
        {
            string staticLabel = node.IsStatic ? " static" : "";
            string returnLabel = node.ReturnType != null ? " hasReturn" : "";
            WriteLine($"MethodDefNode [line={node.Line}] id={node.Id} params={node.Parameters.Count}{staticLabel}{returnLabel}");
            indentLevel++;
            foreach (ParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            if (node.ReturnType != null)
            {
                WriteLine("returnType:");
                VisitChild(node.ReturnType);
            }
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitCallStatementNode(CallStatementNode node)
        {
            string chain = string.Join(".", node.TargetIds);
            WriteLine($"CallStatementNode [line={node.Line}] target={chain} args={node.Args.Count}");
            VisitChildren(node.Args);
        }

        public void VisitReturnStatementNode(ReturnStatementNode node)
        {
            bool hasValue = node.ReturnExpr != null;
            WriteLine($"ReturnStatementNode [line={node.Line}] hasValue={hasValue}");
            VisitChild(node.ReturnExpr);
        }

        public void VisitClassDefNode(ClassDefNode node)
        {
            string extendsLabel = node.ExtendsId != null ? $" extends={node.ExtendsId}" : "";
            string staticLabel = node.IsStatic ? " static" : "";
            string implementsLabel = node.ImplementsIds.Count > 0
                ? $" implements={string.Join(",", node.ImplementsIds)}"
                : "";
            WriteLine($"ClassDefNode [line={node.Line}] id={node.Id}{staticLabel}{extendsLabel}{implementsLabel}");
            VisitChildren(node.Members);
        }

        public void VisitConstructorDefNode(ConstructorDefNode node)
        {
            WriteLine($"ConstructorDefNode [line={node.Line}] params={node.Parameters.Count}");
            indentLevel++;
            foreach (ParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitFieldDeclareNode(FieldDeclareNode node)
        {
            string typeLabel = node.DeclaredType != null ? " typed" : " untyped";
            WriteLine($"FieldDeclareNode [line={node.Line}] kind={node.Kind} id={node.Id}{typeLabel}");
            VisitChild(node.DeclaredType);
            VisitChild(node.Expr);
        }

        public void VisitModuleDefNode(ModuleDefNode node)
        {
            WriteLine($"ModuleDefNode [line={node.Line}] id={node.Id} imports={node.ImportList.Count}");
            indentLevel++;
            foreach (ModuleImportNode importNode in node.ImportList)
            {
                importNode.Accept(this);
            }
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitModuleImportNode(ModuleImportNode node)
        {
            string aliasLabel = node.AliasId != null ? $" as={node.AliasId}" : "";
            WriteLine($"ModuleImportNode [line={node.Line}] id={node.Id}{aliasLabel}");
        }

        public void VisitImportStatementNode(ImportStatementNode node)
        {
            string aliasLabel = node.AliasId != null ? $" as={node.AliasId}" : "";
            WriteLine($"ImportStatementNode [line={node.Line}] id={node.Id}{aliasLabel}");
        }

        public void VisitExportStatementNode(ExportStatementNode node)
        {
            WriteLine($"ExportStatementNode [line={node.Line}] id={node.Id}");
        }

        public void VisitTryStatementNode(TryStatementNode node)
        {
            bool hasFinally = node.FinallyStatements.Count > 0;
            WriteLine($"TryStatementNode [line={node.Line}] hasFinally={hasFinally}");
            indentLevel++;
            WriteLine("try:");
            VisitChildren(node.TryStatements);
            indentLevel--;
            VisitChild(node.CatchClause);
            if (hasFinally)
            {
                indentLevel++;
                WriteLine("finally:");
                VisitChildren(node.FinallyStatements);
                indentLevel--;
            }
        }

        public void VisitCatchClauseNode(CatchClauseNode node)
        {
            string typeLabel = node.ExceptionType != null ? " typed" : " bare";
            WriteLine($"CatchClauseNode [line={node.Line}] id={node.Id}{typeLabel}");
            VisitChild(node.ExceptionType);
            VisitChildren(node.BodyStatements);
        }

        public void VisitThrowStatementNode(ThrowStatementNode node)
        {
            WriteLine($"ThrowStatementNode [line={node.Line}]");
            VisitChild(node.Expr);
        }

        public void VisitPatternMatchNode(PatternMatchNode node)
        {
            WriteLine($"PatternMatchNode [line={node.Line}] cases={node.Cases.Count}");
            indentLevel++;
            WriteLine("matchExpr:");
            VisitChild(node.MatchExpr);
            foreach (PatternCaseNode patternCase in node.Cases)
            {
                patternCase.Accept(this);
            }
            indentLevel--;
        }

        public void VisitPatternCaseNode(PatternCaseNode node)
        {
            bool hasGuard = node.GuardExpr != null;
            WriteLine($"PatternCaseNode [line={node.Line}] hasGuard={hasGuard}");
            indentLevel++;
            WriteLine("pattern:");
            VisitChild(node.Pattern);
            if (hasGuard)
            {
                WriteLine("guard:");
                VisitChild(node.GuardExpr);
            }
            WriteLine("body:");
            VisitChildren(node.BodyStatements);
            indentLevel--;
        }

        public void VisitAnnotatedStatementNode(AnnotatedStatementNode node)
        {
            WriteLine($"AnnotatedStatementNode [line={node.Line}]");
            indentLevel++;
            node.Annotation.Accept(this);
            node.WrappedStatement.Accept(this);
            indentLevel--;
        }

        public void VisitAnnotationNode(AnnotationNode node)
        {
            WriteLine($"AnnotationNode [line={node.Line}] id={node.Id} params={node.Parameters.Count}");
            indentLevel++;
            foreach (AnnotationParamNode param in node.Parameters)
            {
                param.Accept(this);
            }
            indentLevel--;
        }

        public void VisitAnnotationParamNode(AnnotationParamNode node)
        {
            WriteLine($"AnnotationParamNode [line={node.Line}] id={node.Id}");
            VisitChild(node.ValueExpr);
        }

        // -----------------------------------------------------------------
        // Patterns
        // -----------------------------------------------------------------

        public void VisitIdentifierPatternNode(IdentifierPatternNode node)
        {
            WriteLine($"IdentifierPatternNode [line={node.Line}] id={node.Id}");
        }

        public void VisitLiteralPatternNode(LiteralPatternNode node)
        {
            WriteLine($"LiteralPatternNode [line={node.Line}]");
            VisitChild(node.LiteralExpr);
        }

        public void VisitWildcardPatternNode(WildcardPatternNode node)
        {
            WriteLine($"WildcardPatternNode [line={node.Line}]");
        }

        public void VisitConstructorPatternNode(ConstructorPatternNode node)
        {
            WriteLine($"ConstructorPatternNode [line={node.Line}] type={node.TypeId} args={node.ArgumentPatterns.Count}");
            indentLevel++;
            foreach (PatternNode argumentPattern in node.ArgumentPatterns)
            {
                argumentPattern.Accept(this);
            }
            indentLevel--;
        }

        public void VisitArrayPatternNode(ArrayPatternNode node)
        {
            WriteLine($"ArrayPatternNode [line={node.Line}] elements={node.ElementPatterns.Count}");
            indentLevel++;
            foreach (PatternNode elementPattern in node.ElementPatterns)
            {
                elementPattern.Accept(this);
            }
            indentLevel--;
        }

        public void VisitFieldPatternNode(FieldPatternNode node)
        {
            WriteLine($"FieldPatternNode [line={node.Line}] type={node.TypeId} fields={node.FieldPatterns.Count}");
            indentLevel++;
            foreach (FieldPatternPair pair in node.FieldPatterns)
            {
                WriteLine($"field={pair.FieldId}:");
                VisitChild(pair.Pattern);
            }
            indentLevel--;
        }

        public void VisitAlternationPatternNode(AlternationPatternNode node)
        {
            WriteLine($"AlternationPatternNode [line={node.Line}]");
            indentLevel++;
            WriteLine("left:");
            VisitChild(node.Left);
            WriteLine("right:");
            VisitChild(node.Right);
            indentLevel--;
        }

        // -----------------------------------------------------------------
        // Expressions
        // -----------------------------------------------------------------

        public void VisitBinaryOpNode(BinaryOpNode node)
        {
            WriteLine($"BinaryOpNode [line={node.Line}] op={node.Op}");
            indentLevel++;
            node.Left.Accept(this);
            node.Right.Accept(this);
            indentLevel--;
        }

        public void VisitUnaryOpNode(UnaryOpNode node)
        {
            WriteLine($"UnaryOpNode [line={node.Line}] op={node.Op}");
            VisitChild(node.Operand);
        }

        public void VisitTernaryNode(TernaryNode node)
        {
            WriteLine($"TernaryNode [line={node.Line}]");
            indentLevel++;
            WriteLine("condition:");
            VisitChild(node.Condition);
            WriteLine("then:");
            VisitChild(node.ThenExpr);
            WriteLine("else:");
            VisitChild(node.ElseExpr);
            indentLevel--;
        }

        public void VisitIdentifierNode(IdentifierNode node)
        {
            WriteLine($"IdentifierNode [line={node.Line}] name={node.Name}");
        }

        public void VisitIntegerLiteralNode(IntegerLiteralNode node)
        {
            WriteLine($"IntegerLiteralNode [line={node.Line}] value={node.Value}");
        }

        public void VisitFloatLiteralNode(FloatLiteralNode node)
        {
            WriteLine($"FloatLiteralNode [line={node.Line}] value={node.Value}");
        }

        public void VisitStringLiteralNode(StringLiteralNode node)
        {
            // Escape the stored value so the output is unambiguous.
            string escapedValue = node.Value.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t");
            WriteLine($"StringLiteralNode [line={node.Line}] value=\"{escapedValue}\"");
        }

        public void VisitBoolLiteralNode(BoolLiteralNode node)
        {
            WriteLine($"BoolLiteralNode [line={node.Line}] value={node.Value}");
        }

        public void VisitNullLiteralNode(NullLiteralNode node)
        {
            WriteLine($"NullLiteralNode [line={node.Line}]");
        }

        public void VisitArrayLiteralNode(ArrayLiteralNode node)
        {
            WriteLine($"ArrayLiteralNode [line={node.Line}] elements={node.Elements.Count}");
            VisitChildren(node.Elements);
        }

        public void VisitIndexAccessNode(IndexAccessNode node)
        {
            WriteLine($"IndexAccessNode [line={node.Line}]");
            indentLevel++;
            WriteLine("target:");
            VisitChild(node.Target);
            WriteLine("index:");
            VisitChild(node.IndexExpr);
            indentLevel--;
        }

        public void VisitMemberAccessNode(MemberAccessNode node)
        {
            WriteLine($"MemberAccessNode [line={node.Line}] member={node.MemberId}");
            VisitChild(node.Target);
        }

        public void VisitMethodCallNode(MethodCallNode node)
        {
            WriteLine($"MethodCallNode [line={node.Line}] method={node.MethodId} args={node.Args.Count}");
            indentLevel++;
            WriteLine("target:");
            VisitChild(node.Target);
            VisitChildren(node.Args);
            indentLevel--;
        }

        public void VisitFunctionCallNode(FunctionCallNode node)
        {
            WriteLine($"FunctionCallNode [line={node.Line}] args={node.Args.Count}");
            indentLevel++;
            WriteLine("callee:");
            VisitChild(node.CalleeExpr);
            VisitChildren(node.Args);
            indentLevel--;
        }

        public void VisitNewExprNode(NewExprNode node)
        {
            WriteLine($"NewExprNode [line={node.Line}] class={node.ClassId} args={node.Args.Count}");
            VisitChildren(node.Args);
        }

        public void VisitCastExprNode(CastExprNode node)
        {
            WriteLine($"CastExprNode [line={node.Line}]");
            indentLevel++;
            WriteLine("targetType:");
            VisitChild(node.TargetType);
            WriteLine("expr:");
            VisitChild(node.Expr);
            indentLevel--;
        }

        public void VisitTypeCheckNode(TypeCheckNode node)
        {
            WriteLine($"TypeCheckNode [line={node.Line}]");
            indentLevel++;
            WriteLine("expr:");
            VisitChild(node.Expr);
            WriteLine("checkType:");
            VisitChild(node.CheckType);
            indentLevel--;
        }

        public void VisitTypeAssertNode(TypeAssertNode node)
        {
            WriteLine($"TypeAssertNode [line={node.Line}]");
            indentLevel++;
            WriteLine("expr:");
            VisitChild(node.Expr);
            WriteLine("assertType:");
            VisitChild(node.AssertType);
            indentLevel--;
        }

        public void VisitConditionalExprNode(ConditionalExprNode node)
        {
            WriteLine($"ConditionalExprNode [line={node.Line}]");
            indentLevel++;
            WriteLine("condition:");
            VisitChild(node.Condition);
            WriteLine("then:");
            VisitChild(node.ThenExpr);
            WriteLine("else:");
            VisitChild(node.ElseExpr);
            indentLevel--;
        }

        public void VisitLambdaExprNode(LambdaExprNode node)
        {
            string formLabel = node.IsBlockForm ? "block" : "expression";
            string returnLabel = node.ReturnType != null ? " hasReturn" : "";
            WriteLine($"LambdaExprNode [line={node.Line}] form={formLabel} params={node.Parameters.Count}{returnLabel}");
            indentLevel++;
            foreach (ParameterNode parameter in node.Parameters)
            {
                parameter.Accept(this);
            }
            if (node.ReturnType != null)
            {
                WriteLine("returnType:");
                VisitChild(node.ReturnType);
            }
            if (node.IsBlockForm)
            {
                WriteLine("body:");
                VisitChildren(node.BodyStatements);
            }
            else
            {
                WriteLine("bodyExpr:");
                VisitChild(node.BodyExpr);
            }
            indentLevel--;
        }

        public void VisitListComprehensionNode(ListComprehensionNode node)
        {
            WriteLine($"ListComprehensionNode [line={node.Line}] var={node.IterationVar}");
            indentLevel++;
            WriteLine("elementExpr:");
            VisitChild(node.ElementExpr);
            WriteLine("iterableExpr:");
            VisitChild(node.IterableExpr);
            indentLevel--;
        }

        // -----------------------------------------------------------------
        // Shared / auxiliary
        // -----------------------------------------------------------------

        public void VisitParameterNode(ParameterNode node)
        {
            string typeLabel = node.ParameterType != null ? " typed" : "";
            string defaultLabel = node.DefaultExpr != null ? " hasDefault" : "";
            WriteLine($"ParameterNode [line={node.Line}] id={node.Id}{typeLabel}{defaultLabel}");
            VisitChild(node.ParameterType);
            VisitChild(node.DefaultExpr);
        }

        public void VisitTypeNode(TypeNode node)
        {
            string suffixText = "";
            foreach (TypeSuffix suffix in node.Suffixes)
            {
                suffixText += suffix == TypeSuffix.ArraySuffix ? "[]" : "?";
            }
            string genericText = node.GenericArgs.Count > 0 ? $" generic={node.GenericArgs.Count}" : "";
            string mapText = node.MapKeyType != null ? " isMap" : "";
            WriteLine($"TypeNode [line={node.Line}] base={node.BaseType}{suffixText}{genericText}{mapText}");
            if (node.MapKeyType != null)
            {
                indentLevel++;
                WriteLine("keyType:");
                VisitChild(node.MapKeyType);
                WriteLine("valueType:");
                VisitChild(node.MapValueType);
                indentLevel--;
            }
            indentLevel++;
            foreach (TypeNode genericArg in node.GenericArgs)
            {
                genericArg.Accept(this);
            }
            indentLevel--;
        }
    }
}
