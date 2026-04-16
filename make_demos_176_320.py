import os

DEMO_DIR = "Z:/repos/TinyLanguage/TinyLanguage.2026.04.16.00/TinyLanguage.DemoFiles"

def write_file(num, name, content):
    filename = f"{num:05d}.{name}"
    with open(os.path.join(DEMO_DIR, f"{filename}.tlg"), 'w') as f:
        f.write(content)
    with open(os.path.join(DEMO_DIR, f"{filename}.cmd"), 'w') as f:
        f.write(f"@echo off\necho Running {filename}.tlg\nif \"%2\"==\"\" (\n    TinyLanguage.exe {filename}.tlg output.txt\n) else (\n    TinyLanguage.exe {filename}.tlg %2\n)\n")

# 176-200: More grammar features and algorithms

write_file(176, "generic_type",
"# Demo: generic type annotation\n"
"var items : array := [1, 2, 3]\n"
"print items\n"
"print len(items)")

write_file(177, "map_type",
"# Demo: map type annotation\n"
"var scores : map<string, int> := []\n"
"print scores")

write_file(178, "nullable_type",
"# Demo: nullable type annotation\n"
"var maybeNull : int? := null\n"
"print maybeNull\n"
"maybeNull := 42\n"
"print maybeNull")

write_file(179, "array_type_2d",
"# Demo: 2D array type annotation\n"
"var grid : int[][] := [[1, 2], [3, 4]]\n"
"print grid[0][0]\n"
"print grid[1][1]")

write_file(180, "hex_number",
"# Demo: hexadecimal number literals\n"
"print 0xFF\n"
"print 0x10\n"
"print 0xDEAD\n"
"let color := 0xFF0000\n"
"print color")

write_file(181, "binary_number",
"# Demo: binary number literals\n"
"print 0b1010\n"
"print 0b11111111\n"
"print 0b0\n"
"let flags := 0b1100\n"
"print flags")

write_file(182, "octal_number",
"# Demo: octal number literals\n"
"print 0o17\n"
"print 0o377\n"
"print 0o10\n"
"let perms := 0o755\n"
"print perms")

write_file(183, "do_block",
"# Demo: do-while loop with multiple conditions\n"
"let x := 1\n"
"do\n"
"    print x\n"
"    x := x * 2\n"
"while x < 100")

write_file(184, "for_each_index",
"# Demo: foreach with manual index tracking\n"
"let fruits := [\"apple\", \"banana\", \"cherry\"]\n"
"let idx := 0\n"
"foreach fruit in fruits do\n"
"    print str(idx) + \": \" + fruit\n"
"    idx := idx + 1\n"
"end")

write_file(185, "string_builder_pattern",
"# Demo: building a string incrementally\n"
"let parts := [\"Hello\", \", \", \"World\", \"!\"]\n"
"let result := \"\"\n"
"foreach part in parts do\n"
"    result := result + part\n"
"end\n"
"print result")

write_file(186, "conditional_expr_nested",
"# Demo: nested conditional expressions\n"
"let n := 5\n"
"let label := if n > 10 then \"large\" else if n > 5 then \"medium\" else \"small\"\n"
"print label")

write_file(187, "ternary_nested",
"# Demo: nested ternary operators\n"
"let x := 0\n"
"let sign := x > 0 ? \"positive\" : x < 0 ? \"negative\" : \"zero\"\n"
"print sign\n"
"let y := -5\n"
"let signY := y > 0 ? \"positive\" : y < 0 ? \"negative\" : \"zero\"\n"
"print signY")

