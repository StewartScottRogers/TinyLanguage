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

write_file(151, "bmi_calculator",
"# Demo: BMI calculator\n"
"function bmi(weight : float, height : float) -> float\n"
"    return weight / (height * height)\n"
"end\n"
"function bmiCategory(bmiVal : float) -> string\n"
"    if bmiVal < 18.5 then\n"
'        return "Underweight"\n'
"    else if bmiVal < 25.0 then\n"
'        return "Normal"\n'
"    else if bmiVal < 30.0 then\n"
'        return "Overweight"\n'
"    else\n"
'        return "Obese"\n'
"    end\n"
"end\n"
"let bmiVal := bmi(70.0, 1.75)\n"
"print bmiVal\n"
"print bmiCategory(bmiVal)")

write_file(152, "celsius_scale",
"# Demo: generate Celsius to Fahrenheit table\n"
"for c := 0 to 100 step 10 do\n"
"    let f := c * 9 // 5 + 32\n"
'    print str(c) + "C = " + str(f) + "F"\n'
"end")

write_file(153, "digit_sum2",
"# Demo: sum the digits of a number\n"
"function digitSumFn(n : int) -> int\n"
"    let sum := 0\n"
"    let remaining := n\n"
"    while remaining > 0 do\n"
"        sum := sum + remaining % 10\n"
"        remaining := remaining // 10\n"
"    end\n"
"    return sum\n"
"end\n"
"print digitSumFn(12345)\n"
"print digitSumFn(999)\n"
"print digitSumFn(100)")

write_file(154, "count_digits2",
"# Demo: count digits of a number\n"
"function countDigitsFn(n : int) -> int\n"
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
"print countDigitsFn(0)\n"
"print countDigitsFn(42)\n"
"print countDigitsFn(12345678)")

write_file(155, "perfect_number2",
"# Demo: perfect number check up to 30\n"
"function isPerfect2(n : int) -> bool\n"
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
"    if isPerfect2(i) then\n"
"        print i\n"
"    end\n"
"end")

write_file(156, "triangle_numbers2",
"# Demo: triangle numbers up to 10\n"
"function triangleNum2(n : int) -> int\n"
"    return n * (n + 1) // 2\n"
"end\n"
"for i := 1 to 10 do\n"
"    print triangleNum2(i)\n"
"end")

write_file(157, "power_of_two",
"# Demo: check if number is power of 2\n"
"function isPow2(n : int) -> bool\n"
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
"print isPow2(1)\n"
"print isPow2(2)\n"
"print isPow2(16)\n"
"print isPow2(15)\n"
"print isPow2(64)")

write_file(158, "abs_function2",
"# Demo: absolute value function\n"
"function absVal2(n : int) -> int\n"
"    if n < 0 then\n"
"        return -n\n"
"    end\n"
"    return n\n"
"end\n"
"print absVal2(-5)\n"
"print absVal2(5)\n"
"print absVal2(0)\n"
"print absVal2(-100)")

write_file(159, "max_min_fns",
"# Demo: max and min functions\n"
"function maxFn(a : int, b : int) -> int\n"
"    return if a > b then a else b\n"
"end\n"
"function minFn(a : int, b : int) -> int\n"
"    return if a < b then a else b\n"
"end\n"
"print maxFn(3, 7)\n"
"print maxFn(10, 2)\n"
"print minFn(3, 7)\n"
"print minFn(10, 2)")

write_file(160, "clamp_fn",
"# Demo: clamp a value between min and max\n"
"function clampFn(val : int, lo : int, hi : int) -> int\n"
"    if val < lo then\n"
"        return lo\n"
"    end\n"
"    if val > hi then\n"
"        return hi\n"
"    end\n"
"    return val\n"
"end\n"
"print clampFn(5, 0, 10)\n"
"print clampFn(-5, 0, 10)\n"
"print clampFn(15, 0, 10)")

write_file(161, "range_array",
"# Demo: generate array of range values\n"
"function makeRange2(start : int, stop : int, step : int)\n"
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
"let evens := makeRange2(0, 10, 2)\n"
"foreach n in evens do\n"
"    print n\n"
"end")

write_file(162, "string_repeat2",
"# Demo: repeat string n times\n"
"function repeatStr2(s : string, n : int) -> string\n"
'    let result := ""\n'
"    let i := 0\n"
"    while i < n do\n"
"        result := result + s\n"
"        i := i + 1\n"
"    end\n"
"    return result\n"
"end\n"
'print repeatStr2("abc", 3)\n'
'print repeatStr2("-", 10)\n'
'print repeatStr2("ha", 5)')

write_file(163, "ltrim_function",
"# Demo: manual string ltrim\n"
"function ltrimFn(s : string) -> string\n"
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
'print ltrimFn("   hello")\n'
'print ltrimFn("world")\n'
'print ltrimFn("  spaces  ")')

