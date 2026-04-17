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

# 176-200: More grammar features and algorithms
write_file(176, "generic_type",
"# Demo: array type annotation\n"
"var items : array := [1, 2, 3]\n"
"print items\n"
"print len(items)")

write_file(177, "nullable_type",
"# Demo: nullable type annotation\n"
"var maybeNull : int? := null\n"
"print maybeNull\n"
"maybeNull := 42\n"
"print maybeNull")

write_file(178, "array_type_2d",
"# Demo: 2D array type annotation\n"
"var grid : int[][] := [[1, 2], [3, 4]]\n"
"print grid[0][0]\n"
"print grid[1][1]")

write_file(179, "hex_number",
"# Demo: hexadecimal number literals\n"
"print 0xFF\n"
"print 0x10\n"
"print 0xDEAD\n"
"let color := 0xFF0000\n"
"print color")

write_file(180, "binary_number",
"# Demo: binary number literals\n"
"print 0b1010\n"
"print 0b11111111\n"
"print 0b0\n"
"let flags := 0b1100\n"
"print flags")

write_file(181, "octal_number",
"# Demo: octal number literals\n"
"print 0o17\n"
"print 0o377\n"
"print 0o10\n"
"let perms := 0o755\n"
"print perms")

write_file(182, "do_while_condition",
"# Demo: do-while loop\n"
"let x := 1\n"
"do\n"
"    print x\n"
"    x := x * 2\n"
"while x < 100")

write_file(183, "for_each_with_index",
"# Demo: foreach with manual index tracking\n"
'let fruits := ["apple", "banana", "cherry"]\n'
"let idx := 0\n"
"foreach fruit in fruits do\n"
"    print str(idx) + \": \" + fruit\n"
"    idx := idx + 1\n"
"end")

write_file(184, "string_builder_pattern",
"# Demo: building a string incrementally\n"
'let parts := ["Hello", ", ", "World", "!"]\n'
'let result := ""\n'
"foreach part in parts do\n"
"    result := result + part\n"
"end\n"
"print result")

write_file(185, "conditional_expr_nested",
"# Demo: nested conditional expressions\n"
"let n := 5\n"
"let label := if n > 10 then \"large\" else if n > 5 then \"medium\" else \"small\"\n"
"print label")

write_file(186, "ternary_nested",
"# Demo: nested ternary operators\n"
"let x := 0\n"
"let sign := x > 0 ? \"positive\" : x < 0 ? \"negative\" : \"zero\"\n"
"print sign\n"
"let y := -5\n"
"let signY := y > 0 ? \"positive\" : y < 0 ? \"negative\" : \"zero\"\n"
"print signY")

write_file(187, "multiple_return_paths",
"# Demo: function with multiple return paths\n"
"function classify(n : int) -> string\n"
"    if n < 0 then\n"
"        return \"negative\"\n"
"    end\n"
"    if n == 0 then\n"
"        return \"zero\"\n"
"    end\n"
"    if n % 2 == 0 then\n"
"        return \"positive even\"\n"
"    end\n"
"    return \"positive odd\"\n"
"end\n"
"print classify(-5)\n"
"print classify(0)\n"
"print classify(4)\n"
"print classify(7)")

write_file(188, "mutual_recursion",
"# Demo: mutually recursive functions\n"
"function isEvenNum(n : int) -> bool\n"
"    if n == 0 then\n"
"        return true\n"
"    end\n"
"    return isOddNum(n - 1)\n"
"end\n"
"function isOddNum(n : int) -> bool\n"
"    if n == 0 then\n"
"        return false\n"
"    end\n"
"    return isEvenNum(n - 1)\n"
"end\n"
"print isEvenNum(4)\n"
"print isOddNum(5)\n"
"print isEvenNum(7)")

write_file(189, "class_static_method2",
"# Demo: static method in a class\n"
"class MathOps {\n"
"    static function square(n : int) -> int\n"
"        return n * n\n"
"    end\n"
"    static function cube(n : int) -> int\n"
"        return n * n * n\n"
"    end\n"
"}\n"
"print MathOps.square(5)\n"
"print MathOps.cube(3)")

write_file(190, "class_const_field2",
"# Demo: const field in a class\n"
"class CircleConst {\n"
"    const PI : float := 3.14159265\n"
"    let Radius : float := 0.0\n"
"    Constructor(r : float)\n"
"        Radius := r\n"
"    end\n"
"    function area() -> float\n"
"        return PI * Radius * Radius\n"
"    end\n"
"    function circumference() -> float\n"
"        return 2.0 * PI * Radius\n"
"    end\n"
"}\n"
"let c := new CircleConst(5.0)\n"
"print c.area()\n"
"print c.circumference()")

write_file(191, "pattern_match_when_guard",
"# Demo: pattern matching with when guards\n"
"let value := 42\n"
"match value {\n"
"    n when n < 0 => print \"negative\"\n"
"    n when n == 0 => print \"zero\"\n"
"    n when n > 0 => print \"positive: \" + str(n)\n"
"}")

write_file(192, "pattern_match_alternation",
"# Demo: pattern matching with alternation\n"
"let n := 15\n"
"match n {\n"
"    0 | 1 => print \"very small\"\n"
"    x when x % 15 == 0 => print \"divisible by 15\"\n"
"    x when x % 3 == 0 => print \"divisible by 3\"\n"
"    x when x % 5 == 0 => print \"divisible by 5\"\n"
"    _ => print \"other\"\n"
"}")

write_file(193, "scope_update_outer",
"# Demo: assignment updates the scope where variable was declared\n"
"let counter := 0\n"
"function increment()\n"
"    counter := counter + 1\n"
"end\n"
"increment()\n"
"increment()\n"
"increment()\n"
"print counter")

write_file(194, "power_operator_float",
"# Demo: power operator with float operands\n"
"print 2.0 ** 10\n"
"print 4.0 ** 0.5\n"
"print 2 ** -1\n"
"print 3.0 ** 3.0")

write_file(195, "floor_div_float",
"# Demo: floor division with float operands\n"
"print 7.0 // 2.0\n"
"print 7.0 // 2\n"
"print 7 // 2.0\n"
"print -7.0 // 2.0")

write_file(196, "truthiness_values",
"# Demo: truthiness of various values\n"
"if 0 then print \"0 is truthy\" else print \"0 is falsy\" end\n"
"if 1 then print \"1 is truthy\" else print \"1 is falsy\" end\n"
"if \"\" then print \"empty string is truthy\" else print \"empty string is falsy\" end\n"
"if \"hello\" then print \"hello is truthy\" else print \"hello is falsy\" end\n"
"if null then print \"null is truthy\" else print \"null is falsy\" end")

write_file(197, "import_alias",
"# Demo: import with alias\n"
"module Utilities {\n"
"    function greetUtil(name : string) -> string\n"
"        return \"Hello, \" + name\n"
"    end\n"
"}\n"
"import Utilities as Utils\n"
"print Utilities.greetUtil(\"World\")")

write_file(198, "module_import_list",
"# Demo: module with import list in header\n"
"module CoreMath {\n"
"    function add(a : int, b : int) -> int\n"
"        return a + b\n"
"    end\n"
"}\n"
"module AppLogic import CoreMath {\n"
"    function compute(x : int) -> int\n"
"        return CoreMath.add(x, x)\n"
"    end\n"
"}\n"
"print AppLogic.compute(21)")

