import os

DEMO_DIR = "Z:/repos/TinyLanguage/TinyLanguage.2026.04.16.19/TinyLanguage.DemoFiles"

def write_file(num, name, content):
    filename = f"{num:05d}.{name}"
    tlg_path = os.path.join(DEMO_DIR, f"{filename}.tlg")
    cmd_path = os.path.join(DEMO_DIR, f"{filename}.cmd")
    with open(tlg_path, 'w') as f:
        f.write(content)
    with open(cmd_path, 'w') as f:
        f.write(f"@echo off\necho Running {filename}.tlg\nif \"%2\"==\"\" (\n    TinyLanguage.exe %~dp0{filename}.tlg output.txt\n) else (\n    TinyLanguage.exe %~dp0{filename}.tlg %2\n)\n")

# 1-10: Basic I/O
write_file(1, "fizzbuzz",
"# Demo: FizzBuzz classic\n"
"for i := 1 to 20 do\n"
"    if i % 15 == 0 then\n"
'        print "FizzBuzz"\n'
"    else if i % 3 == 0 then\n"
'        print "Fizz"\n'
"    else if i % 5 == 0 then\n"
'        print "Buzz"\n'
"    else\n"
"        print i\n"
"    end\n"
"end")

write_file(2, "fibonacci",
"# Demo: Fibonacci sequence\n"
"function fib(n : int) -> int\n"
"    if n <= 1 then\n"
"        return n\n"
"    end\n"
"    return fib(n - 1) + fib(n - 2)\n"
"end\n"
"for i := 0 to 10 do\n"
"    print fib(i)\n"
"end")

write_file(3, "hello_world",
"# Demo: Hello World\n"
'print "Hello, World!"')

write_file(4, "hello_name",
"# Demo: greeting with variable\n"
'let name := "TinyLanguage"\n'
'print "Hello, " + name + "!"')

write_file(5, "print_integer",
"# Demo: print integer literals\n"
"print 42\n"
"print 0\n"
"print 1000\n"
"print -7")

write_file(6, "print_float",
"# Demo: print float literals\n"
"print 3.14\n"
"print 0.0\n"
"print 2.718\n"
"print -1.5")

write_file(7, "print_boolean",
"# Demo: print boolean literals\n"
"print true\n"
"print false")

write_file(8, "print_string",
"# Demo: print string literals\n"
'print "hello"\n'
'print "world"\n'
'print "TinyLanguage demo"')

write_file(9, "print_null",
"# Demo: print null value\n"
"print null")

write_file(10, "print_expression",
"# Demo: print evaluated expressions\n"
"print 2 + 3\n"
"print 10 - 4\n"
"print 3 * 7\n"
"print 15 / 5")

# 11-20: Variables
write_file(11, "let_declaration",
"# Demo: let variable declarations\n"
"let x := 10\n"
"let y := 20\n"
"print x\n"
"print y\n"
"print x + y")

write_file(12, "var_declaration",
"# Demo: var variable with type annotation\n"
"var count : int := 0\n"
"var name : string := \"Alice\"\n"
"var pi : float := 3.14\n"
"print count\n"
"print name\n"
"print pi")

write_file(13, "const_declaration",
"# Demo: const declarations\n"
"const MAX := 100\n"
"const PI := 3.14159\n"
'const GREETING := "Hello"\n'
"print MAX\n"
"print PI\n"
"print GREETING")

write_file(14, "assignment",
"# Demo: variable reassignment\n"
"let x := 5\n"
"print x\n"
"x := 10\n"
"print x\n"
"x := x + 1\n"
"print x")

write_file(15, "augmented_add",
"# Demo: augmented addition via reassignment\n"
"let total := 0\n"
"total := total + 10\n"
"total := total + 20\n"
"total := total + 30\n"
"print total")

write_file(16, "augmented_subtract",
"# Demo: augmented subtraction via reassignment\n"
"let n := 100\n"
"n := n - 25\n"
"n := n - 15\n"
"print n")

write_file(17, "augmented_multiply",
"# Demo: augmented multiplication via reassignment\n"
"let val := 2\n"
"val := val * 3\n"
"val := val * 4\n"
"print val")

write_file(18, "let_typed",
"# Demo: let with type annotation\n"
"let count : int := 42\n"
"let ratio : float := 0.75\n"
'let message : string := "typed"\n'
"let flag : bool := true\n"
"print count\n"
"print ratio\n"
"print message\n"
"print flag")

write_file(19, "multiple_variables",
"# Demo: multiple variables in program\n"
"let a := 1\n"
"let b := 2\n"
"let c := 3\n"
"let sumAll := a + b + c\n"
"print sumAll\n"
"let product := a * b * c\n"
"print product")

write_file(20, "variable_scope_outer",
"# Demo: outer scope variable updated from inner block\n"
"let counter := 0\n"
"while counter < 5 do\n"
"    counter := counter + 1\n"
"end\n"
"print counter")

# 21-30: Arithmetic
write_file(21, "arithmetic_add",
"# Demo: addition operator\n"
"print 1 + 1\n"
"print 5 + 3\n"
"print 100 + 200\n"
"print 0 + 0")