write_file(164, "is_vowel",
"# Demo: check if character is a vowel\n"
"function isVowel(ch : string) -> bool\n"
'    return ch == "a" or ch == "e" or ch == "i" or ch == "o" or ch == "u" or ch == "A" or ch == "E" or ch == "I" or ch == "O" or ch == "U"\n'
"end\n"
'print isVowel("a")\n'
'print isVowel("b")\n'
'print isVowel("E")\n'
'print isVowel("z")')

write_file(165, "count_vowels",
"# Demo: count vowels in a string\n"
"function isVowelChar(ch : string) -> bool\n"
'    return ch == "a" or ch == "e" or ch == "i" or ch == "o" or ch == "u"\n'
"end\n"
"function countVowels(s : string) -> int\n"
"    let count := 0\n"
"    foreach ch in s do\n"
"        if isVowelChar(ch) then\n"
"            count := count + 1\n"
"        end\n"
"    end\n"
"    return count\n"
"end\n"
'print countVowels("Hello World")\n'
'print countVowels("aeiou")\n'
'print countVowels("rhythm")')

write_file(166, "reduce_pattern",
"# Demo: reduce/fold pattern over array\n"
"function reduceArr(arr, initial : int, combine)\n"
"    let acc := initial\n"
"    foreach item in arr do\n"
"        acc := combine(acc, item)\n"
"    end\n"
"    return acc\n"
"end\n"
"let nums := [1, 2, 3, 4, 5]\n"
"let sum := reduceArr(nums, 0, function(a : int, b : int) a + b)\n"
"let product := reduceArr(nums, 1, function(a : int, b : int) a * b)\n"
"print sum\n"
"print product")

write_file(167, "zip_arrays2",
"# Demo: zip two arrays pair-wise\n"
"function zipSumArr(a, b)\n"
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
"let zipped := zipSumArr(xs, ys)\n"
"foreach item in zipped do\n"
"    print item\n"
"end")

write_file(168, "flatten_matrix",
"# Demo: flatten one level of nested arrays\n"
"function flattenMatrix(matrix)\n"
"    let result := []\n"
"    let idx := 0\n"
"    foreach row in matrix do\n"
"        foreach item in row do\n"
"            result[idx] := item\n"
"            idx := idx + 1\n"
"        end\n"
"    end\n"
"    return result\n"
"end\n"
"let matrix := [[1, 2], [3, 4], [5, 6]]\n"
"let flat := flattenMatrix(matrix)\n"
"foreach item in flat do\n"
"    print item\n"
"end")

write_file(169, "rotate_array2",
"# Demo: rotate array left by k positions\n"
"function rotateLeftArr(arr, k : int)\n"
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
"let rotated := rotateLeftArr(data, 2)\n"
"foreach item in rotated do\n"
"    print item\n"
"end")

write_file(170, "fibonacci_memo2",
"# Demo: Fibonacci with memoization array cache\n"
"let fib_memo := [0, 1]\n"
"function fibWithMemo(n : int) -> int\n"
"    if n < len(fib_memo) then\n"
"        return fib_memo[n]\n"
"    end\n"
"    let result := fibWithMemo(n - 1) + fibWithMemo(n - 2)\n"
"    fib_memo[n] := result\n"
"    return result\n"
"end\n"
"for i := 0 to 15 do\n"
"    print fibWithMemo(i)\n"
"end")

write_file(171, "exception_validation",
"# Demo: throwing and catching validation errors\n"
"function validateAgeVal(age : int) -> int\n"
"    if age < 0 then\n"
'        throw "Age cannot be negative"\n'
"    end\n"
"    if age > 150 then\n"
'        throw "Age is unrealistically high"\n'
"    end\n"
"    return age\n"
"end\n"
"try\n"
"    print validateAgeVal(25)\n"
"    print validateAgeVal(-1)\n"
"catch msg\n"
"    print \"Validation error: \" + str(msg)\n"
"end\n"
"try\n"
"    print validateAgeVal(200)\n"
"catch msg\n"
"    print \"Validation error: \" + str(msg)\n"
"end")

write_file(172, "nested_try_catch2",
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

write_file(173, "finally_cleanup2",
"# Demo: finally block for cleanup\n"
"function riskyOperation2(shouldFail : bool)\n"
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
"riskyOperation2(false)\n"
"riskyOperation2(true)")

write_file(174, "annotation_basic2",
"# Demo: annotation syntax on function\n"
"@deprecated\n"
"function oldFunction2()\n"
'    print "This is deprecated"\n'
"end\n"
"oldFunction2()")

write_file(175, "annotation_with_params2",
"# Demo: annotation with parameters\n"
"@version(major = 1, minor = 0)\n"
"function stableFunction2() -> string\n"
"    return \"stable\"\n"
"end\n"
"print stableFunction2()")

print("Done batch 151-175")