write_file(199, "empty_function_body",
"# Demo: function with empty body (valid - epsilon production)\n"
"function doNothing()\n"
"end\n"
"doNothing()\n"
'print "after doNothing"')

write_file(200, "empty_class_body2",
"# Demo: empty class body is valid\n"
"class EmptyClass2 {\n"
"}\n"
'print "empty class defined"')

# 201-225: Edge cases and more algorithms
write_file(201, "class_var_field2",
"# Demo: var field in class (requires type annotation)\n"
"class TempClass {\n"
"    var Celsius : float := 0.0\n"
"    Constructor(c : float)\n"
"        Celsius := c\n"
"    end\n"
"    function toFahrenheit() -> float\n"
"        return Celsius * 9.0 / 5.0 + 32.0\n"
"    end\n"
"}\n"
"let t := new TempClass(100.0)\n"
"print t.toFahrenheit()")

write_file(202, "deeply_nested",
"# Demo: deeply nested control flow\n"
"let result := 0\n"
"for i := 1 to 5 do\n"
"    if i % 2 != 0 then\n"
"        let j := 1\n"
"        while j <= i do\n"
"            if j % 2 == 1 then\n"
"                result := result + 1\n"
"            end\n"
"            j := j + 1\n"
"        end\n"
"    end\n"
"end\n"
"print result")

write_file(203, "string_bubble_sort",
"# Demo: comparing strings for sort order\n"
'let words := ["banana", "apple", "cherry", "date"]\n'
"let n := len(words)\n"
"let i := 0\n"
"while i < n - 1 do\n"
"    let j := 0\n"
"    while j < n - i - 1 do\n"
"        if words[j] > words[j + 1] then\n"
"            let temp := words[j]\n"
"            words[j] := words[j + 1]\n"
"            words[j + 1] := temp\n"
"        end\n"
"        j := j + 1\n"
"    end\n"
"    i := i + 1\n"
"end\n"
"foreach word in words do\n"
"    print word\n"
"end")

write_file(204, "integer_division_float",
"# Demo: integer division returns float when fractional\n"
"let a := 7\n"
"let b := 2\n"
"print a / b\n"
"print 10 / 5\n"
"print 1 / 3")

write_file(205, "string_amp_concat",
"# Demo: string concatenation with & converts non-strings\n"
"let n := 42\n"
"let f := 3.14\n"
"let b := true\n"
"print \"Number: \" & n\n"
"print \"Float: \" & f\n"
"print \"Bool: \" & b")

write_file(206, "not_operator_bang",
"# Demo: ! operator\n"
"print !true\n"
"print !false\n"
"let x := 5\n"
"print !(x > 10)\n"
"print !(x == 5)")

write_file(207, "chained_logical",
"# Demo: chained logical operators\n"
"let x := 5\n"
"print x > 0 and x < 10\n"
"print x >= 5 and x <= 5\n"
"print x < 0 or x > 3")

write_file(208, "type_check_is",
"# Demo: is type check in expressions\n"
'let values := [42, "hello", true, 3.14]\n'
"foreach v in values do\n"
"    if v is int then\n"
"        print \"int: \" + str(v)\n"
"    else if v is string then\n"
"        print \"string: \" + str(v)\n"
"    else if v is bool then\n"
"        print \"bool: \" + str(v)\n"
"    else\n"
"        print \"other: \" + str(v)\n"
"    end\n"
"end")

write_file(209, "power_right_assoc",
"# Demo: power operator is right-associative\n"
"print 2 ** 3 ** 2\n"
"print (2 ** 3) ** 2\n"
"print 2 ** (3 ** 2)")

write_file(210, "unary_minus_precedence",
"# Demo: unary minus precedence\n"
"let x := 2\n"
"print (-x) ** 2\n"
"print -(x ** 2)")

write_file(211, "additive_vs_multiplicative",
"# Demo: additive vs multiplicative precedence\n"
"print 2 + 3 * 4\n"
"print (2 + 3) * 4\n"
"print 10 - 2 * 3\n"
"print (10 - 2) * 3\n"
"print 4 + 6 / 2\n"
"print (4 + 6) / 2")

write_file(212, "parenthesized_expr",
"# Demo: parenthesized expressions\n"
"let result := (1 + 2) * (3 + 4)\n"
"print result\n"
"print (10 - 3) * (2 + 1)\n"
"let x := (5 + 3) ** 2\n"
"print x")

write_file(213, "cast_int_expression",
"# Demo: cast expression\n"
"let x := 3.7\n"
"let n := (int) x\n"
"print n\n"
"let sum := (int) 3.9 + (int) 2.1\n"
"print sum")

write_file(214, "foreach_nested_loops",
"# Demo: nested foreach loops over 2D array\n"
"let matrix := [[1, 2, 3], [4, 5, 6], [7, 8, 9]]\n"
"foreach row in matrix do\n"
"    foreach item in row do\n"
"        print item\n"
"    end\n"
"end")

write_file(215, "array_build_dynamic2",
"# Demo: building an array dynamically with for\n"
"let squares := []\n"
"for i := 1 to 10 do\n"
"    squares[i - 1] := i * i\n"
"end\n"
"foreach sq in squares do\n"
"    print sq\n"
"end")

write_file(216, "array_mixed_types2",
"# Demo: array with mixed types using str()\n"
'let mixed := [1, "two", 3.0, true, null]\n'
"foreach item in mixed do\n"
"    print str(item)\n"
"end")

write_file(217, "switch_no_default2",
"# Demo: switch without default\n"
"let x := 2\n"
"switch x {\n"
"    case 1: print \"one\"\n"
"    case 2: print \"two\"\n"
"    case 3: print \"three\"\n"
"}")

write_file(218, "switch_multiline_case2",
"# Demo: switch case with multiple statements\n"
'let grade := "A"\n'
"switch grade {\n"
'    case "A":\n'
'        print "Excellent"\n'
'        print "Top grade"\n'
'    case "B":\n'
'        print "Good"\n'
'        print "Above average"\n'
'    default:\n'
'        print "Needs improvement"\n'
"}")

write_file(219, "for_to_inclusive2",
"# Demo: for loop with Gauss sum\n"
"let sum := 0\n"
"for i := 1 to 10 do\n"
"    sum := sum + i\n"
"end\n"
"print sum")

write_file(220, "string_join_function",
"# Demo: join strings with separator\n"
"function join(arr, separator : string) -> string\n"
'    let result := ""\n'
"    let first := true\n"
"    foreach item in arr do\n"
"        if !first then\n"
"            result := result + separator\n"
"        end\n"
"        result := result + str(item)\n"
"        first := false\n"
"    end\n"
"    return result\n"
"end\n"
'let words := ["one", "two", "three"]\n'
'print join(words, ", ")\n'
'let nums := [1, 2, 3, 4, 5]\n'
'print join(nums, "-")')

write_file(221, "list_comprehension",
"# Demo: list comprehension\n"
"let squares := [x * x for x in [1, 2, 3, 4, 5]]\n"
"foreach s in squares do\n"
"    print s\n"
"end")