write_file(188, "multiple_return_paths",
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

write_file(189, "mutual_recursion",
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

write_file(190, "class_static_method",
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

write_file(191, "class_const_field",
"# Demo: const field in a class\n"
"class Circle {\n"
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
"let c := new Circle(5.0)\n"
"print c.area()\n"
"print c.circumference()")

write_file(192, "class_chained_calls",
"# Demo: chained method calls\n"
"class Builder {\n"
"    let Parts : string := \"\"\n"
"    function add(part : string) -> Builder\n"
"        Parts := Parts + part\n"
"        return this\n"
"    end\n"
"    function build() -> string\n"
"        return Parts\n"
"    end\n"
"}\n"
"let b := new Builder()\n"
"b.add(\"Hello\")\n"
"b.add(\", \")\n"
"b.add(\"World\")\n"
"print b.build()")

write_file(193, "pattern_match_type",
"# Demo: pattern matching on variable (identifier pattern)\n"
"let value := 42\n"
"match value {\n"
"    n when n < 0 => print \"negative\"\n"
"    n when n == 0 => print \"zero\"\n"
"    n when n > 0 => print \"positive: \" + str(n)\n"
"}")

write_file(194, "pattern_match_complex",
"# Demo: complex pattern matching with alternation and guards\n"
"let n := 15\n"
"match n {\n"
"    0 | 1 => print \"very small\"\n"
"    x when x % 15 == 0 => print \"divisible by 15\"\n"
"    x when x % 3 == 0 => print \"divisible by 3\"\n"
"    x when x % 5 == 0 => print \"divisible by 5\"\n"
"    _ => print \"other\"\n"
"}")

write_file(195, "scope_assignment_outer",
"# Demo: assignment updates the scope where variable was declared\n"
"let counter := 0\n"
"function increment()\n"
"    counter := counter + 1\n"
"end\n"
"increment()\n"
"increment()\n"
"increment()\n"
"print counter")

write_file(196, "power_operator_float",
"# Demo: power operator with float operands\n"
"print 2.0 ** 10\n"
"print 4.0 ** 0.5\n"
"print 2 ** -1\n"
"print 3.0 ** 3.0")

write_file(197, "floor_div_float",
"# Demo: floor division with float operands\n"
"print 7.0 // 2.0\n"
"print 7.0 // 2\n"
"print 7 // 2.0\n"
"print -7.0 // 2.0")

write_file(198, "truthiness_values",
"# Demo: truthiness of various values\n"
"if 0 then print \"0 is truthy\" else print \"0 is falsy\" end\n"
"if 1 then print \"1 is truthy\" else print \"1 is falsy\" end\n"
"if \"\" then print \"empty string is truthy\" else print \"empty string is falsy\" end\n"
"if \"hello\" then print \"hello is truthy\" else print \"hello is falsy\" end\n"
"if null then print \"null is truthy\" else print \"null is falsy\" end")

write_file(199, "import_alias",
"# Demo: import with alias\n"
"module Utilities {\n"
"    function greet(name : string) -> string\n"
"        return \"Hello, \" + name\n"
"    end\n"
"}\n"
"import Utilities as Utils\n"
"print Utilities.greet(\"World\")")

write_file(200, "module_import_list",
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

# 201-225: Edge cases and more algorithms

write_file(201, "empty_function_body",
"# Demo: function with empty body (valid - epsilon production)\n"
"function doNothing()\n"
"end\n"
"doNothing()\n"
"print \"after doNothing\"")

write_file(202, "empty_class_body",
"# Demo: empty class body is valid\n"
"class EmptyClass {\n"
"}\n"
"print \"empty class defined\"")

write_file(203, "class_var_field",
"# Demo: var field in class (requires type annotation)\n"
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

write_file(204, "deeply_nested",
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

write_file(205, "string_comparison_sort",
"# Demo: comparing strings for sort order\n"
"let words := [\"banana\", \"apple\", \"cherry\", \"date\"]\n"
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

write_file(206, "integer_division_promotion",
"# Demo: integer division returns float when there is a fractional part\n"
"let a := 7\n"
"let b := 2\n"
"print a / b\n"
"print 10 / 5\n"
"print 1 / 3")

write_file(207, "string_amp_concat",
"# Demo: string concatenation with & converts non-strings\n"
"let n := 42\n"
"let f := 3.14\n"
"let b := true\n"
"print \"Number: \" & n\n"
"print \"Float: \" & f\n"
"print \"Bool: \" & b")

write_file(208, "not_operator",
"# Demo: not / ! operator\n"
"print not true\n"
"print not false\n"
"print !true\n"
"print !false\n"
"let x := 5\n"
"print not (x > 10)\n"
"print !(x == 5)")

write_file(209, "chained_comparison",
"# Demo: comparison operators chained with logical operators\n"
"let x := 5\n"
"print x > 0 and x < 10\n"
"print x >= 5 and x <= 5\n"
"print x < 0 or x > 3")

write_file(210, "type_check_expression",
"# Demo: is and as in expressions\n"
"let values := [42, \"hello\", true, 3.14]\n"
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

write_file(211, "power_right_assoc",
"# Demo: power operator is right-associative\n"
"print 2 ** 3 ** 2\n"
"print (2 ** 3) ** 2\n"
"print 2 ** (3 ** 2)")

write_file(212, "unary_minus_precedence",
"# Demo: unary minus precedence with power\n"
"let x := 2\n"
"print -x ** 2\n"
"print (-x) ** 2\n"
"print -(x ** 2)")

write_file(213, "additive_precedence",
"# Demo: additive vs multiplicative precedence\n"
"print 2 + 3 * 4\n"
"print (2 + 3) * 4\n"
"print 10 - 2 * 3\n"
"print (10 - 2) * 3\n"
"print 4 + 6 / 2\n"
"print (4 + 6) / 2")

write_file(214, "parenthesized_expr",
"# Demo: parenthesized expressions\n"
"let result := (1 + 2) * (3 + 4)\n"
"print result\n"
"print (10 - 3) * (2 + 1)\n"
"let x := (5 + 3) ** 2\n"
"print x")

write_file(215, "cast_precedence",
"# Demo: cast expression precedence\n"
"let x := 3.7\n"
"let n := (int) x\n"
"print n\n"
"let sum := (int) 3.9 + (int) 2.1\n"
"print sum")

write_file(216, "foreach_nested",
"# Demo: nested foreach loops\n"
"let matrix := [[1, 2, 3], [4, 5, 6], [7, 8, 9]]\n"
"foreach row in matrix do\n"
"    foreach item in row do\n"
"        print item\n"
"    end\n"
"end")

write_file(217, "array_build_dynamic",
"# Demo: building an array dynamically\n"
"let squares := []\n"
"for i := 1 to 10 do\n"
"    squares[i - 1] := i * i\n"
"end\n"
"foreach sq in squares do\n"
"    print sq\n"
"end")

write_file(218, "array_multi_type",
"# Demo: array with mixed types\n"
"let mixed := [1, \"two\", 3.0, true, null]\n"
"foreach item in mixed do\n"
"    print str(item)\n"
"end")

write_file(219, "switch_no_default",
"# Demo: switch with cases only (no default)\n"
"let x := 2\n"
"switch x {\n"
"    case 1: print \"one\"\n"
"    case 2: print \"two\"\n"
"    case 3: print \"three\"\n"
"}")

write_file(220, "switch_multiline_case",
"# Demo: switch case with multiple statements\n"
"let grade := \"A\"\n"
"switch grade {\n"
"    case \"A\":\n"
"        print \"Excellent\"\n"
"        print \"Top grade\"\n"
"    case \"B\":\n"
"        print \"Good\"\n"
"        print \"Above average\"\n"
"    default:\n"
"        print \"Needs improvement\"\n"
"}")

write_file(221, "break_in_switch",
"# Demo: break in switch case\n"
"let val := 2\n"
"switch val {\n"
"    case 1:\n"
"        print \"one\"\n"
"        break\n"
"    case 2:\n"
"        print \"two\"\n"
"        break\n"
"    default:\n"
"        print \"other\"\n"
"}")

write_file(222, "for_to_inclusive",
"# Demo: for loop is inclusive on both ends\n"
"let sum := 0\n"
"for i := 1 to 10 do\n"
"    sum := sum + i\n"
"end\n"
"print sum")

write_file(223, "string_ops_combined",
"# Demo: combined string operations\n"
"function join(arr, separator : string) -> string\n"
"    let result := \"\"\n"
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
"let nums := [1, 2, 3, 4, 5]\n"
"print join(nums, \", \")\n"
"let words := [\"hello\", \"world\"]\n"
"print join(words, \" \")")

write_file(224, "number_formatting",
"# Demo: formatting numbers as strings\n"
"function padLeft(s : string, width : int) -> string\n"
"    let result := s\n"
"    while len(result) < width do\n"
"        result := \" \" + result\n"
"    end\n"
"    return result\n"
"end\n"
"for i := 1 to 10 do\n"
"    print padLeft(str(i * i), 5)\n"
"end")

write_file(225, "recursive_flatten",
"# Demo: recursive sum counting\n"
"function sumTo(n : int) -> int\n"
"    if n <= 0 then\n"
"        return 0\n"
"    end\n"
"    return n + sumTo(n - 1)\n"
"end\n"
"print sumTo(10)\n"
"print sumTo(100)")

# 226-250: More specific features

write_file(226, "lambda_stored_in_array",
"# Demo: storing lambdas in an array\n"
"let ops := [\n"
"    function(x : int) x + 1,\n"
"    function(x : int) x * 2,\n"
"    function(x : int) x * x\n"
"]\n"
"let n := 5\n"
"let i := 0\n"
"while i < len(ops) do\n"
"    let f := ops[i]\n"
"    print f(n)\n"
"    i := i + 1\n"
"end")

write_file(227, "function_as_value",
"# Demo: function stored in variable and called\n"
"function add(a : int, b : int) -> int\n"
"    return a + b\n"
"end\n"
"let myAdd := add\n"
"print myAdd(3, 4)\n"
"print myAdd(10, 20)")

write_file(228, "const_in_function",
"# Demo: const declaration inside function scope\n"
"function computeArea(radius : float) -> float\n"
"    const PI : float := 3.14159265\n"
"    return PI * radius * radius\n"
"end\n"
"print computeArea(5.0)\n"
"print computeArea(10.0)")

write_file(229, "let_in_loop",
"# Demo: let declaration inside loop body\n"
"for i := 1 to 5 do\n"
"    let squared := i * i\n"
"    let cubed := i * i * i\n"
"    print str(i) + \" squared=\" + str(squared) + \" cubed=\" + str(cubed)\n"
"end")

write_file(230, "multiple_functions",
"# Demo: multiple top-level function definitions\n"
"function double(n : int) -> int\n"
"    return n * 2\n"
"end\n"
"function triple(n : int) -> int\n"
"    return n * 3\n"
"end\n"
"function quadruple(n : int) -> int\n"
"    return n * 4\n"
"end\n"
"print double(5)\n"
"print triple(5)\n"
"print quadruple(5)")

write_file(231, "function_call_chain",
"# Demo: function call chain\n"
"function add1(n : int) -> int\n"
"    return n + 1\n"
"end\n"
"function double(n : int) -> int\n"
"    return n * 2\n"
"end\n"
"print double(add1(double(add1(1))))")

write_file(232, "class_with_array_field",
"# Demo: class with an array field\n"
"class Stack {\n"
"    let Items : array := []\n"
"    let Size : int := 0\n"
"    function push(item)\n"
"        Items[Size] := item\n"
"        Size := Size + 1\n"
"    end\n"
"    function pop()\n"
"        if Size == 0 then\n"
"            throw \"Stack underflow\"\n"
"        end\n"
"        Size := Size - 1\n"
"        return Items[Size]\n"
"    end\n"
"    function peek()\n"
"        return Items[Size - 1]\n"
"    end\n"
"    function isEmpty() -> bool\n"
"        return Size == 0\n"
"    end\n"
"}\n"
"let stack := new Stack()\n"
"stack.push(1)\n"
"stack.push(2)\n"
"stack.push(3)\n"
"print stack.peek()\n"
"print stack.pop()\n"
"print stack.pop()\n"
"print stack.isEmpty()")

write_file(233, "class_queue",
"# Demo: queue implementation using a class\n"
"class Queue {\n"
"    let Items : array := []\n"
"    let Front : int := 0\n"
"    let Back : int := 0\n"
"    function enqueue(item)\n"
"        Items[Back] := item\n"
"        Back := Back + 1\n"
"    end\n"
"    function dequeue()\n"
"        if Front >= Back then\n"
"            throw \"Queue empty\"\n"
"        end\n"
"        let item := Items[Front]\n"
"        Front := Front + 1\n"
"        return item\n"
"    end\n"
"    function size() -> int\n"
"        return Back - Front\n"
"    end\n"
"}\n"
"let q := new Queue()\n"
"q.enqueue(\"first\")\n"
"q.enqueue(\"second\")\n"
"q.enqueue(\"third\")\n"
"print q.size()\n"
"print q.dequeue()\n"
"print q.dequeue()")

write_file(234, "recursive_list_ops",
"# Demo: recursive list operations\n"
"function sum(arr, i : int) -> int\n"
"    if i >= len(arr) then\n"
"        return 0\n"
"    end\n"
"    return arr[i] + sum(arr, i + 1)\n"
"end\n"
"function product(arr, i : int) -> int\n"
"    if i >= len(arr) then\n"
"        return 1\n"
"    end\n"
"    return arr[i] * product(arr, i + 1)\n"
"end\n"
"let nums := [1, 2, 3, 4, 5]\n"
"print sum(nums, 0)\n"
"print product(nums, 0)")

write_file(235, "number_to_roman",
"# Demo: integer to roman numeral conversion\n"
"function toRoman(n : int) -> string\n"
"    let result := \"\"\n"
"    let values := [1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1]\n"
"    let symbols := [\"M\", \"CM\", \"D\", \"CD\", \"C\", \"XC\", \"L\", \"XL\", \"X\", \"IX\", \"V\", \"IV\", \"I\"]\n"
"    let i := 0\n"
"    while i < len(values) do\n"
"        while n >= values[i] do\n"
"            result := result + symbols[i]\n"
"            n := n - values[i]\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"print toRoman(42)\n"
"print toRoman(2024)\n"
"print toRoman(14)")

write_file(236, "luhn_algorithm",
"# Demo: Luhn algorithm for credit card validation\n"
"function luhnCheck(numStr : string) -> bool\n"
"    let sum := 0\n"
"    let isOdd := true\n"
"    let i := len(numStr) - 1\n"
"    while i >= 0 do\n"
"        let digit := int(numStr[i])\n"
"        if isOdd then\n"
"            sum := sum + digit\n"
"        else\n"
"            let doubled := digit * 2\n"
"            if doubled > 9 then\n"
"                doubled := doubled - 9\n"
"            end\n"
"            sum := sum + doubled\n"
"        end\n"
"        isOdd := !isOdd\n"
"        i := i - 1\n"
"    end\n"
"    return sum % 10 == 0\n"
"end\n"
"print luhnCheck(\"4532015112830366\")\n"
"print luhnCheck(\"1234567890123456\")")

write_file(237, "caesar_cipher",
"# Demo: Caesar cipher encoding\n"
"function caesarShift(ch : string, shift : int) -> string\n"
"    let code := int(ch)\n"
"    if code >= 65 and code <= 90 then\n"
"        return str((code - 65 + shift) % 26 + 65)\n"
"    end\n"
"    if code >= 97 and code <= 122 then\n"
"        return str((code - 97 + shift) % 26 + 97)\n"
"    end\n"
"    return ch\n"
"end\n"
"function encode(msg : string, shift : int) -> string\n"
"    let result := \"\"\n"
"    foreach ch in msg do\n"
"        result := result + caesarShift(ch, shift)\n"
"    end\n"
"    return result\n"
"end\n"
"print encode(\"Hello\", 3)")

write_file(238, "run_length_encoding",
"# Demo: run-length encoding\n"
"function rle(s : string) -> string\n"
"    if len(s) == 0 then\n"
"        return \"\"\n"
"    end\n"
"    let result := \"\"\n"
"    let current := s[0]\n"
"    let count := 1\n"
"    let i := 1\n"
"    while i < len(s) do\n"
"        if s[i] == current then\n"
"            count := count + 1\n"
"        else\n"
"            result := result + str(count) + current\n"
"            current := s[i]\n"
"            count := 1\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    result := result + str(count) + current\n"
"    return result\n"
"end\n"
"print rle(\"aaabbbcccc\")\n"
"print rle(\"aabaa\")\n"
"print rle(\"abc\")")

write_file(239, "armstrong_numbers",
"# Demo: Armstrong numbers (narcissistic numbers)\n"
"function isArmstrong(n : int) -> bool\n"
"    let digits := countDigits(n)\n"
"    let sum := 0\n"
"    let remaining := n\n"
"    while remaining > 0 do\n"
"        let digit := remaining % 10\n"
"        sum := sum + digit ** digits\n"
"        remaining := remaining // 10\n"
"    end\n"
"    return sum == n\n"
"end\n"
"function countDigits(n : int) -> int\n"
"    if n == 0 then return 1 end\n"
"    let count := 0\n"
"    let r := n\n"
"    while r > 0 do\n"
"        count := count + 1\n"
"        r := r // 10\n"
"    end\n"
"    return count\n"
"end\n"
"for i := 1 to 500 do\n"
"    if isArmstrong(i) then\n"
"        print i\n"
"    end\n"
"end")

write_file(240, "string_split_manual",
"# Demo: split a string by a delimiter character\n"
"function split(s : string, delim : string)\n"
"    let parts := []\n"
"    let current := \"\"\n"
"    let idx := 0\n"
"    foreach ch in s do\n"
"        if ch == delim then\n"
"            parts[idx] := current\n"
"            idx := idx + 1\n"
"            current := \"\"\n"
"        else\n"
"            current := current + ch\n"
"        end\n"
"    end\n"
"    parts[idx] := current\n"
"    return parts\n"
"end\n"
"let csv := \"apple,banana,cherry,date\"\n"
"let parts := split(csv, \",\")\n"
"foreach part in parts do\n"
"    print part\n"
"end")

write_file(241, "recursive_binary_search",
"# Demo: recursive binary search\n"
"function bSearch(arr, target : int, lo : int, hi : int) -> int\n"
"    if lo > hi then\n"
"        return -1\n"
"    end\n"
"    let mid := (lo + hi) // 2\n"
"    if arr[mid] == target then\n"
"        return mid\n"
"    end\n"
"    if arr[mid] < target then\n"
"        return bSearch(arr, target, mid + 1, hi)\n"
"    end\n"
"    return bSearch(arr, target, lo, mid - 1)\n"
"end\n"
"let sorted := [2, 4, 6, 8, 10, 12, 14, 16, 18, 20]\n"
"print bSearch(sorted, 12, 0, len(sorted) - 1)\n"
"print bSearch(sorted, 7, 0, len(sorted) - 1)")

write_file(242, "merge_sort",
"# Demo: merge sort\n"
"function merge(left, right)\n"
"    let result := []\n"
"    let i := 0\n"
"    let j := 0\n"
"    let k := 0\n"
"    while i < len(left) and j < len(right) do\n"
"        if left[i] <= right[j] then\n"
"            result[k] := left[i]\n"
"            i := i + 1\n"
"        else\n"
"            result[k] := right[j]\n"
"            j := j + 1\n"
"        end\n"
"        k := k + 1\n"
"    end\n"
"    while i < len(left) do\n"
"        result[k] := left[i]\n"
"        i := i + 1\n"
"        k := k + 1\n"
"    end\n"
"    while j < len(right) do\n"
"        result[k] := right[j]\n"
"        j := j + 1\n"
"        k := k + 1\n"
"    end\n"
"    return result\n"
"end\n"
"function mergeSort(arr)\n"
"    let n := len(arr)\n"
"    if n <= 1 then\n"
"        return arr\n"
"    end\n"
"    let mid := n // 2\n"
"    let left := []\n"
"    let right := []\n"
"    let i := 0\n"
"    while i < mid do\n"
"        left[i] := arr[i]\n"
"        i := i + 1\n"
"    end\n"
"    let j := 0\n"
"    while mid + j < n do\n"
"        right[j] := arr[mid + j]\n"
"        j := j + 1\n"
"    end\n"
"    return merge(mergeSort(left), mergeSort(right))\n"
"end\n"
"let data := [38, 27, 43, 3, 9, 82, 10]\n"
"let sorted := mergeSort(data)\n"
"foreach item in sorted do\n"
"    print item\n"
"end")

write_file(243, "quick_sort",
"# Demo: quicksort\n"
"function quickSort(arr, lo : int, hi : int)\n"
"    if lo < hi then\n"
"        let pivot := arr[hi]\n"
"        let i := lo - 1\n"
"        let j := lo\n"
"        while j < hi do\n"
"            if arr[j] <= pivot then\n"
"                i := i + 1\n"
"                let temp := arr[i]\n"
"                arr[i] := arr[j]\n"
"                arr[j] := temp\n"
"            end\n"
"            j := j + 1\n"
"        end\n"
"        let temp := arr[i + 1]\n"
"        arr[i + 1] := arr[hi]\n"
"        arr[hi] := temp\n"
"        let pi := i + 1\n"
"        quickSort(arr, lo, pi - 1)\n"
"        quickSort(arr, pi + 1, hi)\n"
"    end\n"
"end\n"
"let data := [64, 25, 12, 22, 11]\n"
"quickSort(data, 0, len(data) - 1)\n"
"foreach item in data do\n"
"    print item\n"
"end")

write_file(244, "linked_list",
"# Demo: linked list using classes\n"
"class Node {\n"
"    let Value : int := 0\n"
"    let Next := null\n"
"    Constructor(v : int)\n"
"        Value := v\n"
"    end\n"
"}\n"
"class LinkedList {\n"
"    let Head := null\n"
"    function prepend(val : int)\n"
"        let node := new Node(val)\n"
"        node.Next := Head\n"
"        Head := node\n"
"    end\n"
"    function printAll()\n"
"        let current := Head\n"
"        while current != null do\n"
"            print current.Value\n"
"            current := current.Next\n"
"        end\n"
"    end\n"
"}\n"
"let list := new LinkedList()\n"
"list.prepend(3)\n"
"list.prepend(2)\n"
"list.prepend(1)\n"
"list.printAll()")

write_file(245, "binary_tree",
"# Demo: binary search tree insertion\n"
"class TreeNode {\n"
"    let Value : int := 0\n"
"    let Left := null\n"
"    let Right := null\n"
"    Constructor(v : int)\n"
"        Value := v\n"
"    end\n"
"}\n"
"function insert(root, val : int)\n"
"    if root == null then\n"
"        return new TreeNode(val)\n"
"    end\n"
"    if val < root.Value then\n"
"        root.Left := insert(root.Left, val)\n"
"    else\n"
"        root.Right := insert(root.Right, val)\n"
"    end\n"
"    return root\n"
"end\n"
"function inOrder(node)\n"
"    if node == null then\n"
"        return\n"
"    end\n"
"    inOrder(node.Left)\n"
"    print node.Value\n"
"    inOrder(node.Right)\n"
"end\n"
"let root := null\n"
"let values := [5, 3, 7, 1, 4, 6, 8]\n"
"foreach v in values do\n"
"    root := insert(root, v)\n"
"end\n"
"inOrder(root)")

# 246-280: Even more coverage

write_file(246, "pattern_field",
"# Demo: field pattern in match\n"
"class Point {\n"
"    let X : int := 0\n"
"    let Y : int := 0\n"
"    Constructor(x : int, y : int)\n"
"        X := x\n"
"        Y := y\n"
"    end\n"
"}\n"
"let p := new Point(0, 5)\n"
"print p.X\n"
"print p.Y")

write_file(247, "empty_array_literal",
"# Demo: empty array literal\n"
"let empty := []\n"
"print len(empty)\n"
"empty[0] := 42\n"
"print len(empty)\n"
"print empty[0]")

write_file(248, "array_of_arrays",
"# Demo: array of arrays (jagged)\n"
"let jagged := [[1], [1, 2], [1, 2, 3]]\n"
"foreach row in jagged do\n"
"    print len(row)\n"
"end")

write_file(249, "string_escape_all",
"# Demo: all supported escape sequences\n"
'print "newline:\\n"'"\n"
'print "tab:\\t"'"\n"
'print "backslash:\\\\"'"\n"
'print "double-quote:\\""'"\n"
"print 'single-quote: \\'ok\\''")

write_file(250, "identifier_underscore",
"# Demo: identifiers with underscores (not leading)\n"
"let count_total := 0\n"
"let maxValue := 100\n"
"let my_variable := 42\n"
"count_total := count_total + my_variable\n"
"print count_total\n"
"print maxValue")

write_file(251, "semicolon_separator",
"# Demo: semicolons as statement separators\n"
"let a := 1; let b := 2; let c := 3\n"
"let sum := a + b + c\n"
"print sum")

write_file(252, "no_trailing_semicolon",
"# Demo: no trailing semicolon before end\n"
"function noTrailing() -> int\n"
"    let x := 1\n"
"    let y := 2\n"
"    return x + y\n"
"end\n"
"print noTrailing()")

write_file(253, "complex_expression",
"# Demo: complex expression combining multiple operators\n"
"let a := 3\n"
"let b := 4\n"
"let c := 5\n"
"print a ** 2 + b ** 2 == c ** 2\n"
"print (a + b) * c - a * (b + c)\n"
"print a > 0 and b > 0 and c > 0")

write_file(254, "let_reassign",
"# Demo: reassigning a let variable\n"
"let x := 1\n"
"print x\n"
"x := 2\n"
"print x\n"
"x := x * 10\n"
"print x")

write_file(255, "var_reassign",
"# Demo: reassigning a var variable\n"
"var count : int := 0\n"
"print count\n"
"count := count + 5\n"
"print count\n"
"count := count * 2\n"
"print count")

write_file(256, "scope_in_if",
"# Demo: scope inside if block\n"
"let x := 10\n"
"if x > 5 then\n"
"    let y := x * 2\n"
"    print y\n"
"end\n"
"print x")

write_file(257, "scope_in_while",
"# Demo: scope inside while block\n"
"let i := 0\n"
"while i < 3 do\n"
"    let doubled := i * 2\n"
"    print doubled\n"
"    i := i + 1\n"
"end")

write_file(258, "scope_in_for",
"# Demo: scope inside for body\n"
"for i := 1 to 5 do\n"
"    let square := i * i\n"
"    print square\n"
"end")

write_file(259, "function_forward_ref",
"# Demo: functions can call each other regardless of order\n"
"function callB() -> int\n"
"    return callA() + 1\n"
"end\n"
"function callA() -> int\n"
"    return 41\n"
"end\n"
"print callB()")

write_file(260, "class_extends_method_override",
"# Demo: overriding a method in a subclass\n"
"class Shape {\n"
"    function name() -> string\n"
"        return \"Shape\"\n"
"    end\n"
"    function describe() -> string\n"
"        return \"I am a \" + name()\n"
"    end\n"
"}\n"
"class Circle extends Shape {\n"
"    function name() -> string\n"
"        return \"Circle\"\n"
"    end\n"
"}\n"
"let s := new Shape()\n"
"let c := new Circle()\n"
"print s.name()\n"
"print c.name()")

write_file(261, "try_catch_rethrow",
"# Demo: catching then re-throwing\n"
"function inner()\n"
"    throw \"original error\"\n"
"end\n"
"function outer()\n"
"    try\n"
"        inner()\n"
"    catch e\n"
"        print \"outer caught: \" + str(e)\n"
"        throw \"wrapped: \" + str(e)\n"
"    end\n"
"end\n"
"try\n"
"    outer()\n"
"catch e\n"
"    print \"top caught: \" + str(e)\n"
"end")

write_file(262, "throw_and_continue",
"# Demo: after catching an exception, execution continues\n"
"let results := []\n"
"let idx := 0\n"
"let inputs := [5, -1, 10, -2, 15]\n"
"foreach input in inputs do\n"
"    try\n"
"        if input < 0 then\n"
"            throw \"negative\"\n"
"        end\n"
"        results[idx] := input * 2\n"
"        idx := idx + 1\n"
"    catch e\n"
"        print \"Skipping \" + str(input) + \": \" + str(e)\n"
"    end\n"
"end\n"
"foreach r in results do\n"
"    print r\n"
"end")

write_file(263, "complex_class_hierarchy",
"# Demo: multiple levels of class hierarchy\n"
"class Vehicle {\n"
"    let Speed : int := 0\n"
"    function getType() -> string\n"
"        return \"Vehicle\"\n"
"    end\n"
"}\n"
"class Car extends Vehicle {\n"
"    let Doors : int := 4\n"
"    Constructor(speed : int, doors : int)\n"
"        Speed := speed\n"
"        Doors := doors\n"
"    end\n"
"    function getType() -> string\n"
"        return \"Car\"\n"
"    end\n"
"}\n"
"let myCar := new Car(120, 4)\n"
"print myCar.getType()\n"
"print myCar.Speed\n"
"print myCar.Doors")

write_file(264, "enum_match",
"# Demo: enum values used in match\n"
"enum Direction {\n"
"    North = 0,\n"
"    South = 1,\n"
"    East = 2,\n"
"    West = 3\n"
"}\n"
"let dir := Direction.North\n"
"match dir {\n"
"    d when d == Direction.North => print \"Going North\"\n"
"    d when d == Direction.South => print \"Going South\"\n"
"    _ => print \"Going another direction\"\n"
"}")

write_file(265, "recursive_tree_sum",
"# Demo: recursive sum of binary tree values\n"
"class TreeNode {\n"
"    let Value : int := 0\n"
"    let Left := null\n"
"    let Right := null\n"
"    Constructor(v : int, l, r)\n"
"        Value := v\n"
"        Left := l\n"
"        Right := r\n"
"    end\n"
"}\n"
"function treeSum(node) -> int\n"
"    if node == null then\n"
"        return 0\n"
"    end\n"
"    return node.Value + treeSum(node.Left) + treeSum(node.Right)\n"
"end\n"
"let leaf1 := new TreeNode(1, null, null)\n"
"let leaf2 := new TreeNode(2, null, null)\n"
"let leaf3 := new TreeNode(3, null, null)\n"
"let root := new TreeNode(10, leaf1, leaf2)\n"
"print treeSum(root)")

write_file(266, "functional_pipeline",
"# Demo: functional pipeline pattern\n"
"function pipe(value : int, transforms)\n"
"    let result := value\n"
"    foreach t in transforms do\n"
"        result := t(result)\n"
"    end\n"
"    return result\n"
"end\n"
"let pipeline := [\n"
"    function(x : int) x + 1,\n"
"    function(x : int) x * 2,\n"
"    function(x : int) x - 3\n"
"]\n"
"print pipe(5, pipeline)")

write_file(267, "variadic_like",
"# Demo: simulating variadic arguments using array\n"
"function sumAll(nums)\n"
"    let total := 0\n"
"    foreach n in nums do\n"
"        total := total + n\n"
"    end\n"
"    return total\n"
"end\n"
"print sumAll([1, 2, 3])\n"
"print sumAll([1, 2, 3, 4, 5, 6, 7, 8, 9, 10])")

write_file(268, "partial_application",
"# Demo: partial application pattern\n"
"function partial(f, first : int)\n"
"    return function(second : int) f(first, second)\n"
"end\n"
"function add(a : int, b : int) -> int\n"
"    return a + b\n"
"end\n"
"let add5 := partial(add, 5)\n"
"print add5(3)\n"
"print add5(10)")

write_file(269, "memoize_pattern",
"# Demo: memoization pattern using array cache\n"
"let cache := []\n"
"function memoFib(n : int) -> int\n"
"    if n <= 1 then\n"
"        return n\n"
"    end\n"
"    if n < len(cache) and cache[n] != null then\n"
"        return cache[n]\n"
"    end\n"
"    let result := memoFib(n - 1) + memoFib(n - 2)\n"
"    cache[n] := result\n"
"    return result\n"
"end\n"
"print memoFib(20)")

write_file(270, "decorator_pattern",
"# Demo: decorator/wrapper pattern\n"
"function logged(f, name : string)\n"
"    return function(x : int)\n"
"        print \"Calling \" + name + \"(\" + str(x) + \")\"\n"
"        let result := f(x)\n"
"        print \"Result: \" + str(result)\n"
"        return result\n"
"    end\n"
"end\n"
"function square(n : int) -> int\n"
"    return n * n\n"
"end\n"
"let loggedSquare := logged(square, \"square\")\n"
"let r := loggedSquare(5)\n"
"print r")

write_file(271, "observer_pattern",
"# Demo: observer/event pattern\n"
"class EventEmitter {\n"
"    let Handlers : array := []\n"
"    let Count : int := 0\n"
"    function on(handler)\n"
"        Handlers[Count] := handler\n"
"        Count := Count + 1\n"
"    end\n"
"    function emit(data)\n"
"        let i := 0\n"
"        while i < Count do\n"
"            Handlers[i](data)\n"
"            i := i + 1\n"
"        end\n"
"    end\n"
"}\n"
"let emitter := new EventEmitter()\n"
"emitter.on(function(x) print \"Handler 1: \" + str(x))\n"
"emitter.on(function(x) print \"Handler 2: \" + str(x * 2))\n"
"emitter.emit(42)")

write_file(272, "strategy_pattern",
"# Demo: strategy pattern using lambda functions\n"
"function sort(arr, compareFn)\n"
"    let n := len(arr)\n"
"    let i := 0\n"
"    while i < n - 1 do\n"
"        let j := 0\n"
"        while j < n - i - 1 do\n"
"            if compareFn(arr[j], arr[j + 1]) > 0 then\n"
"                let temp := arr[j]\n"
"                arr[j] := arr[j + 1]\n"
"                arr[j + 1] := temp\n"
"            end\n"
"            j := j + 1\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return arr\n"
"end\n"
"let nums := [3, 1, 4, 1, 5, 9, 2, 6]\n"
"let ascending := sort(nums, function(a : int, b : int) a - b)\n"
"foreach n in ascending do\n"
"    print n\n"
"end")

write_file(273, "prime_factorization",
"# Demo: prime factorization\n"
"function primeFactors(n : int)\n"
"    let factors := []\n"
"    let idx := 0\n"
"    let d := 2\n"
"    while d * d <= n do\n"
"        while n % d == 0 do\n"
"            factors[idx] := d\n"
"            idx := idx + 1\n"
"            n := n // d\n"
"        end\n"
"        d := d + 1\n"
"    end\n"
"    if n > 1 then\n"
"        factors[idx] := n\n"
"    end\n"
"    return factors\n"
"end\n"
"let factors := primeFactors(360)\n"
"foreach f in factors do\n"
"    print f\n"
"end")

write_file(274, "number_base_convert",
"# Demo: decimal to binary conversion\n"
"function decToBin(n : int) -> string\n"
"    if n == 0 then\n"
"        return \"0\"\n"
"    end\n"
"    let result := \"\"\n"
"    let remaining := n\n"
"    while remaining > 0 do\n"
"        result := str(remaining % 2) + result\n"
"        remaining := remaining // 2\n"
"    end\n"
"    return result\n"
"end\n"
"print decToBin(0)\n"
"print decToBin(10)\n"
"print decToBin(255)\n"
"print decToBin(42)")

write_file(275, "string_starts_with",
"# Demo: check if string starts with a prefix\n"
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
'print startsWith("Hello, World", "Hello")\n'
'print startsWith("Hello, World", "World")\n'
'print startsWith("abc", "abcd")')

write_file(276, "string_ends_with",
"# Demo: check if string ends with a suffix\n"
"function endsWith(s : string, suffix : string) -> bool\n"
"    let sLen := len(s)\n"
"    let sufLen := len(suffix)\n"
"    if sufLen > sLen then\n"
"        return false\n"
"    end\n"
"    let i := 0\n"
"    while i < sufLen do\n"
"        if s[sLen - sufLen + i] != suffix[i] then\n"
"            return false\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
'print endsWith("Hello, World", "World")\n'
'print endsWith("Hello, World", "Hello")\n'
'print endsWith("test.tlg", ".tlg")')

write_file(277, "string_index_of",
"# Demo: find index of first occurrence of a character\n"
"function indexOf(s : string, ch : string) -> int\n"
"    let i := 0\n"
"    while i < len(s) do\n"
"        if s[i] == ch then\n"
"            return i\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return -1\n"
"end\n"
'print indexOf("Hello, World", "W")\n'
'print indexOf("Hello, World", "z")\n'
'print indexOf("abcabc", "b")')

write_file(278, "string_substring",
"# Demo: manual substring extraction\n"
"function substring(s : string, start : int, length : int) -> string\n"
"    let result := \"\"\n"
"    let i := start\n"
"    while i < start + length and i < len(s) do\n"
"        result := result + s[i]\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
'print substring("Hello, World", 7, 5)\n'
'print substring("TinyLanguage", 0, 4)\n'
'print substring("abcdef", 2, 3)')

write_file(279, "matrix_multiply",
"# Demo: 2x2 matrix multiplication\n"
"function matMul(a, b)\n"
"    let result := [[0, 0], [0, 0]]\n"
"    let i := 0\n"
"    while i < 2 do\n"
"        let j := 0\n"
"        while j < 2 do\n"
"            let k := 0\n"
"            while k < 2 do\n"
"                result[i][j] := result[i][j] + a[i][k] * b[k][j]\n"
"                k := k + 1\n"
"            end\n"
"            j := j + 1\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let a := [[1, 2], [3, 4]]\n"
"let b := [[5, 6], [7, 8]]\n"
"let c := matMul(a, b)\n"
"print c[0][0]\n"
"print c[0][1]\n"
"print c[1][0]\n"
"print c[1][1]")

write_file(280, "gcd_lcm",
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
"print gcd(48, 18)\n"
"print gcd(100, 75)\n"
"print lcm(4, 6)\n"
"print lcm(12, 18)")

# 281-320: Final batch for 300+ total

write_file(281, "catalan_numbers",
"# Demo: Catalan numbers\n"
"function catalan(n : int) -> int\n"
"    if n <= 1 then\n"
"        return 1\n"
"    end\n"
"    let result := 0\n"
"    let i := 0\n"
"    while i < n do\n"
"        result := result + catalan(i) * catalan(n - 1 - i)\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"for i := 0 to 8 do\n"
"    print catalan(i)\n"
"end")

write_file(282, "pascal_triangle",
"# Demo: Pascal triangle\n"
"function pascal(rows : int)\n"
"    let triangle := [[1]]\n"
"    for row := 1 to rows - 1 do\n"
"        let prev := triangle[row - 1]\n"
"        let current := [1]\n"
"        let j := 1\n"
"        while j < len(prev) do\n"
"            current[j] := prev[j - 1] + prev[j]\n"
"            j := j + 1\n"
"        end\n"
"        current[len(prev)] := 1\n"
"        triangle[row] := current\n"
"    end\n"
"    return triangle\n"
"end\n"
"let tri := pascal(5)\n"
"foreach row in tri do\n"
"    foreach val in row do\n"
"        print val\n"
"    end\n"
"end")

write_file(283, "memoized_catalan",
"# Demo: memoized Catalan numbers\n"
"let catalanCache := [1, 1]\n"
"function catalanMemo(n : int) -> int\n"
"    if n < len(catalanCache) then\n"
"        return catalanCache[n]\n"
"    end\n"
"    let result := 0\n"
"    let i := 0\n"
"    while i < n do\n"
"        result := result + catalanMemo(i) * catalanMemo(n - 1 - i)\n"
"        i := i + 1\n"
"    end\n"
"    catalanCache[n] := result\n"
"    return result\n"
"end\n"
"for i := 0 to 10 do\n"
"    print catalanMemo(i)\n"
"end")

write_file(284, "sieve_optimized",
"# Demo: sieve of Eratosthenes with explicit array\n"
"let limit := 50\n"
"let sieve := []\n"
"let i := 0\n"
"while i <= limit do\n"
"    sieve[i] := true\n"
"    i := i + 1\n"
"end\n"
"sieve[0] := false\n"
"sieve[1] := false\n"
"let p := 2\n"
"while p * p <= limit do\n"
"    if sieve[p] then\n"
"        let m := p * p\n"
"        while m <= limit do\n"
"            sieve[m] := false\n"
"            m := m + p\n"
"        end\n"
"    end\n"
"    p := p + 1\n"
"end\n"
"let n := 2\n"
"while n <= limit do\n"
"    if sieve[n] then\n"
"        print n\n"
"    end\n"
"    n := n + 1\n"
"end")

write_file(285, "knapsack_greedy",
"# Demo: greedy knapsack approximation\n"
"let weights := [2, 3, 4, 5]\n"
"let values := [3, 4, 5, 6]\n"
"let capacity := 8\n"
"let totalValue := 0\n"
"let totalWeight := 0\n"
"let i := 0\n"
"while i < len(weights) do\n"
"    if totalWeight + weights[i] <= capacity then\n"
"        totalWeight := totalWeight + weights[i]\n"
"        totalValue := totalValue + values[i]\n"
"    end\n"
"    i := i + 1\n"
"end\n"
"print totalValue\n"
"print totalWeight")

write_file(286, "string_to_array_of_chars",
"# Demo: convert string to array of characters\n"
"function strToChars(s : string)\n"
"    let chars := []\n"
"    let i := 0\n"
"    foreach ch in s do\n"
"        chars[i] := ch\n"
"        i := i + 1\n"
"    end\n"
"    return chars\n"
"end\n"
"let chars := strToChars(\"Hello\")\n"
"foreach c in chars do\n"
"    print c\n"
"end")

write_file(287, "int_to_str_manual",
"# Demo: manual integer to string conversion\n"
"function intToStr(n : int) -> string\n"
"    if n == 0 then\n"
"        return \"0\"\n"
"    end\n"
"    let result := \"\"\n"
"    let negative := n < 0\n"
"    if negative then\n"
"        n := -n\n"
"    end\n"
"    while n > 0 do\n"
"        let digit := n % 10\n"
"        result := str(digit) + result\n"
"        n := n // 10\n"
"    end\n"
"    if negative then\n"
"        result := \"-\" + result\n"
"    end\n"
"    return result\n"
"end\n"
"print intToStr(0)\n"
"print intToStr(12345)\n"
"print intToStr(-42)")

write_file(288, "fizzbuzz_functional",
"# Demo: FizzBuzz using functional style\n"
"function fizzBuzzLabel(n : int) -> string\n"
"    if n % 15 == 0 then return \"FizzBuzz\" end\n"
"    if n % 3 == 0 then return \"Fizz\" end\n"
"    if n % 5 == 0 then return \"Buzz\" end\n"
"    return str(n)\n"
"end\n"
"for i := 1 to 20 do\n"
"    print fizzBuzzLabel(i)\n"
"end")

write_file(289, "fizzbuzz_match",
"# Demo: FizzBuzz using pattern matching\n"
"for i := 1 to 20 do\n"
"    let f := i % 3 == 0\n"
"    let b := i % 5 == 0\n"
"    match true {\n"
"        x when f and b => print \"FizzBuzz\"\n"
"        x when f => print \"Fizz\"\n"
"        x when b => print \"Buzz\"\n"
"        _ => print i\n"
"    }\n"
"end")

write_file(290, "coin_change",
"# Demo: coin change (greedy)\n"
"function coinChange(amount : int) -> int\n"
"    let coins := [25, 10, 5, 1]\n"
"    let total := 0\n"
"    let remaining := amount\n"
"    foreach coin in coins do\n"
"        let count := remaining // coin\n"
"        total := total + count\n"
"        remaining := remaining % coin\n"
"    end\n"
"    return total\n"
"end\n"
"print coinChange(41)\n"
"print coinChange(99)\n"
"print coinChange(100)")

write_file(291, "number_patterns",
"# Demo: number patterns (triangle of stars)\n"
"for i := 1 to 5 do\n"
"    let line := \"\"\n"
"    for j := 1 to i do\n"
"        line := line + \"*\"\n"
"    end\n"
"    print line\n"
"end")

write_file(292, "diamond_pattern",
"# Demo: diamond pattern\n"
"let n := 5\n"
"for i := 1 to n do\n"
"    let spaces := n - i\n"
"    let stars := 2 * i - 1\n"
"    let line := \"\"\n"
"    let s := 0\n"
"    while s < spaces do\n"
"        line := line + \" \"\n"
"        s := s + 1\n"
"    end\n"
"    let st := 0\n"
"    while st < stars do\n"
"        line := line + \"*\"\n"
"        st := st + 1\n"
"    end\n"
"    print line\n"
"end\n"
"for i := n - 1 to 1 step -1 do\n"
"    let spaces := n - i\n"
"    let stars := 2 * i - 1\n"
"    let line := \"\"\n"
"    let s := 0\n"
"    while s < spaces do\n"
"        line := line + \" \"\n"
"        s := s + 1\n"
"    end\n"
"    let st := 0\n"
"    while st < stars do\n"
"        line := line + \"*\"\n"
"        st := st + 1\n"
"    end\n"
"    print line\n"
"end")

write_file(293, "stack_based_eval",
"# Demo: simple stack-based RPN evaluator\n"
"let stack := []\n"
"let top := 0\n"
"function push(v)\n"
"    stack[top] := v\n"
"    top := top + 1\n"
"end\n"
"function pop() -> int\n"
"    top := top - 1\n"
"    return stack[top]\n"
"end\n"
"# Evaluate: 3 4 + 2 *  = (3+4)*2 = 14\n"
"push(3)\n"
"push(4)\n"
"push(pop() + pop())\n"
"push(2)\n"
"push(pop() * pop())\n"
"print pop()")

write_file(294, "fibonacci_closed_form",
"# Demo: Fibonacci approximation via ratio\n"
"let phi := (1.0 + 5.0 ** 0.5) / 2.0\n"
"print phi\n"
"let fib10 := phi ** 10 / (5.0 ** 0.5)\n"
"print fib10")

write_file(295, "euler_problem1",
"# Demo: Euler Project Problem 1 - sum of multiples of 3 or 5 below 1000\n"
"let sum := 0\n"
"for i := 1 to 999 do\n"
"    if i % 3 == 0 or i % 5 == 0 then\n"
"        sum := sum + i\n"
"    end\n"
"end\n"
"print sum")

write_file(296, "euler_problem2",
"# Demo: Euler Project Problem 2 - even Fibonacci numbers below 4 million\n"
"let sum := 0\n"
"let a := 1\n"
"let b := 2\n"
"while b < 4000000 do\n"
"    if b % 2 == 0 then\n"
"        sum := sum + b\n"
"    end\n"
"    let temp := a + b\n"
"    a := b\n"
"    b := temp\n"
"end\n"
"print sum")

write_file(297, "list_all_divs",
"# Demo: list all divisors of a number\n"
"function divisors(n : int)\n"
"    let result := []\n"
"    let idx := 0\n"
"    let i := 1\n"
"    while i * i <= n do\n"
"        if n % i == 0 then\n"
"            result[idx] := i\n"
"            idx := idx + 1\n"
"            if i != n // i then\n"
"                result[idx] := n // i\n"
"                idx := idx + 1\n"
"            end\n"
"        end\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let divs := divisors(36)\n"
"foreach d in divs do\n"
"    print d\n"
"end")

write_file(298, "matrix_transpose",
"# Demo: matrix transpose\n"
"function transpose(matrix)\n"
"    let rows := len(matrix)\n"
"    let cols := len(matrix[0])\n"
"    let result := []\n"
"    let j := 0\n"
"    while j < cols do\n"
"        let row := []\n"
"        let i := 0\n"
"        while i < rows do\n"
"            row[i] := matrix[i][j]\n"
"            i := i + 1\n"
"        end\n"
"        result[j] := row\n"
"        j := j + 1\n"
"    end\n"
"    return result\n"
"end\n"
"let m := [[1, 2, 3], [4, 5, 6]]\n"
"let t := transpose(m)\n"
"foreach row in t do\n"
"    foreach val in row do\n"
"        print val\n"
"    end\n"
"end")

write_file(299, "words_frequency",
"# Demo: count word frequency in a sentence\n"
"function split(s : string, delim : string)\n"
"    let parts := []\n"
"    let current := \"\"\n"
"    let idx := 0\n"
"    foreach ch in s do\n"
"        if ch == delim then\n"
"            if len(current) > 0 then\n"
"                parts[idx] := current\n"
"                idx := idx + 1\n"
"                current := \"\"\n"
"            end\n"
"        else\n"
"            current := current + ch\n"
"        end\n"
"    end\n"
"    if len(current) > 0 then\n"
"        parts[idx] := current\n"
"    end\n"
"    return parts\n"
"end\n"
"let sentence := \"the cat sat on the mat and the cat\"\n"
"let words := split(sentence, \" \")\n"
"print len(words)")

write_file(300, "blackjack_hand_value",
"# Demo: calculate blackjack hand value\n"
"function cardValue(card : string) -> int\n"
"    if card == \"A\" then return 11 end\n"
"    if card == \"K\" or card == \"Q\" or card == \"J\" then return 10 end\n"
"    return int(card)\n"
"end\n"
"function handValue(hand)\n"
"    let total := 0\n"
"    let aces := 0\n"
"    foreach card in hand do\n"
"        let v := cardValue(card)\n"
"        total := total + v\n"
"        if card == \"A\" then\n"
"            aces := aces + 1\n"
"        end\n"
"    end\n"
"    while total > 21 and aces > 0 do\n"
"        total := total - 10\n"
"        aces := aces - 1\n"
"    end\n"
"    return total\n"
"end\n"
"print handValue([\"A\", \"K\"])\n"
"print handValue([\"7\", \"8\", \"9\"])\n"
"print handValue([\"A\", \"A\", \"9\"])")

write_file(301, "countdown_timer",
"# Demo: countdown timer logic\n"
"function countdown(seconds : int)\n"
"    let remaining := seconds\n"
"    while remaining > 0 do\n"
"        let mins := remaining // 60\n"
"        let secs := remaining % 60\n"
"        print str(mins) + \":\" + (if secs < 10 then \"0\" + str(secs) else str(secs))\n"
"        remaining := remaining - 1\n"
"    end\n"
"    print \"Done!\"\n"
"end\n"
"countdown(5)")

write_file(302, "binary_ops_demo",
"# Demo: bitwise-style operations using arithmetic\n"
"# In TinyLanguage, bitwise ops are not keywords,\n"
"# but & at additive level does string concat.\n"
"# Demonstrate bit manipulation via arithmetic:\n"
"function bitwiseAnd(a : int, b : int) -> int\n"
"    let result := 0\n"
"    let bit := 1\n"
"    while a > 0 and b > 0 do\n"
"        if a % 2 == 1 and b % 2 == 1 then\n"
"            result := result + bit\n"
"        end\n"
"        a := a // 2\n"
"        b := b // 2\n"
"        bit := bit * 2\n"
"    end\n"
"    return result\n"
"end\n"
"print bitwiseAnd(12, 10)\n"
"print bitwiseAnd(255, 15)")

write_file(303, "all_ops_in_expr",
"# Demo: expression using many operators\n"
"let a := 10\n"
"let b := 3\n"
"let c := 2\n"
"print a + b - c * 2\n"
"print a / b\n"
"print a // b\n"
"print a % b\n"
"print a ** c\n"
"print a > b and b > c\n"
"print a != b or b == c\n"
"print !false and true")

write_file(304, "mixed_declarations",
"# Demo: mixing let, var, const in one scope\n"
"const MAX_SIZE : int := 100\n"
"var counter : int := 0\n"
"let message := \"Hello\"\n"
"counter := counter + 1\n"
"print MAX_SIZE\n"
"print counter\n"
"print message")

write_file(305, "class_with_all_member_types",
"# Demo: class with all member types (let, var, const, function, constructor)\n"
"class Config {\n"
"    const VERSION : string := \"1.0\"\n"
"    let Name : string := \"\"\n"
"    var MaxItems : int := 10\n"
"    Constructor(name : string)\n"
"        Name := name\n"
"    end\n"
"    function describe() -> string\n"
"        return Name + \" v\" + VERSION + \" (max=\" + str(MaxItems) + \")\"\n"
"    end\n"
"    static function create(name : string) -> Config\n"
"        return new Config(name)\n"
"    end\n"
"}\n"
"let cfg := Config.create(\"MyApp\")\n"
"print cfg.describe()")

write_file(306, "inline_if_in_function_call",
"# Demo: inline conditional expression in function call argument\n"
"function double(n : int) -> int\n"
"    return n * 2\n"
"end\n"
"let x := 5\n"
"print double(if x > 0 then x else -x)")

write_file(307, "ternary_in_function_call",
"# Demo: ternary operator in function call argument\n"
"function label(n : int) -> string\n"
"    return \"[\" + str(n) + \"]\"\n"
"end\n"
"let x := 42\n"
"print label(x > 0 ? x : -x)")

write_file(308, "lambda_returning_lambda",
"# Demo: lambda that returns another lambda\n"
"let makeMultiplier := function(factor : int)\n"
"    return function(x : int) x * factor\n"
"end\n"
"let times3 := makeMultiplier(3)\n"
"let times5 := makeMultiplier(5)\n"
"print times3(7)\n"
"print times5(7)")

write_file(309, "exception_in_loop",
"# Demo: exception handling inside a loop\n"
"let data := [10, 0, 5, 0, 2]\n"
"let numerator := 100\n"
"foreach divisor in data do\n"
"    try\n"
"        if divisor == 0 then\n"
"            throw \"division by zero\"\n"
"        end\n"
"        print numerator // divisor\n"
"    catch e\n"
"        print \"Error: \" + str(e)\n"
"    end\n"
"end")

write_file(310, "recursive_palindrome",
"# Demo: recursive palindrome check\n"
"function isPalindrome(s : string, lo : int, hi : int) -> bool\n"
"    if lo >= hi then\n"
"        return true\n"
"    end\n"
"    if s[lo] != s[hi] then\n"
"        return false\n"
"    end\n"
"    return isPalindrome(s, lo + 1, hi - 1)\n"
"end\n"
"function checkPalindrome(s : string) -> bool\n"
"    return isPalindrome(s, 0, len(s) - 1)\n"
"end\n"
"print checkPalindrome(\"racecar\")\n"
"print checkPalindrome(\"hello\")\n"
"print checkPalindrome(\"level\")")

write_file(311, "while_break_continue",
"# Demo: while loop with both break and continue\n"
"let i := 0\n"
"let sum := 0\n"
"while true do\n"
"    i := i + 1\n"
"    if i > 20 then\n"
"        break\n"
"    end\n"
"    if i % 2 == 0 then\n"
"        continue\n"
"    end\n"
"    sum := sum + i\n"
"end\n"
"print sum")

write_file(312, "multiple_catch_patterns",
"# Demo: multiple try-catch blocks for different scenarios\n"
"function safeDivide(a : int, b : int) -> int\n"
"    if b == 0 then\n"
"        throw \"ZeroDivisionError\"\n"
"    end\n"
"    return a // b\n"
"end\n"
"let pairs := [[10, 2], [15, 0], [9, 3]]\n"
"foreach pair in pairs do\n"
"    try\n"
"        print safeDivide(pair[0], pair[1])\n"
"    catch e\n"
"        print \"Error dividing \" + str(pair[0]) + \" by \" + str(pair[1]) + \": \" + str(e)\n"
"    end\n"
"end")

write_file(313, "class_counter_with_reset",
"# Demo: class with reset method\n"
"class Counter {\n"
"    let Value : int := 0\n"
"    let Step : int := 1\n"
"    Constructor(step : int)\n"
"        Step := step\n"
"    end\n"
"    function increment()\n"
"        Value := Value + Step\n"
"    end\n"
"    function reset()\n"
"        Value := 0\n"
"    end\n"
"    function get() -> int\n"
"        return Value\n"
"    end\n"
"}\n"
"let c := new Counter(5)\n"
"c.increment()\n"
"c.increment()\n"
"c.increment()\n"
"print c.get()\n"
"c.reset()\n"
"print c.get()")

write_file(314, "compound_assignments_all",
"# Demo: all compound assignment operators\n"
"let n := 10\n"
"n += 5\n"
"print n\n"
"n -= 3\n"
"print n\n"
"n *= 2\n"
"print n\n"
"n /= 4\n"
"print n")

write_file(315, "incr_decr_patterns",
"# Demo: increment and decrement in various contexts\n"
"let a := 5\n"
"a++\n"
"print a\n"
"a++\n"
"print a\n"
"a--\n"
"print a\n"
"a--\n"
"print a\n"
"a--\n"
"print a")

write_file(316, "nested_lambda",
"# Demo: nested lambda expressions\n"
"let compose := function(f, g)\n"
"    return function(x : int) f(g(x))\n"
"end\n"
"let double := function(x : int) x * 2\n"
"let addOne := function(x : int) x + 1\n"
"let doubleAfterAddOne := compose(double, addOne)\n"
"print doubleAfterAddOne(5)\n"
"let addOneAfterDouble := compose(addOne, double)\n"
"print addOneAfterDouble(5)")

write_file(317, "match_with_guards_complex",
"# Demo: match with complex when guards\n"
"function categorize(n : int) -> string\n"
"    match n {\n"
"        x when x < 0 => return \"negative\"\n"
"        0 => return \"zero\"\n"
"        x when x % 2 == 0 and x < 100 => return \"small even positive\"\n"
"        x when x % 2 != 0 and x < 100 => return \"small odd positive\"\n"
"        _ => return \"large positive\"\n"
"    }\n"
"end\n"
"print categorize(-5)\n"
"print categorize(0)\n"
"print categorize(42)\n"
"print categorize(99)\n"
"print categorize(1000)")

write_file(318, "class_linked_list",
"# Demo: doubly-linked list\n"
"class DNode {\n"
"    let Val : int := 0\n"
"    let Next := null\n"
"    let Prev := null\n"
"    Constructor(v : int)\n"
"        Val := v\n"
"    end\n"
"}\n"
"class DList {\n"
"    let Head := null\n"
"    let Tail := null\n"
"    function append(v : int)\n"
"        let node := new DNode(v)\n"
"        if Head == null then\n"
"            Head := node\n"
"            Tail := node\n"
"        else\n"
"            node.Prev := Tail\n"
"            Tail.Next := node\n"
"            Tail := node\n"
"        end\n"
"    end\n"
"    function printForward()\n"
"        let cur := Head\n"
"        while cur != null do\n"
"            print cur.Val\n"
"            cur := cur.Next\n"
"        end\n"
"    end\n"
"    function printBackward()\n"
"        let cur := Tail\n"
"        while cur != null do\n"
"            print cur.Val\n"
"            cur := cur.Prev\n"
"        end\n"
"    end\n"
"}\n"
"let dl := new DList()\n"
"dl.append(1)\n"
"dl.append(2)\n"
"dl.append(3)\n"
"dl.printForward()\n"
"dl.printBackward()")

write_file(319, "fibonacci_sequence_match",
"# Demo: fibonacci using match expression\n"
"function fib(n : int) -> int\n"
"    match n {\n"
"        0 => return 0\n"
"        1 => return 1\n"
"        _ => return fib(n - 1) + fib(n - 2)\n"
"    }\n"
"end\n"
"for i := 0 to 10 do\n"
"    print fib(i)\n"
"end")

write_file(320, "all_features_combined",
"# Demo: combining many features in one program\n"
"const LIMIT : int := 10\n"
"function sumOfPrimes(limit : int) -> int\n"
"    let sum := 0\n"
"    for n := 2 to limit do\n"
"        let isPrime := true\n"
"        let d := 2\n"
"        while d * d <= n do\n"
"            if n % d == 0 then\n"
"                isPrime := false\n"
"                break\n"
"            end\n"
"            d := d + 1\n"
"        end\n"
"        if isPrime then\n"
"            sum := sum + n\n"
"        end\n"
"    end\n"
"    return sum\n"
"end\n"
"print sumOfPrimes(LIMIT)\n"
"let fibs := [0, 1]\n"
"for i := 2 to LIMIT do\n"
"    fibs[i] := fibs[i - 1] + fibs[i - 2]\n"
"end\n"
"let total := 0\n"
"foreach f in fibs do\n"
"    total := total + f\n"
"end\n"
"print total")

# Additional batch to ensure we're well over 300

write_file(321, "number_sequence_squares",
"# Demo: print squares of numbers in a range\n"
"let start := 1\n"
"let stop := 20\n"
"for i := start to stop do\n"
"    print i * i\n"
"end")

write_file(322, "chain_string_ops",
"# Demo: chain of string operations\n"
"function process(s : string) -> string\n"
"    let step1 := s + \"!\"\n"
"    let step2 := step1 + step1\n"
"    return step2\n"
"end\n"
"print process(\"Hello\")\n"
"print process(\"World\")")

write_file(323, "fibonacci_ratio",
"# Demo: ratio of consecutive Fibonacci numbers approaches phi\n"
"let a := 1\n"
"let b := 1\n"
"for i := 1 to 10 do\n"
"    let ratio := b / a\n"
"    print ratio\n"
"    let temp := a + b\n"
"    a := b\n"
"    b := temp\n"
"end")

write_file(324, "leap_year",
"# Demo: leap year check\n"
"function isLeapYear(year : int) -> bool\n"
"    if year % 400 == 0 then return true end\n"
"    if year % 100 == 0 then return false end\n"
"    return year % 4 == 0\n"
"end\n"
"print isLeapYear(2000)\n"
"print isLeapYear(1900)\n"
"print isLeapYear(2024)\n"
"print isLeapYear(2023)")

write_file(325, "days_in_month",
"# Demo: days in month\n"
"function daysInMonth(month : int, year : int) -> int\n"
"    if month == 2 then\n"
"        let isLeap := year % 400 == 0 or (year % 4 == 0 and year % 100 != 0)\n"
"        return if isLeap then 29 else 28\n"
"    end\n"
"    if month == 4 or month == 6 or month == 9 or month == 11 then\n"
"        return 30\n"
"    end\n"
"    return 31\n"
"end\n"
"for month := 1 to 12 do\n"
"    print daysInMonth(month, 2024)\n"
"end")

write_file(326, "statistics_mean",
"# Demo: compute mean of an array\n"
"function mean(arr) -> float\n"
"    let total := 0\n"
"    foreach x in arr do\n"
"        total := total + x\n"
"    end\n"
"    return total / len(arr)\n"
"end\n"
"let data := [2, 4, 4, 4, 5, 5, 7, 9]\n"
"print mean(data)")

write_file(327, "statistics_variance",
"# Demo: compute variance of an array\n"
"function mean(arr) -> float\n"
"    let total := 0\n"
"    foreach x in arr do\n"
"        total := total + x\n"
"    end\n"
"    return total / len(arr)\n"
"end\n"
"function variance(arr) -> float\n"
"    let m := mean(arr)\n"
"    let sumSq := 0.0\n"
"    foreach x in arr do\n"
"        let diff := x - m\n"
"        sumSq := sumSq + diff * diff\n"
"    end\n"
"    return sumSq / len(arr)\n"
"end\n"
"let data := [2, 4, 4, 4, 5, 5, 7, 9]\n"
"print mean(data)\n"
"print variance(data)")

write_file(328, "number_classifier",
"# Demo: number classifier using multiple functions\n"
"function isPositive(n : int) -> bool\n"
"    return n > 0\n"
"end\n"
"function isEven(n : int) -> bool\n"
"    return n % 2 == 0\n"
"end\n"
"function isPrime(n : int) -> bool\n"
"    if n < 2 then return false end\n"
"    let i := 2\n"
"    while i * i <= n do\n"
"        if n % i == 0 then return false end\n"
"        i := i + 1\n"
"    end\n"
"    return true\n"
"end\n"
"for n := -5 to 20 do\n"
"    let desc := str(n) + \":\"\n"
"    if isPositive(n) then desc := desc + \" positive\" end\n"
"    if isEven(n) then desc := desc + \" even\" else desc := desc + \" odd\" end\n"
"    if isPrime(n) then desc := desc + \" prime\" end\n"
"    print desc\n"
"end")

write_file(329, "curry_function",
"# Demo: currying via lambdas\n"
"function curry(f)\n"
"    return function(a : int)\n"
"        return function(b : int)\n"
"            return f(a, b)\n"
"        end\n"
"    end\n"
"end\n"
"function add(a : int, b : int) -> int\n"
"    return a + b\n"
"end\n"
"let curriedAdd := curry(add)\n"
"let add10 := curriedAdd(10)\n"
"print add10(5)\n"
"print add10(20)")

write_file(330, "array_partition",
"# Demo: partition array into two groups based on predicate\n"
"function partition(arr, pred)\n"
"    let trueArr := []\n"
"    let falseArr := []\n"
"    let ti := 0\n"
"    let fi := 0\n"
"    foreach item in arr do\n"
"        if pred(item) then\n"
"            trueArr[ti] := item\n"
"            ti := ti + 1\n"
"        else\n"
"            falseArr[fi] := item\n"
"            fi := fi + 1\n"
"        end\n"
"    end\n"
"    return [trueArr, falseArr]\n"
"end\n"
"let nums := [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]\n"
"let parts := partition(nums, function(n : int) n % 2 == 0)\n"
"print \"Even:\"\n"
"foreach e in parts[0] do\n"
"    print e\n"
"end\n"
"print \"Odd:\"\n"
"foreach o in parts[1] do\n"
"    print o\n"
"end")

print("Done batch 176-330")