write_file(22, "arithmetic_subtract",
"# Demo: subtraction operator\n"
"print 10 - 3\n"
"print 0 - 5\n"
"print 100 - 50")

write_file(23, "arithmetic_multiply",
"# Demo: multiplication operator\n"
"print 4 * 5\n"
"print 3 * 3\n"
"print 0 * 100\n"
"print 7 * 8")

write_file(24, "arithmetic_divide",
"# Demo: division operator\n"
"print 10 / 2\n"
"print 7 / 2\n"
"print 1 / 4")

write_file(25, "arithmetic_floor_div",
"# Demo: floor division operator (//)\n"
"print 10 // 3\n"
"print 7 // 2\n"
"print 100 // 7\n"
"print 15 // 5")

write_file(26, "arithmetic_modulo",
"# Demo: modulo operator\n"
"print 10 % 3\n"
"print 7 % 2\n"
"print 15 % 5\n"
"print 0 % 7")

write_file(27, "arithmetic_power",
"# Demo: exponentiation operator (**)\n"
"print 2 ** 10\n"
"print 3 ** 3\n"
"print 5 ** 2\n"
"print 10 ** 0")

write_file(28, "operator_precedence",
"# Demo: operator precedence\n"
"print 2 + 3 * 4\n"
"print (2 + 3) * 4\n"
"print 10 - 2 * 3\n"
"print 2 ** 3 + 1\n"
"print 10 // 3 + 1")

write_file(29, "float_arithmetic",
"# Demo: float arithmetic\n"
"print 1.5 + 2.5\n"
"print 3.14 * 2.0\n"
"print 10.0 / 4.0\n"
"print 2.5 ** 2")

write_file(30, "mixed_arithmetic",
"# Demo: mixed integer and float arithmetic\n"
"print 1 + 2.0\n"
"print 3 * 1.5\n"
"print 10 / 3\n"
"let x : float := 2.5\n"
"print x * 4")

# 31-40: Comparisons
write_file(31, "compare_equal",
"# Demo: equality comparison\n"
"print 5 == 5\n"
"print 5 == 6\n"
'print "hello" == "hello"\n'
'print "hello" == "world"')

write_file(32, "compare_not_equal",
"# Demo: not-equal comparison\n"
"print 5 != 6\n"
"print 5 != 5\n"
'print "a" != "b"\n'
'print "a" != "a"')

write_file(33, "compare_less",
"# Demo: less-than comparison\n"
"print 3 < 5\n"
"print 5 < 3\n"
"print 5 < 5\n"
"print 1.5 < 2.0")

write_file(34, "compare_greater",
"# Demo: greater-than comparison\n"
"print 5 > 3\n"
"print 3 > 5\n"
"print 5 > 5\n"
"print 2.0 > 1.5")

write_file(35, "compare_less_equal",
"# Demo: less-than-or-equal comparison\n"
"print 3 <= 5\n"
"print 5 <= 5\n"
"print 6 <= 5\n"
"print 1.0 <= 1.0")

write_file(36, "compare_greater_equal",
"# Demo: greater-than-or-equal comparison\n"
"print 5 >= 3\n"
"print 5 >= 5\n"
"print 4 >= 5\n"
"print 2.0 >= 1.5")

write_file(37, "compare_strings",
"# Demo: string comparisons\n"
'print "apple" < "banana"\n'
'print "zebra" > "apple"\n'
'print "abc" == "abc"\n'
'print "abc" != "xyz"')

write_file(38, "compare_mixed",
"# Demo: comparisons in conditions\n"
"let x := 42\n"
"if x > 40 then\n"
'    print "greater than 40"\n'
"end\n"
"if x == 42 then\n"
'    print "equal to 42"\n'
"end\n"
"if x != 0 then\n"
'    print "not zero"\n'
"end")

write_file(39, "compare_boolean_result",
"# Demo: comparison results stored in variables\n"
"let a := 5\n"
"let b := 10\n"
"let isLess := a < b\n"
"let isEqual := a == b\n"
"print isLess\n"
"print isEqual")

write_file(40, "compare_chain_logic",
"# Demo: comparison with logical operators\n"
"let x := 7\n"
"print x > 0 and x < 10\n"
"print x < 0 or x > 5\n"
"print not (x == 7)")

# 41-50: Boolean logic
write_file(41, "boolean_and",
"# Demo: logical and operator\n"
"print true and true\n"
"print true and false\n"
"print false and true\n"
"print false and false")

write_file(42, "boolean_or",
"# Demo: logical or operator\n"
"print true or true\n"
"print true or false\n"
"print false or true\n"
"print false or false")

write_file(43, "boolean_not",
"# Demo: logical not operator\n"
"print not true\n"
"print not false\n"
"let x := true\n"
"print not x")

write_file(44, "boolean_and_or_combined",
"# Demo: and/or combined\n"
"let a := true\n"
"let b := false\n"
"let c := true\n"
"print a and b or c\n"
"print a or b and c\n"
"print not a or b")

write_file(45, "boolean_short_circuit",
"# Demo: boolean operator symbols && and ||\n"
"print true && false\n"
"print false || true\n"
"print true && true\n"
"print false || false")