write_file(222, "pattern_match_literal",
"# Demo: pattern matching on literal values\n"
"let x := 3\n"
"match x {\n"
"    1 => print \"one\"\n"
"    2 => print \"two\"\n"
"    3 => print \"three\"\n"
"    _ => print \"other\"\n"
"}")

write_file(223, "pattern_match_string",
"# Demo: pattern matching on strings\n"
'let color := "green"\n'
"match color {\n"
'    "red" => print "warm"\n'
'    "green" => print "cool"\n'
'    "blue" => print "cold"\n'
'    _ => print "unknown"\n'
"}")

write_file(224, "pattern_null",
"# Demo: pattern matching on null\n"
"let val := null\n"
"match val {\n"
"    null => print \"got null\"\n"
"    _ => print \"not null\"\n"
"}")

write_file(225, "pattern_bool",
"# Demo: pattern matching on booleans\n"
"let b := true\n"
"match b {\n"
"    true => print \"it is true\"\n"
"    false => print \"it is false\"\n"
"}")

# 226-250: Modules, exceptions, algorithms
write_file(226, "module_basic",
"# Demo: basic module definition\n"
"module Greeter {\n"
"    function hello(name : string) -> string\n"
"        return \"Hello, \" + name + \"!\"\n"
"    end\n"
"}\n"
'print Greeter.hello("World")')

write_file(227, "module_export",
"# Demo: module export statement\n"
"module MyModule {\n"
"    function compute(n : int) -> int\n"
"        return n * 2\n"
"    end\n"
"    export compute\n"
"}\n"
"print MyModule.compute(21)")

write_file(228, "try_catch_simple",
"# Demo: simple try-catch\n"
"try\n"
'    throw "something went wrong"\n'
"catch err\n"
"    print \"Caught: \" + str(err)\n"
"end")

write_file(229, "try_catch_finally",
"# Demo: try-catch-finally\n"
"try\n"
'    throw "error"\n'
"catch e\n"
"    print \"Caught: \" + str(e)\n"
"finally\n"
'    print "Finally runs"\n'
"end")

write_file(230, "try_no_throw",
"# Demo: try block without throw\n"
"try\n"
'    print "no error"\n'
"catch e\n"
'    print "never reaches here"\n'
"finally\n"
'    print "finally always runs"\n'
"end")

write_file(231, "throw_in_function",
"# Demo: throw from function\n"
"function validate(n : int)\n"
"    if n < 0 then\n"
'        throw "negative value: " + str(n)\n'
"    end\n"
"    print n\n"
"end\n"
"try\n"
"    validate(5)\n"
"    validate(-1)\n"
"catch err\n"
"    print \"Error: \" + str(err)\n"
"end")

write_file(232, "nested_try_catch",
"# Demo: nested try-catch blocks\n"
"try\n"
"    try\n"
'        throw "inner error"\n'
"    catch inner\n"
"        print \"Inner caught: \" + str(inner)\n"
'        throw "re-thrown"\n'
"    end\n"
"catch outer\n"
"    print \"Outer caught: \" + str(outer)\n"
"end")

write_file(233, "annotation_basic",
"# Demo: basic annotation on function\n"
"@deprecated\n"
"function oldFunction()\n"
'    print "This is deprecated"\n'
"end\n"
"oldFunction()")

write_file(234, "annotation_with_param",
"# Demo: annotation with parameter\n"
"@version(major = 1, minor = 0)\n"
"function stableFunction() -> string\n"
"    return \"stable\"\n"
"end\n"
"print stableFunction()")

write_file(235, "annotation_multiple",
"# Demo: multiple annotations\n"
"@deprecated\n"
"@author(name = \"Alice\")\n"
"function annotatedFunction()\n"
'    print "annotated"\n'
"end\n"
"annotatedFunction()")

write_file(236, "primes_sieve",
"# Demo: Sieve of Eratosthenes\n"
"let limit := 50\n"
"let sieve := []\n"
"for i := 0 to limit do\n"
"    sieve[i] := true\n"
"end\n"
"sieve[0] := false\n"
"sieve[1] := false\n"
"for i := 2 to limit do\n"
"    if sieve[i] then\n"
"        let j := i * 2\n"
"        while j <= limit do\n"
"            sieve[j] := false\n"
"            j := j + i\n"
"        end\n"
"    end\n"
"end\n"
"for i := 2 to limit do\n"
"    if sieve[i] then\n"
"        print i\n"
"    end\n"
"end")

write_file(237, "selection_sort",
"# Demo: selection sort algorithm\n"
"let arr := [64, 25, 12, 22, 11]\n"
"let n := len(arr)\n"
"for i := 0 to n - 2 do\n"
"    let minIdx := i\n"
"    for j := i + 1 to n - 1 do\n"
"        if arr[j] < arr[minIdx] then\n"
"            minIdx := j\n"
"        end\n"
"    end\n"
"    if minIdx != i then\n"
"        let temp := arr[i]\n"
"        arr[i] := arr[minIdx]\n"
"        arr[minIdx] := temp\n"
"    end\n"
"end\n"
"foreach val in arr do\n"
"    print val\n"
"end")

write_file(238, "insertion_sort",
"# Demo: insertion sort algorithm\n"
"let arr := [12, 11, 13, 5, 6]\n"
"let n := len(arr)\n"
"for i := 1 to n - 1 do\n"
"    let key := arr[i]\n"
"    let j := i - 1\n"
"    while j >= 0 and arr[j] > key do\n"
"        arr[j + 1] := arr[j]\n"
"        j := j - 1\n"
"    end\n"
"    arr[j + 1] := key\n"
"end\n"
"foreach val in arr do\n"
"    print val\n"
"end")

write_file(239, "binary_search",
"# Demo: binary search algorithm\n"
"function binarySearch(arr, target : int) -> int\n"
"    let low := 0\n"
"    let high := len(arr) - 1\n"
"    while low <= high do\n"
"        let mid := (low + high) // 2\n"
"        if arr[mid] == target then\n"
"            return mid\n"
"        else if arr[mid] < target then\n"
"            low := mid + 1\n"
"        else\n"
"            high := mid - 1\n"
"        end\n"
"    end\n"
"    return -1\n"
"end\n"
"let sorted := [1, 3, 5, 7, 9, 11, 13, 15]\n"
"print binarySearch(sorted, 7)\n"
"print binarySearch(sorted, 13)\n"
"print binarySearch(sorted, 4)")

write_file(240, "factorial_iterative",
"# Demo: iterative factorial\n"
"function factIter(n : int) -> int\n"
"    let result := 1\n"
"    for i := 2 to n do\n"
"        result := result * i\n"
"    end\n"
"    return result\n"
"end\n"
"print factIter(0)\n"
"print factIter(1)\n"
"print factIter(5)\n"
"print factIter(10)")

write_file(241, "fibonacci_iterative",
"# Demo: iterative Fibonacci\n"
"function fibIter(n : int) -> int\n"
"    if n <= 1 then\n"
"        return n\n"
"    end\n"
"    let a := 0\n"
"    let b := 1\n"
"    for i := 2 to n do\n"
"        let temp := a + b\n"
"        a := b\n"
"        b := temp\n"
"    end\n"
"    return b\n"
"end\n"
"for i := 0 to 12 do\n"
"    print fibIter(i)\n"
"end")

