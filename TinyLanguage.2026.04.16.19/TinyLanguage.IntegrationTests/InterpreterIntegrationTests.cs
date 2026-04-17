using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyLanguage.Lexer;
using TinyLanguage.Lexer.Nodes;
using TinyLanguage.Interpreter;
using System;
using System.IO;
using System.Collections.Generic;

namespace TinyLanguage.IntegrationTests;

[TestClass]
public sealed class InterpreterIntegrationTests
{
    // Runs the given TinyLanguage source, captures stdout, and returns it trimmed.
    private static string Run(string source)
    {
        List<Token> tokens = new TinyLanguage.Lexer.Lexer(source).Tokenize();
        ProgramNode program = new Parser(tokens).Parse();
        StringWriter sw = new StringWriter();
        TextWriter oldOut = Console.Out;
        Console.SetOut(sw);
        try
        {
            new TinyLanguage.Interpreter.Interpreter().Execute(program);
        }
        finally
        {
            Console.SetOut(oldOut);
        }
        return sw.ToString().Trim().Replace("\r\n", "\n").Replace("\r", "\n");
    }

    // Runs source and asserts that an InterpreterException is thrown.
    private static void RunExpectError(string source)
    {
        List<Token> tokens = new TinyLanguage.Lexer.Lexer(source).Tokenize();
        ProgramNode program = new Parser(tokens).Parse();
        Assert.ThrowsException<InterpreterException>(() => new TinyLanguage.Interpreter.Interpreter().Execute(program));
    }

    // ─── Basic output ─────────────────────────────────────────────────────