write_file(46, "truthiness_integer",
"# Demo: integer truthiness\n"
"if 1 then\n"
'    print "1 is truthy"\n'
"end\n"
"if 0 then\n"
'    print "0 is truthy"\n'
"else\n"
'    print "0 is falsy"\n'
"end\n"
"if -5 then\n"
'    print "negative is truthy"\n'
"end")

write_file(47, "truthiness_string",
"# Demo: string truthiness\n"
'if "hello" then\n'
'    print "non-empty is truthy"\n'
"end\n"
'if "" then\n'
'    print "empty is truthy"\n'
"else\n"
'    print "empty is falsy"\n'
"end")

write_file(48, "truthiness_null",
"# Demo: null truthiness\n"
"if null then\n"
'    print "null is truthy"\n'
"else\n"
'    print "null is falsy"\n'
"end")

write_file(49, "boolean_variables",
"# Demo: boolean variables and operations\n"
"let flag := true\n"
"let other := false\n"
"print flag\n"
"print other\n"
"flag := not flag\n"
"print flag\n"
"print flag and other")

write_file(50, "bool_builtin",
"# Demo: bool() built-in function\n"
"print bool(0)\n"
"print bool(1)\n"
'print bool("")\n'
'print bool("text")\n'
"print bool(null)\n"
"print bool(true)")

# 51-60: String operations
write_file(51, "string_concat_plus",
"# Demo: string concatenation with +\n"
'let a := "Hello"\n'
'let b := ", "\n'
'let c := "World!"\n'
"print a + b + c")

write_file(52, "string_concat_amp",
"# Demo: string concatenation with &\n"
'print "Name: " & "Alice"\n'
"print \"Number: \" & 42\n"
"print \"Pi: \" & 3.14\n"
"print \"Flag: \" & true")

write_file(53, "string_len",
"# Demo: len() function on strings\n"
'print len("hello")\n'
'print len("")\n'
'print len("TinyLanguage")\n'
'let s := "test"\n'
"print len(s)")

write_file(54, "str_builtin",
"# Demo: str() built-in function\n"
"print str(42)\n"
"print str(3.14)\n"
"print str(true)\n"
"print str(false)\n"
"print str(null)")

write_file(55, "int_builtin",
"# Demo: int() built-in function\n"
'print int("42")\n'
'print int("0")\n'
"print int(3.7)\n"
"print int(true)\n"
"print int(false)")

write_file(56, "string_indexing",
"# Demo: string character indexing\n"
'let s := "hello"\n'
"print s[0]\n"
"print s[1]\n"
"print s[4]")

write_file(57, "string_foreach",
"# Demo: iterate string character by character\n"
'let word := "cat"\n'
"foreach ch in word do\n"
"    print ch\n"
"end")

write_file(58, "string_build",
"# Demo: build string incrementally\n"
'let result := ""\n'
"let i := 0\n"
"while i < 5 do\n"
"    result := result + str(i)\n"
"    i := i + 1\n"
"end\n"
"print result")

write_file(59, "string_number_mix",
"# Demo: number + string gives string concatenation\n"
'print 1 + " apple"\n'
'print "count: " + 5\n'
"print 3.14 + \" is pi\"")

write_file(60, "string_comparison_ops",
"# Demo: string comparison operations\n"
'let s1 := "alpha"\n'
'let s2 := "beta"\n'
"print s1 < s2\n"
"print s2 > s1\n"
"print s1 == s1\n"
"print s1 != s2")

# 61-70: Control flow - if
write_file(61, "if_simple",
"# Demo: simple if statement\n"
"let x := 10\n"
"if x > 5 then\n"
'    print "x is greater than 5"\n'
"end")

write_file(62, "if_else",
"# Demo: if-else statement\n"
"let n := 7\n"
"if n % 2 == 0 then\n"
'    print "even"\n'
"else\n"
'    print "odd"\n'
"end")

write_file(63, "if_elif_else",
"# Demo: if-elif-else chain\n"
"let score := 75\n"
"if score >= 90 then\n"
'    print "A"\n'
"else if score >= 80 then\n"
'    print "B"\n'
"else if score >= 70 then\n"
'    print "C"\n'
"else if score >= 60 then\n"
'    print "D"\n'
"else\n"
'    print "F"\n'
"end")

write_file(64, "if_nested",
"# Demo: nested if statements\n"
"let x := 5\n"
"let y := 10\n"
"if x > 0 then\n"
"    if y > 0 then\n"
'        print "both positive"\n'
"    else\n"
'        print "x positive, y not"\n'
"    end\n"
"else\n"
'    print "x not positive"\n'
"end")

write_file(65, "if_complex_condition",
"# Demo: complex conditions in if\n"
"let a := 5\n"
"let b := 10\n"
"let c := 15\n"
"if a < b and b < c then\n"
'    print "a < b < c"\n'
"end\n"
"if a == 5 or b == 5 then\n"
'    print "a or b is 5"\n'
"end")