write_file(242, "prime_check_function",
"# Demo: is prime function\n"
"function isPrime(n : int) -> bool\n"
"    if n < 2 then\n"
"        return false\n"
"    end\n"
"    let i := 2\n"
"    while i * i <= n do\n"
"        if n % i == 0 then\n"
"            return false\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
"for i := 1 to 30 do\n"
"    if isPrime(i) then\n"
"        print i\n"
"    end\n"
"end")

write_file(243, "collatz_sequence",
"# Demo: Collatz conjecture sequence\n"
"function collatz(n : int)\n"
"    let current := n\n"
"    let steps := 0\n"
"    while current != 1 do\n"
"        print current\n"
"        if current % 2 == 0 then\n"
"            current := current // 2\n"
"        else\n"
"            current := current * 3 + 1\n"
"        end\n"
"        steps := steps + 1\n"
"    end\n"
"    print 1\n"
"    print \"Steps: \" + str(steps)\n"
"end\n"
"collatz(6)")

write_file(244, "gcd_lcm",
"# Demo: GCD and LCM\n"
"function gcd(a : int, b : int) -> int\n"
"    while b != 0 do\n"
"        let temp := b\n"
"        b := a % b\n"
"        a := temp\n"
"    end\n"
"    return a\n"
"end\n"
"function lcm(a : int, b : int) -> int\n"
"    return a * b // gcd(a, b)\n"
"end\n"
"print gcd(12, 8)\n"
"print lcm(4, 6)\n"
"print gcd(100, 75)")

write_file(245, "power_function",
"# Demo: power function\n"
"function powerInt(base : int, exp : int) -> int\n"
"    if exp == 0 then\n"
"        return 1\n"
"    end\n"
"    let result := 1\n"
"    for i := 1 to exp do\n"
"        result := result * base\n"
"    end\n"
"    return result\n"
"end\n"
"print powerInt(2, 10)\n"
"print powerInt(3, 5)\n"
"print powerInt(5, 0)")

write_file(246, "string_reverse",
"# Demo: reverse a string\n"
"function reverseStr(s : string) -> string\n"
'    let result := ""\n'
"    let i := len(s) - 1\n"
"    while i >= 0 do\n"
"        result := result + s[i]\n"
"        i := i - 1\n"
"    end\n"
"    return result\n"
"end\n"
'print reverseStr("hello")\n'
'print reverseStr("abcde")\n'
'print reverseStr("racecar")')

write_file(247, "palindrome_check",
"# Demo: palindrome check\n"
"function isPalindrome(s : string) -> bool\n"
"    let n := len(s)\n"
"    let i := 0\n"
"    while i < n // 2 do\n"
"        if s[i] != s[n - i - 1] then\n"
"            return false\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
'print isPalindrome("racecar")\n'
'print isPalindrome("hello")\n'
'print isPalindrome("aba")')

write_file(248, "string_count_char",
"# Demo: count occurrences of character in string\n"
"function countChar(s : string, ch : string) -> int\n"
"    let count := 0\n"
"    foreach c in s do\n"
"        if c == ch then\n"
"            count := count + 1\n"
"        end\n"
"    end\n"
"    return count\n"
"end\n"
'print countChar("hello world", "l")\n'
'print countChar("mississippi", "s")\n'
'print countChar("abcabc", "a")')

write_file(249, "caesar_cipher",
"# Demo: Caesar cipher encode\n"
"function encodeChar(ch : string, shift : int) -> string\n"
'    if ch >= "a" and ch <= "z" then\n'
"        let code := int(ch) + shift\n"
'        if code > int("z") then\n'
'            code := code - 26\n'
"        end\n"
"        return str(code)\n"
"    end\n"
"    return ch\n"
"end\n"
'print "caesar cipher demo done"')

write_file(250, "matrix_multiply",
"# Demo: 2x2 matrix multiplication\n"
"let a := [[1, 2], [3, 4]]\n"
"let b := [[5, 6], [7, 8]]\n"
"let c := [[0, 0], [0, 0]]\n"
"for i := 0 to 1 do\n"
"    for j := 0 to 1 do\n"
"        let sum := 0\n"
"        for k := 0 to 1 do\n"
"            sum := sum + a[i][k] * b[k][j]\n"
"        end\n"
"        c[i][j] := sum\n"
"    end\n"
"end\n"
"print c[0][0]\n"
"print c[0][1]\n"
"print c[1][0]\n"
"print c[1][1]")

# 251-275: More features
write_file(251, "enum_basic",
"# Demo: enum definition\n"
"enum Direction {\n"
"    North,\n"
"    South,\n"
"    East,\n"
"    West\n"
"}\n"
"print Direction.North\n"
"print Direction.South")

write_file(252, "enum_with_values",
"# Demo: enum with explicit values\n"
"enum Status {\n"
"    Active = 1,\n"
"    Inactive = 0,\n"
"    Pending = 2\n"
"}\n"
"print Status.Active\n"
"print Status.Inactive\n"
"print Status.Pending")

write_file(253, "type_cast_int",
"# Demo: cast to int\n"
"let f := 9.9\n"
"let n := (int) f\n"
"print n\n"
'let s := "42"\n'
"print int(s)\n"
"print (int) 3.5")

write_file(254, "type_check_is_ops",
"# Demo: is type-check operator\n"
"let x := 42\n"
"print x is int\n"
'let s := "hello"\n'
"print s is string\n"
"let b := true\n"
"print b is bool")

write_file(255, "scope_function_isolation",
"# Demo: function cannot see call-site variables\n"
"let globalVal := 100\n"
"function getGlobal() -> int\n"
"    return globalVal\n"
"end\n"
"print getGlobal()")

write_file(256, "scope_let_in_block",
"# Demo: let in block doesn't leak to outer scope\n"
"let x := 10\n"
"if true then\n"
"    let inner := 999\n"
"    print inner\n"
"end\n"
"print x")

write_file(257, "scope_function_param_shadow",
"# Demo: function parameters shadow global names\n"
"let x := 10\n"
"function foo(x : int) -> int\n"
"    return x * 2\n"
"end\n"
"print foo(5)\n"
"print x")

write_file(258, "recursive_sum",
"# Demo: recursive sum of array\n"
"function sumArray(arr, idx : int) -> int\n"
"    if idx >= len(arr) then\n"
"        return 0\n"
"    end\n"
"    return arr[idx] + sumArray(arr, idx + 1)\n"
"end\n"
"let nums := [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]\n"
"print sumArray(nums, 0)")

write_file(259, "recursive_max",
"# Demo: recursive max of array\n"
"function maxArray(arr, idx : int) -> int\n"
"    if idx == len(arr) - 1 then\n"
"        return arr[idx]\n"
"    end\n"
"    let rest := maxArray(arr, idx + 1)\n"
"    return if arr[idx] > rest then arr[idx] else rest\n"
"end\n"
"let nums := [3, 7, 1, 9, 4, 6]\n"
"print maxArray(nums, 0)")

write_file(260, "recursive_flatten",
"# Demo: count elements in nested structure\n"
"function countNested(arr) -> int\n"
"    let total := 0\n"
"    foreach item in arr do\n"
"        total := total + 1\n"
"    end\n"
"    return total\n"
"end\n"
"let data := [1, 2, 3, 4, 5]\n"
"print countNested(data)")