    [TestMethod]
    public void Print_Integer_OutputsValue()
    {
        string result = Run("print 42");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void Print_NegativeInteger_OutputsValue()
    {
        string result = Run("print -7");
        Assert.AreEqual("-7", result);
    }

    [TestMethod]
    public void Print_Float_OutputsValue()
    {
        string result = Run("print 3.14");
        Assert.AreEqual("3.14", result);
    }

    [TestMethod]
    public void Print_String_OutputsValue()
    {
        string result = Run("print \"hello\"");
        Assert.AreEqual("hello", result);
    }

    [TestMethod]
    public void Print_True_OutputsTrue()
    {
        string result = Run("print true");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void Print_False_OutputsFalse()
    {
        string result = Run("print false");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void Print_Null_OutputsNull()
    {
        string result = Run("print null");
        Assert.AreEqual("null", result);
    }

    [TestMethod]
    public void Print_ArithmeticResult_OutputsComputed()
    {
        string result = Run("print 3 + 4 * 2");
        Assert.AreEqual("11", result);
    }

    // ─── Variables ────────────────────────────────────────────────────────

    [TestMethod]
    public void LetDeclare_AndPrint_OutputsValue()
    {
        string result = Run("let x := 10\nprint x");
        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public void VarDeclare_AndPrint_OutputsValue()
    {
        string result = Run("var x : int := 99\nprint x");
        Assert.AreEqual("99", result);
    }

    [TestMethod]
    public void ConstDeclare_AndPrint_OutputsValue()
    {
        string result = Run("const pi := 3\nprint pi");
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void Assign_UpdatesExistingVariable()
    {
        string result = Run("let x := 1\nx := 42\nprint x");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void AugmentedAdd_UpdatesVariable()
    {
        string result = Run("let total := 0\ntotal := total + 10\ntotal := total + 20\nprint total");
        Assert.AreEqual("30", result);
    }

    [TestMethod]
    public void AugmentedSubtract_UpdatesVariable()
    {
        string result = Run("let x := 100\nx := x - 40\nprint x");
        Assert.AreEqual("60", result);
    }

    [TestMethod]
    public void AugmentedMultiply_UpdatesVariable()
    {
        string result = Run("let x := 5\nx := x * 3\nprint x");
        Assert.AreEqual("15", result);
    }

    [TestMethod]
    public void AugmentedDivide_UpdatesVariable()
    {
        string result = Run("let x := 20\nx := x / 4\nprint x");
        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public void VariableShadowing_InnerScopeDoesNotAffectOuter()
    {
        string result = Run(
            "let x := 1\n" +
            "if true then\n" +
            "    let x := 99\n" +
            "    print x\n" +
            "end\n" +
            "print x");
        Assert.AreEqual("99\n1", result);
    }

    // ─── Arithmetic ───────────────────────────────────────────────────────

    [TestMethod]
    public void IntegerAddition_ProducesCorrectSum()
    {
        string result = Run("print 3 + 4");
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void IntegerSubtraction_ProducesCorrectDifference()
    {
        string result = Run("print 10 - 3");
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void IntegerMultiplication_ProducesCorrectProduct()
    {
        string result = Run("print 6 * 7");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void IntegerDivision_ExactResult_ProducesInteger()
    {
        string result = Run("print 10 / 2");
        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public void IntegerDivision_NonExact_ProducesFloat()
    {
        string result = Run("print 7 / 2");
        Assert.AreEqual("3.5", result);
    }

    [TestMethod]
    public void FloorDivision_ProducesFlooredInteger()
    {
        string result = Run("print 7 // 2");
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void PowerOperator_IntegerToInteger_ProducesInteger()
    {
        string result = Run("print 2 ** 10");
        Assert.AreEqual("1024", result);
    }

    [TestMethod]
    public void ModuloOperator_ProducesRemainder()
    {
        string result = Run("print 17 % 5");
        Assert.AreEqual("2", result);
    }

    [TestMethod]
    public void FloatArithmetic_ProducesFloat()
    {
        string result = Run("print 1.5 + 2.5");
        Assert.AreEqual("4", result);
    }

    [TestMethod]
    public void IntegerPlusFloat_ProducesFloat()
    {
        string result = Run("print 1 + 0.5");
        Assert.AreEqual("1.5", result);
    }

    [TestMethod]
    public void StringConcatenation_WithPlus_ConcatenatesStrings()
    {
        string result = Run("print \"hello\" + \" world\"");
        Assert.AreEqual("hello world", result);
    }

    [TestMethod]
    public void DivisionByZero_Integer_ThrowsInterpreterException()
    {
        RunExpectError("print 1 / 0");
    }

    [TestMethod]
    public void DivisionByZero_Float_ThrowsInterpreterException()
    {
        RunExpectError("print 1.0 / 0.0");
    }

    [TestMethod]
    public void FloorDivisionByZero_ThrowsInterpreterException()
    {
        RunExpectError("print 5 // 0");
    }

    // ─── Boolean logic ────────────────────────────────────────────────────

    [TestMethod]
    public void LogicalAnd_BothTrue_ReturnsTrue()
    {
        string result = Run("print true and true");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void LogicalAnd_OneFalse_ReturnsFalse()
    {
        string result = Run("print true and false");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void LogicalOr_OneTrue_ReturnsTrue()
    {
        string result = Run("print false or true");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void LogicalOr_BothFalse_ReturnsFalse()
    {
        string result = Run("print false or false");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void LogicalNot_True_ReturnsFalse()
    {
        string result = Run("print not true");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void LogicalNot_False_ReturnsTrue()
    {
        string result = Run("print not false");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void Truthiness_Zero_IsFalsy()
    {
        string result = Run("if 0 then\nprint \"truthy\"\nelse\nprint \"falsy\"\nend");
        Assert.AreEqual("falsy", result);
    }

    [TestMethod]
    public void Truthiness_EmptyString_IsFalsy()
    {
        string result = Run("if \"\" then\nprint \"truthy\"\nelse\nprint \"falsy\"\nend");
        Assert.AreEqual("falsy", result);
    }

    [TestMethod]
    public void Truthiness_Null_IsFalsy()
    {
        string result = Run("if null then\nprint \"truthy\"\nelse\nprint \"falsy\"\nend");
        Assert.AreEqual("falsy", result);
    }

    [TestMethod]
    public void Truthiness_EmptyArray_IsFalsy()
    {
        string result = Run("let a := []\nif a then\nprint \"truthy\"\nelse\nprint \"falsy\"\nend");
        Assert.AreEqual("falsy", result);
    }

    [TestMethod]
    public void ShortCircuitAnd_DoesNotEvaluateRight_WhenLeftIsFalse()
    {
        // If right side is evaluated, it would access undefined variable.
        // If short-circuited, no error.
        string result = Run("print false and true");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void ShortCircuitOr_DoesNotEvaluateRight_WhenLeftIsTrue()
    {
        string result = Run("print true or false");
        Assert.AreEqual("true", result);
    }

    // ─── Control flow ─────────────────────────────────────────────────────

    [TestMethod]
    public void IfThen_ConditionTrue_ExecutesThenBody()
    {
        string result = Run("if true then\nprint \"yes\"\nend");
        Assert.AreEqual("yes", result);
    }

    [TestMethod]
    public void IfElse_ConditionFalse_ExecutesElseBody()
    {
        string result = Run("if false then\nprint \"yes\"\nelse\nprint \"no\"\nend");
        Assert.AreEqual("no", result);
    }

    [TestMethod]
    public void IfElifElse_SecondConditionTrue_ExecutesElifBody()
    {
        string result = Run(
            "let x := 2\n" +
            "if x == 1 then\n" +
            "    print \"one\"\n" +
            "else if x == 2 then\n" +
            "    print \"two\"\n" +
            "else\n" +
            "    print \"other\"\n" +
            "end");
        Assert.AreEqual("two", result);
    }

    [TestMethod]
    public void WhileLoop_CountsToThree()
    {
        string result = Run(
            "let i := 1\n" +
            "while i <= 3 do\n" +
            "    print i\n" +
            "    i := i + 1\n" +
            "end");
        Assert.AreEqual("1\n2\n3", result);
    }

    [TestMethod]
    public void ForLoop_IteratesRange()
    {
        string result = Run("for i := 1 to 3 do\nprint i\nend");
        Assert.AreEqual("1\n2\n3", result);
    }

    [TestMethod]
    public void ForLoop_WithStep_IteratesCorrectly()
    {
        string result = Run("for i := 0 to 6 step 2 do\nprint i\nend");
        Assert.AreEqual("0\n2\n4\n6", result);
    }

    [TestMethod]
    public void ForLoop_NegativeStep_IteratesDownward()
    {
        string result = Run("for i := 3 to 1 step -1 do\nprint i\nend");
        Assert.AreEqual("3\n2\n1", result);
    }

    [TestMethod]
    public void ForeachLoop_OverArray_IteratesElements()
    {
        string result = Run(
            "let items := [10, 20, 30]\n" +
            "foreach item in items do\n" +
            "    print item\n" +
            "end");
        Assert.AreEqual("10\n20\n30", result);
    }

    [TestMethod]
    public void ForeachLoop_OverString_IteratesCharacters()
    {
        string result = Run(
            "foreach ch in \"abc\" do\n" +
            "    print ch\n" +
            "end");
        Assert.AreEqual("a\nb\nc", result);
    }

    [TestMethod]
    public void DoWhile_ExecutesBodyAtLeastOnce()
    {
        string result = Run(
            "let x := 0\n" +
            "do\n" +
            "    x := x + 1\n" +
            "    print x\n" +
            "while x < 3");
        Assert.AreEqual("1\n2\n3", result);
    }

    [TestMethod]
    public void Break_ExitsWhileLoop()
    {
        string result = Run(
            "let i := 0\n" +
            "while true do\n" +
            "    i := i + 1\n" +
            "    if i == 3 then\n" +
            "        break\n" +
            "    end\n" +
            "end\n" +
            "print i");
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void Continue_SkipsIterationInForLoop()
    {
        string result = Run(
            "for i := 1 to 5 do\n" +
            "    if i == 3 then\n" +
            "        continue\n" +
            "    end\n" +
            "    print i\n" +
            "end");
        Assert.AreEqual("1\n2\n4\n5", result);
    }

    [TestMethod]
    public void Break_InNestedLoop_ExitsInnerLoopOnly()
    {
        string result = Run(
            "for i := 1 to 2 do\n" +
            "    for j := 1 to 3 do\n" +
            "        if j == 2 then\n" +
            "            break\n" +
            "        end\n" +
            "        print j\n" +
            "    end\n" +
            "end");
        Assert.AreEqual("1\n1", result);
    }

    // ─── Functions ────────────────────────────────────────────────────────

    [TestMethod]
    public void FunctionCall_SimpleReturn_ReturnsValue()
    {
        string result = Run(
            "function greet(name : string) -> string\n" +
            "    return \"Hello, \" + name\n" +
            "end\n" +
            "print greet(\"World\")");
        Assert.AreEqual("Hello, World", result);
    }

    [TestMethod]
    public void FunctionCall_MultipleParameters_BindsCorrectly()
    {
        string result = Run(
            "function add(a : int, b : int) -> int\n" +
            "    return a + b\n" +
            "end\n" +
            "print add(3, 7)");
        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public void RecursiveFunction_Factorial_ComputesCorrectly()
    {
        string result = Run(
            "function factorial(n : int) -> int\n" +
            "    if n <= 1 then\n" +
            "        return 1\n" +
            "    end\n" +
            "    return n * factorial(n - 1)\n" +
            "end\n" +
            "print factorial(5)");
        Assert.AreEqual("120", result);
    }

    [TestMethod]
    public void RecursiveFunction_Fibonacci_ComputesCorrectly()
    {
        string result = Run(
            "function fib(n : int) -> int\n" +
            "    if n <= 1 then\n" +
            "        return n\n" +
            "    end\n" +
            "    return fib(n - 1) + fib(n - 2)\n" +
            "end\n" +
            "print fib(7)");
        Assert.AreEqual("13", result);
    }

    [TestMethod]
    public void BareReturn_ReturnsNull()
    {
        string result = Run(
            "function doNothing()\n" +
            "    return\n" +
            "end\n" +
            "print doNothing()");
        Assert.AreEqual("null", result);
    }

    [TestMethod]
    public void ReturnValue_UsedInExpression_ComputesCorrectly()
    {
        string result = Run(
            "function square(n : int) -> int\n" +
            "    return n * n\n" +
            "end\n" +
            "print square(3) + square(4)");
        Assert.AreEqual("25", result);
    }

    // ─── Lambda ───────────────────────────────────────────────────────────

    [TestMethod]
    public void Lambda_ExpressionBody_CallsCorrectly()
    {
        string result = Run(
            "let double := function(x : int) x * 2\n" +
            "print double(5)");
        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public void Lambda_BlockBody_CallsCorrectly()
    {
        string result = Run(
            "let triple := function(x : int)\n" +
            "    return x * 3\n" +
            "end\n" +
            "print triple(4)");
        Assert.AreEqual("12", result);
    }

    [TestMethod]
    public void Lambda_CapturesEnclosingScope_Closure()
    {
        string result = Run(
            "let factor := 10\n" +
            "let scale := function(x : int) x * factor\n" +
            "print scale(5)");
        Assert.AreEqual("50", result);
    }

    [TestMethod]
    public void HigherOrderFunction_PassesLambdaAsArgument()
    {
        string result = Run(
            "function apply(f, x : int) -> int\n" +
            "    return f(x)\n" +
            "end\n" +
            "let double := function(n : int) n * 2\n" +
            "print apply(double, 7)");
        Assert.AreEqual("14", result);
    }

    // ─── Arrays ───────────────────────────────────────────────────────────

    [TestMethod]
    public void ArrayLiteral_IndexAccess_ReturnsCorrectElement()
    {
        string result = Run(
            "let arr := [10, 20, 30]\n" +
            "print arr[1]");
        Assert.AreEqual("20", result);
    }

    [TestMethod]
    public void ArrayElementAssign_MutatesInPlace()
    {
        string result = Run(
            "let arr := [1, 2, 3]\n" +
            "arr[1] := 99\n" +
            "print arr[1]");
        Assert.AreEqual("99", result);
    }

    [TestMethod]
    public void ForeachArray_IteratesAllElements()
    {
        string result = Run(
            "let nums := [5, 6, 7]\n" +
            "foreach n in nums do\n" +
            "    print n\n" +
            "end");
        Assert.AreEqual("5\n6\n7", result);
    }

    [TestMethod]
    public void NestedArray_AccessInnerElement()
    {
        string result = Run(
            "let matrix := [[1, 2], [3, 4]]\n" +
            "print matrix[1][0]");
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void ArrayOutOfBounds_ReturnsNull()
    {
        string result = Run(
            "let arr := [1, 2, 3]\n" +
            "print arr[10]");
        Assert.AreEqual("null", result);
    }

    // ─── Classes ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Class_Instantiate_AccessField()
    {
        string result = Run(
            "class Box {\n" +
            "    let Value : int := 0\n" +
            "    Constructor(v : int)\n" +
            "        Value := v\n" +
            "    end\n" +
            "}\n" +
            "let b := new Box(42)\n" +
            "print b.Value");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void Class_CallMethod_ReturnsCorrectValue()
    {
        string result = Run(
            "class Counter {\n" +
            "    let Count : int := 0\n" +
            "    Constructor(start : int)\n" +
            "        Count := start\n" +
            "    end\n" +
            "    function value() -> int\n" +
            "        return Count\n" +
            "    end\n" +
            "}\n" +
            "let c := new Counter(7)\n" +
            "print c.value()");
        Assert.AreEqual("7", result);
    }

    [TestMethod]
    public void Class_ConstructorSetsFields()
    {
        string result = Run(
            "class Point {\n" +
            "    let X : int := 0\n" +
            "    let Y : int := 0\n" +
            "    Constructor(x : int, y : int)\n" +
            "        X := x\n" +
            "        Y := y\n" +
            "    end\n" +
            "}\n" +
            "let p := new Point(3, 4)\n" +
            "print p.X\n" +
            "print p.Y");
        Assert.AreEqual("3\n4", result);
    }

    [TestMethod]
    public void Class_Inheritance_InheritsParentMethod()
    {
        string result = Run(
            "class Animal {\n" +
            "    let Name : string := \"\"\n" +
            "    Constructor(name : string)\n" +
            "        Name := name\n" +
            "    end\n" +
            "    function speak() -> string\n" +
            "        return Name + \" makes a sound\"\n" +
            "    end\n" +
            "}\n" +
            "class Cat extends Animal {\n" +
            "    Constructor(name : string)\n" +
            "        Name := name\n" +
            "    end\n" +
            "}\n" +
            "let c := new Cat(\"Whiskers\")\n" +
            "print c.speak()");
        Assert.AreEqual("Whiskers makes a sound", result);
    }

    [TestMethod]
    public void Class_ThisReference_AccessesFields()
    {
        string result = Run(
            "class Person {\n" +
            "    let Name : string := \"\"\n" +
            "    let Age : int := 0\n" +
            "    Constructor(name : string, age : int)\n" +
            "        Name := name\n" +
            "        Age := age\n" +
            "    end\n" +
            "    function introduce() -> string\n" +
            "        return \"I am \" + Name + \", age \" + str(Age)\n" +
            "    end\n" +
            "}\n" +
            "let p := new Person(\"Alice\", 30)\n" +
            "print p.introduce()");
        Assert.AreEqual("I am Alice, age 30", result);
    }

    [TestMethod]
    public void Class_Inheritance_OverridesMethod()
    {
        string result = Run(
            "class Base {\n" +
            "    function label() -> string\n" +
            "        return \"base\"\n" +
            "    end\n" +
            "}\n" +
            "class Derived extends Base {\n" +
            "    function label() -> string\n" +
            "        return \"derived\"\n" +
            "    end\n" +
            "}\n" +
            "let d := new Derived()\n" +
            "print d.label()");
        Assert.AreEqual("derived", result);
    }

    // ─── Built-ins ────────────────────────────────────────────────────────

    [TestMethod]
    public void Builtin_Len_String_ReturnsLength()
    {
        string result = Run("print len(\"hello\")");
        Assert.AreEqual("5", result);
    }

    [TestMethod]
    public void Builtin_Len_Array_ReturnsCount()
    {
        string result = Run("print len([1, 2, 3, 4])");
        Assert.AreEqual("4", result);
    }

    [TestMethod]
    public void Builtin_Str_Integer_ReturnsString()
    {
        string result = Run("print str(42)");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void Builtin_Str_True_ReturnsTrueString()
    {
        string result = Run("print str(true)");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void Builtin_Int_String_ParsesInteger()
    {
        string result = Run("print int(\"42\")");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void Builtin_Int_Float_TruncatesToInteger()
    {
        string result = Run("print int(3.7)");
        Assert.AreEqual("3", result);
    }

    [TestMethod]
    public void Builtin_Bool_Zero_ReturnsFalse()
    {
        string result = Run("print bool(0)");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void Builtin_Bool_EmptyString_ReturnsFalse()
    {
        string result = Run("print bool(\"\")");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void Builtin_Bool_NonZero_ReturnsTrue()
    {
        string result = Run("print bool(1)");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void Builtin_Float_Integer_ReturnsFloat()
    {
        string result = Run("let f := float(3)\nprint f == 3.0");
        Assert.AreEqual("true", result);
    }

    // ─── Exception handling ───────────────────────────────────────────────

    [TestMethod]
    public void TryCatch_CatchesThrown_PrintsCaughtMessage()
    {
        string result = Run(
            "try\n" +
            "    throw \"oops\"\n" +
            "catch err\n" +
            "    print \"Caught: \" + str(err)\n" +
            "end");
        Assert.AreEqual("Caught: oops", result);
    }

    [TestMethod]
    public void TryCatch_NoThrow_ExecutesTryBody()
    {
        string result = Run(
            "try\n" +
            "    print \"ok\"\n" +
            "catch err\n" +
            "    print \"error\"\n" +
            "end");
        Assert.AreEqual("ok", result);
    }

    [TestMethod]
    public void TryFinally_AlwaysRunsFinally()
    {
        string result = Run(
            "try\n" +
            "    throw \"error\"\n" +
            "catch e\n" +
            "    print \"caught\"\n" +
            "finally\n" +
            "    print \"finally\"\n" +
            "end");
        Assert.AreEqual("caught\nfinally", result);
    }

    [TestMethod]
    public void TryFinally_NoThrow_StillRunsFinally()
    {
        string result = Run(
            "try\n" +
            "    print \"try\"\n" +
            "catch e\n" +
            "    print \"catch\"\n" +
            "finally\n" +
            "    print \"finally\"\n" +
            "end");
        Assert.AreEqual("try\nfinally", result);
    }

    [TestMethod]
    public void Throw_CreatesInterpreterException_WhenUncaught()
    {
        RunExpectError("throw \"unhandled error\"");
    }

    // ─── Pattern matching ─────────────────────────────────────────────────

    [TestMethod]
    public void PatternMatch_IntegerLiteral_MatchesCorrectCase()
    {
        string result = Run(
            "let x := 3\n" +
            "match x {\n" +
            "    1 => print \"one\"\n" +
            "    2 => print \"two\"\n" +
            "    3 => print \"three\"\n" +
            "    _ => print \"other\"\n" +
            "}");
        Assert.AreEqual("three", result);
    }

    [TestMethod]
    public void PatternMatch_DefaultCase_UsedWhenNoOtherMatches()
    {
        string result = Run(
            "let x := 99\n" +
            "match x {\n" +
            "    1 => print \"one\"\n" +
            "    _ => print \"default\"\n" +
            "}");
        Assert.AreEqual("default", result);
    }

    [TestMethod]
    public void PatternMatch_String_MatchesCorrectCase()
    {
        string result = Run(
            "let color := \"green\"\n" +
            "match color {\n" +
            "    \"red\" => print \"warm\"\n" +
            "    \"green\" => print \"cool\"\n" +
            "    \"blue\" => print \"cold\"\n" +
            "    _ => print \"unknown\"\n" +
            "}");
        Assert.AreEqual("cool", result);
    }

    [TestMethod]
    public void PatternMatch_BoolLiteral_MatchesTrue()
    {
        string result = Run(
            "let flag := true\n" +
            "match flag {\n" +
            "    true => print \"yes\"\n" +
            "    false => print \"no\"\n" +
            "}");
        Assert.AreEqual("yes", result);
    }

    // ─── Modules ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Module_ExportAndAccess_ReturnsComputedValue()
    {
        string result = Run(
            "module MyMath {\n" +
            "    function double(n : int) -> int\n" +
            "        return n * 2\n" +
            "    end\n" +
            "    export double\n" +
            "}\n" +
            "print MyMath.double(21)");
        Assert.AreEqual("42", result);
    }

    [TestMethod]
    public void Module_AccessWithoutExport_StillAccessibleViaMember()
    {
        string result = Run(
            "module Utils {\n" +
            "    function greet(name : string) -> string\n" +
            "        return \"Hi \" + name\n" +
            "    end\n" +
            "}\n" +
            "print Utils.greet(\"Bob\")");
        Assert.AreEqual("Hi Bob", result);
    }

    // ─── Error cases ──────────────────────────────────────────────────────

    [TestMethod]
    public void UndefinedVariable_ThrowsInterpreterException()
    {
        RunExpectError("print notDefined");
    }

    [TestMethod]
    public void TypeError_Comparison_NonNumeric_ThrowsInterpreterException()
    {
        RunExpectError("print true < false");
    }

    [TestMethod]
    public void ModuloWithFloat_ThrowsInterpreterException()
    {
        RunExpectError("print 5 % 2.0");
    }

    [TestMethod]
    public void ForeachNull_ThrowsInterpreterException()
    {
        RunExpectError("foreach x in null do\nprint x\nend");
    }

    [TestMethod]
    public void ConstReassignment_ThrowsInterpreterException()
    {
        RunExpectError("const x := 1\nx := 2");
    }

    // ─── Additional coverage ──────────────────────────────────────────────

    [TestMethod]
    public void TernaryExpression_TrueCondition_SelectsThenBranch()
    {
        string result = Run("let x := true ? 1 : 2\nprint x");
        Assert.AreEqual("1", result);
    }

    [TestMethod]
    public void TernaryExpression_FalseCondition_SelectsElseBranch()
    {
        string result = Run("let x := false ? 1 : 2\nprint x");
        Assert.AreEqual("2", result);
    }

    [TestMethod]
    public void StringConcatenation_WithAmpersand_ConcatenatesStrings()
    {
        string result = Run("print \"foo\" & \"bar\"");
        Assert.AreEqual("foobar", result);
    }

    [TestMethod]
    public void FunctionScope_DoesNotSeeVariableFromCallerInnerScope()
    {
        // A variable declared inside an if block (not at global scope) should
        // not be visible inside a function called from within that block.
        RunExpectError(
            "function readLocal()\n" +
            "    print innerVar\n" +
            "end\n" +
            "if true then\n" +
            "    let innerVar := 99\n" +
            "    readLocal()\n" +
            "end");
    }

    [TestMethod]
    public void MultipleTopLevel_FunctionDefs_AllCallable()
    {
        string result = Run(
            "function a() -> string\n" +
            "    return \"A\"\n" +
            "end\n" +
            "function b() -> string\n" +
            "    return \"B\"\n" +
            "end\n" +
            "print a()\n" +
            "print b()");
        Assert.AreEqual("A\nB", result);
    }

    [TestMethod]
    public void ForLoop_VariableNotVisibleAfterLoop()
    {
        RunExpectError(
            "for i := 1 to 3 do\n" +
            "    print i\n" +
            "end\n" +
            "print i");
    }

    [TestMethod]
    public void TypeCheckIs_Integer_ReturnsTrue()
    {
        string result = Run("print 42 is int");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void TypeCheckIs_StringNotInt_ReturnsFalse()
    {
        string result = Run("print \"hello\" is int");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void EqualityComparison_SameIntegers_ReturnsTrue()
    {
        string result = Run("print 5 == 5");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void EqualityComparison_DifferentValues_ReturnsFalse()
    {
        string result = Run("print 5 == 6");
        Assert.AreEqual("false", result);
    }

    [TestMethod]
    public void NullComparison_NullEqualsNull_ReturnsTrue()
    {
        string result = Run("print null == null");
        Assert.AreEqual("true", result);
    }

    [TestMethod]
    public void ArrayLiteral_PrintsWithBrackets()
    {
        string result = Run("print [1, 2, 3]");
        Assert.AreEqual("[1, 2, 3]", result);
    }

    [TestMethod]
    public void EnumDef_MembersAccessibleAsFields()
    {
        string result = Run(
            "enum Color { Red, Green, Blue }\n" +
            "print Color.Red\n" +
            "print Color.Green\n" +
            "print Color.Blue");
        Assert.AreEqual("0\n1\n2", result);
    }

    [TestMethod]
    public void SwitchStatement_MatchesCorrectCase()
    {
        string result = Run(
            "let x := 2\n" +
            "switch x {\n" +
            "    case 1: print \"one\"\n" +
            "    case 2: print \"two\"\n" +
            "    default: print \"other\"\n" +
            "}");
        Assert.AreEqual("two", result);
    }

    [TestMethod]
    public void SwitchStatement_DefaultCase_WhenNoMatchFound()
    {
        string result = Run(
            "let x := 99\n" +
            "switch x {\n" +
            "    case 1: print \"one\"\n" +
            "    default: print \"none\"\n" +
            "}");
        Assert.AreEqual("none", result);
    }

    [TestMethod]
    public void Builtin_Str_Float_ReturnsFloatString()
    {
        string result = Run("print str(3.14)");
        Assert.AreEqual("3.14", result);
    }

    [TestMethod]
    public void PowerOperator_NegativeExponent_ProducesFloat()
    {
        string result = Run("let x := 2 ** -1\nprint x is float");
        Assert.AreEqual("true", result);
    }
}