write_file(66, "if_as_expression",
"# Demo: inline conditional expression\n"
"let x := 7\n"
"let label := if x % 2 == 0 then \"even\" else \"odd\"\n"
"print label\n"
"let y := 0\n"
"let signStr := if y > 0 then \"positive\" else if y < 0 then \"negative\" else \"zero\"\n"
"print signStr")

write_file(67, "ternary_operator",
"# Demo: ternary operator (?:)\n"
"let x := 10\n"
"let result := x > 5 ? \"big\" : \"small\"\n"
"print result\n"
"let n := -3\n"
"let absN := n >= 0 ? n : -n\n"
"print absN")

write_file(68, "if_multiline_body",
"# Demo: if with multiple statements in body\n"
"let x := 42\n"
"if x > 0 then\n"
'    print "positive"\n'
"    print x\n"
"    let doubled := x * 2\n"
"    print doubled\n"
"end")

write_file(69, "if_not_condition",
"# Demo: if with not in condition\n"
"let flag := false\n"
"if not flag then\n"
'    print "flag is false"\n'
"end\n"
"let x := 0\n"
"if not (x > 0) then\n"
'    print "x is not positive"\n'
"end")

write_file(70, "if_multiple_elif",
"# Demo: multiple elif branches\n"
"let day := 3\n"
"if day == 1 then\n"
'    print "Monday"\n'
"else if day == 2 then\n"
'    print "Tuesday"\n'
"else if day == 3 then\n"
'    print "Wednesday"\n'
"else if day == 4 then\n"
'    print "Thursday"\n'
"else if day == 5 then\n"
'    print "Friday"\n'
"else\n"
'    print "Weekend"\n'
"end")

# 71-80: Control flow - while
write_file(71, "while_simple",
"# Demo: simple while loop\n"
"let i := 1\n"
"while i <= 5 do\n"
"    print i\n"
"    i := i + 1\n"
"end")

write_file(72, "while_countdown",
"# Demo: while countdown\n"
"let n := 5\n"
"while n > 0 do\n"
"    print n\n"
"    n := n - 1\n"
"end\n"
'print "Blast off!"')

write_file(73, "while_sum",
"# Demo: while loop summing numbers\n"
"let sum := 0\n"
"let i := 1\n"
"while i <= 100 do\n"
"    sum := sum + i\n"
"    i := i + 1\n"
"end\n"
"print sum")

write_file(74, "while_break",
"# Demo: while with break\n"
"let i := 0\n"
"while i < 100 do\n"
"    if i == 5 then\n"
"        break\n"
"    end\n"
"    print i\n"
"    i := i + 1\n"
"end")

write_file(75, "while_continue",
"# Demo: while with continue\n"
"let i := 0\n"
"while i < 10 do\n"
"    i := i + 1\n"
"    if i % 2 == 0 then\n"
"        continue\n"
"    end\n"
"    print i\n"
"end")

write_file(76, "while_nested",
"# Demo: nested while loops\n"
"let i := 1\n"
"while i <= 3 do\n"
"    let j := 1\n"
"    while j <= 3 do\n"
"        print str(i) + \"x\" + str(j) + \"=\" + str(i * j)\n"
"        j := j + 1\n"
"    end\n"
"    i := i + 1\n"
"end")

write_file(77, "while_false_condition",
"# Demo: while that never executes\n"
"let x := 10\n"
"while x < 0 do\n"
'    print "never executed"\n'
"end\n"
'print "done"')

write_file(78, "while_with_array",
"# Demo: while loop building array\n"
"let data := []\n"
"let i := 0\n"
"while i < 5 do\n"
"    data[i] := i * i\n"
"    i := i + 1\n"
"end\n"
"foreach val in data do\n"
"    print val\n"
"end")

write_file(79, "while_find_first",
"# Demo: while to find first match\n"
"let nums := [3, 7, 2, 9, 4, 6, 1, 8, 5]\n"
"let i := 0\n"
"let found := -1\n"
"while i < len(nums) do\n"
"    if nums[i] > 7 then\n"
"        found := nums[i]\n"
"        break\n"
"    end\n"
"    i := i + 1\n"
"end\n"
"print found")

write_file(80, "while_gcd",
"# Demo: greatest common divisor via Euclidean algorithm\n"
"function gcd(a : int, b : int) -> int\n"
"    while b != 0 do\n"
"        let temp := b\n"
"        b := a % b\n"
"        a := temp\n"
"    end\n"
"    return a\n"
"end\n"
"print gcd(48, 18)\n"
"print gcd(100, 75)\n"
"print gcd(7, 13)")

# 81-90: Control flow - for
write_file(81, "for_simple",
"# Demo: simple for loop\n"
"for i := 1 to 5 do\n"
"    print i\n"
"end")

write_file(82, "for_sum",
"# Demo: for loop accumulation\n"
"let sum := 0\n"
"for i := 1 to 10 do\n"
"    sum := sum + i\n"
"end\n"
"print sum")

write_file(83, "for_step_positive",
"# Demo: for loop with step\n"
"for i := 0 to 20 step 2 do\n"
"    print i\n"
"end")

write_file(84, "for_step_5",
"# Demo: for loop with step 5\n"
"for i := 0 to 100 step 5 do\n"
"    print i\n"
"end")

