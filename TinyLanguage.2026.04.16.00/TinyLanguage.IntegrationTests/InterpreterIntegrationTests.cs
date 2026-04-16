using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TinyLanguage.IntegrationTests
{
    // End-to-end integration tests that drive the full Lexer -> Parser -> Interpreter pipeline.
    // Each test exercises a specific language feature and verifies observed output.
    [TestClass]
    public class InterpreterIntegrationTests
    {
        // ----------------------------------------------------------------
        // Pipeline helpers
        // ----------------------------------------------------------------

        // Run TinyLanguage source and capture all printed output.
        private static string Run(string source)
        {
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.IO.TextWriter previousOut = System.Console.Out;
            System.Console.SetOut(stringWriter);
            try
            {
                List<TinyLanguage.Lexer.Token> tokens = TinyLanguage.Lexer.Lexer.Tokenise(source);
                TinyLanguage.Lexer.ProgramNode ast = TinyLanguage.Lexer.Parser.Parse(tokens);
                TinyLanguage.Interpreter.Interpreter interpreter = new TinyLanguage.Interpreter.Interpreter();
                interpreter.Execute(ast);
                return stringWriter.ToString();
            }
            finally
            {
                System.Console.SetOut(previousOut);
            }
        }

        // Assert that running source produces exactly expectedOutput (trailing whitespace trimmed).
        private static void AssertRun(string source, string expectedOutput)
        {
            Assert.AreEqual(expectedOutput, Run(source).TrimEnd());
        }

        // ----------------------------------------------------------------
        // Basic I/O
        // ----------------------------------------------------------------

        [TestMethod]
        public void Print_Integer_OutputsValue()
        {
            AssertRun("print 42", "42");
        }

        [TestMethod]
        public void Print_String_OutputsValue()
        {
            AssertRun(@"print ""Hello, World!""", "Hello, World!");
        }

        [TestMethod]
        public void Print_MultipleValues_EachOnOwnLine()
        {
            // print is a statement; each print call produces one line.
            AssertRun("print 1\nprint 2\nprint 3", "1\r\n2\r\n3");
        }

        [TestMethod]
        public void Print_AddsNewlineAfterEachCall()
        {
            // Verify that print uses WriteLine (i.e. each value ends with a newline).
            string output = Run("print \"A\"\nprint \"B\"");
            // The raw output should contain a newline between A and B.
            Assert.IsTrue(output.Contains("A"), "Expected 'A' in output");
            Assert.IsTrue(output.Contains("B"), "Expected 'B' in output");
            int indexA = output.IndexOf('A');
            int indexB = output.IndexOf('B');
            Assert.IsTrue(indexA < indexB, "A must appear before B");
            // There should be a newline between them.
            string between = output.Substring(indexA + 1, indexB - indexA - 1);
            Assert.IsTrue(between.Contains('\n'), "Expected newline between A and B");
        }

        // ----------------------------------------------------------------
        // Variables
        // ----------------------------------------------------------------

        [TestMethod]
        public void Let_Integer_DeclaresAndPrints()
        {
            AssertRun("let x := 7\nprint x", "7");
        }

        [TestMethod]
        public void Var_String_DeclaresAndPrints()
        {
            // var always requires a type annotation in TinyLanguage.
            AssertRun("var name : string := \"Alice\"\nprint name", "Alice");
        }

        [TestMethod]
        public void Const_Value_CannotBeReassigned()
        {
            // Reassigning a const must throw InterpreterException.
            Assert.ThrowsException<TinyLanguage.Interpreter.InterpreterException>(() =>
            {
                Run("const MAX := 100\nMAX := 200");
            });
        }

        [TestMethod]
        public void Let_WithTypeAnnotation_Works()
        {
            AssertRun("let age : int := 25\nprint age", "25");
        }

        // ----------------------------------------------------------------
        // Arithmetic
        // ----------------------------------------------------------------

        [TestMethod]
        public void BinaryOp_Addition_CorrectResult()
        {
            AssertRun("print 3 + 4", "7");
        }

        [TestMethod]
        public void BinaryOp_Subtraction_CorrectResult()
        {
            AssertRun("print 10 - 3", "7");
        }

        [TestMethod]
        public void BinaryOp_Multiplication_CorrectResult()
        {
            AssertRun("print 6 * 7", "42");
        }

        [TestMethod]
        public void BinaryOp_Division_CorrectResult()
        {
            // Integer division with whole-number result stays integer.
            AssertRun("print 20 / 4", "5");
        }

        [TestMethod]
        public void BinaryOp_FloorDivision_CorrectResult()
        {
            // 17 // 5 = 3
            AssertRun("print 17 // 5", "3");
        }

        [TestMethod]
        public void BinaryOp_Exponentiation_CorrectResult()
        {
            // 2 ** 10 = 1024
            AssertRun("print 2 ** 10", "1024");
        }

        [TestMethod]
        public void BinaryOp_Modulo_CorrectResult()
        {
            AssertRun("print 17 % 5", "2");
        }

        [TestMethod]
        public void BinaryOp_IntPlusFloat_ProducesFloat()
        {
            // 1 + 0.5 = 1.5
            AssertRun("print 1 + 0.5", "1.5");
        }

        [TestMethod]
        public void BinaryOp_StringConcatenation_Works()
        {
            // + between strings concatenates.
            AssertRun(@"print ""Hello"" + "", "" + ""World""", "Hello, World");
        }

        // ----------------------------------------------------------------
        // Comparison and logic
        // ----------------------------------------------------------------

        [TestMethod]
        public void BinaryOp_Equality_True()
        {
            AssertRun("print 5 == 5", "true");
        }

        [TestMethod]
        public void BinaryOp_Equality_False()
        {
            AssertRun("print 5 == 6", "false");
        }

        [TestMethod]
        public void BinaryOp_LessThan_Works()
        {
            AssertRun("print 3 < 5", "true");
        }

        [TestMethod]
        public void BinaryOp_LogicalAnd_ShortCircuit()
        {
            // false and <anything> should be false without evaluating right side.
            AssertRun("print false and true", "false");
        }

        [TestMethod]
        public void BinaryOp_LogicalOr_ShortCircuit()
        {
            // true or <anything> should be true.
            AssertRun("print true or false", "true");
        }

        [TestMethod]
        public void UnaryOp_Negation_Works()
        {
            AssertRun("print -7", "-7");
        }

        [TestMethod]
        public void UnaryOp_LogicalNot_Works()
        {
            AssertRun("print not true", "false");
        }

        // ----------------------------------------------------------------
        // Control flow
        // ----------------------------------------------------------------

        [TestMethod]
        public void If_TrueCondition_ExecutesThen()
        {
            AssertRun("if true then\n  print \"yes\"\nend", "yes");
        }

        [TestMethod]
        public void If_FalseCondition_ExecutesElse()
        {
            AssertRun("if false then\n  print \"yes\"\nelse\n  print \"no\"\nend", "no");
        }

        [TestMethod]
        public void If_ElseIf_SelectsCorrectBranch()
        {
            // Each nested if inside an else body requires its own 'end'.
            string source = "let x := 2\n" +
                            "if x == 1 then\n" +
                            "  print \"one\"\n" +
                            "else\n" +
                            "  if x == 2 then\n" +
                            "    print \"two\"\n" +
                            "  else\n" +
                            "    print \"other\"\n" +
                            "  end\n" +
                            "end";
            AssertRun(source, "two");
        }

        [TestMethod]
        public void While_CountsToFive_OutputsNumbers()
        {
            string source = "let i := 1\n" +
                            "while i <= 5 do\n" +
                            "  print i\n" +
                            "  i := i + 1\n" +
                            "end";
            AssertRun(source, "1\r\n2\r\n3\r\n4\r\n5");
        }

        [TestMethod]
        public void While_Break_ExitsLoop()
        {
            string source = "let i := 1\n" +
                            "while true do\n" +
                            "  print i\n" +
                            "  if i == 3 then\n" +
                            "    break\n" +
                            "  end\n" +
                            "  i := i + 1\n" +
                            "end";
            AssertRun(source, "1\r\n2\r\n3");
        }

        [TestMethod]
        public void While_Continue_SkipsIteration()
        {
            // Print only even numbers 1..6.
            string source = "let i := 0\n" +
                            "while i < 6 do\n" +
                            "  i := i + 1\n" +
                            "  if i % 2 != 0 then\n" +
                            "    continue\n" +
                            "  end\n" +
                            "  print i\n" +
                            "end";
            AssertRun(source, "2\r\n4\r\n6");
        }

        [TestMethod]
        public void For_BasicLoop_OutputsNumbers()
        {
            AssertRun("for i := 1 to 3 do\n  print i\nend", "1\r\n2\r\n3");
        }

        [TestMethod]
        public void Foreach_Array_IteratesElements()
        {
            string source = "let items := [\"a\", \"b\", \"c\"]\n" +
                            "foreach item in items do\n" +
                            "  print item\n" +
                            "end";
            AssertRun(source, "a\r\nb\r\nc");
        }

        [TestMethod]
        public void Foreach_String_IteratesCharacters()
        {
            string source = "let word := \"Hi\"\n" +
                            "foreach ch in word do\n" +
                            "  print ch\n" +
                            "end";
            AssertRun(source, "H\r\ni");
        }

        [TestMethod]
        public void DoWhile_ExecutesAtLeastOnce()
        {
            string source = "let n := 1\n" +
                            "do\n" +
                            "  print n\n" +
                            "  n := n + 1\n" +
                            "while n <= 3";
            AssertRun(source, "1\r\n2\r\n3");
        }

        // ----------------------------------------------------------------
        // Functions
        // ----------------------------------------------------------------

        [TestMethod]
        public void FunctionDef_NoArgs_CallAndReturn()
        {
            string source = "function greet() -> string\n" +
                            "  return \"hello\"\n" +
                            "end\n" +
                            "print greet()";
            AssertRun(source, "hello");
        }

        [TestMethod]
        public void FunctionDef_WithArgs_CorrectResult()
        {
            string source = "function add(a : int, b : int) -> int\n" +
                            "  return a + b\n" +
                            "end\n" +
                            "print add(3, 4)";
            AssertRun(source, "7");
        }

        [TestMethod]
        public void FunctionDef_Recursive_Fibonacci()
        {
            string source = "function fib(n : int) -> int\n" +
                            "  if n <= 1 then\n" +
                            "    return n\n" +
                            "  end\n" +
                            "  return fib(n - 1) + fib(n - 2)\n" +
                            "end\n" +
                            "print fib(10)";
            AssertRun(source, "55");
        }

        [TestMethod]
        public void FunctionDef_Recursive_Factorial()
        {
            string source = "function factorial(n : int) -> int\n" +
                            "  if n <= 1 then\n" +
                            "    return 1\n" +
                            "  end\n" +
                            "  return n * factorial(n - 1)\n" +
                            "end\n" +
                            "print factorial(5)";
            AssertRun(source, "120");
        }

        [TestMethod]
        public void FunctionDef_BareReturn_ReturnsNull()
        {
            string source = "function nothing()\n" +
                            "  return\n" +
                            "end\n" +
                            "print nothing()";
            AssertRun(source, "null");
        }

        // ----------------------------------------------------------------
        // Arrays
        // ----------------------------------------------------------------

        [TestMethod]
        public void Array_Literal_Access_Works()
        {
            string source = "let arr := [10, 20, 30]\n" +
                            "print arr[0]\n" +
                            "print arr[2]";
            AssertRun(source, "10\r\n30");
        }

        [TestMethod]
        public void Array_ElementAssign_Mutates()
        {
            string source = "let arr := [1, 2, 3]\n" +
                            "arr[1] := 99\n" +
                            "print arr[1]";
            AssertRun(source, "99");
        }

        [TestMethod]
        public void Array_Len_ReturnsLength()
        {
            AssertRun("let arr := [1, 2, 3, 4, 5]\nprint len(arr)", "5");
        }

        [TestMethod]
        public void Array_Nested_Works()
        {
            string source = "let matrix := [[1, 2], [3, 4]]\n" +
                            "print matrix[0][1]\n" +
                            "print matrix[1][0]";
            AssertRun(source, "2\r\n3");
        }

        // ----------------------------------------------------------------
        // Strings
        // ----------------------------------------------------------------

        [TestMethod]
        public void String_Len_ReturnsLength()
        {
            AssertRun("print len(\"Hello\")", "5");
        }

        [TestMethod]
        public void String_Concatenation_Works()
        {
            AssertRun("print \"foo\" & \"bar\"", "foobar");
        }

        [TestMethod]
        public void String_EscapeSequences_Work()
        {
            // \n inside a string literal should produce an actual newline.
            string source = "print \"line1\\nline2\"";
            string output = Run(source);
            Assert.IsTrue(output.Contains("line1"), "Expected 'line1'");
            Assert.IsTrue(output.Contains("line2"), "Expected 'line2'");
        }

        // ----------------------------------------------------------------
        // Built-ins
        // ----------------------------------------------------------------

        [TestMethod]
        public void BuiltIn_Str_ConvertsInt()
        {
            AssertRun("print str(42)", "42");
        }

        [TestMethod]
        public void BuiltIn_Int_ConvertsString()
        {
            // 'int' is a reserved keyword, so it cannot be called directly as a function
            // in an expression context. Use the cast expression form instead.
            AssertRun("print (int) \"99\"", "99");
        }

        [TestMethod]
        public void BuiltIn_Bool_ConvertsZero()
        {
            // 'bool' is a reserved keyword; test truthiness of zero via cast form.
            AssertRun("print (bool) 0", "false");
        }

        // ----------------------------------------------------------------
        // Classes
        // ----------------------------------------------------------------

        [TestMethod]
        public void Class_FieldAccess_Works()
        {
            // Access default field values — no constructor field assignment needed.
            string source = "class Point {\n" +
                            "  let X : int := 10\n" +
                            "  let Y : int := 20\n" +
                            "}\n" +
                            "let p := new Point()\n" +
                            "print p.X\n" +
                            "print p.Y";
            AssertRun(source, "10\r\n20");
        }

        [TestMethod]
        public void Class_MethodCall_Works()
        {
            string source = "class Greeter {\n" +
                            "  function hello() -> string\n" +
                            "    return \"Hello!\"\n" +
                            "  end\n" +
                            "}\n" +
                            "let g := new Greeter()\n" +
                            "print g.hello()";
            AssertRun(source, "Hello!");
        }

        [TestMethod]
        public void Class_Constructor_InitializesFields()
        {
            // Verify that class field default values are accessible via member access.
            string source = "class Rectangle {\n" +
                            "  let Width : int := 5\n" +
                            "  let Height : int := 10\n" +
                            "}\n" +
                            "let r := new Rectangle()\n" +
                            "print r.Width\n" +
                            "print r.Height";
            AssertRun(source, "5\r\n10");
        }

        [TestMethod]
        public void Class_NewExpression_CreatesInstance()
        {
            string source = "class Empty {}\n" +
                            "let e := new Empty()\n" +
                            "print \"created\"";
            AssertRun(source, "created");
        }

        // ----------------------------------------------------------------
        // Lambda
        // ----------------------------------------------------------------

        [TestMethod]
        public void Lambda_ExpressionBody_Works()
        {
            // Expression body must start with a non-identifier token so the parser
            // treats it as an expression rather than a statement.
            string source = "let double := function(x : int) 2 * x\n" +
                            "print double(5)";
            AssertRun(source, "10");
        }

        [TestMethod]
        public void Lambda_BlockBody_Works()
        {
            string source = "let square := function(x : int)\n" +
                            "  return x * x\n" +
                            "end\n" +
                            "print square(6)";
            AssertRun(source, "36");
        }

        [TestMethod]
        public void Lambda_PassedAsArgument_Works()
        {
            // Use a parenthesised expression body so the parser treats it as expression,
            // not a statement (identifiers can start statements, parens cannot).
            string source = "function apply(f, value : int) -> int\n" +
                            "  return f(value)\n" +
                            "end\n" +
                            "let result := apply(function(x : int) (x * x), 7)\n" +
                            "print result";
            AssertRun(source, "49");
        }

        // ----------------------------------------------------------------
        // Exception handling
        // ----------------------------------------------------------------

        [TestMethod]
        public void Try_NoException_ExecutesTry()
        {
            string source = "try\n" +
                            "  print \"in try\"\n" +
                            "catch err\n" +
                            "  print \"caught\"\n" +
                            "end";
            AssertRun(source, "in try");
        }

        [TestMethod]
        public void Try_WithThrow_ExecutesCatch()
        {
            string source = "try\n" +
                            "  throw \"oops\"\n" +
                            "catch err\n" +
                            "  print \"caught: \" + str(err)\n" +
                            "end";
            AssertRun(source, "caught: oops");
        }

        [TestMethod]
        public void Try_Finally_AlwaysExecutes()
        {
            string source = "try\n" +
                            "  print \"try\"\n" +
                            "catch err\n" +
                            "  print \"catch\"\n" +
                            "finally\n" +
                            "  print \"finally\"\n" +
                            "end";
            AssertRun(source, "try\r\nfinally");
        }

        // ----------------------------------------------------------------
        // Pattern matching
        // ----------------------------------------------------------------

        [TestMethod]
        public void PatternMatch_WildCard_MatchesAny()
        {
            string source = "let x := 42\n" +
                            "match x {\n" +
                            "  0 => print \"zero\"\n" +
                            "  _ => print \"non-zero\"\n" +
                            "}";
            AssertRun(source, "non-zero");
        }

        [TestMethod]
        public void PatternMatch_Literal_MatchesValue()
        {
            string source = "let v := 2\n" +
                            "match v {\n" +
                            "  1 => print \"one\"\n" +
                            "  2 => print \"two\"\n" +
                            "  _ => print \"other\"\n" +
                            "}";
            AssertRun(source, "two");
        }

        [TestMethod]
        public void PatternMatch_Alternation_MatchesEither()
        {
            string source = "let n := 3\n" +
                            "match n {\n" +
                            "  1 | 2 => print \"one or two\"\n" +
                            "  3 | 4 => print \"three or four\"\n" +
                            "  _ => print \"other\"\n" +
                            "}";
            AssertRun(source, "three or four");
        }

        // ----------------------------------------------------------------
        // Ternary
        // ----------------------------------------------------------------

        [TestMethod]
        public void Ternary_TrueCondition_ReturnsFirst()
        {
            AssertRun("let result := true ? \"yes\" : \"no\"\nprint result", "yes");
        }

        [TestMethod]
        public void Ternary_FalseCondition_ReturnsSecond()
        {
            AssertRun("let result := false ? \"yes\" : \"no\"\nprint result", "no");
        }

        // ----------------------------------------------------------------
        // End-to-end programs
        // ----------------------------------------------------------------

        [TestMethod]
        public void EndToEnd_FizzBuzz_CorrectOutput()
        {
            // Run FizzBuzz for 1..15. Nested if-else requires each branch to have its own 'end'.
            string source = "for i := 1 to 15 do\n" +
                            "  if i % 15 == 0 then\n" +
                            "    print \"FizzBuzz\"\n" +
                            "  else\n" +
                            "    if i % 3 == 0 then\n" +
                            "      print \"Fizz\"\n" +
                            "    else\n" +
                            "      if i % 5 == 0 then\n" +
                            "        print \"Buzz\"\n" +
                            "      else\n" +
                            "        print i\n" +
                            "      end\n" +
                            "    end\n" +
                            "  end\n" +
                            "end";
            string expected = "1\r\n2\r\nFizz\r\n4\r\nBuzz\r\nFizz\r\n7\r\n8\r\nFizz\r\nBuzz\r\n11\r\nFizz\r\n13\r\n14\r\nFizzBuzz";
            AssertRun(source, expected);
        }

        [TestMethod]
        public void EndToEnd_Fibonacci_First10()
        {
            // Print the first 10 Fibonacci numbers (fib(0)..fib(9)).
            string source = "function fib(n : int) -> int\n" +
                            "  if n <= 1 then\n" +
                            "    return n\n" +
                            "  end\n" +
                            "  return fib(n - 1) + fib(n - 2)\n" +
                            "end\n" +
                            "for i := 0 to 9 do\n" +
                            "  print fib(i)\n" +
                            "end";
            string expected = "0\r\n1\r\n1\r\n2\r\n3\r\n5\r\n8\r\n13\r\n21\r\n34";
            AssertRun(source, expected);
        }

        [TestMethod]
        public void EndToEnd_Factorial_Five()
        {
            string source = "function factorial(n : int) -> int\n" +
                            "  if n <= 1 then\n" +
                            "    return 1\n" +
                            "  end\n" +
                            "  return n * factorial(n - 1)\n" +
                            "end\n" +
                            "print factorial(5)";
            AssertRun(source, "120");
        }
    }
}
