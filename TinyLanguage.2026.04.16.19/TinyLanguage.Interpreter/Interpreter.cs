using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TinyLanguage.Lexer;
using TinyLanguage.Lexer.Nodes;

namespace TinyLanguage.Interpreter;

// Tree-walking interpreter for TinyLanguage.
//
// Implements IInterpreter (public entry points) and INodeVisitor (visitor
// dispatch for every AST node). Evaluation works by:
//   * expression nodes — Visit stores the produced value in LastValue
//   * statement nodes — Visit executes the effect; LastValue is cleared
//
// Flow-control (break / continue / return / throw) is implemented with
// internal exception signals caught at the appropriate enclosing construct.
public sealed class Interpreter : IInterpreter, INodeVisitor
{
    // ─── Runtime state ────────────────────────────────────────────────────
    private readonly Scope GlobalScope;
    private Scope CurrentScope;
    private object LastValue;
    private int CallDepth;
    private const int MaxCallDepth = 500;

    // Registered modules (by name) for import resolution.
    private readonly Dictionary<string, TinyLanguageModule> Modules;

    public Interpreter()
    {
        GlobalScope = new Scope();
        CurrentScope = GlobalScope;
        LastValue = TinyLanguageNull.Instance;
        CallDepth = 0;
        Modules = new Dictionary<string, TinyLanguageModule>();
    }

    // ─── IInterpreter ─────────────────────────────────────────────────────

    public void Execute(ProgramNode program)
    {
        program.Accept(this);
    }

    public object Evaluate(AstNode expression)
    {
        expression.Accept(this);
        return LastValue;
    }

    // ─── Helpers ──────────────────────────────────────────────────────────

    // Evaluates an AST node and returns its produced value. Used internally
    // wherever a child expression must be reduced before the parent acts.
    private object EvaluateChild(AstNode node)
    {
        node.Accept(this);
        return LastValue;
    }

    // Executes a list of statements inside a freshly created child scope.
    // Used by block-style constructs (if / while / for / foreach / etc.)
    // so that variables declared inside the block do not leak to the parent.
    private void ExecuteBlock(List<AstNode> statements, Scope blockScope)
    {
        Scope previous = CurrentScope;
        CurrentScope = blockScope;
        try
        {
            for (int index = 0; index < statements.Count; index = index + 1)
            {
                statements[index].Accept(this);
            }
        }
        finally
        {
            CurrentScope = previous;
        }
    }

    // Applies the language's truthiness rules. See spec: null / 0 / 0.0 /
    // "" / empty array are falsy; everything else is truthy.
    private static bool IsTruthy(object value)
    {
        if (value == null || value is TinyLanguageNull)
        {
            return false;
        }
        if (value is bool boolValue)
        {
            return boolValue;
        }
        if (value is long longValue)
        {
            return longValue != 0L;
        }
        if (value is double doubleValue)
        {
            return doubleValue != 0.0;
        }
        if (value is string stringValue)
        {
            return stringValue.Length != 0;
        }
        if (value is List<object> listValue)
        {
            return listValue.Count != 0;
        }
        return true;
    }

    // Renders any runtime value as a string, matching the language's
    // built-in str() behaviour (also used by print and string concatenation).
    private static string ValueToString(object value)
    {
        if (value == null || value is TinyLanguageNull)
        {
            return "null";
        }
        if (value is bool boolValue)
        {
            return boolValue ? "true" : "false";
        }
        if (value is long longValue)
        {
            return longValue.ToString(CultureInfo.InvariantCulture);
        }
        if (value is double doubleValue)
        {
            // Mirror C#'s round-trip double formatting but use invariant culture.
            return doubleValue.ToString("R", CultureInfo.InvariantCulture);
        }
        if (value is string stringValue)
        {
            return stringValue;
        }
        if (value is List<object> listValue)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            for (int index = 0; index < listValue.Count; index = index + 1)
            {
                if (index > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(ValueToString(listValue[index]));
            }
            builder.Append(']');
            return builder.ToString();
        }
        if (value is TinyLanguageObject objectValue)
        {
            return "<" + objectValue.Class.Name + " instance>";
        }
        if (value is TinyLanguageClass classValue)
        {
            return "<class " + classValue.Name + ">";
        }
        if (value is TinyLanguageFunction functionValue)
        {
            return functionValue.Name.Length == 0
                ? "<lambda>"
                : "<function " + functionValue.Name + ">";
        }
        if (value is TinyLanguageModule moduleValue)
        {
            return "<module " + moduleValue.Name + ">";
        }
        return value.ToString();
    }

    // Unified numeric classifier: determines whether a value is an integer
    // or a float — used by arithmetic to pick the correct result type.
    private static bool IsInteger(object value)
    {
        return value is long;
    }

    private static bool IsFloat(object value)
    {
        return value is double;
    }

    private static bool IsNumeric(object value)
    {
        return value is long || value is double;
    }

    // Converts any numeric value to double for float-producing operations.
    private static double ToDouble(object value)
    {
        if (value is long longValue)
        {
            return (double)longValue;
        }
        if (value is double doubleValue)
        {
            return doubleValue;
        }
        return 0.0;
    }

    // ─── Statement visitors ───────────────────────────────────────────────

    public void Visit(ProgramNode node)
    {
        for (int index = 0; index < node.Statements.Count; index = index + 1)
        {
            node.Statements[index].Accept(this);
        }
    }