write_file(85, "for_nested",
"# Demo: nested for loops\n"
"for i := 1 to 5 do\n"
"    for j := 1 to 5 do\n"
"        if i == j then\n"
"            print str(i) + \",\" + str(j)\n"
"        end\n"
"    end\n"
"end")

write_file(86, "for_break",
"# Demo: for loop with break\n"
"for i := 1 to 100 do\n"
"    if i > 5 then\n"
"        break\n"
"    end\n"
"    print i\n"
"end")

write_file(87, "for_continue",
"# Demo: for loop with continue\n"
"for i := 1 to 10 do\n"
"    if i % 3 == 0 then\n"
"        continue\n"
"    end\n"
"    print i\n"
"end")

write_file(88, "for_multiplication_table",
"# Demo: multiplication table using for\n"
"for i := 1 to 10 do\n"
"    print i + \" x 7 = \" + str(i * 7)\n"
"end")

write_file(89, "for_inclusive_ends",
"# Demo: for loop is inclusive on both ends\n"
"let sum := 0\n"
"for i := 1 to 10 do\n"
"    sum := sum + i\n"
"end\n"
"print sum")

write_file(90, "for_counting_down",
"# Demo: for loop with negative step\n"
"for i := 10 to 1 step -1 do\n"
"    print i\n"
"end")

# 91-100: Control flow - foreach
write_file(91, "foreach_array",
"# Demo: foreach over array\n"
"let items := [1, 2, 3, 4, 5]\n"
"foreach item in items do\n"
"    print item\n"
"end")

write_file(92, "foreach_strings",
"# Demo: foreach over string array\n"
'let fruits := ["apple", "banana", "cherry"]\n'
"foreach fruit in fruits do\n"
"    print fruit\n"
"end")

write_file(93, "foreach_char",
"# Demo: foreach over string yields characters\n"
'let word := "hello"\n'
"foreach ch in word do\n"
"    print ch\n"
"end")

write_file(94, "foreach_sum",
"# Demo: foreach to sum array\n"
"let numbers := [10, 20, 30, 40, 50]\n"
"let total := 0\n"
"foreach n in numbers do\n"
"    total := total + n\n"
"end\n"
"print total")

write_file(95, "foreach_nested",
"# Demo: nested foreach loops\n"
"let matrix := [[1, 2], [3, 4], [5, 6]]\n"
"foreach row in matrix do\n"
"    foreach val in row do\n"
"        print val\n"
"    end\n"
"end")

write_file(96, "foreach_break",
"# Demo: foreach with break\n"
"let nums := [1, 2, 3, 4, 5, 6, 7, 8]\n"
"foreach n in nums do\n"
"    if n > 4 then\n"
"        break\n"
"    end\n"
"    print n\n"
"end")

write_file(97, "foreach_continue",
"# Demo: foreach with continue\n"
"let nums := [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]\n"
"foreach n in nums do\n"
"    if n % 2 == 0 then\n"
"        continue\n"
"    end\n"
"    print n\n"
"end")

write_file(98, "foreach_index_track",
"# Demo: foreach with manual index tracking\n"
'let colors := ["red", "green", "blue"]\n'
"let idx := 0\n"
"foreach color in colors do\n"
"    print str(idx) + \": \" + color\n"
"    idx := idx + 1\n"
"end")

write_file(99, "foreach_mixed_array",
"# Demo: foreach over mixed-type array\n"
'let mixed := [1, "two", 3.0, true]\n'
"foreach item in mixed do\n"
"    print str(item)\n"
"end")

write_file(100, "foreach_build_result",
"# Demo: foreach to build new array\n"
"let src := [1, 2, 3, 4, 5]\n"
"let doubled := []\n"
"let i := 0\n"
"foreach n in src do\n"
"    doubled[i] := n * 2\n"
"    i := i + 1\n"
"end\n"
"foreach val in doubled do\n"
"    print val\n"
"end")

# 101-110: Do-while
write_file(101, "do_while_simple",
"# Demo: simple do-while loop\n"
"let i := 1\n"
"do\n"
"    print i\n"
"    i := i + 1\n"
"while i <= 5")

write_file(102, "do_while_once",
"# Demo: do-while executes at least once\n"
"let x := 100\n"
"do\n"
'    print "executed once"\n'
"    x := x + 1\n"
"while x < 5")

write_file(103, "do_while_countdown",
"# Demo: do-while countdown\n"
"let n := 3\n"
"do\n"
"    print n\n"
"    n := n - 1\n"
"while n > 0")

write_file(104, "do_while_sum",
"# Demo: do-while summing\n"
"let sum := 0\n"
"let i := 1\n"
"do\n"
"    sum := sum + i\n"
"    i := i + 1\n"
"while i <= 10\n"
"print sum")

write_file(105, "do_while_condition",
"# Demo: do-while with complex condition\n"
"let x := 1\n"
"do\n"
"    x := x * 2\n"
"    print x\n"
"while x < 50 and x > 0")

