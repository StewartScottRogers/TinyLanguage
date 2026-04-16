using System;
using System.Collections.Generic;
using System.IO;
using TinyLanguage.Lexer;

namespace TinyLanguage.Interpreter
{
    // Tree-walking interpreter that implements the INodeVisitor interface.
    // Evaluates a TinyLanguage AST by visiting each node and maintaining
    // a scope chain for variable resolution.
    public class Interpreter : IInterpreter, INodeVisitor
    {
        // The global (root) scope — parent of all other scopes.
        private readonly Scope GlobalScope;

        // The currently active scope during execution.
        private Scope CurrentScope;

        // The last evaluated expression result — used to pass values between Visit calls.
        private object LastResult;

        // Current call depth for stack overflow detection.
        private int CallDepth;

        // Maximum allowed call depth per spec.
        private const int MaxCallDepth = 500;

        // Output writer for print statements — defaults to Console.Out.
        private readonly TextWriter Output;

        // Input reader for input statements — defaults to Console.In.
        private readonly TextReader Input;

        // Module registry — maps module names to their ModuleValue.
        private readonly Dictionary<string, ModuleValue> Modules;

        public Interpreter() : this(Console.Out, Console.In)
        {
        }

        public Interpreter(TextWriter output, TextReader input)
        {
            Output = output;
            Input = input;
            GlobalScope = new Scope(null);
            CurrentScope = GlobalScope;
            LastResult = null;
            CallDepth = 0;
            Modules = new Dictionary<string, ModuleValue>();
            RegisterBuiltIns();
        }

        // Register the built-in functions: len, str, int, bool.
        private void RegisterBuiltIns()
        {
            GlobalScope.Define("len", new BuiltInFunction("len", 1, EvalLen));
            GlobalScope.Define("str", new BuiltInFunction("str", 1, EvalStr));
            GlobalScope.Define("int", new BuiltInFunction("int", 1, EvalInt));
            GlobalScope.Define("bool", new BuiltInFunction("bool", 1, EvalBool));
        }

        // Execute the program and return the last result value.
        public object Execute(ProgramNode program)
        {
            program.Accept(this);
            return LastResult;
        }

        // ----------------------------------------------------------------
        // Helper: evaluate an AST node and return its value.
        // ----------------------------------------------------------------
        private object Evaluate(AstNode node)
        {
            node.Accept(this);
            return LastResult;
        }

        // ----------------------------------------------------------------
        // Helper: execute a list of statements in the given scope.
        // ----------------------------------------------------------------
        private void ExecuteStatements(List<AstNode> statements, Scope scope)
        {
            Scope previousScope = CurrentScope;
            CurrentScope = scope;
            try
            {
                foreach (AstNode statement in statements)
                {
                    statement.Accept(this);
                }
            }
            finally
            {
                CurrentScope = previousScope;
            }
        }

        // ----------------------------------------------------------------
        // Helper: execute a list of statements in the current scope.
        // ----------------------------------------------------------------
        private void ExecuteStatements(List<AstNode> statements)
        {
            foreach (AstNode statement in statements)
            {
                statement.Accept(this);
            }
        }