write_file(261, "string_starts_with",
"# Demo: check if string starts with prefix\n"
"function startsWith(s : string, prefix : string) -> bool\n"
"    if len(prefix) > len(s) then\n"
"        return false\n"
"    end\n"
"    let i := 0\n"
"    while i < len(prefix) do\n"
"        if s[i] != prefix[i] then\n"
"            return false\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
'print startsWith("hello world", "hello")\n'
'print startsWith("hello", "world")\n'
'print startsWith("abc", "ab")')

write_file(262, "string_ends_with",
"# Demo: check if string ends with suffix\n"
"function endsWith(s : string, suffix : string) -> bool\n"
"    let sl := len(s)\n"
"    let pl := len(suffix)\n"
"    if pl > sl then\n"
"        return false\n"
"    end\n"
"    let i := 0\n"
"    while i < pl do\n"
"        if s[sl - pl + i] != suffix[i] then\n"
"            return false\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
'print endsWith("hello.txt", ".txt")\n'
'print endsWith("hello.txt", ".csv")\n'
'print endsWith("world", "rld")')

write_file(263, "string_trim_function",
"# Demo: ltrim removes leading spaces\n"
"function ltrim(s : string) -> string\n"
"    let i := 0\n"
"    while i < len(s) and s[i] == \" \" do\n"
"        i := i + 1\n"
"    end\n"
'    let result := ""\n'
"    while i < len(s) do\n"
"        result := result + s[i]\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
'print ltrim("   hello")\n'
'print ltrim("world")\n'
'print ltrim("  spaces  ")')

write_file(264, "string_repeat_function",
"# Demo: repeat a string n times\n"
"function repeatStr(s : string, n : int) -> string\n"
'    let result := ""\n'
"    let i := 0\n"
"    while i < n do\n"
"        result := result + s\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
'print repeatStr("abc", 3)\n'
'print repeatStr("-", 10)\n'
'print repeatStr("ha", 5)')

write_file(265, "digit_sum",
"# Demo: sum the digits of a number\n"
"function digitSum(n : int) -> int\n"
"    let sum := 0\n"
"    let remaining := n\n"
"    while remaining > 0 do\n"
"        sum := sum + remaining % 10\n"
"        remaining := remaining // 10\n"
"    end\n"
"    return sum\n"
"end\n"
"print digitSum(12345)\n"
"print digitSum(999)\n"
"print digitSum(100)")

write_file(266, "count_digits",
"# Demo: count digits of a number\n"
"function countDigits(n : int) -> int\n"
"    if n == 0 then\n"
"        return 1\n"
"    end\n"
"    let count := 0\n"
"    let remaining := n\n"
"    while remaining > 0 do\n"
"        count := count + 1\n"
"        remaining := remaining // 10\n"
"    end\n"
"    return count\n"
"end\n"
"print countDigits(0)\n"
"print countDigits(42)\n"
"print countDigits(12345678)")

write_file(267, "perfect_number",
"# Demo: perfect number check\n"
"function isPerfect(n : int) -> bool\n"
"    let sum := 0\n"
"    let i := 1\n"
"    while i < n do\n"
"        if n % i == 0 then\n"
"            sum := sum + i\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return sum == n\n"
"end\n"
"for i := 1 to 30 do\n"
"    if isPerfect(i) then\n"
"        print i\n"
"    end\n"
"end")

write_file(268, "triangle_numbers",
"# Demo: triangle numbers\n"
"function triangleNum(n : int) -> int\n"
"    return n * (n + 1) // 2\n"
"end\n"
"for i := 1 to 10 do\n"
"    print triangleNum(i)\n"
"end")

write_file(269, "power_of_two_check",
"# Demo: check if number is power of 2\n"
"function isPowerOfTwo(n : int) -> bool\n"
"    if n <= 0 then\n"
"        return false\n"
"    end\n"
"    let remaining := n\n"
"    while remaining > 1 do\n"
"        if remaining % 2 != 0 then\n"
"            return false\n"
"        end\n"
"        remaining := remaining // 2\n"
"    end\n"
"    return true\n"
"end\n"
"print isPowerOfTwo(1)\n"
"print isPowerOfTwo(2)\n"
"print isPowerOfTwo(16)\n"
"print isPowerOfTwo(15)\n"
"print isPowerOfTwo(64)")

write_file(270, "range_array_function",
"# Demo: generate an array of values in a range\n"
"function makeRange(start : int, stop : int, step : int)\n"
"    let result := []\n"
"    let i := start\n"
"    let idx := 0\n"
"    while i < stop do\n"
"        result[idx] := i\n"
"        idx := idx + 1\n"
"        i := i + step\n"
"    end\n"
"    return result\n"
"end\n"
"let evens := makeRange(0, 10, 2)\n"
"foreach n in evens do\n"
"    print n\n"
"end")

write_file(271, "zip_sum_arrays",
"# Demo: zip two arrays together (pair-wise sum)\n"
"function zipSum(a, b)\n"
"    let result := []\n"
"    let n := len(a)\n"
"    let i := 0\n"
"    while i < n do\n"
"        result[i] := a[i] + b[i]\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let xs := [1, 2, 3, 4, 5]\n"
"let ys := [10, 20, 30, 40, 50]\n"
"let zipped := zipSum(xs, ys)\n"
"foreach item in zipped do\n"
"    print item\n"
"end")

write_file(272, "fibonacci_memo",
"# Demo: Fibonacci with memoization\n"
"let memo := [0, 1]\n"
"function fibMemo(n : int) -> int\n"
"    if n < len(memo) then\n"
"        return memo[n]\n"
"    end\n"
"    let result := fibMemo(n - 1) + fibMemo(n - 2)\n"
"    memo[n] := result\n"
"    return result\n"
"end\n"
"for i := 0 to 15 do\n"
"    print fibMemo(i)\n"
"end")

write_file(273, "max_min_functions",
"# Demo: max and min helper functions\n"
"function maxOf(a : int, b : int) -> int\n"
"    return if a > b then a else b\n"
"end\n"
"function minOf(a : int, b : int) -> int\n"
"    return if a < b then a else b\n"
"end\n"
"print maxOf(3, 7)\n"
"print maxOf(10, 2)\n"
"print minOf(3, 7)\n"
"print minOf(10, 2)")

write_file(274, "clamp_function",
"# Demo: clamp a value between min and max\n"
"function clamp(val : int, lo : int, hi : int) -> int\n"
"    if val < lo then\n"
"        return lo\n"
"    end\n"
"    if val > hi then\n"
"        return hi\n"
"    end\n"
"    return val\n"
"end\n"
"print clamp(5, 0, 10)\n"
"print clamp(-5, 0, 10)\n"
"print clamp(15, 0, 10)")

write_file(275, "abs_function",
"# Demo: absolute value function\n"
"function absVal(n : int) -> int\n"
"    if n < 0 then\n"
"        return -n\n"
"    end\n"
"    return n\n"
"end\n"
"print absVal(-5)\n"
"print absVal(5)\n"
"print absVal(0)\n"
"print absVal(-100)")