# 106-110: Switch
write_file(106, "switch_simple",
"# Demo: switch with integer cases\n"
"let x := 2\n"
"switch x {\n"
"    case 1: print \"one\"\n"
"    case 2: print \"two\"\n"
"    case 3: print \"three\"\n"
"    default: print \"other\"\n"
"}")

write_file(107, "switch_string",
"# Demo: switch on string value\n"
'let color := "blue"\n'
"switch color {\n"
'    case "red": print "warm"\n'
'    case "blue": print "cool"\n'
'    case "green": print "nature"\n'
'    default: print "unknown"\n'
"}")

write_file(108, "switch_no_default",
"# Demo: switch without default case\n"
"let n := 2\n"
"switch n {\n"
"    case 1: print \"one\"\n"
"    case 2: print \"two\"\n"
"    case 3: print \"three\"\n"
"}")

write_file(109, "switch_break",
"# Demo: switch with break\n"
"let val := 1\n"
"switch val {\n"
"    case 1:\n"
"        print \"first\"\n"
"        break\n"
"    case 2:\n"
"        print \"second\"\n"
"        break\n"
"    default:\n"
"        print \"other\"\n"
"}")

write_file(110, "switch_multistatement",
"# Demo: switch case with multiple statements\n"
'let day := "Sat"\n'
"switch day {\n"
'    case "Sat":\n'
'        print "Saturday"\n'
'        print "Weekend"\n'
'    case "Sun":\n'
'        print "Sunday"\n'
'        print "Weekend"\n'
'    default:\n'
'        print "Weekday"\n'
"}")

# 111-120: Functions
write_file(111, "function_basic",
"# Demo: basic function definition and call\n"
"function greet()\n"
'    print "Hello from function!"\n'
"end\n"
"greet()")

write_file(112, "function_params",
"# Demo: function with parameters\n"
"function add(a : int, b : int) -> int\n"
"    return a + b\n"
"end\n"
"print add(3, 4)\n"
"print add(10, 20)")

write_file(113, "function_return_string",
"# Demo: function returning string\n"
"function greetPerson(name : string) -> string\n"
"    return \"Hello, \" + name + \"!\"\n"
"end\n"
'print greetPerson("Alice")\n'
'print greetPerson("Bob")')

write_file(114, "function_recursion",
"# Demo: recursive function - factorial\n"
"function factorial(n : int) -> int\n"
"    if n <= 1 then\n"
"        return 1\n"
"    end\n"
"    return n * factorial(n - 1)\n"
"end\n"
"print factorial(1)\n"
"print factorial(5)\n"
"print factorial(10)")

write_file(115, "function_bare_return",
"# Demo: bare return (void function)\n"
"function printIfPositive(n : int)\n"
"    if n <= 0 then\n"
"        return\n"
"    end\n"
"    print n\n"
"end\n"
"printIfPositive(5)\n"
"printIfPositive(-3)\n"
"printIfPositive(10)")

write_file(116, "function_default_param",
"# Demo: function with default parameter\n"
"function power(base : int, exp : int := 2) -> int\n"
"    let result := 1\n"
"    let i := 0\n"
"    while i < exp do\n"
"        result := result * base\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"print power(3)\n"
"print power(2, 10)\n"
"print power(5, 3)")

write_file(117, "function_multiple_return",
"# Demo: function with multiple return paths\n"
"function abs_val(n : int) -> int\n"
"    if n < 0 then\n"
"        return -n\n"
"    end\n"
"    return n\n"
"end\n"
"print abs_val(-5)\n"
"print abs_val(3)\n"
"print abs_val(0)")

write_file(118, "function_higher_order",
"# Demo: function taking lambda as parameter\n"
"function apply(f, x : int) -> int\n"
"    return f(x)\n"
"end\n"
"let double := function(n : int) n * 2\n"
"let square := function(n : int) n * n\n"
"print apply(double, 5)\n"
"print apply(square, 4)")

write_file(119, "function_multiple_calls",
"# Demo: function called multiple times\n"
"function square(n : int) -> int\n"
"    return n * n\n"
"end\n"
"let sumOfSquares := 0\n"
"for i := 1 to 5 do\n"
"    sumOfSquares := sumOfSquares + square(i)\n"
"end\n"
"print sumOfSquares")

write_file(120, "function_no_params",
"# Demo: function with no parameters\n"
"function getCurrentYear() -> int\n"
"    return 2026\n"
"end\n"
"print getCurrentYear()\n"
"let year := getCurrentYear()\n"
"print year + 1")

# 121-130: Lambda expressions
write_file(121, "lambda_basic",
"# Demo: basic lambda expression\n"
"let double := function(x : int) x * 2\n"
"let triple := function(x : int) x * 3\n"
"print double(5)\n"
"print triple(4)")

write_file(122, "lambda_two_params",
"# Demo: lambda with two parameters\n"
"let add := function(a : int, b : int) a + b\n"
"let mul := function(a : int, b : int) a * b\n"
"print add(3, 4)\n"
"print mul(3, 4)")

write_file(123, "lambda_stored",
"# Demo: lambda stored in variable\n"
"let isEven := function(n : int) n % 2 == 0\n"
"print isEven(4)\n"
"print isEven(7)\n"
"print isEven(0)")