    public void Visit(AssignStatementNode node)
    {
        object value = EvaluateChild(node.Value);
        if (CurrentScope.IsDefinedAnywhere(node.Name))
        {
            CurrentScope.Assign(node.Name, value, node.Line);
        }
        else
        {
            // Bare assignment to an undeclared name creates the binding in
            // the current scope (common in loop bodies and top-level code).
            CurrentScope.Define(node.Name, value);
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(LetDeclareNode node)
    {
        object value = EvaluateChild(node.Value);
        CurrentScope.Define(node.Name, value);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(VarDeclareNode node)
    {
        object value = EvaluateChild(node.Value);
        CurrentScope.Define(node.Name, value);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ConstDeclareNode node)
    {
        object value = EvaluateChild(node.Value);
        CurrentScope.DefineConst(node.Name, value);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ArrayElementAssignNode node)
    {
        object target = CurrentScope.Lookup(node.ArrayName, node.Line);
        if (!(target is List<object> listTarget))
        {
            throw new InterpreterException("Cannot assign to index of non-array value '" + node.ArrayName + "'", node.Line);
        }
        object indexValue = EvaluateChild(node.IndexExpr);
        if (!(indexValue is long indexLong))
        {
            throw new InterpreterException("Array index must be an integer", node.Line);
        }
        int index = (int)indexLong;
        if (index < 0 || index >= listTarget.Count)
        {
            throw new InterpreterException("Array index " + index + " is out of bounds (length " + listTarget.Count + ")", node.Line);
        }
        object newValue = EvaluateChild(node.Value);
        listTarget[index] = newValue;
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(IfStatementNode node)
    {
        object condition = EvaluateChild(node.Condition);
        if (IsTruthy(condition))
        {
            ExecuteBlock(node.ThenBody, CurrentScope.CreateChild());
        }
        else if (node.ElseBody != null && node.ElseBody.Count > 0)
        {
            ExecuteBlock(node.ElseBody, CurrentScope.CreateChild());
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(WhileStatementNode node)
    {
        while (true)
        {
            object condition = EvaluateChild(node.Condition);
            if (!IsTruthy(condition))
            {
                break;
            }
            try
            {
                ExecuteBlock(node.Body, CurrentScope.CreateChild());
            }
            catch (BreakSignal)
            {
                break;
            }
            catch (ContinueSignal)
            {
                continue;
            }
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ForStatementNode node)
    {
        object startValue = EvaluateChild(node.StartExpr);
        object endValue = EvaluateChild(node.EndExpr);
        object stepValue = node.StepExpr != null ? EvaluateChild(node.StepExpr) : (object)(long)1;

        if (!(startValue is long startLong) || !(endValue is long endLong) || !(stepValue is long stepLong))
        {
            throw new InterpreterException("for loop bounds and step must be integers", node.Line);
        }
        if (stepLong == 0)
        {
            throw new InterpreterException("for loop step cannot be zero", node.Line);
        }

        long current = startLong;
        bool ascending = stepLong > 0;
        while ((ascending && current <= endLong) || (!ascending && current >= endLong))
        {
            Scope loopScope = CurrentScope.CreateChild();
            loopScope.Define(node.VariableName, current);
            try
            {
                ExecuteBlock(node.Body, loopScope);
            }
            catch (BreakSignal)
            {
                LastValue = TinyLanguageNull.Instance;
                return;
            }
            catch (ContinueSignal)
            {
                // fall through to increment
            }
            current = current + stepLong;
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ForeachStatementNode node)
    {
        object collectionValue = EvaluateChild(node.Collection);
        if (collectionValue == null || collectionValue is TinyLanguageNull)
        {
            throw new InterpreterException("Cannot iterate null with foreach", node.Line);
        }

        List<object> elements;
        if (collectionValue is List<object> listValue)
        {
            elements = listValue;
            for (int index = 0; index < elements.Count; index = index + 1)
            {
                Scope loopScope = CurrentScope.CreateChild();
                loopScope.Define(node.VariableName, elements[index]);
                try
                {
                    ExecuteBlock(node.Body, loopScope);
                }
                catch (BreakSignal)
                {
                    LastValue = TinyLanguageNull.Instance;
                    return;
                }
                catch (ContinueSignal)
                {
                    continue;
                }
            }
        }
        else if (collectionValue is string stringValue)
        {
            // Iterating a string yields each character as a single-character string.
            for (int index = 0; index < stringValue.Length; index = index + 1)
            {
                string charElement = stringValue[index].ToString();
                Scope loopScope = CurrentScope.CreateChild();
                loopScope.Define(node.VariableName, charElement);
                try
                {
                    ExecuteBlock(node.Body, loopScope);
                }
                catch (BreakSignal)
                {
                    LastValue = TinyLanguageNull.Instance;
                    return;
                }
                catch (ContinueSignal)
                {
                    continue;
                }
            }
        }
        else
        {
            throw new InterpreterException("Cannot iterate value of this type with foreach", node.Line);
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(DoWhileStatementNode node)
    {
        while (true)
        {
            try
            {
                ExecuteBlock(node.Body, CurrentScope.CreateChild());
            }
            catch (BreakSignal)
            {
                break;
            }
            catch (ContinueSignal)
            {
                // fall through to condition check
            }
            object condition = EvaluateChild(node.Condition);
            if (!IsTruthy(condition))
            {
                break;
            }
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(DoBlockNode node)
    {
        ExecuteBlock(node.Body, CurrentScope.CreateChild());
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(SwitchStatementNode node)
    {
        object subjectValue = EvaluateChild(node.Subject);
        bool matched = false;
        try
        {
            for (int caseIndex = 0; caseIndex < node.Cases.Count; caseIndex = caseIndex + 1)
            {
                SwitchCaseNode caseClause = node.Cases[caseIndex];
                if (caseClause.IsDefault)
                {
                    continue; // handled below if nothing else matched
                }
                object caseValue = EvaluateChild(caseClause.CaseExpr);
                if (AreEqual(subjectValue, caseValue))
                {
                    matched = true;
                    ExecuteBlock(caseClause.Body, CurrentScope.CreateChild());
                    break;
                }
            }
            if (!matched)
            {
                for (int defaultIndex = 0; defaultIndex < node.Cases.Count; defaultIndex = defaultIndex + 1)
                {
                    SwitchCaseNode caseClause = node.Cases[defaultIndex];
                    if (caseClause.IsDefault)
                    {
                        ExecuteBlock(caseClause.Body, CurrentScope.CreateChild());
                        break;
                    }
                }
            }
        }
        catch (BreakSignal)
        {
            // Break leaves the switch — no further action required.
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(BreakStatementNode node)
    {
        throw BreakSignal.Instance;
    }

    public void Visit(ContinueStatementNode node)
    {
        throw ContinueSignal.Instance;
    }

    public void Visit(PrintStatementNode node)
    {
        object value = EvaluateChild(node.Expression);
        Console.WriteLine(ValueToString(value));
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(InputStatementNode node)
    {
        string line = Console.ReadLine();
        object value = line == null ? (object)TinyLanguageNull.Instance : line;
        if (CurrentScope.IsDefinedAnywhere(node.VariableName))
        {
            CurrentScope.Assign(node.VariableName, value, node.Line);
        }
        else
        {
            CurrentScope.Define(node.VariableName, value);
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(FunctionDefNode node)
    {
        TinyLanguageFunction function = new TinyLanguageFunction(node.Name, node.Parameters, node.Body, true, null);
        CurrentScope.Define(node.Name, function);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ReturnStatementNode node)
    {
        object value = node.Value == null
            ? (object)TinyLanguageNull.Instance
            : EvaluateChild(node.Value);
        throw new ReturnSignal(value);
    }

    public void Visit(CallStatementNode node)
    {
        LastValue = InvokeMemberPathCall(node.MemberPath, node.Arguments, node.Line);
    }

    public void Visit(EnumDefNode node)
    {
        // Enum values are expressed as a nested dictionary-like object:
        // each member becomes a binding on a synthesised TinyLanguageObject.
        TinyLanguageObject enumContainer = new TinyLanguageObject(
            new TinyLanguageClass(node.Name, null,
                new List<FieldDeclareNode>(),
                new Dictionary<string, MethodDefNode>(),
                new Dictionary<string, MethodDefNode>(),
                new Dictionary<string, object>(),
                null,
                true));

        long autoValue = 0;
        for (int index = 0; index < node.Members.Count; index = index + 1)
        {
            EnumMember member = node.Members[index];
            object value;
            if (member.Value == null)
            {
                value = autoValue;
                autoValue = autoValue + 1;
            }
            else
            {
                value = EvaluateChild(member.Value);
                if (value is long longValue)
                {
                    autoValue = longValue + 1;
                }
            }
            enumContainer.Fields[member.Name] = value;
        }
        CurrentScope.DefineConst(node.Name, enumContainer);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ExportStatementNode node)
    {
        // Exports are recorded on the current module (if any) or otherwise
        // act as a no-op when executed at global scope.
        LastValue = TinyLanguageNull.Instance;
        if (CurrentModuleBeingDefined != null)
        {
            CurrentModuleBeingDefined.Exports.Add(node.Name);
        }
    }

    public void Visit(ImportStatementNode node)
    {
        TinyLanguageModule module;
        if (!Modules.TryGetValue(node.ModuleName, out module))
        {
            throw new InterpreterException("Unknown module '" + node.ModuleName + "'", node.Line);
        }
        string bindingName = node.Alias != null ? node.Alias : node.ModuleName;
        CurrentScope.Define(bindingName, module);
        // Additionally expose exported names directly in the current scope.
        foreach (string exportedName in module.Exports)
        {
            object value;
            if (module.ModuleScope.Bindings.TryGetValue(exportedName, out value))
            {
                CurrentScope.Define(exportedName, value);
            }
        }
        LastValue = TinyLanguageNull.Instance;
    }

    // Tracks the module currently being defined so export statements can
    // register exports against it. Null outside of any module definition.
    private TinyLanguageModule CurrentModuleBeingDefined;

    public void Visit(ModuleDefNode node)
    {
        Scope moduleScope = GlobalScope.CreateChild();
        TinyLanguageModule module = new TinyLanguageModule(node.Name, moduleScope);

        // Process header imports first — these bring other modules into scope.
        for (int index = 0; index < node.Imports.Count; index = index + 1)
        {
            ModuleImport moduleImport = node.Imports[index];
            TinyLanguageModule imported;
            if (!Modules.TryGetValue(moduleImport.ModuleName, out imported))
            {
                throw new InterpreterException("Unknown imported module '" + moduleImport.ModuleName + "'", node.Line);
            }
            string bindingName = moduleImport.Alias != null ? moduleImport.Alias : moduleImport.ModuleName;
            moduleScope.Define(bindingName, imported);
        }

        Scope previousScope = CurrentScope;
        TinyLanguageModule previousModule = CurrentModuleBeingDefined;
        CurrentScope = moduleScope;
        CurrentModuleBeingDefined = module;
        try
        {
            for (int index = 0; index < node.Body.Count; index = index + 1)
            {
                node.Body[index].Accept(this);
            }
        }
        finally
        {
            CurrentScope = previousScope;
            CurrentModuleBeingDefined = previousModule;
        }

        Modules[node.Name] = module;
        CurrentScope.Define(node.Name, module);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ClassDefNode node)
    {
        TinyLanguageClass parentClass = null;
        if (node.ExtendsName != null)
        {
            object parentValue = CurrentScope.Lookup(node.ExtendsName, node.Line);
            if (!(parentValue is TinyLanguageClass parentClassValue))
            {
                throw new InterpreterException("Cannot extend non-class '" + node.ExtendsName + "'", node.Line);
            }
            parentClass = parentClassValue;
        }

        List<FieldDeclareNode> fields = new List<FieldDeclareNode>();
        Dictionary<string, MethodDefNode> instanceMethods = new Dictionary<string, MethodDefNode>();
        Dictionary<string, MethodDefNode> staticMethods = new Dictionary<string, MethodDefNode>();
        Dictionary<string, object> staticFields = new Dictionary<string, object>();
        ConstructorDefNode constructor = null;

        for (int index = 0; index < node.Members.Count; index = index + 1)
        {
            AstNode member = node.Members[index];
            if (member is FieldDeclareNode fieldDeclare)
            {
                fields.Add(fieldDeclare);
            }
            else if (member is MethodDefNode methodDef)
            {
                if (methodDef.IsStatic)
                {
                    staticMethods[methodDef.Name] = methodDef;
                }
                else
                {
                    instanceMethods[methodDef.Name] = methodDef;
                }
            }
            else if (member is ConstructorDefNode constructorDef)
            {
                constructor = constructorDef;
            }
            else if (member is ConstDeclareNode constDeclare)
            {
                object value = EvaluateChild(constDeclare.Value);
                staticFields[constDeclare.Name] = value;
            }
        }

        TinyLanguageClass definition = new TinyLanguageClass(node.Name, parentClass, fields,
            instanceMethods, staticMethods, staticFields, constructor, node.IsStatic);
        CurrentScope.Define(node.Name, definition);
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ConstructorDefNode node)
    {
        // Constructors are handled by the class definition visitor and
        // invoked by NewExprNode — there is nothing to do when one is
        // encountered at statement-list level.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(MethodDefNode node)
    {
        // Same reasoning as ConstructorDefNode — methods are processed by
        // the enclosing class definition.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(FieldDeclareNode node)
    {
        // Field declarations inside a class body are collected by the class
        // definition visitor. If one appears at statement-list level (not
        // valid per the grammar) treat it as a no-op.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(TryStatementNode node)
    {
        try
        {
            try
            {
                ExecuteBlock(node.TryBody, CurrentScope.CreateChild());
            }
            catch (ThrowSignal throwSignal)
            {
                Scope catchScope = CurrentScope.CreateChild();
                catchScope.Define(node.CatchVariableName, throwSignal.Value);
                ExecuteBlock(node.CatchBody, catchScope);
            }
            catch (InterpreterException interpreterException)
            {
                Scope catchScope = CurrentScope.CreateChild();
                catchScope.Define(node.CatchVariableName, interpreterException.Message);
                ExecuteBlock(node.CatchBody, catchScope);
            }
        }
        finally
        {
            if (node.FinallyBody != null && node.FinallyBody.Count > 0)
            {
                ExecuteBlock(node.FinallyBody, CurrentScope.CreateChild());
            }
        }
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ThrowStatementNode node)
    {
        object value = EvaluateChild(node.Value);
        throw new ThrowSignal(value, node.Line);
    }

    public void Visit(AnnotatedStatementNode node)
    {
        // Annotations are pass-through metadata — execute the wrapped statement.
        node.Statement.Accept(this);
    }

    public void Visit(PatternMatchNode node)
    {
        object subjectValue = EvaluateChild(node.Subject);
        for (int index = 0; index < node.Cases.Count; index = index + 1)
        {
            PatternCaseNode patternCase = node.Cases[index];
            Scope caseScope = CurrentScope.CreateChild();
            if (PatternMatches(patternCase.Pattern, subjectValue, caseScope))
            {
                if (patternCase.Guard != null)
                {
                    Scope previous = CurrentScope;
                    CurrentScope = caseScope;
                    object guardValue;
                    try
                    {
                        guardValue = EvaluateChild(patternCase.Guard);
                    }
                    finally
                    {
                        CurrentScope = previous;
                    }
                    if (!IsTruthy(guardValue))
                    {
                        continue;
                    }
                }
                ExecuteBlock(patternCase.Body, caseScope);
                LastValue = TinyLanguageNull.Instance;
                return;
            }
        }
        LastValue = TinyLanguageNull.Instance;
    }

    // Attempts to match a pattern against a value. If the pattern matches,
    // records any identifier-bound sub-values in the supplied scope and
    // returns true. If not, returns false (scope may be partially mutated).
    private bool PatternMatches(PatternNode pattern, object value, Scope bindScope)
    {
        switch (pattern.Kind)
        {
            case PatternKind.Wildcard:
                return true;
            case PatternKind.Identifier:
                bindScope.Define(pattern.Name, value);
                return true;
            case PatternKind.IntegerLiteral:
                return value is long longValue && longValue == pattern.IntegerValue;
            case PatternKind.FloatLiteral:
                return value is double doubleValue && doubleValue == pattern.FloatValue;
            case PatternKind.StringLiteral:
                return value is string stringValue && stringValue == pattern.StringValue;
            case PatternKind.BoolLiteral:
                return value is bool boolValue && boolValue == pattern.BoolValue;
            case PatternKind.NullLiteral:
                return value == null || value is TinyLanguageNull;
            case PatternKind.ArrayPattern:
                if (!(value is List<object> listValue))
                {
                    return false;
                }
                if (listValue.Count != pattern.SubPatterns.Count)
                {
                    return false;
                }
                for (int elementIndex = 0; elementIndex < pattern.SubPatterns.Count; elementIndex = elementIndex + 1)
                {
                    if (!PatternMatches(pattern.SubPatterns[elementIndex], listValue[elementIndex], bindScope))
                    {
                        return false;
                    }
                }
                return true;
            case PatternKind.Constructor:
                // Match an object against a constructor pattern: the object's
                // class name must equal the pattern name; sub-patterns match
                // against constructor-position-equivalent fields in order.
                if (!(value is TinyLanguageObject objectValue))
                {
                    return false;
                }
                if (objectValue.Class.Name != pattern.Name)
                {
                    return false;
                }
                if (pattern.SubPatterns.Count == 0)
                {
                    return true;
                }
                if (pattern.SubPatterns.Count != objectValue.Class.Fields.Count)
                {
                    return false;
                }
                for (int fieldIndex = 0; fieldIndex < pattern.SubPatterns.Count; fieldIndex = fieldIndex + 1)
                {
                    FieldDeclareNode field = objectValue.Class.Fields[fieldIndex];
                    object fieldValue;
                    if (!objectValue.Fields.TryGetValue(field.Name, out fieldValue))
                    {
                        return false;
                    }
                    if (!PatternMatches(pattern.SubPatterns[fieldIndex], fieldValue, bindScope))
                    {
                        return false;
                    }
                }
                return true;
            case PatternKind.FieldPattern:
                if (!(value is TinyLanguageObject objectFieldValue))
                {
                    return false;
                }
                if (objectFieldValue.Class.Name != pattern.Name)
                {
                    return false;
                }
                for (int entryIndex = 0; entryIndex < pattern.FieldPatterns.Count; entryIndex = entryIndex + 1)
                {
                    FieldPatternEntry entry = pattern.FieldPatterns[entryIndex];
                    object fieldValue;
                    if (!objectFieldValue.Fields.TryGetValue(entry.FieldName, out fieldValue))
                    {
                        return false;
                    }
                    if (!PatternMatches(entry.SubPattern, fieldValue, bindScope))
                    {
                        return false;
                    }
                }
                return true;
            case PatternKind.Alternation:
                if (PatternMatches(pattern.Left, value, bindScope))
                {
                    return true;
                }
                return PatternMatches(pattern.Right, value, bindScope);
        }
        return false;
    }

    public void Visit(PatternCaseNode node)
    {
        // Cases are driven by PatternMatchNode.Visit.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(PatternNode node)
    {
        // Standalone pattern visits have no runtime effect.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(ParameterNode node)
    {
        // Parameters are bound at call sites — no standalone action.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(TypeNode node)
    {
        // TypeNode has no runtime effect on its own.
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(AnnotationNode node)
    {
        // Annotation metadata is not evaluated at runtime.
        LastValue = TinyLanguageNull.Instance;
    }

    // ─── Expression visitors ──────────────────────────────────────────────

    public void Visit(IntegerLiteralNode node)
    {
        LastValue = node.Value;
    }

    public void Visit(FloatLiteralNode node)
    {
        LastValue = node.Value;
    }

    public void Visit(StringLiteralNode node)
    {
        LastValue = node.Value;
    }

    public void Visit(BoolLiteralNode node)
    {
        LastValue = node.Value;
    }

    public void Visit(NullLiteralNode node)
    {
        LastValue = TinyLanguageNull.Instance;
    }

    public void Visit(IdentifierNode node)
    {
        LastValue = CurrentScope.Lookup(node.Name, node.Line);
    }

    public void Visit(ArrayLiteralNode node)
    {
        List<object> elements = new List<object>(node.Elements.Count);
        for (int index = 0; index < node.Elements.Count; index = index + 1)
        {
            elements.Add(EvaluateChild(node.Elements[index]));
        }
        LastValue = elements;
    }

    public void Visit(BinaryOpNode node)
    {
        string op = node.Operator;

        // Logical operators short-circuit and always yield Bool.
        if (op == "&&" || op == "and")
        {
            object leftShortValue = EvaluateChild(node.Left);
            if (!IsTruthy(leftShortValue))
            {
                LastValue = false;
                return;
            }
            object rightShortValue = EvaluateChild(node.Right);
            LastValue = IsTruthy(rightShortValue);
            return;
        }
        if (op == "||" || op == "or")
        {
            object leftShortValue = EvaluateChild(node.Left);
            if (IsTruthy(leftShortValue))
            {
                LastValue = true;
                return;
            }
            object rightShortValue = EvaluateChild(node.Right);
            LastValue = IsTruthy(rightShortValue);
            return;
        }

        object left = EvaluateChild(node.Left);
        object right = EvaluateChild(node.Right);

        switch (op)
        {
            case "+":
                LastValue = EvaluatePlus(left, right, node.Line);
                return;
            case "-":
                LastValue = EvaluateMinus(left, right, node.Line);
                return;
            case "*":
                LastValue = EvaluateMultiply(left, right, node.Line);
                return;
            case "/":
                LastValue = EvaluateDivide(left, right, node.Line);
                return;
            case "//":
                LastValue = EvaluateFloorDivide(left, right, node.Line);
                return;
            case "%":
                LastValue = EvaluateModulo(left, right, node.Line);
                return;
            case "**":
                LastValue = EvaluatePower(left, right, node.Line);
                return;
            case "&":
                LastValue = ValueToString(left) + ValueToString(right);
                return;
            case "==":
                LastValue = AreEqual(left, right);
                return;
            case "!=":
                LastValue = !AreEqual(left, right);
                return;
            case "<":
                LastValue = CompareNumeric(left, right, node.Line) < 0;
                return;
            case ">":
                LastValue = CompareNumeric(left, right, node.Line) > 0;
                return;
            case "<=":
                LastValue = CompareNumeric(left, right, node.Line) <= 0;
                return;
            case ">=":
                LastValue = CompareNumeric(left, right, node.Line) >= 0;
                return;
        }

        throw new InterpreterException("Unknown binary operator '" + op + "'", node.Line);
    }

    // Equality uses value semantics for primitives and reference identity
    // for object/list/function references. Numeric mixing (int vs float) is
    // normalised to double so that 1 == 1.0.
    private static bool AreEqual(object left, object right)
    {
        bool leftIsNull = left == null || left is TinyLanguageNull;
        bool rightIsNull = right == null || right is TinyLanguageNull;
        if (leftIsNull && rightIsNull)
        {
            return true;
        }
        if (leftIsNull || rightIsNull)
        {
            return false;
        }
        if (left is long leftLong && right is long rightLong)
        {
            return leftLong == rightLong;
        }
        if (IsNumeric(left) && IsNumeric(right))
        {
            return ToDouble(left) == ToDouble(right);
        }
        if (left is bool leftBool && right is bool rightBool)
        {
            return leftBool == rightBool;
        }
        if (left is string leftString && right is string rightString)
        {
            return leftString == rightString;
        }
        return ReferenceEquals(left, right);
    }

    // Compares two numeric values (or two strings) returning a traditional
    // negative / zero / positive ordering result.
    private static int CompareNumeric(object left, object right, int line)
    {
        if (left is string leftString && right is string rightString)
        {
            return string.CompareOrdinal(leftString, rightString);
        }
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Comparison requires numeric or string operands", line);
        }
        if (left is long leftLong && right is long rightLong)
        {
            return leftLong.CompareTo(rightLong);
        }
        double leftDouble = ToDouble(left);
        double rightDouble = ToDouble(right);
        return leftDouble.CompareTo(rightDouble);
    }

    private static object EvaluatePlus(object left, object right, int line)
    {
        // If either side is a string, concatenate string representations.
        if (left is string || right is string)
        {
            return ValueToString(left) + ValueToString(right);
        }
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Operator '+' requires numeric or string operands", line);
        }
        if (left is long leftLong && right is long rightLong)
        {
            return leftLong + rightLong;
        }
        return ToDouble(left) + ToDouble(right);
    }

    private static object EvaluateMinus(object left, object right, int line)
    {
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Operator '-' requires numeric operands", line);
        }
        if (left is long leftLong && right is long rightLong)
        {
            return leftLong - rightLong;
        }
        return ToDouble(left) - ToDouble(right);
    }

    private static object EvaluateMultiply(object left, object right, int line)
    {
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Operator '*' requires numeric operands", line);
        }
        if (left is long leftLong && right is long rightLong)
        {
            return leftLong * rightLong;
        }
        return ToDouble(left) * ToDouble(right);
    }

    private static object EvaluateDivide(object left, object right, int line)
    {
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Operator '/' requires numeric operands", line);
        }
        if (left is long leftLong && right is long rightLong)
        {
            if (rightLong == 0)
            {
                throw new InterpreterException("Division by zero", line);
            }
            // Integer / integer: if remainder is zero produce an integer,
            // otherwise promote to float so 7 / 2 = 3.5.
            if (leftLong % rightLong == 0)
            {
                return leftLong / rightLong;
            }
            return (double)leftLong / (double)rightLong;
        }
        double rightDouble = ToDouble(right);
        if (rightDouble == 0.0)
        {
            throw new InterpreterException("Division by zero", line);
        }
        return ToDouble(left) / rightDouble;
    }

    private static object EvaluateFloorDivide(object left, object right, int line)
    {
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Operator '//' requires numeric operands", line);
        }
        if (left is long leftLong && right is long rightLong)
        {
            if (rightLong == 0)
            {
                throw new InterpreterException("Division by zero", line);
            }
            // C# truncates toward zero; we want mathematical floor.
            long quotient = leftLong / rightLong;
            long remainder = leftLong % rightLong;
            if ((remainder != 0) && ((remainder < 0) != (rightLong < 0)))
            {
                quotient = quotient - 1;
            }
            return quotient;
        }
        double rightDouble = ToDouble(right);
        if (rightDouble == 0.0)
        {
            throw new InterpreterException("Division by zero", line);
        }
        return Math.Floor(ToDouble(left) / rightDouble);
    }

    private static object EvaluateModulo(object left, object right, int line)
    {
        if (left is double || right is double)
        {
            throw new InterpreterException("Operator '%' requires integer operands", line);
        }
        if (!(left is long leftLong) || !(right is long rightLong))
        {
            throw new InterpreterException("Operator '%' requires integer operands", line);
        }
        if (rightLong == 0)
        {
            throw new InterpreterException("Division by zero", line);
        }
        return leftLong % rightLong;
    }

    private static object EvaluatePower(object left, object right, int line)
    {
        if (!IsNumeric(left) || !IsNumeric(right))
        {
            throw new InterpreterException("Operator '**' requires numeric operands", line);
        }
        // Integer ** non-negative integer stays integer.
        if (left is long leftLong && right is long rightLong && rightLong >= 0)
        {
            long result = 1;
            long baseValue = leftLong;
            long exponent = rightLong;
            while (exponent > 0)
            {
                if ((exponent & 1L) == 1L)
                {
                    result = result * baseValue;
                }
                exponent = exponent >> 1;
                if (exponent > 0)
                {
                    baseValue = baseValue * baseValue;
                }
            }
            return result;
        }
        return Math.Pow(ToDouble(left), ToDouble(right));
    }

    public void Visit(UnaryOpNode node)
    {
        object operand = EvaluateChild(node.Operand);
        if (node.Operator == "-")
        {
            if (operand is long longValue)
            {
                LastValue = -longValue;
                return;
            }
            if (operand is double doubleValue)
            {
                LastValue = -doubleValue;
                return;
            }
            throw new InterpreterException("Unary '-' requires a numeric operand", node.Line);
        }
        if (node.Operator == "not")
        {
            LastValue = !IsTruthy(operand);
            return;
        }
        throw new InterpreterException("Unknown unary operator '" + node.Operator + "'", node.Line);
    }

    public void Visit(TernaryNode node)
    {
        object condition = EvaluateChild(node.Condition);
        LastValue = IsTruthy(condition)
            ? EvaluateChild(node.ThenExpr)
            : EvaluateChild(node.ElseExpr);
    }

    public void Visit(ConditionalExprNode node)
    {
        object condition = EvaluateChild(node.Condition);
        LastValue = IsTruthy(condition)
            ? EvaluateChild(node.ThenExpr)
            : EvaluateChild(node.ElseExpr);
    }

    public void Visit(CastExprNode node)
    {
        object operand = EvaluateChild(node.Operand);
        LastValue = ApplyCast(operand, node.TargetType, node.Line);
    }

    private object ApplyCast(object value, TypeNode targetType, int line)
    {
        switch (targetType.Kind)
        {
            case TypeKind.Int:
                return CoerceToInteger(value, line);
            case TypeKind.Float:
                return CoerceToFloat(value, line);
            case TypeKind.String:
                return ValueToString(value);
            case TypeKind.Bool:
                return IsTruthy(value);
            case TypeKind.Null:
                return TinyLanguageNull.Instance;
            case TypeKind.Object:
            case TypeKind.Array:
            case TypeKind.Named:
            case TypeKind.GenericType:
            case TypeKind.MapType:
            case TypeKind.ArraySuffix:
            case TypeKind.NullableSuffix:
            case TypeKind.Void:
                return value;
        }
        return value;
    }

    private static long CoerceToInteger(object value, int line)
    {
        if (value is long longValue)
        {
            return longValue;
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
            long parsed;
            if (long.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }
            double parsedDouble;
            if (double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedDouble))
            {
                return (long)parsedDouble;
            }
            throw new InterpreterException("Cannot convert string '" + stringValue + "' to int", line);
        }
        if (value == null || value is TinyLanguageNull)
        {
            return 0L;
        }
        throw new InterpreterException("Cannot convert value to int", line);
    }

    private static double CoerceToFloat(object value, int line)
    {
        if (value is double doubleValue)
        {
            return doubleValue;
        }
        if (value is long longValue)
        {
            return (double)longValue;
        }
        if (value is bool boolValue)
        {
            return boolValue ? 1.0 : 0.0;
        }
        if (value is string stringValue)
        {
            double parsed;
            if (double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }
            throw new InterpreterException("Cannot convert string '" + stringValue + "' to float", line);
        }
        if (value == null || value is TinyLanguageNull)
        {
            return 0.0;
        }
        throw new InterpreterException("Cannot convert value to float", line);
    }

    public void Visit(TypeCheckNode node)
    {
        object operand = EvaluateChild(node.Operand);
        LastValue = MatchesType(operand, node.CheckType);
    }

    public void Visit(TypeAssertNode node)
    {
        object operand = EvaluateChild(node.Operand);
        if (!MatchesType(operand, node.AssertType))
        {
            throw new InterpreterException("Type assertion failed", node.Line);
        }
        LastValue = operand;
    }

    private static bool MatchesType(object value, TypeNode typeNode)
    {
        switch (typeNode.Kind)
        {
            case TypeKind.Int:
                return value is long;
            case TypeKind.Float:
                return value is double;
            case TypeKind.String:
                return value is string;
            case TypeKind.Bool:
                return value is bool;
            case TypeKind.Null:
                return value == null || value is TinyLanguageNull;
            case TypeKind.Array:
            case TypeKind.ArraySuffix:
                return value is List<object>;
            case TypeKind.Object:
                return value is TinyLanguageObject;
            case TypeKind.Void:
                return value == null || value is TinyLanguageNull;
            case TypeKind.NullableSuffix:
                if (value == null || value is TinyLanguageNull)
                {
                    return true;
                }
                return MatchesType(value, typeNode.InnerType);
            case TypeKind.Named:
                if (value is TinyLanguageObject objectValue)
                {
                    TinyLanguageClass current = objectValue.Class;
                    while (current != null)
                    {
                        if (current.Name == typeNode.Name)
                        {
                            return true;
                        }
                        current = current.Parent;
                    }
                }
                return false;
        }
        return false;
    }

    public void Visit(IndexAccessNode node)
    {
        object target = EvaluateChild(node.Target);
        object indexValue = EvaluateChild(node.IndexExpr);
        if (target is List<object> listValue)
        {
            if (!(indexValue is long indexLong))
            {
                throw new InterpreterException("Array index must be an integer", node.Line);
            }
            int index = (int)indexLong;
            if (index < 0 || index >= listValue.Count)
            {
                LastValue = TinyLanguageNull.Instance;
                return;
            }
            LastValue = listValue[index];
            return;
        }
        if (target is string stringValue)
        {
            if (!(indexValue is long indexLong))
            {
                throw new InterpreterException("String index must be an integer", node.Line);
            }
            int index = (int)indexLong;
            if (index < 0 || index >= stringValue.Length)
            {
                LastValue = TinyLanguageNull.Instance;
                return;
            }
            LastValue = stringValue[index].ToString();
            return;
        }
        throw new InterpreterException("Value does not support index access", node.Line);
    }

    public void Visit(MemberAccessNode node)
    {
        object target = EvaluateChild(node.Target);
        LastValue = AccessMember(target, node.MemberName, node.Line);
    }

    private object AccessMember(object target, string memberName, int line)
    {
        if (target is TinyLanguageObject objectValue)
        {
            object fieldValue;
            if (objectValue.Fields.TryGetValue(memberName, out fieldValue))
            {
                return fieldValue;
            }
            // Fall through to class static field search.
            if (objectValue.Class.StaticFields.TryGetValue(memberName, out fieldValue))
            {
                return fieldValue;
            }
        }
        if (target is TinyLanguageClass classValue)
        {
            object staticField;
            if (classValue.StaticFields.TryGetValue(memberName, out staticField))
            {
                return staticField;
            }
        }
        if (target is TinyLanguageModule moduleValue)
        {
            object bindingValue;
            if (moduleValue.ModuleScope.Bindings.TryGetValue(memberName, out bindingValue))
            {
                return bindingValue;
            }
        }
        throw new InterpreterException("Member '" + memberName + "' not found", line);
    }

    public void Visit(MethodCallNode node)
    {
        object target = EvaluateChild(node.Target);
        List<object> arguments = new List<object>(node.Arguments.Count);
        for (int index = 0; index < node.Arguments.Count; index = index + 1)
        {
            arguments.Add(EvaluateChild(node.Arguments[index]));
        }
        LastValue = InvokeMethodOn(target, node.MethodName, arguments, node.Line);
    }

    private object InvokeMethodOn(object target, string methodName, List<object> arguments, int line)
    {
        if (target is TinyLanguageObject objectValue)
        {
            MethodDefNode method = objectValue.Class.FindInstanceMethod(methodName);
            if (method != null)
            {
                return InvokeMethod(method, objectValue, arguments, line);
            }
            // Fallback: field holding a function value.
            object fieldValue;
            if (objectValue.Fields.TryGetValue(methodName, out fieldValue))
            {
                return InvokeCallableValue(fieldValue, arguments, line);
            }
        }
        if (target is TinyLanguageClass classValue)
        {
            MethodDefNode staticMethod;
            if (classValue.StaticMethods.TryGetValue(methodName, out staticMethod))
            {
                return InvokeMethod(staticMethod, null, arguments, line);
            }
            object staticField;
            if (classValue.StaticFields.TryGetValue(methodName, out staticField))
            {
                return InvokeCallableValue(staticField, arguments, line);
            }
        }
        if (target is TinyLanguageModule moduleValue)
        {
            object bindingValue;
            if (moduleValue.ModuleScope.Bindings.TryGetValue(methodName, out bindingValue))
            {
                return InvokeCallableValue(bindingValue, arguments, line);
            }
        }
        throw new InterpreterException("Method '" + methodName + "' not found", line);
    }

    private object InvokeCallableValue(object callable, List<object> arguments, int line)
    {
        if (callable is TinyLanguageFunction function)
        {
            return InvokeFunction(function, arguments, null, line);
        }
        throw new InterpreterException("Value is not callable", line);
    }

    public void Visit(FunctionCallNode node)
    {
        List<object> arguments = new List<object>(node.Arguments.Count);
        for (int index = 0; index < node.Arguments.Count; index = index + 1)
        {
            arguments.Add(EvaluateChild(node.Arguments[index]));
        }

        // Try built-in functions first.
        object builtinResult;
        if (TryInvokeBuiltin(node.Name, arguments, node.Line, out builtinResult))
        {
            LastValue = builtinResult;
            return;
        }

        // Otherwise resolve a user-defined function in the current scope.
        object callable = CurrentScope.Lookup(node.Name, node.Line);
        LastValue = InvokeCallableValue(callable, arguments, node.Line);
    }

    // ─── Built-in functions ───────────────────────────────────────────────

    private static bool TryInvokeBuiltin(string name, List<object> arguments, int line, out object result)
    {
        switch (name)
        {
            case "len":
                RequireArity(name, arguments, 1, line);
                result = BuiltinLen(arguments[0], line);
                return true;
            case "str":
                RequireArity(name, arguments, 1, line);
                result = ValueToString(arguments[0]);
                return true;
            case "int":
                RequireArity(name, arguments, 1, line);
                result = CoerceToInteger(arguments[0], line);
                return true;
            case "float":
                RequireArity(name, arguments, 1, line);
                result = CoerceToFloat(arguments[0], line);
                return true;
            case "bool":
                RequireArity(name, arguments, 1, line);
                result = IsTruthy(arguments[0]);
                return true;
        }
        result = null;
        return false;
    }

    private static void RequireArity(string name, List<object> arguments, int expected, int line)
    {
        if (arguments.Count != expected)
        {
            throw new InterpreterException("Built-in '" + name + "' expects " + expected + " argument(s), got " + arguments.Count, line);
        }
    }

    private static long BuiltinLen(object value, int line)
    {
        if (value is string stringValue)
        {
            return stringValue.Length;
        }
        if (value is List<object> listValue)
        {
            return listValue.Count;
        }
        throw new InterpreterException("len() requires a string or array argument", line);
    }

    // ─── Function / method invocation ─────────────────────────────────────

    // Resolves a member-chain call statement: foo(), obj.method(...), a.b.c(...).
    private object InvokeMemberPathCall(List<string> memberPath, List<AstNode> argumentNodes, int line)
    {
        List<object> arguments = new List<object>(argumentNodes.Count);
        for (int index = 0; index < argumentNodes.Count; index = index + 1)
        {
            arguments.Add(EvaluateChild(argumentNodes[index]));
        }

        if (memberPath.Count == 1)
        {
            string name = memberPath[0];
            object builtinResult;
            if (TryInvokeBuiltin(name, arguments, line, out builtinResult))
            {
                return builtinResult;
            }
            object callable = CurrentScope.Lookup(name, line);
            return InvokeCallableValue(callable, arguments, line);
        }

        // memberPath.Count >= 2 — walk members, invoke on last.
        object current = CurrentScope.Lookup(memberPath[0], line);
        for (int index = 1; index < memberPath.Count - 1; index = index + 1)
        {
            current = AccessMember(current, memberPath[index], line);
        }
        string finalName = memberPath[memberPath.Count - 1];
        return InvokeMethodOn(current, finalName, arguments, line);
    }

    private object InvokeMethod(MethodDefNode method, TinyLanguageObject thisObject, List<object> arguments, int line)
    {
        CheckCallDepth(line);
        CallDepth = CallDepth + 1;
        try
        {
            Scope callScope = GlobalScope.CreateChild();
            if (thisObject != null)
            {
                callScope.Define("this", thisObject);
                // Also expose fields directly as implicit bindings? Keep
                // explicit via "this.field" only — the spec does not require
                // implicit field exposure.
            }
            BindParameters(method.Parameters, arguments, callScope, line);

            Scope previous = CurrentScope;
            CurrentScope = callScope;
            try
            {
                for (int index = 0; index < method.Body.Count; index = index + 1)
                {
                    method.Body[index].Accept(this);
                }
            }
            catch (ReturnSignal returnSignal)
            {
                return returnSignal.Value;
            }
            finally
            {
                CurrentScope = previous;
            }
            return TinyLanguageNull.Instance;
        }
        finally
        {
            CallDepth = CallDepth - 1;
        }
    }

    private object InvokeFunction(TinyLanguageFunction function, List<object> arguments, TinyLanguageObject thisObject, int line)
    {
        CheckCallDepth(line);
        CallDepth = CallDepth + 1;
        try
        {
            Scope parentScope = function.Closure != null ? function.Closure : GlobalScope;
            Scope callScope = parentScope.CreateChild();
            if (thisObject != null)
            {
                callScope.Define("this", thisObject);
            }
            BindParameters(function.Parameters, arguments, callScope, line);

            Scope previous = CurrentScope;
            CurrentScope = callScope;
            try
            {
                if (function.IsBlockBody)
                {
                    for (int index = 0; index < function.Body.Count; index = index + 1)
                    {
                        function.Body[index].Accept(this);
                    }
                    return TinyLanguageNull.Instance;
                }
                // Expression-body lambda: body is a single expression node.
                return EvaluateChild(function.Body[0]);
            }
            catch (ReturnSignal returnSignal)
            {
                return returnSignal.Value;
            }
            finally
            {
                CurrentScope = previous;
            }
        }
        finally
        {
            CallDepth = CallDepth - 1;
        }
    }

    private void BindParameters(List<ParameterNode> parameters, List<object> arguments, Scope callScope, int line)
    {
        for (int index = 0; index < parameters.Count; index = index + 1)
        {
            ParameterNode parameter = parameters[index];
            object value;
            if (index < arguments.Count)
            {
                value = arguments[index];
            }
            else if (parameter.DefaultValue != null)
            {
                value = EvaluateChild(parameter.DefaultValue);
            }
            else
            {
                throw new InterpreterException("Missing argument for parameter '" + parameter.Name + "'", line);
            }
            callScope.Define(parameter.Name, value);
        }
    }

    private void CheckCallDepth(int line)
    {
        if (CallDepth >= MaxCallDepth)
        {
            throw new InterpreterException("Stack overflow: maximum call depth of " + MaxCallDepth + " exceeded", line);
        }
    }

    public void Visit(NewExprNode node)
    {
        object classValue = CurrentScope.Lookup(node.ClassName, node.Line);
        if (!(classValue is TinyLanguageClass classDefinition))
        {
            throw new InterpreterException("'" + node.ClassName + "' is not a class", node.Line);
        }

        TinyLanguageObject instance = new TinyLanguageObject(classDefinition);
        // Initialise fields top-down through the inheritance chain so that
        // sub-class fields can shadow parent fields if desired.
        InitialiseFields(classDefinition, instance);

        List<object> arguments = new List<object>(node.Arguments.Count);
        for (int index = 0; index < node.Arguments.Count; index = index + 1)
        {
            arguments.Add(EvaluateChild(node.Arguments[index]));
        }

        // Invoke the constructor if one is defined (walk chain root-first so
        // parent constructors do not automatically run — explicit call only).
        if (classDefinition.Constructor != null)
        {
            InvokeConstructor(classDefinition.Constructor, instance, arguments, node.Line);
        }

        LastValue = instance;
    }

    private void InitialiseFields(TinyLanguageClass classDefinition, TinyLanguageObject instance)
    {
        if (classDefinition.Parent != null)
        {
            InitialiseFields(classDefinition.Parent, instance);
        }
        for (int index = 0; index < classDefinition.Fields.Count; index = index + 1)
        {
            FieldDeclareNode field = classDefinition.Fields[index];
            object value = EvaluateChild(field.Value);
            instance.Fields[field.Name] = value;
        }
    }

    private object InvokeConstructor(ConstructorDefNode constructor, TinyLanguageObject instance, List<object> arguments, int line)
    {
        CheckCallDepth(line);
        CallDepth = CallDepth + 1;
        try
        {
            Scope callScope = GlobalScope.CreateChild();
            callScope.Define("this", instance);
            BindParameters(constructor.Parameters, arguments, callScope, line);

            Scope previous = CurrentScope;
            CurrentScope = callScope;
            try
            {
                for (int index = 0; index < constructor.Body.Count; index = index + 1)
                {
                    constructor.Body[index].Accept(this);
                }
            }
            catch (ReturnSignal)
            {
                // Constructor returns are silently allowed.
            }
            finally
            {
                CurrentScope = previous;
            }
            return instance;
        }
        finally
        {
            CallDepth = CallDepth - 1;
        }
    }

    public void Visit(LambdaExprNode node)
    {
        // Lambdas capture the enclosing scope so free names resolve lexically.
        TinyLanguageFunction lambda = new TinyLanguageFunction(
            string.Empty,
            node.Parameters,
            node.Body,
            node.IsBlockBody,
            CurrentScope);
        LastValue = lambda;
    }

    public void Visit(ListComprehensionNode node)
    {
        object sourceValue = EvaluateChild(node.SourceExpr);
        List<object> result = new List<object>();
        Scope previous = CurrentScope;
        Scope comprehensionScope = CurrentScope.CreateChild();
        CurrentScope = comprehensionScope;
        try
        {
            if (sourceValue is List<object> listValue)
            {
                for (int index = 0; index < listValue.Count; index = index + 1)
                {
                    comprehensionScope.Bindings[node.VariableName] = listValue[index];
                    result.Add(EvaluateChild(node.ProjectionExpr));
                }
            }
            else if (sourceValue is string stringValue)
            {
                for (int index = 0; index < stringValue.Length; index = index + 1)
                {
                    comprehensionScope.Bindings[node.VariableName] = stringValue[index].ToString();
                    result.Add(EvaluateChild(node.ProjectionExpr));
                }
            }
            else
            {
                throw new InterpreterException("List comprehension source must be a list or string", node.Line);
            }
        }
        finally
        {
            CurrentScope = previous;
        }
        LastValue = result;
    }
}