# 276-300: Complex programs and final features
write_file(276, "exception_validate",
"# Demo: throwing and catching validation errors\n"
"function validateAge(age : int) -> int\n"
"    if age < 0 then\n"
'        throw "Age cannot be negative"\n'
"    end\n"
"    if age > 150 then\n"
'        throw "Age is unrealistically high"\n'
"    end\n"
"    return age\n"
"end\n"
"try\n"
"    print validateAge(25)\n"
"    print validateAge(-1)\n"
"catch msg\n"
"    print \"Validation error: \" + str(msg)\n"
"end\n"
"try\n"
"    print validateAge(200)\n"
"catch msg\n"
"    print \"Validation error: \" + str(msg)\n"
"end")

write_file(277, "try_finally_cleanup",
"# Demo: finally block for cleanup\n"
"function riskyOp(shouldFail : bool)\n"
'    let cleanup := "pending"\n'
"    try\n"
"        if shouldFail then\n"
'            throw "deliberate failure"\n'
"        end\n"
'        print "Success!"\n'
"    catch err\n"
"        print \"Caught: \" + str(err)\n"
"    finally\n"
'        cleanup := "done"\n'
"        print \"Cleaned up: \" + cleanup\n"
"    end\n"
"end\n"
"riskyOp(false)\n"
"riskyOp(true)")

write_file(278, "complex_class_program",
"# Demo: complex class usage with methods and fields\n"
"class Stack {\n"
"    let Data : array := []\n"
"    let Size : int := 0\n"
"    Constructor()\n"
"    end\n"
"    function push(val)\n"
"        Data[Size] := val\n"
"        Size := Size + 1\n"
"    end\n"
"    function pop() -> int\n"
"        if Size == 0 then\n"
'            throw "Stack underflow"\n'
"        end\n"
"        Size := Size - 1\n"
"        return Data[Size]\n"
"    end\n"
"    function peek() -> int\n"
"        if Size == 0 then\n"
'            throw "Stack empty"\n'
"        end\n"
"        return Data[Size - 1]\n"
"    end\n"
"    function isEmpty() -> bool\n"
"        return Size == 0\n"
"    end\n"
"}\n"
"let s := new Stack()\n"
"s.push(1)\n"
"s.push(2)\n"
"s.push(3)\n"
"print s.peek()\n"
"print s.pop()\n"
"print s.pop()\n"
"print s.isEmpty()\n"
"print s.pop()\n"
"print s.isEmpty()")

write_file(279, "complex_algorithm_power_set",
"# Demo: generate power set size calculation\n"
"function powerSetSize(n : int) -> int\n"
"    let result := 1\n"
"    for i := 1 to n do\n"
"        result := result * 2\n"
"    end\n"
"    return result\n"
"end\n"
"for i := 0 to 8 do\n"
"    print \"2^\" + str(i) + \" = \" + str(powerSetSize(i))\n"
"end")

write_file(280, "complex_string_program",
"# Demo: word counting in a sentence\n"
"function countWords(sentence : string) -> int\n"
"    let count := 0\n"
"    let inWord := false\n"
"    foreach ch in sentence do\n"
"        if ch == \" \" then\n"
"            inWord := false\n"
"        else if !inWord then\n"
"            count := count + 1\n"
"            inWord := true\n"
"        end\n"
"    end\n"
"    return count\n"
"end\n"
'print countWords("hello world")\n'
'print countWords("the quick brown fox")\n'
'print countWords("one")')

write_file(281, "string_operations_combined",
"# Demo: combined string operations\n"
'let s := "Hello, World!"\n'
"print len(s)\n"
"print s[0]\n"
"print s[7]\n"
'let rev := ""\n'
"let i := len(s) - 1\n"
"while i >= 0 do\n"
"    rev := rev + s[i]\n"
"    i := i - 1\n"
"end\n"
"print rev")

write_file(282, "higher_order_functions",
"# Demo: higher-order functions\n"
"function compose(f, g)\n"
"    return function(x : int) f(g(x))\n"
"end\n"
"let double := function(x : int) x * 2\n"
"let addOne := function(x : int) x + 1\n"
"let doubleThenAdd := compose(addOne, double)\n"
"print doubleThenAdd(5)\n"
"print doubleThenAdd(10)")

write_file(283, "pattern_match_variable_capture",
"# Demo: pattern matching capturing variable\n"
"let items := [1, 2, 3, 4, 5]\n"
"foreach item in items do\n"
"    match item {\n"
"        1 => print \"one\"\n"
"        2 => print \"two\"\n"
"        x when x % 2 == 0 => print str(x) + \" is even\"\n"
"        x => print str(x) + \" is odd\"\n"
"    }\n"
"end")

write_file(284, "advanced_recursion_tower",
"# Demo: Tower of Hanoi step counter\n"
"function hanoi(n : int, from : string, to : string, via : string) -> int\n"
"    if n == 0 then\n"
"        return 0\n"
"    end\n"
"    let moves := hanoi(n - 1, from, via, to) + 1 + hanoi(n - 1, via, to, from)\n"
"    return moves\n"
"end\n"
"for i := 1 to 8 do\n"
'    print "Hanoi(" + str(i) + ") = " + str(hanoi(i, "A", "C", "B"))\n'
"end")

write_file(285, "class_builder_pattern",
"# Demo: builder pattern with class\n"
"class StringBuilder {\n"
'    let Parts : string := ""\n'
"    function add(part : string)\n"
"        Parts := Parts + part\n"
"    end\n"
"    function build() -> string\n"
"        return Parts\n"
"    end\n"
"}\n"
"let sb := new StringBuilder()\n"
"sb.add(\"Hello\")\n"
"sb.add(\", \")\n"
"sb.add(\"World\")\n"
"sb.add(\"!\")\n"
"print sb.build()")

write_file(286, "comprehension_evens",
"# Demo: list comprehension for even numbers\n"
"let evens := [x for x in [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]]\n"
"foreach n in evens do\n"
"    if n % 2 == 0 then\n"
"        print n\n"
"    end\n"
"end")

write_file(287, "complex_fizzbuzz_extended",
"# Demo: extended FizzBuzz with Bazz for 7\n"
"for i := 1 to 30 do\n"
"    let output := \"\"\n"
"    if i % 3 == 0 then\n"
"        output := output + \"Fizz\"\n"
"    end\n"
"    if i % 5 == 0 then\n"
"        output := output + \"Buzz\"\n"
"    end\n"
"    if i % 7 == 0 then\n"
"        output := output + \"Bazz\"\n"
"    end\n"
"    if output == \"\" then\n"
"        output := str(i)\n"
"    end\n"
"    print output\n"
"end")

write_file(288, "class_inheritance_poly",
"# Demo: polymorphism via inheritance\n"
"class Shape {\n"
"    let Name : string := \"shape\"\n"
"    Constructor()\n"
"    end\n"
"    function area() -> float\n"
"        return 0.0\n"
"    end\n"
"    function describe() -> string\n"
"        return Name + \" area=\" + str(area())\n"
"    end\n"
"}\n"
"class Rect extends Shape {\n"
"    let W : float := 0.0\n"
"    let H : float := 0.0\n"
"    Constructor(w : float, h : float)\n"
"        Name := \"rect\"\n"
"        W := w\n"
"        H := h\n"
"    end\n"
"    function area() -> float\n"
"        return W * H\n"
"    end\n"
"}\n"
"let r := new Rect(4.0, 5.0)\n"
"print r.describe()")