write_file(124, "lambda_block_body",
"# Demo: lambda with block body\n"
"let classify := function(n : int)\n"
"    if n > 0 then\n"
"        return \"positive\"\n"
"    else if n < 0 then\n"
"        return \"negative\"\n"
"    else\n"
"        return \"zero\"\n"
"    end\n"
"end\n"
"print classify(5)\n"
"print classify(-3)\n"
"print classify(0)")

write_file(125, "lambda_no_params",
"# Demo: lambda with no parameters\n"
"let greet := function() \"Hello!\"\n"
"print greet()")

write_file(126, "lambda_passed_to_function",
"# Demo: passing lambda to function\n"
"function applyToArray(arr, transform)\n"
"    let result := []\n"
"    let i := 0\n"
"    foreach item in arr do\n"
"        result[i] := transform(item)\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let nums := [1, 2, 3, 4, 5]\n"
"let squared := applyToArray(nums, function(x : int) x * x)\n"
"foreach n in squared do\n"
"    print n\n"
"end")

write_file(127, "lambda_in_array",
"# Demo: lambda stored in array\n"
"let ops := [function(x : int) x + 1, function(x : int) x * 2, function(x : int) x - 1]\n"
"let val := 5\n"
"let f0 := ops[0]\n"
"let f1 := ops[1]\n"
"let f2 := ops[2]\n"
"print f0(val)\n"
"print f1(val)\n"
"print f2(val)")

write_file(128, "lambda_string_op",
"# Demo: lambda for string operations\n"
"let shout := function(s : string) s + \"!\"\n"
"let repeat := function(s : string) s + s\n"
'print shout("Hello")\n'
'print repeat("abc")')

write_file(129, "lambda_filter",
"# Demo: filter pattern with lambda\n"
"function filter(arr, predicate)\n"
"    let result := []\n"
"    let i := 0\n"
"    foreach item in arr do\n"
"        if predicate(item) then\n"
"            result[i] := item\n"
"            i := i + 1\n"
"        end\n"
"    end\n"
"    return result\n"
"end\n"
"let nums := [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]\n"
"let evens := filter(nums, function(n : int) n % 2 == 0)\n"
"foreach n in evens do\n"
"    print n\n"
"end")

write_file(130, "lambda_map",
"# Demo: map pattern with lambda\n"
"function mapArr(arr, transform)\n"
"    let result := []\n"
"    let i := 0\n"
"    foreach item in arr do\n"
"        result[i] := transform(item)\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let nums := [1, 2, 3, 4, 5]\n"
"let doubled := mapArr(nums, function(x : int) x * 2)\n"
"foreach n in doubled do\n"
"    print n\n"
"end")

# 131-140: Arrays
write_file(131, "array_literal",
"# Demo: array literal\n"
"let nums := [1, 2, 3, 4, 5]\n"
"print nums\n"
"print len(nums)")

write_file(132, "array_index",
"# Demo: array indexing\n"
"let arr := [10, 20, 30, 40, 50]\n"
"print arr[0]\n"
"print arr[2]\n"
"print arr[4]")

write_file(133, "array_element_assign",
"# Demo: array element assignment\n"
"let arr := [1, 2, 3]\n"
"arr[0] := 10\n"
"arr[1] := 20\n"
"arr[2] := 30\n"
"print arr[0]\n"
"print arr[1]\n"
"print arr[2]")

write_file(134, "array_empty",
"# Demo: empty array and dynamic growth\n"
"let data := []\n"
"data[0] := 100\n"
"data[1] := 200\n"
"data[2] := 300\n"
"print data[0]\n"
"print data[1]\n"
"print data[2]")

write_file(135, "array_len",
"# Demo: len() function on arrays\n"
"let arr := [1, 2, 3, 4, 5]\n"
"print len(arr)\n"
"let empty := []\n"
"print len(empty)")

write_file(136, "array_strings",
"# Demo: array of strings\n"
'let words := ["hello", "world", "foo", "bar"]\n'
"foreach w in words do\n"
"    print w\n"
"end")

write_file(137, "array_nested",
"# Demo: nested arrays (2D)\n"
"let matrix := [[1, 2, 3], [4, 5, 6], [7, 8, 9]]\n"
"print matrix[0][0]\n"
"print matrix[1][1]\n"
"print matrix[2][2]")

write_file(138, "array_compute",
"# Demo: computing with array elements\n"
"let primes := [2, 3, 5, 7, 11, 13]\n"
"let sum := 0\n"
"foreach p in primes do\n"
"    sum := sum + p\n"
"end\n"
"print sum\n"
"print len(primes)")

write_file(139, "array_bubble_sort",
"# Demo: bubble sort algorithm\n"
"let arr := [64, 34, 25, 12, 22, 11, 90]\n"
"let n := len(arr)\n"
"for i := 0 to n - 2 do\n"
"    for j := 0 to n - i - 2 do\n"
"        if arr[j] > arr[j + 1] then\n"
"            let temp := arr[j]\n"
"            arr[j] := arr[j + 1]\n"
"            arr[j + 1] := temp\n"
"        end\n"
"    end\n"
"end\n"
"foreach val in arr do\n"
"    print val\n"
"end")