        // ----------------------------------------------------------------
        // Truthiness per spec: false, null, 0, 0.0, "" are falsy.
        // ----------------------------------------------------------------
        private static bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is bool boolValue)
            {
                return boolValue;
            }
            if (value is long longValue)
            {
                return longValue != 0;
            }
            if (value is double doubleValue)
            {
                return doubleValue != 0.0;
            }
            if (value is string stringValue)
            {
                return stringValue.Length > 0;
            }
            return true;
        }

        // ----------------------------------------------------------------
        // Convert a value to its string representation.
        // ----------------------------------------------------------------
        private static string Stringify(object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }
            if (value is double doubleValue)
            {
                // Use a format that preserves the decimal point for whole numbers.
                if (doubleValue == Math.Floor(doubleValue) && !double.IsInfinity(doubleValue))
                {
                    return doubleValue.ToString("0.0");
                }
                return doubleValue.ToString();
            }
            if (value is List<object> listValue)
            {
                List<string> elements = new List<string>();
                foreach (object element in listValue)
                {
                    elements.Add(Stringify(element));
                }
                return "[" + string.Join(", ", elements) + "]";
            }
            return value.ToString();
        }

        // ----------------------------------------------------------------
        // Call a function value with given arguments.
        // ----------------------------------------------------------------
        private object CallFunction(FunctionValue function, List<object> arguments, int line)
        {
            CallDepth++;
            if (CallDepth > MaxCallDepth)
            {
                throw new InterpreterException("Stack overflow: maximum call depth of 500 exceeded", line);
            }

            try
            {
                // Function scope is a fresh child of the closure scope (global per spec note 9).
                Scope functionScope = function.ClosureScope.CreateChild();

                // Bind parameters.
                for (int index = 0; index < function.Parameters.Count; index++)
                {
                    ParameterNode parameter = function.Parameters[index];
                    if (index < arguments.Count)
                    {
                        functionScope.Define(parameter.ParameterName, arguments[index]);
                    }
                    else if (parameter.DefaultExpression != null)
                    {
                        object defaultValue = Evaluate(parameter.DefaultExpression);
                        functionScope.Define(parameter.ParameterName, defaultValue);
                    }
                    else
                    {
                        throw new InterpreterException(
                            $"Missing argument for parameter '{parameter.ParameterName}' in call to '{function.Name}'",
                            line);
                    }
                }

                // Execute body.
                Scope previousScope = CurrentScope;
                CurrentScope = functionScope;
                try
                {
                    foreach (AstNode statement in function.Body)
                    {
                        statement.Accept(this);
                    }
                }
                catch (ReturnException returnException)
                {
                    return returnException.Value;
                }
                finally
                {
                    CurrentScope = previousScope;
                }

                // Functions that don't explicitly return produce null.
                return null;
            }
            finally
            {
                CallDepth--;
            }
        }

        // ----------------------------------------------------------------
        // Program root
        // ----------------------------------------------------------------
        public void Visit(ProgramNode node)
        {
            foreach (AstNode statement in node.Statements)
            {
                statement.Accept(this);
            }
        }

        // ----------------------------------------------------------------
        // Statement nodes
        // ----------------------------------------------------------------

        public void Visit(AssignStatementNode node)
        {
            object value = Evaluate(node.ValueExpression);
            CurrentScope.Assign(node.VariableName, value, node.Line);
            LastResult = value;
        }

        public void Visit(LetDeclareNode node)
        {
            object value = Evaluate(node.InitialiserExpression);
            CurrentScope.Define(node.VariableName, value);
            LastResult = value;
        }

        public void Visit(VarDeclareNode node)
        {
            object value = Evaluate(node.InitialiserExpression);
            CurrentScope.Define(node.VariableName, value);
            LastResult = value;
        }

        public void Visit(ConstDeclareNode node)
        {
            object value = Evaluate(node.InitialiserExpression);
            CurrentScope.DefineConstant(node.ConstantName, value);
            LastResult = value;
        }

        public void Visit(EnumDefNode node)
        {
            Dictionary<string, long> members = new Dictionary<string, long>();
            long nextValue = 0;
            foreach (EnumValueNode member in node.Members)
            {
                if (member.ValueExpression != null)
                {
                    object evaluatedValue = Evaluate(member.ValueExpression);
                    if (evaluatedValue is long longVal)
                    {
                        nextValue = longVal;
                    }
                    else
                    {
                        throw new InterpreterException(
                            $"Enum value for '{member.MemberName}' must be an integer", member.Line);
                    }
                }
                members[member.MemberName] = nextValue;
                nextValue++;
            }
            EnumValue enumValue = new EnumValue(node.EnumName, members);
            CurrentScope.Define(node.EnumName, enumValue);
            LastResult = null;
        }

        public void Visit(EnumValueNode node)
        {
            // Handled by EnumDefNode visitor.
        }

        public void Visit(IfStatementNode node)
        {
            object condition = Evaluate(node.Condition);
            if (IsTruthy(condition))
            {
                Scope childScope = CurrentScope.CreateChild();
                ExecuteStatements(node.ThenBody, childScope);
            }
            else if (node.ElseBody != null)
            {
                Scope childScope = CurrentScope.CreateChild();
                ExecuteStatements(node.ElseBody, childScope);
            }
        }

        public void Visit(WhileStatementNode node)
        {
            while (IsTruthy(Evaluate(node.Condition)))
            {
                Scope loopScope = CurrentScope.CreateChild();
                try
                {
                    ExecuteStatements(node.Body, loopScope);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    continue;
                }
            }
        }

        public void Visit(ForStatementNode node)
        {
            object startValue = Evaluate(node.StartExpression);
            object endValue = Evaluate(node.EndExpression);
            object stepValue = node.StepExpression != null ? Evaluate(node.StepExpression) : (object)1L;

            if (startValue is long startLong && endValue is long endLong && stepValue is long stepLong)
            {
                if (stepLong == 0)
                {
                    throw new InterpreterException("Step value cannot be zero", node.Line);
                }
                for (long current = startLong;
                     stepLong > 0 ? current <= endLong : current >= endLong;
                     current += stepLong)
                {
                    Scope loopScope = CurrentScope.CreateChild();
                    loopScope.Define(node.LoopVariable, current);
                    try
                    {
                        ExecuteStatements(node.Body, loopScope);
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }
            }
            else
            {
                double startDouble = ConvertToDouble(startValue, node.Line);
                double endDouble = ConvertToDouble(endValue, node.Line);
                double stepDouble = ConvertToDouble(stepValue, node.Line);
                if (stepDouble == 0.0)
                {
                    throw new InterpreterException("Step value cannot be zero", node.Line);
                }
                for (double current = startDouble;
                     stepDouble > 0 ? current <= endDouble : current >= endDouble;
                     current += stepDouble)
                {
                    Scope loopScope = CurrentScope.CreateChild();
                    loopScope.Define(node.LoopVariable, current);
                    try
                    {
                        ExecuteStatements(node.Body, loopScope);
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }
            }
        }

        public void Visit(ForeachStatementNode node)
        {
            object iterable = Evaluate(node.IterableExpression);
            if (iterable == null)
            {
                throw new InterpreterException("Cannot iterate over null", node.Line);
            }
            if (iterable is List<object> list)
            {
                foreach (object element in list)
                {
                    Scope loopScope = CurrentScope.CreateChild();
                    loopScope.Define(node.ElementVariable, element);
                    try
                    {
                        ExecuteStatements(node.Body, loopScope);
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }
            }
            else if (iterable is string stringValue)
            {
                // Spec note 6: foreach over string yields each character as a single-character string.
                foreach (char character in stringValue)
                {
                    Scope loopScope = CurrentScope.CreateChild();
                    loopScope.Define(node.ElementVariable, character.ToString());
                    try
                    {
                        ExecuteStatements(node.Body, loopScope);
                    }
                    catch (BreakException)
                    {
                        break;
                    }
                    catch (ContinueException)
                    {
                        continue;
                    }
                }
            }
            else
            {
                throw new InterpreterException(
                    $"Cannot iterate over value of type '{iterable.GetType().Name}'", node.Line);
            }
        }

        public void Visit(DoWhileStatementNode node)
        {
            do
            {
                Scope loopScope = CurrentScope.CreateChild();
                try
                {
                    ExecuteStatements(node.Body, loopScope);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    // Continue to condition check.
                }
            }
            while (IsTruthy(Evaluate(node.Condition)));
        }

        public void Visit(SwitchStatementNode node)
        {
            object subject = Evaluate(node.SubjectExpression);
            bool matched = false;
            foreach (SwitchCaseNode caseNode in node.Cases)
            {
                if (caseNode.IsDefault)
                {
                    Scope caseScope = CurrentScope.CreateChild();
                    try
                    {
                        ExecuteStatements(caseNode.Body, caseScope);
                    }
                    catch (BreakException)
                    {
                        // Break exits the switch.
                    }
                    matched = true;
                    break;
                }
                object caseValue = Evaluate(caseNode.ValueExpression);
                if (AreEqual(subject, caseValue))
                {
                    Scope caseScope = CurrentScope.CreateChild();
                    try
                    {
                        ExecuteStatements(caseNode.Body, caseScope);
                    }
                    catch (BreakException)
                    {
                        // Break exits the switch.
                    }
                    matched = true;
                    break;
                }
            }
            // If no case matched and no default was found, do nothing.
            if (!matched)
            {
                // Check if there is a default case that was not at the end.
                foreach (SwitchCaseNode caseNode in node.Cases)
                {
                    if (caseNode.IsDefault)
                    {
                        Scope caseScope = CurrentScope.CreateChild();
                        try
                        {
                            ExecuteStatements(caseNode.Body, caseScope);
                        }
                        catch (BreakException)
                        {
                            // Break exits the switch.
                        }
                        break;
                    }
                }
            }
        }

        public void Visit(SwitchCaseNode node)
        {
            // Handled by SwitchStatementNode visitor.
        }

        public void Visit(BreakStatementNode node)
        {
            throw new BreakException();
        }

        public void Visit(ContinueStatementNode node)
        {
            throw new ContinueException();
        }

        public void Visit(PrintStatementNode node)
        {
            object value = Evaluate(node.Expression);
            Output.WriteLine(Stringify(value));
        }

        public void Visit(InputStatementNode node)
        {
            string inputValue = Input.ReadLine();
            if (CurrentScope.IsDefined(node.VariableName))
            {
                CurrentScope.Assign(node.VariableName, inputValue, node.Line);
            }
            else
            {
                CurrentScope.Define(node.VariableName, inputValue);
            }
        }

        public void Visit(FunctionDefNode node)
        {
            // Functions capture only the global scope per spec note 9.
            FunctionValue function = new FunctionValue(
                node.FunctionName,
                node.Parameters,
                node.Body,
                GlobalScope);
            CurrentScope.Define(node.FunctionName, function);
        }

        public void Visit(ReturnStatementNode node)
        {
            object value = null;
            if (node.ReturnExpression != null)
            {
                value = Evaluate(node.ReturnExpression);
            }
            throw new ReturnException(value);
        }

        public void Visit(CallStatementNode node)
        {
            // Resolve the receiver chain.
            if (node.ReceiverChain.Count == 1)
            {
                // Simple function call.
                string functionName = node.ReceiverChain[0];
                object callee = CurrentScope.Lookup(functionName, node.Line);
                List<object> arguments = EvaluateArguments(node.Arguments);
                CallCallable(callee, arguments, node.Line);
            }
            else
            {
                // Method call: obj.method() or obj.a.b.method()
                string firstName = node.ReceiverChain[0];
                object target = CurrentScope.Lookup(firstName, node.Line);

                // Walk through intermediate member accesses.
                for (int index = 1; index < node.ReceiverChain.Count - 1; index++)
                {
                    target = GetMember(target, node.ReceiverChain[index], node.Line);
                }

                // Final element is the method name.
                string methodName = node.ReceiverChain[node.ReceiverChain.Count - 1];
                List<object> arguments = EvaluateArguments(node.Arguments);
                CallMethod(target, methodName, arguments, node.Line);
            }
        }

        public void Visit(ClassDefNode node)
        {
            ClassValue classValue = new ClassValue(
                node.ClassName,
                node.Members,
                CurrentScope,
                node.BaseClassName);
            CurrentScope.Define(node.ClassName, classValue);
        }

        public void Visit(ConstructorDefNode node)
        {
            // Handled during object instantiation in NewExprNode.
        }

        public void Visit(FieldDeclareNode node)
        {
            // Handled during object instantiation in NewExprNode.
        }

        public void Visit(ModuleDefNode node)
        {
            Scope moduleScope = GlobalScope.CreateChild();
            Scope previousScope = CurrentScope;
            CurrentScope = moduleScope;
            HashSet<string> exportedNames = new HashSet<string>();

            try
            {
                // Process header imports.
                foreach (ModuleImportNode importNode in node.HeaderImports)
                {
                    ProcessModuleImport(importNode.ModuleName, importNode.Alias, importNode.Line);
                }

                // Execute the module body; track exports.
                foreach (AstNode statement in node.Body)
                {
                    if (statement is ExportStatementNode exportStatement)
                    {
                        exportedNames.Add(exportStatement.ExportedName);
                    }
                    statement.Accept(this);
                }
            }
            finally
            {
                CurrentScope = previousScope;
            }

            ModuleValue moduleValue = new ModuleValue(node.ModuleName, moduleScope, exportedNames);
            Modules[node.ModuleName] = moduleValue;
            CurrentScope.Define(node.ModuleName, moduleValue);
        }

        public void Visit(ModuleImportNode node)
        {
            // Handled by ModuleDefNode visitor.
        }

        public void Visit(ImportStatementNode node)
        {
            ProcessModuleImport(node.ModuleName, node.Alias, node.Line);
        }

        public void Visit(ExportStatementNode node)
        {
            // Export is recorded during module execution — nothing to do here
            // as the module visitor already collected it.
        }

        public void Visit(TryStatementNode node)
        {
            try
            {
                Scope tryScope = CurrentScope.CreateChild();
                ExecuteStatements(node.TryBody, tryScope);
            }
            catch (ThrownException thrownException)
            {
                Scope catchScope = CurrentScope.CreateChild();
                catchScope.Define(node.CatchClause.ExceptionVariable, thrownException.ThrownValue);
                ExecuteStatements(node.CatchClause.Body, catchScope);
            }
            catch (InterpreterException interpreterException)
            {
                Scope catchScope = CurrentScope.CreateChild();
                catchScope.Define(node.CatchClause.ExceptionVariable, interpreterException.Message);
                ExecuteStatements(node.CatchClause.Body, catchScope);
            }
            finally
            {
                if (node.FinallyBody != null)
                {
                    Scope finallyScope = CurrentScope.CreateChild();
                    ExecuteStatements(node.FinallyBody, finallyScope);
                }
            }
        }

        public void Visit(CatchClauseNode node)
        {
            // Handled by TryStatementNode visitor.
        }

        public void Visit(ThrowStatementNode node)
        {
            object value = Evaluate(node.ThrownExpression);
            throw new ThrownException(value, node.Line);
        }

        public void Visit(PatternMatchNode node)
        {
            object subject = Evaluate(node.SubjectExpression);
            foreach (PatternCaseNode caseNode in node.Cases)
            {
                Scope matchScope = CurrentScope.CreateChild();
                if (MatchPattern(caseNode.Pattern, subject, matchScope, caseNode.Line))
                {
                    // Check guard if present.
                    if (caseNode.WhenGuard != null)
                    {
                        Scope previousScope = CurrentScope;
                        CurrentScope = matchScope;
                        object guardResult = Evaluate(caseNode.WhenGuard);
                        CurrentScope = previousScope;
                        if (!IsTruthy(guardResult))
                        {
                            continue;
                        }
                    }
                    ExecuteStatements(caseNode.Body, matchScope);
                    return;
                }
            }
            // No pattern matched — do nothing per spec.
        }

        public void Visit(PatternCaseNode node)
        {
            // Handled by PatternMatchNode visitor.
        }

        public void Visit(AnnotatedStatementNode node)
        {
            // Annotations are metadata only — execute the inner statement normally.
            node.InnerStatement.Accept(this);
        }

        public void Visit(AnnotationNode node)
        {
            // Annotations are metadata — no runtime effect.
        }

        public void Visit(AnnotationParamNode node)
        {
            // Annotations are metadata — no runtime effect.
        }

        public void Visit(ArrayElementAssignNode node)
        {
            // Spec note 7: mutate the existing list in-place.
            object arrayValue = CurrentScope.Lookup(node.ArrayName, node.Line);
            if (arrayValue is List<object> list)
            {
                object indexValue = Evaluate(node.IndexExpression);
                if (indexValue is long indexLong)
                {
                    int index = (int)indexLong;
                    if (index < 0 || index >= list.Count)
                    {
                        throw new InterpreterException(
                            $"Array index {index} out of bounds (length {list.Count})", node.Line);
                    }
                    object value = Evaluate(node.ValueExpression);
                    list[index] = value;
                    LastResult = value;
                }
                else
                {
                    throw new InterpreterException("Array index must be an integer", node.Line);
                }
            }
            else
            {
                throw new InterpreterException(
                    $"Cannot index into non-array value '{node.ArrayName}'", node.Line);
            }
        }

        // ----------------------------------------------------------------
        // Expression nodes
        // ----------------------------------------------------------------

        public void Visit(BinaryOpNode node)
        {
            string op = node.Operator;

            // Short-circuit logical operators.
            if (op == "||" || op == "or")
            {
                object left = Evaluate(node.Left);
                if (IsTruthy(left))
                {
                    LastResult = true;
                    return;
                }
                object right = Evaluate(node.Right);
                LastResult = IsTruthy(right);
                return;
            }
            if (op == "&&" || op == "and")
            {
                object left = Evaluate(node.Left);
                if (!IsTruthy(left))
                {
                    LastResult = false;
                    return;
                }
                object right = Evaluate(node.Right);
                LastResult = IsTruthy(right);
                return;
            }

            // Evaluate both operands.
            object leftValue = Evaluate(node.Left);
            object rightValue = Evaluate(node.Right);

            // String concatenation with + or &.
            if (op == "&")
            {
                LastResult = Stringify(leftValue) + Stringify(rightValue);
                return;
            }
            if (op == "+")
            {
                if (leftValue is string || rightValue is string)
                {
                    LastResult = Stringify(leftValue) + Stringify(rightValue);
                    return;
                }
            }

            // Equality operators — work on any type.
            if (op == "==")
            {
                LastResult = AreEqual(leftValue, rightValue);
                return;
            }
            if (op == "!=")
            {
                LastResult = !AreEqual(leftValue, rightValue);
                return;
            }

            // Numeric operations.
            if (IsNumeric(leftValue) && IsNumeric(rightValue))
            {
                EvaluateNumericBinaryOp(op, leftValue, rightValue, node.Line);
                return;
            }

            // Comparison operators on strings.
            if (leftValue is string leftStr && rightValue is string rightStr)
            {
                int comparison = string.Compare(leftStr, rightStr, StringComparison.Ordinal);
                switch (op)
                {
                    case "<":
                        LastResult = comparison < 0;
                        return;
                    case ">":
                        LastResult = comparison > 0;
                        return;
                    case "<=":
                        LastResult = comparison <= 0;
                        return;
                    case ">=":
                        LastResult = comparison >= 0;
                        return;
                }
            }

            // Boolean operators without short-circuit (& on bools).
            if (leftValue is bool leftBool && rightValue is bool rightBool)
            {
                if (op == "+")
                {
                    throw new InterpreterException("Cannot apply '+' to boolean values", node.Line);
                }
            }

            throw new InterpreterException(
                $"Cannot apply operator '{op}' to values of type " +
                $"'{GetTypeName(leftValue)}' and '{GetTypeName(rightValue)}'",
                node.Line);
        }

        private void EvaluateNumericBinaryOp(string op, object leftValue, object rightValue, int line)
        {
            bool leftIsLong = leftValue is long;
            bool rightIsLong = rightValue is long;

            switch (op)
            {
                case "+":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue + (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) + ConvertToDouble(rightValue, line);
                    }
                    return;

                case "-":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue - (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) - ConvertToDouble(rightValue, line);
                    }
                    return;

                case "*":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue * (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) * ConvertToDouble(rightValue, line);
                    }
                    return;

                case "/":
                    {
                        double leftDouble = ConvertToDouble(leftValue, line);
                        double rightDouble = ConvertToDouble(rightValue, line);
                        if (rightDouble == 0.0)
                        {
                            throw new InterpreterException("Division by zero", line);
                        }
                        double result = leftDouble / rightDouble;
                        // int / int -> int when result is whole, float otherwise.
                        if (leftIsLong && rightIsLong && result == Math.Floor(result))
                        {
                            LastResult = (long)result;
                        }
                        else
                        {
                            LastResult = result;
                        }
                        return;
                    }

                case "//":
                    // Floor division.
                    if (leftIsLong && rightIsLong)
                    {
                        long leftLong = (long)leftValue;
                        long rightLong = (long)rightValue;
                        if (rightLong == 0)
                        {
                            throw new InterpreterException("Division by zero", line);
                        }
                        // Floor division for integers: Math.DivRem gives truncated result;
                        // need to adjust for floor behavior with negative results.
                        long quotient = leftLong / rightLong;
                        long remainder = leftLong % rightLong;
                        if (remainder != 0 && ((leftLong ^ rightLong) < 0))
                        {
                            quotient--;
                        }
                        LastResult = quotient;
                    }
                    else
                    {
                        double leftDouble = ConvertToDouble(leftValue, line);
                        double rightDouble = ConvertToDouble(rightValue, line);
                        if (rightDouble == 0.0)
                        {
                            throw new InterpreterException("Division by zero", line);
                        }
                        LastResult = Math.Floor(leftDouble / rightDouble);
                    }
                    return;

                case "%":
                    // Modulo — integers only per spec.
                    if (leftIsLong && rightIsLong)
                    {
                        long rightLong = (long)rightValue;
                        if (rightLong == 0)
                        {
                            throw new InterpreterException("Division by zero", line);
                        }
                        LastResult = (long)leftValue % rightLong;
                    }
                    else
                    {
                        throw new InterpreterException("Modulo operator '%' requires integer operands", line);
                    }
                    return;

                case "**":
                    // Exponentiation.
                    if (leftIsLong && rightIsLong)
                    {
                        long exponent = (long)rightValue;
                        if (exponent >= 0)
                        {
                            LastResult = IntPower((long)leftValue, exponent);
                        }
                        else
                        {
                            LastResult = Math.Pow(ConvertToDouble(leftValue, line), ConvertToDouble(rightValue, line));
                        }
                    }
                    else
                    {
                        LastResult = Math.Pow(ConvertToDouble(leftValue, line), ConvertToDouble(rightValue, line));
                    }
                    return;

                case "<":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue < (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) < ConvertToDouble(rightValue, line);
                    }
                    return;

                case ">":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue > (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) > ConvertToDouble(rightValue, line);
                    }
                    return;

                case "<=":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue <= (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) <= ConvertToDouble(rightValue, line);
                    }
                    return;

                case ">=":
                    if (leftIsLong && rightIsLong)
                    {
                        LastResult = (long)leftValue >= (long)rightValue;
                    }
                    else
                    {
                        LastResult = ConvertToDouble(leftValue, line) >= ConvertToDouble(rightValue, line);
                    }
                    return;

                default:
                    throw new InterpreterException($"Unknown operator '{op}'", line);
            }
        }

        public void Visit(UnaryOpNode node)
        {
            object operand = Evaluate(node.Operand);
            switch (node.Operator)
            {
                case "-":
                    if (operand is long longValue)
                    {
                        LastResult = -longValue;
                    }
                    else if (operand is double doubleValue)
                    {
                        LastResult = -doubleValue;
                    }
                    else
                    {
                        throw new InterpreterException(
                            $"Cannot negate value of type '{GetTypeName(operand)}'", node.Line);
                    }
                    return;

                case "not":
                    LastResult = !IsTruthy(operand);
                    return;

                default:
                    throw new InterpreterException($"Unknown unary operator '{node.Operator}'", node.Line);
            }
        }

        public void Visit(ConditionalExprNode node)
        {
            object condition = Evaluate(node.Condition);
            if (IsTruthy(condition))
            {
                LastResult = Evaluate(node.ThenExpression);
            }
            else
            {
                LastResult = Evaluate(node.ElseExpression);
            }
        }

        public void Visit(IdentifierNode node)
        {
            LastResult = CurrentScope.Lookup(node.Name, node.Line);
        }

        public void Visit(IntegerLiteralNode node)
        {
            LastResult = node.Value;
        }

        public void Visit(FloatLiteralNode node)
        {
            LastResult = node.Value;
        }

        public void Visit(StringLiteralNode node)
        {
            LastResult = node.Value;
        }

        public void Visit(BoolLiteralNode node)
        {
            LastResult = node.Value;
        }

        public void Visit(NullLiteralNode node)
        {
            LastResult = null;
        }

        public void Visit(ArrayLiteralNode node)
        {
            List<object> elements = new List<object>();
            foreach (AstNode element in node.Elements)
            {
                elements.Add(Evaluate(element));
            }
            LastResult = elements;
        }

        public void Visit(IndexAccessNode node)
        {
            object target = Evaluate(node.TargetExpression);
            object index = Evaluate(node.IndexExpression);

            if (target is List<object> list)
            {
                if (index is long longIndex)
                {
                    int intIndex = (int)longIndex;
                    if (intIndex < 0 || intIndex >= list.Count)
                    {
                        throw new InterpreterException(
                            $"Array index {intIndex} out of bounds (length {list.Count})", node.Line);
                    }
                    LastResult = list[intIndex];
                }
                else
                {
                    throw new InterpreterException("Array index must be an integer", node.Line);
                }
            }
            else if (target is string stringTarget)
            {
                if (index is long longIndex)
                {
                    int intIndex = (int)longIndex;
                    if (intIndex < 0 || intIndex >= stringTarget.Length)
                    {
                        throw new InterpreterException(
                            $"String index {intIndex} out of bounds (length {stringTarget.Length})", node.Line);
                    }
                    LastResult = stringTarget[intIndex].ToString();
                }
                else
                {
                    throw new InterpreterException("String index must be an integer", node.Line);
                }
            }
            else
            {
                throw new InterpreterException(
                    $"Cannot index into value of type '{GetTypeName(target)}'", node.Line);
            }
        }

        public void Visit(MemberAccessNode node)
        {
            object target = Evaluate(node.TargetExpression);
            LastResult = GetMember(target, node.MemberName, node.Line);
        }

        public void Visit(MethodCallNode node)
        {
            object target = Evaluate(node.TargetExpression);
            List<object> arguments = EvaluateArguments(node.Arguments);
            LastResult = CallMethod(target, node.MethodName, arguments, node.Line);
        }

        public void Visit(FunctionCallNode node)
        {
            object callee = Evaluate(node.CalleeExpression);
            List<object> arguments = EvaluateArguments(node.Arguments);
            LastResult = CallCallable(callee, arguments, node.Line);
        }

        public void Visit(NewExprNode node)
        {
            object classDef = CurrentScope.Lookup(node.ClassName, node.Line);
            if (classDef is ClassValue classValue)
            {
                ClassInstance instance = new ClassInstance(classValue);
                List<object> constructorArguments = EvaluateArguments(node.Arguments);

                // Initialize fields and find constructor.
                ConstructorDefNode constructorDef = null;
                Scope instanceScope = GlobalScope.CreateChild();
                instanceScope.Define("this", instance);

                foreach (AstNode member in classValue.Members)
                {
                    if (member is FieldDeclareNode fieldNode)
                    {
                        Scope previousScope = CurrentScope;
                        CurrentScope = instanceScope;
                        object fieldValue = Evaluate(fieldNode.InitialiserExpression);
                        CurrentScope = previousScope;
                        instance.Fields[fieldNode.FieldName] = fieldValue;
                    }
                    else if (member is FunctionDefNode methodNode)
                    {
                        FunctionValue method = new FunctionValue(
                            methodNode.FunctionName,
                            methodNode.Parameters,
                            methodNode.Body,
                            GlobalScope);
                        instance.Fields[methodNode.FunctionName] = method;
                    }
                    else if (member is ConstructorDefNode ctorNode)
                    {
                        constructorDef = ctorNode;
                    }
                    else if (member is ConstDeclareNode constNode)
                    {
                        Scope previousScope = CurrentScope;
                        CurrentScope = instanceScope;
                        object constValue = Evaluate(constNode.InitialiserExpression);
                        CurrentScope = previousScope;
                        instance.Fields[constNode.ConstantName] = constValue;
                    }
                }

                // Execute constructor if present.
                if (constructorDef != null)
                {
                    CallDepth++;
                    if (CallDepth > MaxCallDepth)
                    {
                        throw new InterpreterException(
                            "Stack overflow: maximum call depth of 500 exceeded", node.Line);
                    }
                    try
                    {
                        Scope constructorScope = GlobalScope.CreateChild();
                        constructorScope.Define("this", instance);

                        for (int index = 0; index < constructorDef.Parameters.Count; index++)
                        {
                            ParameterNode parameter = constructorDef.Parameters[index];
                            if (index < constructorArguments.Count)
                            {
                                constructorScope.Define(parameter.ParameterName, constructorArguments[index]);
                            }
                            else if (parameter.DefaultExpression != null)
                            {
                                object defaultValue = Evaluate(parameter.DefaultExpression);
                                constructorScope.Define(parameter.ParameterName, defaultValue);
                            }
                            else
                            {
                                throw new InterpreterException(
                                    $"Missing argument for constructor parameter '{parameter.ParameterName}'",
                                    node.Line);
                            }
                        }

                        Scope previousScope = CurrentScope;
                        CurrentScope = constructorScope;
                        try
                        {
                            foreach (AstNode statement in constructorDef.Body)
                            {
                                statement.Accept(this);
                            }
                        }
                        catch (ReturnException)
                        {
                            // Constructor may use bare return to exit early.
                        }
                        finally
                        {
                            CurrentScope = previousScope;
                        }
                    }
                    finally
                    {
                        CallDepth--;
                    }
                }

                LastResult = instance;
            }
            else
            {
                throw new InterpreterException($"'{node.ClassName}' is not a class", node.Line);
            }
        }

        public void Visit(CastExprNode node)
        {
            object operand = Evaluate(node.Operand);
            string targetType = node.TargetType.TypeName;

            switch (targetType)
            {
                case "int":
                    if (operand is long)
                    {
                        LastResult = operand;
                    }
                    else if (operand is double doubleVal)
                    {
                        LastResult = (long)doubleVal;
                    }
                    else if (operand is bool boolVal)
                    {
                        LastResult = boolVal ? 1L : 0L;
                    }
                    else if (operand is string strVal)
                    {
                        if (long.TryParse(strVal, out long parsed))
                        {
                            LastResult = parsed;
                        }
                        else
                        {
                            throw new InterpreterException(
                                $"Cannot cast string '{strVal}' to int", node.Line);
                        }
                    }
                    else
                    {
                        throw new InterpreterException(
                            $"Cannot cast {GetTypeName(operand)} to int", node.Line);
                    }
                    return;

                case "float":
                    if (operand is double)
                    {
                        LastResult = operand;
                    }
                    else if (operand is long longVal)
                    {
                        LastResult = (double)longVal;
                    }
                    else if (operand is string strVal2)
                    {
                        if (double.TryParse(strVal2, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out double parsed))
                        {
                            LastResult = parsed;
                        }
                        else
                        {
                            throw new InterpreterException(
                                $"Cannot cast string '{strVal2}' to float", node.Line);
                        }
                    }
                    else
                    {
                        throw new InterpreterException(
                            $"Cannot cast {GetTypeName(operand)} to float", node.Line);
                    }
                    return;

                case "string":
                    LastResult = Stringify(operand);
                    return;

                case "bool":
                    LastResult = IsTruthy(operand);
                    return;

                default:
                    throw new InterpreterException(
                        $"Cannot cast to type '{targetType}'", node.Line);
            }
        }

        public void Visit(TypeCheckNode node)
        {
            object subject = Evaluate(node.SubjectExpression);
            LastResult = CheckType(subject, node.CheckedType);
        }

        public void Visit(TypeAssertNode node)
        {
            object subject = Evaluate(node.SubjectExpression);
            if (!CheckType(subject, node.AssertedType))
            {
                throw new InterpreterException(
                    $"Type assertion failed: value is {GetTypeName(subject)}, expected {node.AssertedType.TypeName}",
                    node.Line);
            }
            LastResult = subject;
        }

        public void Visit(LambdaExprNode node)
        {
            // Build a function value. Lambdas see only global scope (no closure capture).
            List<AstNode> body;
            if (node.IsBlockBody)
            {
                body = node.StatementBody;
            }
            else
            {
                // Wrap expression body in a return statement for uniform handling.
                body = new List<AstNode>
                {
                    new ReturnStatementNode(node.ExpressionBody, node.Line)
                };
            }

            FunctionValue lambda = new FunctionValue(null, node.Parameters, body, GlobalScope);
            LastResult = lambda;
        }

        public void Visit(ListComprehensionNode node)
        {
            object source = Evaluate(node.SourceExpression);
            if (source == null)
            {
                throw new InterpreterException("Cannot iterate over null in list comprehension", node.Line);
            }

            List<object> result = new List<object>();

            if (source is List<object> sourceList)
            {
                foreach (object element in sourceList)
                {
                    Scope iterScope = CurrentScope.CreateChild();
                    iterScope.Define(node.LoopVariable, element);
                    Scope previousScope = CurrentScope;
                    CurrentScope = iterScope;
                    result.Add(Evaluate(node.ElementExpression));
                    CurrentScope = previousScope;
                }
            }
            else if (source is string sourceString)
            {
                foreach (char character in sourceString)
                {
                    Scope iterScope = CurrentScope.CreateChild();
                    iterScope.Define(node.LoopVariable, character.ToString());
                    Scope previousScope = CurrentScope;
                    CurrentScope = iterScope;
                    result.Add(Evaluate(node.ElementExpression));
                    CurrentScope = previousScope;
                }
            }
            else
            {
                throw new InterpreterException(
                    $"Cannot iterate over value of type '{GetTypeName(source)}' in list comprehension",
                    node.Line);
            }

            LastResult = result;
        }

        // ----------------------------------------------------------------
        // Pattern nodes — these are dispatched via MatchPattern, not directly.
        // ----------------------------------------------------------------

        public void Visit(WildcardPatternNode node)
        {
            // Handled by MatchPattern.
        }

        public void Visit(ConstructorPatternNode node)
        {
            // Handled by MatchPattern.
        }

        public void Visit(ArrayPatternNode node)
        {
            // Handled by MatchPattern.
        }

        public void Visit(FieldPatternNode node)
        {
            // Handled by MatchPattern.
        }

        public void Visit(FieldPatternEntryNode node)
        {
            // Handled by MatchPattern.
        }

        public void Visit(AlternationPatternNode node)
        {
            // Handled by MatchPattern.
        }

        // ----------------------------------------------------------------
        // Support nodes
        // ----------------------------------------------------------------

        public void Visit(ParameterNode node)
        {
            // Parameters are handled by function call logic.
        }

        public void Visit(TypeNode node)
        {
            // Type nodes are inspected directly, not visited for side effects.
        }

        // ================================================================
        // Private helper methods
        // ================================================================

        // Evaluate a list of argument expressions and return their values.
        private List<object> EvaluateArguments(List<AstNode> argumentNodes)
        {
            List<object> arguments = new List<object>();
            foreach (AstNode argumentNode in argumentNodes)
            {
                arguments.Add(Evaluate(argumentNode));
            }
            return arguments;
        }

        // Call any callable value (FunctionValue or BuiltInFunction).
        private object CallCallable(object callee, List<object> arguments, int line)
        {
            if (callee is FunctionValue function)
            {
                return CallFunction(function, arguments, line);
            }
            if (callee is BuiltInFunction builtIn)
            {
                if (arguments.Count != builtIn.Arity)
                {
                    throw new InterpreterException(
                        $"Built-in function '{builtIn.Name}' expects {builtIn.Arity} argument(s), got {arguments.Count}",
                        line);
                }
                return builtIn.Implementation(arguments, line);
            }
            throw new InterpreterException(
                $"Value of type '{GetTypeName(callee)}' is not callable", line);
        }

        // Call a method on a target object.
        private object CallMethod(object target, string methodName, List<object> arguments, int line)
        {
            if (target is ClassInstance instance)
            {
                if (instance.Fields.TryGetValue(methodName, out object memberValue))
                {
                    if (memberValue is FunctionValue method)
                    {
                        // Create method scope with "this" bound.
                        CallDepth++;
                        if (CallDepth > MaxCallDepth)
                        {
                            throw new InterpreterException(
                                "Stack overflow: maximum call depth of 500 exceeded", line);
                        }
                        try
                        {
                            Scope methodScope = GlobalScope.CreateChild();
                            methodScope.Define("this", instance);

                            for (int index = 0; index < method.Parameters.Count; index++)
                            {
                                ParameterNode parameter = method.Parameters[index];
                                if (index < arguments.Count)
                                {
                                    methodScope.Define(parameter.ParameterName, arguments[index]);
                                }
                                else if (parameter.DefaultExpression != null)
                                {
                                    object defaultValue = Evaluate(parameter.DefaultExpression);
                                    methodScope.Define(parameter.ParameterName, defaultValue);
                                }
                                else
                                {
                                    throw new InterpreterException(
                                        $"Missing argument for parameter '{parameter.ParameterName}'", line);
                                }
                            }

                            Scope previousScope = CurrentScope;
                            CurrentScope = methodScope;
                            try
                            {
                                foreach (AstNode statement in method.Body)
                                {
                                    statement.Accept(this);
                                }
                            }
                            catch (ReturnException returnException)
                            {
                                return returnException.Value;
                            }
                            finally
                            {
                                CurrentScope = previousScope;
                            }
                            return null;
                        }
                        finally
                        {
                            CallDepth--;
                        }
                    }
                }
                throw new InterpreterException(
                    $"Object of class '{instance.ClassDefinition.Name}' has no method '{methodName}'", line);
            }

            // Built-in methods on arrays.
            if (target is List<object> list)
            {
                return CallListMethod(list, methodName, arguments, line);
            }

            // Built-in methods on strings.
            if (target is string stringTarget)
            {
                return CallStringMethod(stringTarget, methodName, arguments, line);
            }

            // Module member access for method calls.
            if (target is ModuleValue moduleValue)
            {
                object member = moduleValue.ModuleScope.Lookup(methodName, line);
                return CallCallable(member, arguments, line);
            }

            throw new InterpreterException(
                $"Cannot call method '{methodName}' on value of type '{GetTypeName(target)}'", line);
        }

        // Get a member (field/property) from a target object.
        private object GetMember(object target, string memberName, int line)
        {
            if (target is ClassInstance instance)
            {
                if (instance.Fields.TryGetValue(memberName, out object fieldValue))
                {
                    return fieldValue;
                }
                throw new InterpreterException(
                    $"Object of class '{instance.ClassDefinition.Name}' has no field '{memberName}'", line);
            }

            if (target is EnumValue enumValue)
            {
                if (enumValue.Members.TryGetValue(memberName, out long enumMemberValue))
                {
                    return enumMemberValue;
                }
                throw new InterpreterException(
                    $"Enum '{enumValue.Name}' has no member '{memberName}'", line);
            }

            if (target is ModuleValue moduleValue)
            {
                return moduleValue.ModuleScope.Lookup(memberName, line);
            }

            // String length property.
            if (target is string stringTarget && memberName == "length")
            {
                return (long)stringTarget.Length;
            }

            // Array length property.
            if (target is List<object> list && memberName == "length")
            {
                return (long)list.Count;
            }

            throw new InterpreterException(
                $"Cannot access member '{memberName}' on value of type '{GetTypeName(target)}'", line);
        }

        // Check if a value matches a type.
        private bool CheckType(object value, TypeNode typeNode)
        {
            string typeName = typeNode.TypeName;
            switch (typeName)
            {
                case "int":
                    return value is long;
                case "float":
                    return value is double;
                case "string":
                    return value is string;
                case "bool":
                    return value is bool;
                case "array":
                    return value is List<object>;
                case "null":
                    return value == null;
                case "object":
                    return value != null;
                default:
                    // Check for class instance type.
                    if (value is ClassInstance instance)
                    {
                        return instance.ClassDefinition.Name == typeName;
                    }
                    return false;
            }
        }

        // Pattern matching: returns true if the pattern matches, binding variables in matchScope.
        private bool MatchPattern(AstNode pattern, object value, Scope matchScope, int line)
        {
            if (pattern is WildcardPatternNode)
            {
                return true;
            }
            if (pattern is IntegerLiteralNode intLiteral)
            {
                return value is long longValue && longValue == intLiteral.Value;
            }
            if (pattern is FloatLiteralNode floatLiteral)
            {
                return value is double doubleValue && doubleValue == floatLiteral.Value;
            }
            if (pattern is StringLiteralNode stringLiteral)
            {
                return value is string stringValue && stringValue == stringLiteral.Value;
            }
            if (pattern is BoolLiteralNode boolLiteral)
            {
                return value is bool boolValue && boolValue == boolLiteral.Value;
            }
            if (pattern is NullLiteralNode)
            {
                return value == null;
            }
            if (pattern is IdentifierNode identifierPattern)
            {
                // Identifier pattern binds the value to the variable name.
                matchScope.Define(identifierPattern.Name, value);
                return true;
            }
            if (pattern is ConstructorPatternNode constructorPattern)
            {
                if (value is ClassInstance instance)
                {
                    if (instance.ClassDefinition.Name != constructorPattern.ConstructorName)
                    {
                        return false;
                    }
                    // Sub-patterns are not standard — match by position is not well-defined
                    // for classes. Accept if class name matches and no sub-patterns.
                    if (constructorPattern.SubPatterns.Count == 0)
                    {
                        return true;
                    }
                    // For sub-patterns, try matching against constructor arguments
                    // (not well-defined in spec, but best effort).
                    return true;
                }
                return false;
            }
            if (pattern is ArrayPatternNode arrayPattern)
            {
                if (value is List<object> list)
                {
                    if (list.Count != arrayPattern.ElementPatterns.Count)
                    {
                        return false;
                    }
                    for (int index = 0; index < arrayPattern.ElementPatterns.Count; index++)
                    {
                        if (!MatchPattern(arrayPattern.ElementPatterns[index], list[index], matchScope, line))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            if (pattern is FieldPatternNode fieldPattern)
            {
                if (value is ClassInstance fieldInstance)
                {
                    if (fieldPattern.TypeName != null && fieldInstance.ClassDefinition.Name != fieldPattern.TypeName)
                    {
                        return false;
                    }
                    foreach (FieldPatternEntryNode entry in fieldPattern.FieldPatterns)
                    {
                        if (!fieldInstance.Fields.TryGetValue(entry.FieldName, out object fieldValue))
                        {
                            return false;
                        }
                        if (!MatchPattern(entry.FieldPattern, fieldValue, matchScope, line))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            if (pattern is AlternationPatternNode alternationPattern)
            {
                // Try left first, then right. First match wins.
                if (MatchPattern(alternationPattern.LeftPattern, value, matchScope, line))
                {
                    return true;
                }
                return MatchPattern(alternationPattern.RightPattern, value, matchScope, line);
            }
            return false;
        }

        // Check if two values are equal.
        private static bool AreEqual(object left, object right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            if (left == null || right == null)
            {
                return false;
            }
            // Numeric comparison with promotion.
            if (IsNumeric(left) && IsNumeric(right))
            {
                if (left is long leftLong && right is long rightLong)
                {
                    return leftLong == rightLong;
                }
                double leftDouble = left is long l ? (double)l : (double)left;
                double rightDouble = right is long r ? (double)r : (double)right;
                return leftDouble == rightDouble;
            }
            return left.Equals(right);
        }

        // Check if a value is numeric (long or double).
        private static bool IsNumeric(object value)
        {
            return value is long || value is double;
        }

        // Convert a value to double for arithmetic.
        private static double ConvertToDouble(object value, int line)
        {
            if (value is long longValue)
            {
                return (double)longValue;
            }
            if (value is double doubleValue)
            {
                return doubleValue;
            }
            throw new InterpreterException(
                $"Expected numeric value, got {GetTypeName(value)}", line);
        }

        // Integer exponentiation for non-negative exponents.
        private static long IntPower(long baseValue, long exponent)
        {
            long result = 1;
            long currentBase = baseValue;
            long currentExponent = exponent;
            while (currentExponent > 0)
            {
                if ((currentExponent & 1) == 1)
                {
                    result *= currentBase;
                }
                currentBase *= currentBase;
                currentExponent >>= 1;
            }
            return result;
        }

        // Get the TinyLanguage type name for a value.
        private static string GetTypeName(object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is long)
            {
                return "int";
            }
            if (value is double)
            {
                return "float";
            }
            if (value is string)
            {
                return "string";
            }
            if (value is bool)
            {
                return "bool";
            }
            if (value is List<object>)
            {
                return "array";
            }
            if (value is FunctionValue)
            {
                return "function";
            }
            if (value is ClassInstance instance)
            {
                return instance.ClassDefinition.Name;
            }
            if (value is ClassValue)
            {
                return "class";
            }
            if (value is EnumValue)
            {
                return "enum";
            }
            if (value is ModuleValue)
            {
                return "module";
            }
            return value.GetType().Name;
        }

        // Process a module import: look up the module and bind its exports.
        private void ProcessModuleImport(string moduleName, string alias, int line)
        {
            if (Modules.TryGetValue(moduleName, out ModuleValue moduleValue))
            {
                string bindingName = alias != null ? alias : moduleName;
                CurrentScope.Define(bindingName, moduleValue);
            }
            else
            {
                throw new InterpreterException($"Module '{moduleName}' not found", line);
            }
        }

        // Built-in method calls on List<object>.
        private object CallListMethod(List<object> list, string methodName, List<object> arguments, int line)
        {
            switch (methodName)
            {
                case "push":
                    if (arguments.Count != 1)
                    {
                        throw new InterpreterException("push() expects 1 argument", line);
                    }
                    list.Add(arguments[0]);
                    return null;

                case "pop":
                    if (list.Count == 0)
                    {
                        throw new InterpreterException("Cannot pop from empty array", line);
                    }
                    object removed = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    return removed;

                case "length":
                    return (long)list.Count;

                default:
                    throw new InterpreterException(
                        $"Array has no method '{methodName}'", line);
            }
        }

        // Built-in method calls on string.
        private static object CallStringMethod(string target, string methodName, List<object> arguments, int line)
        {
            switch (methodName)
            {
                case "length":
                    return (long)target.Length;

                case "substring":
                    if (arguments.Count < 1 || arguments.Count > 2)
                    {
                        throw new InterpreterException("substring() expects 1 or 2 arguments", line);
                    }
                    if (arguments[0] is long startIndex)
                    {
                        int start = (int)startIndex;
                        if (arguments.Count == 2 && arguments[1] is long lengthArg)
                        {
                            return target.Substring(start, (int)lengthArg);
                        }
                        return target.Substring(start);
                    }
                    throw new InterpreterException("substring() expects integer arguments", line);

                case "indexOf":
                    if (arguments.Count != 1)
                    {
                        throw new InterpreterException("indexOf() expects 1 argument", line);
                    }
                    if (arguments[0] is string searchStr)
                    {
                        return (long)target.IndexOf(searchStr, StringComparison.Ordinal);
                    }
                    throw new InterpreterException("indexOf() expects a string argument", line);

                case "toUpper":
                    return target.ToUpper();

                case "toLower":
                    return target.ToLower();

                case "trim":
                    return target.Trim();

                case "split":
                    if (arguments.Count != 1)
                    {
                        throw new InterpreterException("split() expects 1 argument", line);
                    }
                    if (arguments[0] is string delimiter)
                    {
                        string[] parts = target.Split(new string[] { delimiter }, StringSplitOptions.None);
                        List<object> result = new List<object>();
                        foreach (string part in parts)
                        {
                            result.Add(part);
                        }
                        return result;
                    }
                    throw new InterpreterException("split() expects a string argument", line);

                default:
                    throw new InterpreterException(
                        $"String has no method '{methodName}'", line);
            }
        }

        // ================================================================
        // Built-in function implementations
        // ================================================================

        private object EvalLen(List<object> arguments, int line)
        {
            object value = arguments[0];
            if (value is string stringValue)
            {
                return (long)stringValue.Length;
            }
            if (value is List<object> listValue)
            {
                return (long)listValue.Count;
            }
            throw new InterpreterException(
                $"len() expects a string or array, got {GetTypeName(value)}", line);
        }

        private object EvalStr(List<object> arguments, int line)
        {
            return Stringify(arguments[0]);
        }

        private object EvalInt(List<object> arguments, int line)
        {
            object value = arguments[0];
            if (value is long)
            {
                return value;
            }
            if (value is double doubleValue)
            {
                return (long)doubleValue;
            }
            if (value is bool boolValue)
            {
                return boolValue ? 1L : 0L;
            }
            if (value is string stringValue)
            {
                if (long.TryParse(stringValue, out long parsed))
                {
                    return parsed;
                }
                throw new InterpreterException(
                    $"Cannot convert string '{stringValue}' to int", line);
            }
            throw new InterpreterException(
                $"Cannot convert {GetTypeName(value)} to int", line);
        }

        private object EvalBool(List<object> arguments, int line)
        {
            return IsTruthy(arguments[0]);
        }
    }
}