write_file(289, "multiple_modules",
"# Demo: multiple modules\n"
"module Math {\n"
"    function square(n : int) -> int\n"
"        return n * n\n"
"    end\n"
"    function cube(n : int) -> int\n"
"        return n * n * n\n"
"    end\n"
"}\n"
"module Print {\n"
"    function printResult(label : string, val : int)\n"
"        print label + \": \" + str(val)\n"
"    end\n"
"}\n"
"Print.printResult(\"square(5)\", Math.square(5))\n"
"Print.printResult(\"cube(3)\", Math.cube(3))")

write_file(290, "string_number_formatting",
"# Demo: number formatting with str()\n"
"let vals := [0, 1, -1, 42, -42, 1000, 3.14, -2.71]\n"
"foreach v in vals do\n"
"    print str(v)\n"
"end")

write_file(291, "all_arithmetic_ops",
"# Demo: all arithmetic operators together\n"
"let a := 17\n"
"let b := 5\n"
"print a + b\n"
"print a - b\n"
"print a * b\n"
"print a / b\n"
"print a // b\n"
"print a % b\n"
"print a ** 2\n"
"print b ** 3")

write_file(292, "all_comparison_ops",
"# Demo: all comparison operators\n"
"let x := 5\n"
"let y := 10\n"
"print x == y\n"
"print x != y\n"
"print x < y\n"
"print x > y\n"
"print x <= y\n"
"print x >= y\n"
"print x == x\n"
"print x <= x")

write_file(293, "all_logical_ops",
"# Demo: all logical operators\n"
"print true and true\n"
"print true and false\n"
"print false or true\n"
"print false or false\n"
"print not true\n"
"print not false\n"
"print true && false\n"
"print false || true")

write_file(294, "all_builtin_functions",
"# Demo: all built-in functions\n"
"print len(\"hello\")\n"
"print len([1, 2, 3])\n"
"print str(42)\n"
"print str(3.14)\n"
"print str(true)\n"
"print str(null)\n"
'print int("10")\n'
"print int(3.9)\n"
"print bool(0)\n"
"print bool(1)\n"
'print bool("")\n'
'print bool("x")')

write_file(295, "complex_data_processing",
"# Demo: data processing pipeline\n"
"let data := [15, 3, 9, 22, 7, 18, 1, 11, 6, 14]\n"
"# Find min, max, sum\n"
"let minVal := data[0]\n"
"let maxVal := data[0]\n"
"let total := 0\n"
"foreach n in data do\n"
"    if n < minVal then\n"
"        minVal := n\n"
"    end\n"
"    if n > maxVal then\n"
"        maxVal := n\n"
"    end\n"
"    total := total + n\n"
"end\n"
"print \"Min: \" + str(minVal)\n"
"print \"Max: \" + str(maxVal)\n"
"print \"Sum: \" + str(total)\n"
"print \"Count: \" + str(len(data))")

write_file(296, "recursive_countdown",
"# Demo: recursive countdown\n"
"function countdown(n : int)\n"
"    if n < 0 then\n"
"        return\n"
"    end\n"
"    print n\n"
"    countdown(n - 1)\n"
"end\n"
"countdown(10)")

write_file(297, "string_contains",
"# Demo: check if string contains substring\n"
"function contains(haystack : string, needle : string) -> bool\n"
"    let hl := len(haystack)\n"
"    let nl := len(needle)\n"
"    if nl > hl then\n"
"        return false\n"
"    end\n"
"    let i := 0\n"
"    while i <= hl - nl do\n"
"        let match := true\n"
"        let j := 0\n"
"        while j < nl do\n"
"            if haystack[i + j] != needle[j] then\n"
"                match := false\n"
"                break\n"
"            end\n"
"            j := j + 1\n"
"        end\n"
"        if match then\n"
"            return true\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return false\n"
"end\n"
'print contains("hello world", "world")\n'
'print contains("hello", "xyz")\n'
'print contains("abcdef", "cde")')

write_file(298, "const_enum_usage",
"# Demo: using enum values in conditions\n"
"enum Color {\n"
"    Red = 1,\n"
"    Green = 2,\n"
"    Blue = 3\n"
"}\n"
"let chosen := Color.Green\n"
"match chosen {\n"
"    1 => print \"Red chosen\"\n"
"    2 => print \"Green chosen\"\n"
"    3 => print \"Blue chosen\"\n"
"    _ => print \"Unknown\"\n"
"}")

write_file(299, "complex_nested_functions",
"# Demo: complex nested function calls\n"
"function square(n : int) -> int\n"
"    return n * n\n"
"end\n"
"function sumOfSquares(a : int, b : int) -> int\n"
"    return square(a) + square(b)\n"
"end\n"
"function pythagorean(a : int, b : int, c : int) -> bool\n"
"    return sumOfSquares(a, b) == square(c)\n"
"end\n"
"print pythagorean(3, 4, 5)\n"
"print pythagorean(5, 12, 13)\n"
"print pythagorean(1, 2, 3)")

write_file(300, "caesar_cipher_simple",
"# Demo: Caesar cipher ROT13 variant\n"
'let message := "HELLO WORLD"\n'
"let shifted := \"\"\n"
"foreach ch in message do\n"
"    if ch >= \"A\" and ch <= \"Z\" then\n"
"        let code := int(ch)\n"
"        let base := int(\"A\")\n"
"        let shifted_code := (code - base + 13) % 26 + base\n"
"        shifted := shifted + str(shifted_code)\n"
"    else\n"
"        shifted := shifted + ch\n"
"    end\n"
"end\n"
"print shifted")

# 301-320: Final features
write_file(301, "scope_chain_demo",
"# Demo: scope chain lookup\n"
"let level1 := 1\n"
"function outerFunc()\n"
"    let level2 := 2\n"
"    function innerFunc()\n"
"        let level3 := 3\n"
"        print level1\n"
"        print level3\n"
"    end\n"
"    innerFunc()\n"
"    print level2\n"
"end\n"
"outerFunc()")

write_file(302, "while_do_comparison",
"# Demo: comparing while and do-while behavior\n"
"# while: condition checked first\n"
"let x := 0\n"
"while x > 5 do\n"
'    print "while body"\n'
"end\n"
'print "while done"\n'
"# do-while: body executes at least once\n"
"let y := 0\n"
"do\n"
'    print "do-while body"\n'
"    y := y + 1\n"
"while y > 5\n"
'print "do-while done"')

write_file(303, "for_step_examples",
"# Demo: various for loop step values\n"
"for i := 0 to 10 step 3 do\n"
"    print i\n"
"end\n"
"for i := 100 to 80 step -10 do\n"
"    print i\n"
"end")

write_file(304, "switch_boolean",
"# Demo: switch on boolean expression\n"
"let x := 42\n"
"let isEven := x % 2 == 0\n"
"switch isEven {\n"
"    case true: print \"even number\"\n"
"    case false: print \"odd number\"\n"
"}")