write_file(140, "array_mixed_types",
"# Demo: array with mixed types\n"
'let mixed := [1, "hello", 3.14, true, null]\n'
"print len(mixed)\n"
"foreach item in mixed do\n"
"    print str(item)\n"
"end")

# 141-150: Classes
write_file(141, "class_basic",
"# Demo: basic class with constructor and method\n"
"class Point {\n"
"    let X : int := 0\n"
"    let Y : int := 0\n"
"    Constructor(x : int, y : int)\n"
"        X := x\n"
"        Y := y\n"
"    end\n"
"    function toString() -> string\n"
"        return \"(\" + str(X) + \",\" + str(Y) + \")\"\n"
"    end\n"
"}\n"
"let p := new Point(3, 4)\n"
"print p.toString()")

write_file(142, "class_method",
"# Demo: class with multiple methods\n"
"class Rectangle {\n"
"    let Width : int := 0\n"
"    let Height : int := 0\n"
"    Constructor(w : int, h : int)\n"
"        Width := w\n"
"        Height := h\n"
"    end\n"
"    function area() -> int\n"
"        return Width * Height\n"
"    end\n"
"    function perimeter() -> int\n"
"        return 2 * (Width + Height)\n"
"    end\n"
"}\n"
"let rect := new Rectangle(5, 3)\n"
"print rect.area()\n"
"print rect.perimeter()")

write_file(143, "class_fields",
"# Demo: class fields read and write\n"
"class Counter {\n"
"    let Value : int := 0\n"
"    Constructor()\n"
"    end\n"
"    function increment()\n"
"        Value := Value + 1\n"
"    end\n"
"    function get() -> int\n"
"        return Value\n"
"    end\n"
"}\n"
"let c := new Counter()\n"
"c.increment()\n"
"c.increment()\n"
"c.increment()\n"
"print c.get()")

write_file(144, "class_static_method",
"# Demo: static method in class\n"
"class MathUtils {\n"
"    static function square(n : int) -> int\n"
"        return n * n\n"
"    end\n"
"    static function cube(n : int) -> int\n"
"        return n * n * n\n"
"    end\n"
"}\n"
"print MathUtils.square(4)\n"
"print MathUtils.cube(3)")

write_file(145, "class_this",
"# Demo: using this in method\n"
"class Person {\n"
"    let Name : string := \"\"\n"
"    let Age : int := 0\n"
"    Constructor(name : string, age : int)\n"
"        Name := name\n"
"        Age := age\n"
"    end\n"
"    function introduce() -> string\n"
"        return \"I am \" + Name + \", age \" + str(Age)\n"
"    end\n"
"}\n"
'let p := new Person("Alice", 30)\n'
"print p.introduce()")

write_file(146, "class_inheritance",
"# Demo: class inheritance with extends\n"
"class Animal {\n"
"    let Name : string := \"\"\n"
"    Constructor(name : string)\n"
"        Name := name\n"
"    end\n"
"    function speak() -> string\n"
"        return Name + \" makes a sound\"\n"
"    end\n"
"}\n"
"class Dog extends Animal {\n"
"    Constructor(name : string)\n"
"        Name := name\n"
"    end\n"
"    function speak() -> string\n"
"        return Name + \" says Woof!\"\n"
"    end\n"
"}\n"
'let d := new Dog("Rex")\n'
"print d.speak()")

write_file(147, "class_empty",
"# Demo: empty class body is valid\n"
"class EmptyClass {\n"
"}\n"
'print "empty class created"')

write_file(148, "class_const_field",
"# Demo: const field in class\n"
"class Circle {\n"
"    const PI : float := 3.14159\n"
"    let Radius : float := 0.0\n"
"    Constructor(r : float)\n"
"        Radius := r\n"
"    end\n"
"    function area() -> float\n"
"        return PI * Radius * Radius\n"
"    end\n"
"}\n"
"let c := new Circle(5.0)\n"
"print c.area()")

write_file(149, "class_multiple_instances",
"# Demo: multiple class instances\n"
"class Box {\n"
"    let Label : string := \"\"\n"
"    let Contents : int := 0\n"
"    Constructor(label : string, contents : int)\n"
"        Label := label\n"
"        Contents := contents\n"
"    end\n"
"    function describe() -> string\n"
"        return Label + \": \" + str(Contents)\n"
"    end\n"
"}\n"
'let b1 := new Box("Box A", 10)\n'
'let b2 := new Box("Box B", 20)\n'
'let b3 := new Box("Box C", 30)\n'
"print b1.describe()\n"
"print b2.describe()\n"
"print b3.describe()")

write_file(150, "class_var_field",
"# Demo: var field requires type annotation\n"
"class Temperature {\n"
"    var Celsius : float := 0.0\n"
"    Constructor(c : float)\n"
"        Celsius := c\n"
"    end\n"
"    function toFahrenheit() -> float\n"
"        return Celsius * 9.0 / 5.0 + 32.0\n"
"    end\n"
"}\n"
"let t := new Temperature(100.0)\n"
"print t.toFahrenheit()")

print("Done batch 001-150")