write_file(305, "class_method_chain",
"# Demo: method returning this for chaining\n"
"class Chain {\n"
"    let Value : int := 0\n"
"    Constructor()\n"
"    end\n"
"    function add(n : int) -> Chain\n"
"        Value := Value + n\n"
"        return this\n"
"    end\n"
"    function multiply(n : int) -> Chain\n"
"        Value := Value * n\n"
"        return this\n"
"    end\n"
"    function result() -> int\n"
"        return Value\n"
"    end\n"
"}\n"
"let c := new Chain()\n"
"c.add(5)\n"
"c.multiply(3)\n"
"c.add(2)\n"
"print c.result()")

write_file(306, "module_nested",
"# Demo: nested module calls\n"
"module StringUtils {\n"
"    function repeat(s : string, n : int) -> string\n"
'        let result := ""\n'
"        let i := 0\n"
"        while i < n do\n"
"            result := result + s\n"
"            i := i + 1\n"
"        end\n"
"        return result\n"
"    end\n"
"    function shout(s : string) -> string\n"
"        return s + \"!\"\n"
"    end\n"
"}\n"
'print StringUtils.repeat("ha", 3)\n'
'print StringUtils.shout("Hello")')

write_file(307, "exception_typed_catch",
"# Demo: typed catch clause\n"
"try\n"
'    throw "string error"\n'
"catch (err : string)\n"
"    print \"String error: \" + err\n"
"end")

write_file(308, "is_operator_all_types",
"# Demo: is operator with all types\n"
"let n := 42\n"
"let f := 3.14\n"
'let s := "hello"\n'
"let b := true\n"
"let arr := [1, 2, 3]\n"
"print n is int\n"
"print f is float\n"
"print s is string\n"
"print b is bool")

write_file(309, "as_operator",
"# Demo: as type assertion operator\n"
"let x := 42\n"
"let n := x as int\n"
"print n\n"
'let s := "hello"\n'
"let str_val := s as string\n"
"print str_val")

write_file(310, "comprehensive_fizzbuzz",
"# Demo: comprehensive FizzBuzz with analysis\n"
"let fizz_count := 0\n"
"let buzz_count := 0\n"
"let fizzbuzz_count := 0\n"
"let other_count := 0\n"
"for i := 1 to 100 do\n"
"    if i % 15 == 0 then\n"
"        fizzbuzz_count := fizzbuzz_count + 1\n"
"    else if i % 3 == 0 then\n"
"        fizz_count := fizz_count + 1\n"
"    else if i % 5 == 0 then\n"
"        buzz_count := buzz_count + 1\n"
"    else\n"
"        other_count := other_count + 1\n"
"    end\n"
"end\n"
"print \"Fizz: \" + str(fizz_count)\n"
"print \"Buzz: \" + str(buzz_count)\n"
"print \"FizzBuzz: \" + str(fizzbuzz_count)\n"
"print \"Other: \" + str(other_count)")

write_file(311, "number_base_literals",
"# Demo: number literals in different bases\n"
"let decimal := 255\n"
"let hex := 0xFF\n"
"let binary := 0b11111111\n"
"let octal := 0o377\n"
"print decimal\n"
"print hex\n"
"print binary\n"
"print octal\n"
"print decimal == hex\n"
"print hex == binary")

write_file(312, "recursive_ackermann",
"# Demo: Ackermann function (small values)\n"
"function ackermann(m : int, n : int) -> int\n"
"    if m == 0 then\n"
"        return n + 1\n"
"    end\n"
"    if n == 0 then\n"
"        return ackermann(m - 1, 1)\n"
"    end\n"
"    return ackermann(m - 1, ackermann(m, n - 1))\n"
"end\n"
"print ackermann(0, 0)\n"
"print ackermann(1, 1)\n"
"print ackermann(2, 2)\n"
"print ackermann(3, 3)")

write_file(313, "array_rotate",
"# Demo: rotate array left by k positions\n"
"function rotateLeft(arr, k : int)\n"
"    let n := len(arr)\n"
"    let result := []\n"
"    let i := 0\n"
"    while i < n do\n"
"        result[i] := arr[(i + k) % n]\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let data := [1, 2, 3, 4, 5]\n"
"let rotated := rotateLeft(data, 2)\n"
"foreach item in rotated do\n"
"    print item\n"
"end")

write_file(314, "pattern_match_wildcard",
"# Demo: pattern matching wildcard\n"
"let nums := [1, 2, 42, 99, 3]\n"
"foreach n in nums do\n"
"    match n {\n"
"        42 => print \"found the answer!\"\n"
"        _ => print str(n)\n"
"    }\n"
"end")

write_file(315, "function_recursion_depth",
"# Demo: count recursion depth\n"
"function depth(n : int) -> int\n"
"    if n <= 0 then\n"
"        return 0\n"
"    end\n"
"    return 1 + depth(n - 1)\n"
"end\n"
"print depth(10)\n"
"print depth(50)\n"
"print depth(100)")

write_file(316, "complex_while_break",
"# Demo: while with multiple break conditions\n"
"let i := 0\n"
"let found := false\n"
"while i < 100 do\n"
"    if i * i > 50 then\n"
"        found := true\n"
"        break\n"
"    end\n"
"    i := i + 1\n"
"end\n"
"print i\n"
"print found")

write_file(317, "string_format_table",
"# Demo: formatted table output\n"
"print \"Num  | Square | Cube\"\n"
"print \"-----|--------|-----\"\n"
"for i := 1 to 8 do\n"
"    print str(i) + \"    | \" + str(i*i) + \"      | \" + str(i*i*i)\n"
"end")

write_file(318, "global_scope_functions",
"# Demo: multiple global-scope function definitions\n"
"function square(n : int) -> int\n"
"    return n * n\n"
"end\n"
"function cube(n : int) -> int\n"
"    return n * n * n\n"
"end\n"
"function hypotenuse(a : int, b : int) -> int\n"
"    return square(a) + square(b)\n"
"end\n"
"print square(7)\n"
"print cube(4)\n"
"print hypotenuse(3, 4)")

write_file(319, "comprehensive_array_ops",
"# Demo: comprehensive array operations\n"
"let arr := [5, 3, 8, 1, 9, 2, 7, 4, 6]\n"
"print \"Length: \" + str(len(arr))\n"
"print \"First: \" + str(arr[0])\n"
"print \"Last: \" + str(arr[len(arr) - 1])\n"
"let sum := 0\n"
"foreach n in arr do\n"
"    sum := sum + n\n"
"end\n"
"print \"Sum: \" + str(sum)\n"
"arr[0] := 100\n"
"print \"Modified first: \" + str(arr[0])")

write_file(320, "final_showcase",
"# Demo: final showcase combining many features\n"
"# Fibonacci, primes check, and output\n"
"function fib(n : int) -> int\n"
"    if n <= 1 then\n"
"        return n\n"
"    end\n"
"    return fib(n - 1) + fib(n - 2)\n"
"end\n"
"function isPrime(n : int) -> bool\n"
"    if n < 2 then\n"
"        return false\n"
"    end\n"
"    let i := 2\n"
"    while i * i <= n do\n"
"        if n % i == 0 then\n"
"            return false\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
"print \"Prime Fibonacci numbers up to fib(15):\"\n"
"for i := 0 to 15 do\n"
"    let f := fib(i)\n"
"    if isPrime(f) then\n"
"        print str(f)\n"
"    end\n"
"end")

print("Done batch 176-320")
