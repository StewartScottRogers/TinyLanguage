# TinyLanguage — Master AI Prompt

Use all sections below as the complete specification. Every rule is mandatory unless explicitly marked optional.

---

# .NET Standards

## .NET Version Requirements
- Create a complete .NET 10.0 Solution.
- Create supporting Solution files.
- Create supporting Project files.
- All solution coding will be C#.
- Solution must be usable in Visual Studio 2026 or later.
- Ensure the solution is fully compliable and executable without additional coding.
- Do not use Implicit Using.
- Do not use Nullable.

## Coding Style
- Never use leading underscores for any variable.
- Use only explicit types, except for Tuple declarations which must use `var`
  (see the Tuples rule below).
- Local variables should use lowerCamelCase.
- Member variables should use UpperCamelCase.
- Class properties should use UpperCamelCase.
- Class names should use UpperCamelCase.
- Interface names must be prefixed with `I` followed by UpperCamelCase
  (e.g., `INodeVisitor`, `IInterpreter`). This follows standard C# convention.
- Enumeration names should use UpperCamelCase.
- Record names should use UpperCamelCase.
- Struct names should use UpperCamelCase.
- Method names should use UpperCamelCase.
- All Tuples should be named from the returning method.
- All Tuples must be Post Fixed with the word 'Tuple'.
- All Tuples must use a `var` type declaration (exception to the explicit-types rule).
- All exceptions should use lowerCamelCase and be the same name as the exception.
- Use `readonly` on all variables whenever possible.
- Local Variables should be named with complete nouns, appending a prefix or postfix for clarification if required.
- Member Variables should be named with complete nouns, appending a prefix or postfix for clarification if required.
- Properties should be named with complete nouns, appending a prefix or postfix for clarification if required.
- Methods should be named with complete nouns, appending a prefix or postfix for clarification if required.
- Classes should be named with complete nouns, appending a prefix or postfix for clarification if required.
- Interfaces should be named with complete nouns, appending a prefix or postfix for clarification if required.
- Structs should be named with complete nouns, appending a prefix or postfix for clarification if required.

## Library Usage
- Use only the .NET Base Class Library (BCL).
- Use defined value types from the BCL.
- Use defined reference types from the BCL.

## Programming Constructs
- Favor use of Tuples for returning multiple values from a method rather than
  Classes or Structs or Records.
- Use `var` types for Tuple declarations (see Tuples rule above).
- Favor Records over Classes where a class hierarchy and mutable state are not
  required. AST node types that participate in the Visitor pattern require
  inheritance and should be implemented as Classes, not Records.
- When reading source files from disk, prefer streams over loading the entire
  file into a string at once. Internal token values and identifiers may use
  `string` as their representation type.

## File System Structure
- Create separate files for each class.
- Create separate files for each interface.
- Create separate files for each enumeration.
- Create separate files for each record.

## Code Documentation
- Add comments to explain complex code structures or logic in a way accessible
  to business analysts or entry-level programmers.

---

# Application Description

## What to Build

- Create Solution in Folder ..\TinyLanguage.YYYY.MM.DD.HH
- Name the Solution TinyLanguage.
- Apply the BNF Grammar Verification Strategy to generate all unit tests.

## Class Library (name: TinyLanguage.Lexer.dll)
- Create a Class Library containing the Lexer, AST node types, and Parser.
- Generate a Lexer that tokenizes source code per the BNF grammar.
- Generate all nodes in the Abstract Syntax Tree.
- Generate an Abstract Syntax Tree Pretty Printer.

## Class Library (name: TinyLanguage.Interpreter.dll)
- Create a Class Library containing the Interpreter.
- Depends on TinyLanguage.Lexer.dll for AST node types.

## UnitTests (name: TinyLanguage.UnitTests.dll)
- Create a Unit Test project containing Lexer and Parser unit tests.

## IntegrationTests (name: TinyLanguage.IntegrationTests.dll)
- Create a Unit Test project containing Interpreter and end-to-end integration tests.

## Console Application (name: TinyLanguage.exe)

**File-processor mode (with arguments):**
- Accept an input file path as the first parameter and an output file path as
  the second parameter.
- Read source code from the input file, run it through the Lexer, Parser,
  and Interpreter, and write the program output to the output file.
- Support piped input and piped output: `TinyLanguage.exe < input.tlg > output.txt`
- If any errors occur during lexing, parsing, or interpreting, write a
  descriptive error message and source line number to Console.Error (stderr),
  not to the output file. The output file should contain only successful output.
- Exit with code 0 on success and code 1 on any error.

**Demo mode (no arguments):**
- When invoked with no arguments, run the built-in demonstration suite.
- Execute each demo program in sequence, printing a header for each demo,
  its output, and a separator line.
- After all demos complete successfully, print the final line:
  `All demos completed successfully.`
- Exit with code 0 if all demos pass, code 1 if any demo fails.

## Tiny Language Demonstration Suite (name: TinyLanguage.DemoFiles)
- The TinyLanguage.DemoFiles must be a `SharedProject` type.
- The `SharedProject` must containing the demo `.tlg`
  source files and their matching `.cmd` runner scripts.
- Demo `.tlg` files are embedded files, not compiled assemblies.
- Demo `.cmd` files are embedded files, not compiled assemblies.
- The demonstration TinyLanguage demo files should be exhaustive and named
  using a zero-padded numeric prefix. EXAMPLE: `00001.fizzbuzz.tlg`
- Create a matching `00001.fizzbuzz.cmd` script for each demo file that
  invokes TinyLanguage.exe with the demo file as input:
- If an output file path is provided as a second argument to the `.cmd` script,
  pass it through to TinyLanguage.exe.
- Generate a suite of demonstration programs that together exercise every
  fully implemented feature of the grammar.
- There should be 300 or more `.tlg` files created.

---

# Unit Testing Strategy & Requirements

## Unit Testing Requirements
- Use only the Microsoft Unit Test Framework (MSTest).
- Do not use XUnit or NUnit.
- Unit test all bounding conditions.
- Apply the BNF Grammar Verification Strategy defined below.
- Test class names must end in `UnitTests` or `IntegrationTests`
  (e.g., `LexerUnitTests`, `ParserUnitTests`, `InterpreterIntegrationTests`).
  Never use the bare word `Tests` or `Test` as the class name suffix.
- Test method names should describe what is being tested and what outcome is
  expected (e.g., `IntegerLiteral_Lexes_CorrectTokenType`). The word "Test"
  should not appear in method names — the `[TestMethod]` attribute is sufficient.

## Test Validation Protocol

### Purpose

After implementing the solution, the AI must validate the code by executing
the unit tests and iterating until every test passes. This section defines
the exact commands, acceptance criteria, and fix-and-retry loop.

### Validation Commands

Run from the solution root directory (`TinyLanguage.YYYY.MM.DD.A/`):

```
# 1. Build the solution
dotnet build. The build must be successful!

# 2. Run the console application with no arguments to execute the demo suite
#    Must exit 0 and print "All demos completed successfully."
dotnet run --project TinyLanguage

# 3. Run the full test suite
dotnet test --verbosity normal
```

### Acceptance Criteria

The implementation is considered complete ONLY when ALL of the following are true:

- `dotnet build` exits with **0 errors** and **0 warnings**
- `dotnet test` reports **Failed: 0** across all test classes
- `dotnet run --project TinyLanguage` (no arguments) prints **"All demos completed successfully."** and exits with code 0

### Fix-and-Retry Loop

If any test fails:
1. Read the full error message and stack trace from the `dotnet test` output.
2. Identify the root cause (logic error, scope bug, parser edge case, etc.).
3. Edit **only** the minimal code needed to fix the failure — do not refactor unrelated code.
4. Re-run `dotnet build` to confirm the fix compiles cleanly.
5. Re-run `dotnet test` to confirm the failure is resolved and no regressions were introduced.
6. Repeat from step 1 until **Failed: 0**.

---

# TinyLanguage Syntax

## Implementation Notes

These rules resolve ambiguities that the BNF alone does not settle. Follow them exactly.

**1. Comment character**
`#` starts a line comment. `//` is **always** the floor-division operator — it is never treated as a comment under any circumstances.

**2. Assignment operators**
`:=` is the only assignment operator for variables and declarations. `=` is only valid inside annotation parameter lists (`@ann(key = value)`) and enum value assignments (`enum E { A = 1 }`). A bare `=` anywhere else is a lexer `Unknown` token and a parse error.

**3. Statement separators and block termination**
`;` is a statement **separator**, not a terminator. It is required between consecutive statements but must **not** appear immediately before a block-ending keyword (`end`, `else`, `catch`, `finally`, `while`, `}`, or EOF).

The parser stops consuming statements in a `<stmt_list>` when the next token cannot begin a statement. `ParseStatementList` must accept an optional set of stop-tokens (required for `do-while`, which stops at `while`; and for `switch` cases, which stop at `case`, `default`, or `}`). Newlines are whitespace; they do not insert implicit semicolons.

**4. Bare return**
`return` followed immediately by a block terminator (`end`, `else`, `catch`, `finally`, `}`), a semicolon, or EOF returns `null`. Parsing `return` must not unconditionally demand an expression.

**5. Keywords as identifiers**
No keyword may be used as a variable or function name under any circumstances. The lexer always produces the keyword token type — never `Identifier` — for every reserved word.

**6. Foreach over string**
Iterating a string with `foreach` yields each character as a single-character string, in order. Iterating `null` must throw. Iterating any other non-list, non-string type must throw.

**7. Array element assignment**
`arr[i] := v` is structurally distinct from `arr := [...]`. It must be represented as its own AST node type (`ArrayElementAssignNode`) and must mutate the existing list object in-place. It must **not** be encoded as a hack inside `AssignStatementNode`.

**8. Scope chain**
Variable lookup walks from the innermost scope outward to global. Assignment (`:=`) updates the variable in the scope where it was declared, not always the innermost scope. `let`, `var`, and `const` declarations always create a new binding in the current (innermost) scope.

**9. Function scope**
A function call creates a fresh scope containing only the global scope as its parent plus the parameter bindings. Variables from the call site are never visible inside the function.

**10. Operator types**

| Expression | Result |
|-----------|--------|
| `Integer ** Integer` (non-negative exponent) | Integer |
| Any other `**` combination | Float |
| `Integer // Integer` | Integer (no float conversion) |
| `Float // anything` or `anything // Float` | Float |
| `% Float` | Type error |

**11. Expression precedence** (lowest to highest)

> Ternary (`?:`) → Or (`\|\|`, `or`) → And (`&&`, `and`) → Not (`not`) → Type-check/assert (`is`, `as`) and Comparison (`==`, `!=`, `<`, `>`, `<=`, `>=`) → Additive (`+`, `-`, `&`) → Multiplicative (`*`, `/`, `%`, `//`) → Power (`**`) → Unary (`-`) → Postfix (`.`, `[]`, call) → Primary

`is` and `as` are at the same precedence level as comparisons, so `a + b is int` parses as `(a + b) is int`, not `a + (b is int)`. See the Expressions section for the full BNF.

**12. Conditional expression disambiguation**
- `if <expr> then <expr> else <expr>` (`<conditional_expr>`) is an inline expression valid only inside `<primary>` — i.e., on the right-hand side of `:=`, as a function argument, inside another expression, etc.
- `if <expr> then <stmt_list> [else <stmt_list>] end` (`<if_stmt>`) is a statement.

The parser disambiguates by context: inside a statement list `if` always begins an `<if_stmt>`; inside an expression context (after `:=`, inside `(`, after a comma in an arg list, etc.) `if` begins a `<conditional_expr>`.

**13. Cast expression disambiguation**
`(type) expr` is a cast expression. The parser distinguishes `(type)` from a parenthesised expression `(<expr>)` by checking whether the first token inside the parens is a primitive type keyword (`int`, `float`, `string`, `bool`, `array`, `object`, `null`, `void`) or a user-defined type identifier followed immediately by `)`. If so, it is a cast; otherwise it is a grouped expression.

**14. Lambda expression disambiguation**
- `function(params) <expr>` — expression body (no `end`)
- `function(params) <stmt_list> end` — block body

Strategy: scan ahead for a matching `end` at the same nesting depth. If a matching `end` is found, parse the block body form; otherwise parse the expression body form. Alternatively, if the first token after the parameter list cannot start a statement, parse as expression body; if it can start a statement, parse as block body.

**15. Object instantiation syntax**
Object instantiation uses the standard assignment form: `myObj := new ClassName(args)`. The `new` keyword begins a `<new_expr>` which is a valid `<primary>`, so it is always the right-hand side of `:=`. The older form `new <id> := <id>(args)` is **not** valid. There is no separate `<object_declare_stmt>` — use `<assign_stmt>` with `<new_expr>`.

**16. Static modifier**
`static` is a separate keyword modifier. It appears as its own token before `function` or `class` — it is **not** fused into a single token. The lexer emits `TokenType.Static` for `static`, then `TokenType.Function` or `TokenType.Class` as the next token.

```
CORRECT:   static function Foo() ... end
CORRECT:   static class Bar { }
```

**17. Empty argument and parameter lists**
Function calls and definitions with no arguments/parameters use empty parentheses: `foo()`. `<arg_list>` and `<param_list>` are optional wherever they appear.

**18. Top-level declarations**
Function definitions, class definitions, and module definitions are valid statements and appear in `<stmt>`. A program is therefore a flat `<stmt_list>` that may contain any mix of definitions and executable statements in any order. Multiple top-level function definitions are fully supported.

**19. For loop variable scoping**
The loop variable in `for <id> := <start> to <end> ...` is implicitly declared in the loop's own inner scope regardless of whether a variable with that name exists in an outer scope. It does not update any outer binding and is not accessible after the loop body exits.

**20. Generic type disambiguation**
The parser must distinguish `<generic_type>` (`Foo<int>`) from an identifier followed by a less-than comparison (`Foo < int`). Strategy: after parsing a user-defined type identifier, look ahead. If `< type >` (valid type tokens followed by `>`) follows, treat it as a generic type; otherwise treat `<` as the start of a comparison operator.

**21. Statement separator strictness**
`;` is required between consecutive statements. A trailing `;` before `end`, `else`, `catch`, `finally`, `while`, `}`, or EOF is a parse error. This follows the Pascal/ML separator convention.

```
CORRECT:   function foo() a := 1; print a end
INCORRECT: function foo() a := 1; print a; end   ← trailing ";"
```

**22. Call statement form**
A function or method call used as a statement must start with an identifier, optionally followed by a chain of `.` member accesses, ending in a call `()`. Examples: `foo()`, `obj.method(x)`, `a.b.c(x, y)`. To call a function stored in an array element (`arr[0](args)`), assign it to a variable first: `let f := arr[0]; f(args)`.

**23. Pattern alternation associativity**
`<pattern> | <pattern>` is left-associative: `A | B | C` parses as `(A | B) | C`. The matched value succeeds if any alternative matches.

---

## BNF Grammar

### Comments

```bnf
<comment> ::= "#" { <any_char> } <newline>
```

Comments begin with `#` and run to end of line. They are ignored by the lexer. Note: `//` is floor-division, **not** a line comment.

---

### Program Structure

A program is a flat statement list. Definitions and executable statements may appear in any order (see notes 18 and 21). Semicolons are statement separators (see notes 3 and 21). The empty alternative allows empty blocks and function bodies.

```bnf
<program>   ::= <stmt_list>

<stmt_list> ::= <stmt> ";" <stmt_list>
              | <stmt>
              |                          # ε — empty body is valid
```

---

### Statements

```bnf
<stmt> ::= <assign_stmt>
         | <var_declare_stmt>
         | <const_declare_stmt>
         | <array_assign_stmt>
         | <if_stmt>
         | <while_stmt>
         | <for_stmt>
         | <foreach_stmt>
         | <do_while_stmt>
         | <switch_stmt>
         | <break_stmt>
         | <continue_stmt>
         | <print_stmt>
         | <input_stmt>
         | <function_def>
         | <call_stmt>
         | <return_stmt>
         | <class_def>
         | <module_def>
         | <import_stmt>
         | <export_stmt>
         | <try_stmt>
         | <throw_stmt>
         | <pattern_match>
         | <annotated_stmt>
```

---

### Assignment and Declaration

```bnf
<assign_stmt> ::= <id> ":=" <expr>
```

`let` declares a new mutable variable with optional type annotation. `var` declares a new mutable variable and always requires a type annotation.

```bnf
<var_declare_stmt> ::= "let" <id> ":=" <expr>
                     | "let" <id> ":" <type> ":=" <expr>
                     | "var" <id> ":" <type> ":=" <expr>
```

`const` declares an immutable binding. The `enum` form declares named constants. Inside enum bodies `=` (not `:=`) assigns a value to a member.

```bnf
<const_declare_stmt> ::= "const" <id> ":=" <expr>
                       | "const" <id> ":" <type> ":=" <expr>
                       | "enum" <id> "{" <enum_value_list> "}"

<enum_value_list> ::= <enum_value> "," <enum_value_list>
                    | <enum_value>

<enum_value> ::= <id>
               | <id> "=" <expr>
```

---

### Control Flow

```bnf
<if_stmt>      ::= "if" <expr> "then" <stmt_list> ("else" <stmt_list>)? "end"

<while_stmt>   ::= "while" <expr> "do" <stmt_list> "end"
```

`for` iterates a numeric range (inclusive on both ends). The loop variable is declared in the loop's inner scope (see note 19).

```bnf
<for_stmt>     ::= "for" <id> ":=" <expr> "to" <expr> ("step" <expr>)? "do" <stmt_list> "end"
```

`foreach` iterates over a list or string (character by character for strings).

```bnf
<foreach_stmt> ::= "foreach" <id> "in" <expr> "do" <stmt_list> "end"
```

`do-while` has no trailing `end` — see note 3 for stop-token handling.

```bnf
<do_while_stmt> ::= "do" <stmt_list> "while" <expr>
```

The `<stmt_list>` inside each switch case body stops naturally at the next `case`, `default`, or `}` token (see note 3). A switch with cases only (no default) is valid.

```bnf
<switch_stmt>    ::= "switch" <expr> "{" <case_list> "}"

<case_list>      ::= <case_clause> <case_list>
                   | <case_clause>
                   | <default_clause>

<case_clause>    ::= "case" <expr> ":" <stmt_list>
<default_clause> ::= "default" ":" <stmt_list>

<break_stmt>    ::= "break"
<continue_stmt> ::= "continue"
```

---

### I/O Statements

```bnf
<print_stmt> ::= "print" <expr>
<input_stmt> ::= "input" <id>
```

---

### Functions

```bnf
<function_def> ::= "function" <id> "(" (<param_list>)? ")" ("->" <type>)? <stmt_list> "end"
```

`static` is an optional keyword modifier — see note 16.

```bnf
<method_def> ::= ("static")? "function" <id> "(" (<param_list>)? ")" ("->" <type>)? <stmt_list> "end"

<param_list> ::= <param> "," <param_list>
               | <param>

<param> ::= <id>
          | <id> ":=" <expr>
          | <id> ":" <type>
          | <id> ":" <type> ":=" <expr>
```

Function or method call used as a statement — see note 22 for restrictions. Supports `foo()`, `obj.method()`, `a.b.c(args)`.

```bnf
<call_stmt> ::= <id> { "." <id> } "(" (<arg_list>)? ")"
```

`expr` is optional — bare `return` returns `null` (used in void functions).

```bnf
<return_stmt> ::= "return" (<expr>)?
```

---

### Lambda Expressions

Expression body form: no `end` — the body is a single expression. Block body form: ends with `end` — the body is a statement list. See note 14 for disambiguation strategy.

```bnf
<lambda_expr> ::= "function" "(" (<param_list>)? ")" ("->" <type>)? <expr>
                | "function" "(" (<param_list>)? ")" ("->" <type>)? <stmt_list> "end"
```

---

### Classes and Objects

Object instantiation uses `<assign_stmt>` with a `<new_expr>` on the right-hand side: `myObj := new ClassName(args)`. See note 15 — there is no separate `<object_declare_stmt>`. `static` is an optional keyword modifier (see note 16). Empty class bodies are valid.

```bnf
<class_def> ::= ("static")? "class" <id> ("extends" <id>)? ("implements" <id_list>)? "{" <member_list> "}"

<member_list> ::= <member> <member_list>
                | <member>
                |                          # ε — empty class body is valid

<member> ::= <method_def>
           | <field_declare_stmt>
           | <const_declare_stmt>
           | <constructor_def>

<field_declare_stmt> ::= "let" <id> ":=" <expr>
                       | "let" <id> ":" <type> ":=" <expr>
                       | "var" <id> ":" <type> ":=" <expr>
                       | "const" <id> ":=" <expr>
                       | "const" <id> ":" <type> ":=" <expr>

<constructor_def> ::= "Constructor" "(" (<param_list>)? ")" <stmt_list> "end"

<id_list> ::= <id> "," <id_list>
            | <id>
```

---

### Module System

The module header `import` list uses `<module_import>` (without the `import` keyword) to avoid the double-keyword parse `module Foo import import Bar {...}`. Standalone `import` statements inside module bodies use `<import_stmt>`.

```bnf
<module_def> ::= "module" <id> "{" <stmt_list> "}"
               | "module" <id> "import" <module_import_list> "{" <stmt_list> "}"

<module_import_list> ::= <module_import> "," <module_import_list>
                       | <module_import>

<module_import> ::= <id>
                  | <id> "as" <id>

<import_stmt> ::= "import" <id>
                | "import" <id> "as" <id>

<export_stmt> ::= "export" <id>
```

---

### Expressions

Precedence hierarchy — each rule delegates to the next-higher level (lower in this list = lower precedence).

> Ternary (`?:`) → Or → And → Not → Comparison/TypeOp → Additive → Multiplicative → Power → Unary → Postfix → Primary

**Ternary** — lowest precedence; right-associative.

```bnf
<expr> ::= <or_expr> "?" <expr> ":" <expr>
         | <or_expr>
```

**Logical or** — left-associative; `or` and `||` are synonyms.

```bnf
<or_expr> ::= <or_expr> ("||" | "or") <and_expr>
            | <and_expr>
```

**Logical and** — left-associative; `and` and `&&` are synonyms.

```bnf
<and_expr> ::= <and_expr> ("&&" | "and") <not_expr>
             | <not_expr>
```

**Logical not** — right-associative unary prefix.

```bnf
<not_expr> ::= "not" <not_expr>
             | <comparison_expr>
```

**Comparison and type-check/assert** — same precedence level. All forms are non-associative: `a < b < c` requires parentheses. `is` returns bool; `as` performs a checked type assertion. Note: `a + b is int` parses as `(a + b) is int` (see note 11).

```bnf
<comparison_expr> ::= <additive_expr> "==" <additive_expr>
                    | <additive_expr> "!=" <additive_expr>
                    | <additive_expr> "<"  <additive_expr>
                    | <additive_expr> ">"  <additive_expr>
                    | <additive_expr> "<=" <additive_expr>
                    | <additive_expr> ">=" <additive_expr>
                    | <additive_expr> "is" <type>
                    | <additive_expr> "as" <type>
                    | <additive_expr>
```

**Additive** — left-associative; `&` is string concatenation at this level.

```bnf
<additive_expr> ::= <additive_expr> ("+" | "-" | "&") <multiplicative_expr>
                  | <multiplicative_expr>
```

**Multiplicative** — left-associative.

```bnf
<multiplicative_expr> ::= <multiplicative_expr> ("*" | "/" | "%" | "//") <power_expr>
                        | <power_expr>
```

**Power** — right-associative: `2 ** 3 ** 2` parses as `2 ** (3 ** 2)`.

```bnf
<power_expr> ::= <unary_expr> "**" <power_expr>
               | <unary_expr>
```

**Unary minus** — right-associative prefix.

```bnf
<unary_expr> ::= "-" <unary_expr>
               | <postfix_expr>
```

**Postfix** — left-associative: member access, index, and value-call. Note: `is` and `as` are **not** postfix — they are at comparison level (see note 11).

```bnf
<postfix_expr> ::= <postfix_expr> "." <id> "(" (<arg_list>)? ")"   # method call
                 | <postfix_expr> "." <id>                           # member access
                 | <postfix_expr> "[" <expr> "]"                     # index access
                 | <postfix_expr> "(" (<arg_list>)? ")"              # call via stored value
                 | <primary>
```

**Primary** — highest precedence. Function call via named identifier is handled as `<id>` followed by the postfix call suffix in `<postfix_expr>` — not duplicated here.

```bnf
<primary> ::= <id>
            | <number>
            | <string>
            | <boolean>
            | "null"
            | "(" <expr> ")"
            | "[" (<expr_list>)? "]"
            | "[" <expr> "for" <id> "in" <expr> "]"
            | <new_expr>
            | <cast_expr>
            | <conditional_expr>
            | <lambda_expr>

<new_expr>  ::= "new" <id> "(" (<arg_list>)? ")"

<arg_list>  ::= <expr> "," <arg_list>
              | <expr>

<expr_list> ::= <expr> "," <expr_list>
              | <expr>
```

---

### Inline Conditional Expression

Only valid in expression contexts (inside `<primary>`). See note 12 for disambiguation from `<if_stmt>`.

```bnf
<conditional_expr> ::= "if" <expr> "then" <expr> "else" <expr>
```

---

### Cast Expression

See note 13 for disambiguation from `(<expr>)`. The cast applies to the immediately following `<unary_expr>`, giving casts high precedence: `(int) a + b` parses as `((int) a) + b`.

```bnf
<cast_expr> ::= "(" <type> ")" <unary_expr>
```

---

### Arrays

Array element assignment produces `ArrayElementAssignNode` and mutates in-place. Array literals `[(<expr_list>)?]` appear in `<primary>` and support empty arrays `[]`. List comprehension is defined but not yet implemented.

```bnf
<array_assign_stmt> ::= <id> "[" <expr> "]" ":=" <expr>
```

---

### Exception Handling

```bnf
<try_stmt> ::= "try" <stmt_list> <catch_clause> ("finally" <stmt_list>)? "end"

<catch_clause> ::= "catch" <id> <stmt_list>
                 | "catch" "(" <id> ":" <type> ")" <stmt_list>

<throw_stmt> ::= "throw" <expr>
```

---

### Pattern Matching

Pattern alternation is left-associative: `A | B | C` parses as `(A | B) | C` (see note 23).

```bnf
<pattern_match> ::= "match" <expr> "{" <pattern_case_list> "}"

<pattern_case_list> ::= <pattern_case> <pattern_case_list>
                      | <pattern_case>

<pattern_case> ::= <pattern> "=>" <stmt_list>
                 | <pattern> "when" <expr> "=>" <stmt_list>

<pattern> ::= <id>
            | <number>
            | <string>
            | <boolean>
            | "null"
            | "_"
            | <id> "(" (<pattern_list>)? ")"
            | "[" (<pattern_list>)? "]"
            | <id> "{" (<field_pattern_list>)? "}"
            | <pattern> "|" <pattern>

<pattern_list> ::= <pattern> "," <pattern_list>
                 | <pattern>

<field_pattern_list> ::= <id> ":" <pattern> "," <field_pattern_list>
                       | <id> ":" <pattern>
```

---

### Annotations

Inside annotation parameter lists `=` (not `:=`) assigns a value.

```bnf
<annotation> ::= "@" <id>
               | "@" <id> "(" (<annotation_param_list>)? ")"

<annotation_param_list> ::= <annotation_param> "," <annotation_param_list>
                          | <annotation_param>

<annotation_param> ::= <id> "=" <expr>

<annotated_stmt> ::= <annotation> <stmt>
```

---

### Type System

`<type>` is defined in two levels to eliminate left-recursion. `<base_type>` matches the core type, then zero or more `<type_suffix>` modifiers apply array (`[]`) and nullable (`?`) transformations. This allows: `int`, `int[]`, `int[][]`, `int?`, `int[]?`, `string[]?[]`, etc.

```bnf
<type> ::= <base_type> { <type_suffix> }

<base_type> ::= "int"
              | "float"
              | "string"
              | "bool"
              | "array"
              | "object"
              | "null"
              | "void"
              | <map_type>
              | <generic_type>
              | <id>

<type_suffix> ::= "[" "]"      # array type
                | "?"           # nullable type

<type_list> ::= <type> "," <type_list>
              | <type>

<map_type> ::= "map" "<" <type> "," <type> ">"
```

See note 20 for disambiguation of generic type vs. comparison.

```bnf
<generic_type> ::= <id> "<" <type_list> ">"
```

---

### Literals and Identifiers

```bnf
<boolean> ::= "true" | "false"

<string> ::= '"' { <string_char> } '"'
           | "'" { <string_char> } "'"
```

`<string_char>` is any printable character except the enclosing quote delimiter and unescaped backslash, or a backslash escape sequence (any character in U+0020–U+007E except the delimiter and `\`, plus the five escape sequences below).

```bnf
<string_char> ::= <printable_non_delimiter_non_backslash_char>
               | "\\" <escape_seq>

<escape_seq> ::= 'n' | 't' | '\\' | '"' | "'"
```

Supported escape sequences: `\n` (newline), `\t` (tab), `\\` (literal backslash), `\"` (double-quote), `\'` (single-quote).

Float must be listed before integer in parser alternatives so that `1.5` is not incorrectly parsed as integer `1` followed by `.5`. Both forms require at least one digit before and after the decimal point: `.5` and `1.` are **not** valid float literals.

```bnf
<number> ::= <digit> { <digit> } "." <digit> { <digit> }   # float
           | <digit> { <digit> }                             # integer
           | <binary_number>
           | <octal_number>
           | <hex_number>

<binary_number> ::= "0b" <binary_digit> { <binary_digit> }
<octal_number>  ::= "0o" <octal_digit>  { <octal_digit>  }
<hex_number>    ::= "0x" <hex_digit>    { <hex_digit>    }

<binary_digit> ::= '0' | '1'
<octal_digit>  ::= '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7'
<hex_digit>    ::= <digit> | 'a'..'f' | 'A'..'F'
```

Identifiers may contain underscores but must not start with one (coding style rule). No keyword may be used as an identifier — the lexer always returns the keyword token type for reserved words (see note 5).

```bnf
<id>     ::= <letter> { (<letter> | <digit> | '_') }
<letter> ::= 'a'..'z' | 'A'..'Z'
<digit>  ::= '0'..'9'
```

---

### Terminal Symbols

```bnf
<terminal> ::= <id>
             | <number>
             | <string>
             | <boolean>
             | <type>
             | keyword
             | operator
```

---

# TinyLanguage Semantics

## Type System

The language has three numeric types: **Integer** (64-bit signed long), **Float** (64-bit double), and **Bool**.

### Arithmetic Promotion

| Left operand | Operator | Right operand | Result  |
|-------------|----------|--------------|---------|
| Integer     | any      | Integer      | Integer |
| Integer     | `/`      | Integer      | Float (when result has a fractional part) |
| Integer     | any      | Float        | Float   |
| Float       | any      | Float        | Float   |

### Operator-Specific Rules

- **`//` (floor division)** — uses integer arithmetic when both operands are Integer; must never promote to Float internally.
- **`**` (exponentiation)** — returns Integer when both operands are Integer and the exponent is a non-negative integer; returns Float if either operand is Float or the exponent is negative.
- **`%` (modulo)** — operates on Integers only; applying it to a Float is a type error.
- **`&` (string concatenation)** — string values only; both sides are converted to their string representation before concatenation if not already strings.
- **`+` (addition / string concatenation)** — performs numeric addition when both operands are numeric. If either operand is a String, both sides are converted to their string representation and concatenated. The coercion is symmetric: `1 + "a"` and `"a" + 1` both produce string concatenation.

### Truthiness

| Value | Truthy? |
|-------|---------|
| `null` | false |
| Integer `0` | false |
| Integer (non-zero) | true |
| Float `0.0` | false |
| Float (non-zero) | true |
| `""` (empty string) | false |
| String (non-empty) | true |
| Bool | as-is |

---

## Built-in Functions

The functions below must be available without any source-level definition and may not be redefined by user code. No other built-ins are permitted.

| Name | Parameter | Returns |
|------|-----------|---------|
| `len(v)` | string or list | Integer length |
| `str(v)` | any value | String representation |
| `int(v)` | string / float / bool | Integer (truncates float) |
| `bool(v)` | any value | Boolean (applies truthiness rules) |

> **Note on `print`:** `print` is a keyword statement (`print <expr>`), not a callable built-in function. The expression `print(x)` is valid syntax and is parsed as the `print` keyword applied to the parenthesized expression `(x)`, which produces the same output. There is no separately callable `print` function — the statement form is the only mechanism for console output.

---

## Scope Rules

### Scope Chain

- Implement a linked-list of Dictionaries (not a flat copy of the environment). Each block — function body, loop body, if branch — creates a child scope that delegates unresolved names upward to its parent.
- Variable lookup walks from the innermost scope outward to global.
- Variables declared inside a loop or `if` block do **not** leak into the enclosing scope when the block exits.
- Assignment (`:=`) updates the variable in the scope where it was declared, not always the innermost scope.
- `let` and `var` declarations always create a new binding in the current (innermost) scope.

### Function Scope

- A function call creates a fresh scope whose only parent is global scope, plus the parameter bindings.
- Variables from the call site are **never** visible inside the function body.

### Closures and Lambdas

- Closures are out of scope — lambdas capture no variables from the enclosing lexical scope other than global scope.
- When invoked, a lambda follows the same rules as a regular function: it sees only global-scope variables and its own parameters. Variables from the enclosing block where the lambda was defined are **not** visible inside it.

---

## Runtime Error Policy

All errors must be reported with a descriptive message and the source line number. Silent failures are forbidden.

| Situation | Required behavior |
|-----------|------------------|
| Undefined variable | Throw a descriptive exception naming the variable and the source line number |
| Call stack depth > 500 | Throw `"Stack overflow: maximum call depth of 500 exceeded"` |
| Division by zero (`/`, `//`, `%`) | Throw a descriptive exception regardless of operand types |
| Type mismatch (e.g. arithmetic on a non-numeric value) | Throw a descriptive exception; never silently coerce to zero |
| Parsed but unimplemented feature | Throw `"Feature not yet implemented: <name>"` |

> **AST requirement:** Store the source line number in every AST node so that runtime errors can report exactly where the fault occurred.

---

## Feature Implementation Status

Every grammar feature must be either fully implemented or raise `"Feature not yet implemented: <name>"` at runtime. Silent no-ops — where parsing succeeds but nothing happens at runtime — are forbidden.

**Features that must be fully implemented:**

| Category | Features |
|----------|----------|
| Declarations | `let`, `var`, `const` |
| Control flow | `if`, `while`, `for`, `foreach`, `do-while`, `switch`, `break`, `continue` |
| Functions | Definition, call, recursion, default parameters, `return` |
| Arrays | Declaration, element read, element assignment, `foreach` iteration |
| Exceptions | `try`, `catch`, `finally`, `throw` |
| Annotations | Parse and pass through — the wrapped statement executes normally |
| Expressions | All operators and all literal types |
| Classes | Class definition and object instantiation |
| Modules | Import and export |
| Pattern matching | `match` / `when` |
| List comprehension | Array syntax |
| Lambdas | Lambdas may be stored as values; calling them is not required |
| Type operations | Cast expressions, type-check (`is`), type-assert (`as`) |

---

## Interpreter Architecture

### Visitor Pattern

Implement the interpreter using the Visitor pattern. Create a `Visitor` interface with a `Visit` method for every AST node type. Do not use a single large `if-else` or `switch` chain to dispatch node types.

---

# BNF Grammar Verification Strategy

Every production rule in the BNF must be verified at four independent layers:
the Lexer, the Parser, the AST Pretty Printer, and the Interpreter. Work
through the BNF top-to-bottom and produce at least one test per layer per
production rule. Tests are organized into four test classes, each in its
own file.

## Layer 1 — Lexer Coverage

For every terminal symbol and keyword in the grammar, verify that the Lexer
produces the correct TokenType and Lexeme.

For each production rule that introduces a new terminal, write a test that:
- Feeds the minimal source string for that terminal to the Lexer.
- Asserts the expected TokenType on the first token.
- Asserts the Lexeme string is exactly correct.
- Asserts the EndOfFile token follows immediately.

Required terminal coverage:
- Every keyword (`let`, `var`, `const`, `if`, `then`, `else`, `end`, `while`,
  `do`, `for`, `to`, `step`, `in`, `foreach`, `switch`, `case`, `default`,
  `break`, `continue`, `function`, `return`, `class`, `extends`, `implements`,
  `new`, `module`, `import`, `as`, `export`, `try`, `catch`, `finally`,
  `throw`, `match`, `when`, `print`, `input`, `not`, `and`, `or`, `is`,
  `static`, `Constructor`, `enum`, `true`, `false`, `null`, `void`,
  `int`, `float`, `string`, `bool`, `array`, `object`, `map`)
- Every operator (`:=`, `=`, `+`, `-`, `*`, `/`, `%`, `**`, `//`, `&`,
  `==`, `!=`, `<`, `>`, `<=`, `>=`, `&&`, `||`, `->`, `=>`, `?`, `:`,
  `.`, `,`, `;`, `@`)
- Every delimiter (`(`, `)`, `{`, `}`, `[`, `]`)
- All number literal forms: decimal integer, decimal float, `0b` binary,
  `0o` octal, `0x` hex
- Double-quoted string literal
- Single-quoted string literal
- String with each supported escape sequence (`\n`, `\t`, `\\`, `\"`, `\'`)
- Identifier (starts with letter, may contain digits and underscores)
- Line comment (`#`) — verify the comment text does NOT appear in the token list
- Unknown/invalid character — verify TokenType.Unknown is produced
- Line and column numbers — verify they increment correctly across newlines
- Multi-token sequences — verify adjacent tokens are each correctly identified

## Layer 2 — Parser Coverage

For every non-terminal production rule in the BNF, verify that the Parser
produces the correct AST node type and structure.

For each production rule, write a test that:
- Lexes a minimal source string that exercises exactly that production.
- Parses the token list into an AST.
- Asserts the root node is the expected type.
- Asserts child nodes are the expected types, in the expected order.
- For optional elements (`?`), write one test with the element present
  and one test with it absent.
- For alternating productions (`|`), write one test per alternative.

Required production coverage:

**Program structure**
- `<program>` containing only a statement list
- `<program>` with a function definition followed by statements
- `<program>` with a class definition followed by statements
- `<program>` with a module definition followed by statements
- `<program>` with multiple function definitions (verifies fix for single-def limitation)
- `<program>` with a const declaration

**Assignment and declaration**
- `<assign_stmt>` — simple assignment
- `<var_declare_stmt>` — `let` form without type annotation
- `<var_declare_stmt>` — `let` form with type annotation
- `<var_declare_stmt>` — `var` form with type annotation
- `<const_declare_stmt>` — without type annotation
- `<const_declare_stmt>` — with type annotation
- `<const_declare_stmt>` — `enum` form with value list
- `<enum_value>` — bare identifier form
- `<enum_value>` — identifier with assigned expression

**Control flow**
- `<if_stmt>` — without else branch
- `<if_stmt>` — with else branch
- `<while_stmt>`
- `<for_stmt>` — without step
- `<for_stmt>` — with step
- `<foreach_stmt>`
- `<do_while_stmt>` — single statement body
- `<do_while_stmt>` — multi-statement body
- `<switch_stmt>` — with one case, no default
- `<switch_stmt>` — with multiple cases and a default
- `<switch_stmt>` — with only a default clause (no cases)
- `<break_stmt>`
- `<continue_stmt>`

**IO**
- `<print_stmt>`
- `<input_stmt>`

**Functions**
- `<function_def>` — no parameters, no return type
- `<function_def>` — with parameters and return type
- `<param>` — bare identifier form
- `<param>` — with type annotation
- `<param>` — with default value
- `<param>` — with type annotation and default value
- `<call_stmt>` — plain function call with no arguments (`foo()`)
- `<call_stmt>` — plain function call with multiple arguments (`foo(a, b)`)
- `<call_stmt>` — method call one level deep (`obj.method()`)
- `<call_stmt>` — chained method call (`a.b.c(args)`)
- `<return_stmt>` — with expression
- `<return_stmt>` — bare return (no expression)

**OOP**
- Object instantiation via `<assign_stmt>` + `<new_expr>`: `myObj := new ClassName(args)`
- `<new_expr>` — verify it is a valid primary expression on the RHS of `:=`
- `<class_def>` — minimal (no extends, no implements, non-empty body)
- `<class_def>` — with extends
- `<class_def>` — with implements
- `<class_def>` — with `static` modifier
- `<class_def>` — with empty body (`class Foo { }`) — must parse without error
- `<constructor_def>`
- `<field_declare_stmt>` — all five forms (let untyped, let typed, var typed, const untyped, const typed)

**Expressions (verify precedence and associativity)**
- Ternary: `a ? b : c` — condition, then-branch, else-branch
- `<or_expr>` with `||` and with `or` keyword synonym
- `<and_expr>` with `&&` and with `and` keyword synonym
- `<not_expr>` with `not` prefix
- `<comparison_expr>` with `==` and `!=`
- `<comparison_expr>` with `<`, `>`, `<=`, `>=`
- `<additive_expr>` with `+`, `-`, `&`
- `<multiplicative_expr>` with `*`, `/`, `%`, `//`
- `<power_expr>` with `**` — verify right-associativity: `2 ** 3 ** 2` parses as `2 ** (3 ** 2)`
- `<unary_expr>` with unary `-`
- `<postfix_expr>` — member access (`expr.id`)
- `<postfix_expr>` — method call (`expr.id(args)`)
- `<postfix_expr>` — index access (`expr[i]`)
- `<postfix_expr>` — value call (`expr(args)` where expr is a stored lambda)
- `<comparison_expr>` — type check: `expr is type` (at comparison level, not postfix)
- `<comparison_expr>` — type assert: `expr as type` (at comparison level, not postfix)
- `is` precedence: `a + b is int` must parse as `(a + b) is int`, NOT `a + (b is int)`
- `as` precedence: `a + b as int` must parse as `(a + b) as int`, NOT `a + (b as int)`
- Operator precedence: `2 + 3 * 4` must parse as `2 + (3 * 4)`, not `(2 + 3) * 4`
- Logical precedence: `a || b && c` must parse as `a || (b && c)`
- Parenthesised expression overrides precedence

**Arrays**
- `<array_assign_stmt>` — element assignment (`id[expr] := expr`)
- Array literal `[expr_list]` — non-empty, verify it appears in assign_stmt RHS
- Array literal `[]` — empty array must parse without error
- `<postfix_expr>` index access — read element by index

**Lambda**
- `<lambda_expr>` — expression body form (single expression, no `end`)
- `<lambda_expr>` — statement body form ending with `end`

**Exception handling**
- `<try_stmt>` — with catch, no finally
- `<try_stmt>` — with catch and finally
- `<catch_clause>` — bare identifier form
- `<catch_clause>` — typed form with parentheses
- `<throw_stmt>`

**Annotations**
- `<annotated_stmt>` — annotation with no parentheses at all (`@ann stmt`)
- `<annotated_stmt>` — annotation with empty parentheses (`@ann() stmt`) — must parse without error
- `<annotated_stmt>` — annotation with one parameter (`@ann(name = value) stmt`)
- `<annotated_stmt>` — annotation with multiple parameters

**Module system**
- `<module_def>` — without import list
- `<module_def>` — with import list (verify NO double "import" keyword)
- `<module_import>` — bare form (`id`)
- `<module_import>` — aliased form (`id as id`)
- `<import_stmt>` — standalone bare import form (`import id`)
- `<import_stmt>` — standalone aliased form (`import id as id`)
- `<export_stmt>`

**Conditional expression**
- `<conditional_expr>` — `if <expr> then <expr> else <expr>` in expression context (RHS of assignment)

**Type operations**
- `<cast_expr>` — `(type) expr`
- `<comparison_expr>` — `expr is type` (at comparison precedence level)
- `<comparison_expr>` — `expr as type` (at comparison precedence level)

**Pattern matching**
- `<pattern_match>` — with one literal pattern case
- `<pattern_match>` — with a `when` guard
- `<pattern_case>` — literal pattern (number or string)
- `<pattern_case>` — wildcard `_` pattern
- `<pattern>` — zero-argument constructor pattern `Foo()` — must parse without error
- `<pattern>` — empty field pattern `Foo {}` — must parse without error
- `<pattern>` — alternation `A | B | C` — verify left-associative parse `(A | B) | C`

**Types**
- Each primitive type keyword (`int`, `float`, `string`, `bool`, `void`, `null`)
- Array type suffix `[]` — e.g. `int[]`
- Multi-dimensional array `[][]` — e.g. `int[][]` (two suffix applications)
- Map type `map<type, type>`
- Nullable type suffix `?` — e.g. `int?`
- Combined modifiers — e.g. `int[]?` (nullable array) and `int?[]` (array of nullable ints)

**Literals**
- Integer, float, binary, octal, hex literals resolve to correct AST values
- Boolean literals `true` / `false`
- Null literal
- Both string delimiter forms

**Parser error cases**
- Missing `end` keyword after if block — expect parse exception
- Missing `do` keyword in while loop — expect parse exception
- Malformed `:=` (using `=` instead) — expect parse exception
- Trailing `;` before `end` (e.g. `function foo() x := 1; end`) — expect parse exception
- Switch with no cases and no default (`switch x { }`) — expect parse exception

## Layer 3 — AST Pretty Printer Coverage

For every AST node type, verify that the Pretty Printer produces correct,
readable output. The Pretty Printer must not crash and must produce a
deterministic string for any valid AST.

For each node type, write a test that:
- Parses a minimal source string that produces that node.
- Runs the AST through the Pretty Printer.
- Asserts the output string contains the expected node type name and key fields.
- Asserts the output is correctly indented relative to its parent node.

Required Pretty Printer coverage:
- Every statement node type (`AssignStatementNode`, `LetDeclareNode`,
  `IfStatementNode`, `WhileStatementNode`, `ForStatementNode`,
  `ForeachStatementNode`, `DoWhileStatementNode`, `SwitchStatementNode`,
  `FunctionDefNode`, `ReturnStatementNode`, `PrintStatementNode`,
  `InputStatementNode`, `ClassDefNode`, `ModuleDefNode`, `TryStatementNode`,
  `ThrowStatementNode`, `PatternMatchNode`, `BreakStatementNode`,
  `ContinueStatementNode`, `AnnotatedStatementNode`, etc.)
- Every expression node type (`AssignExprNode`, `BinaryOpNode`, `UnaryOpNode`,
  `TernaryNode`, `IdentifierNode`, `IntegerLiteralNode`, `FloatLiteralNode`,
  `StringLiteralNode`, `BoolLiteralNode`, `NullLiteralNode`, `ArrayLiteralNode`,
  `IndexAccessNode`, `MemberAccessNode`, `MethodCallNode`, `FunctionCallNode`,
  `NewExprNode`, `CastExprNode`, `TypeCheckNode`, `TypeAssertNode`,
  `ConditionalExprNode`, `LambdaExprNode`, etc.)
- Nesting: verify that child nodes are indented one level deeper than parent
- Multi-statement program: verify each statement appears on its own line

## Layer 4 — Interpreter Coverage

For every executable production rule, verify that the Interpreter produces
the correct runtime result.

For each production rule, write a test that:
- Composes a complete source string exercising that production.
- Runs it through Lexer → Parser → Interpreter.
- Captures printed output and asserts expected values.
- For rules that produce a value (expressions), stores the result in a
  variable and prints it for assertion.

Required execution coverage:

**Variables and assignment**
- `let` declaration — verify value is stored and retrievable
- `var` declaration with type annotation — verify value
- `const` declaration — verify value
- Assignment (`id := expr`) — verify updated value

**Arithmetic correctness**
- Integer addition, subtraction, multiplication, integer division
- Float promotion: integer op float returns float
- Modulo (`%`)
- Floor division (`//`) — verify `7 // 2 = 3`, `-7 // 2 = -4`
- Power (`**`) — verify `2 ** 10 = 1024`, `2 ** 3 ** 2 = 512`
- String concatenation with `&`
- String concatenation with `+` when one operand is a string (both `str + x` and `x + str`)

**Comparison and boolean**
- All six comparison operators with both true and false outcomes
- `&&` short-circuits: right side not evaluated if left is false
- `||` short-circuits: right side not evaluated if left is true
- `not` negation
- `and` / `or` keyword synonyms for `&&` / `||`
- Ternary expression: true branch and false branch

**Control flow**
- `if` true branch executes, false branch does not
- `if/else` — correct branch executes
- `while` loop — body executes correct number of times
- `while` with `break` — exits early
- `while` with `continue` — skips remainder of body iteration
- `for` loop — iterates correct range inclusive
- `for` loop with `step` — iterates with custom step
- `for` loop with negative step — counts down
- `foreach` over a list — visits each element in order
- `foreach` over a string — visits each character
- `do...while` — body executes at least once when condition is initially false
- `do...while` — body executes multiple times while condition is true
- `switch` — matching case body executes
- `switch` — non-matching case body does not execute
- `switch` — default executes when no case matches
- `switch` with `break` — exits after matched case

**Functions**
- Function with no parameters returns correct value
- Function with multiple parameters receives correct argument values
- Recursive function — factorial or fibonacci
- Function with default parameter — called with and without that argument
- Bare `return` in void function — does not crash

**Arrays**
- Array literal is created with correct elements
- Array element read by index
- Array element assignment mutates the array
- Out-of-bounds index returns null without crashing

**Exception handling**
- `try/catch` — catch block executes on thrown exception
- `try/catch` — catch variable holds the exception message
- `try/catch/finally` — finally always executes (both normal and exception paths)
- `throw` — message propagates to enclosing catch
- Division by zero is caught by try/catch
- `try` with no exception — catch block does not execute, finally does

**Annotations**
- Annotated statement executes its wrapped statement normally

**Number literals**
- Binary literal evaluates to correct integer value
- Octal literal evaluates to correct integer value
- Hex literal evaluates to correct integer value
- Float literal retains decimal precision

**Truthiness rules**
- `null` is falsy
- `0` and `0.0` are falsy
- Empty string `""` is falsy
- Non-zero number is truthy
- Non-empty string is truthy

## Layer 5 — Integration Coverage

Write end-to-end programs that combine multiple grammar features and verify
the complete output. Each test must use at least three distinct production
rules together.

Required integration scenarios:
- FizzBuzz (1–20): combines for loop, if/else, modulo, print, string literals
- Fibonacci sequence (first 10 terms): recursive function, for loop, print
- Bubble sort of a list: foreach, array assignment, while, swap logic
- String builder: foreach over string, concatenation, function, return
- Calculator: switch statement, functions, arithmetic, print
- Exception safety: nested try/catch/finally, throw, variable mutation
- Accumulator with do-while: proves multi-statement do-while body works
- Mixed number bases: binary + octal + hex in a single expression, print result
- Annotation passthrough: annotated function call still executes and prints
- Nested control flow: for inside while inside if — verify all levels interact correctly
- Pattern matching: match expression with multiple literal cases and a wildcard `_`
- Module isolation: define a module, export a name, use it from outer scope

## Verification Checklist

After generating all code and tests, confirm every item below before
reporting the work complete. Do not report complete if any item is unchecked.

**Lexer**
- [ ] Every keyword in the grammar has at least one lexer test
- [ ] Every operator in the grammar has at least one lexer test
- [ ] Every delimiter has at least one lexer test
- [ ] All number literal formats (decimal int, float, 0b, 0o, 0x) are tested
- [ ] Both string delimiters and all five escape sequences are tested
- [ ] Line comment (`#`) is tested — comment text must not appear in token list
- [ ] Line and column tracking are tested across a multi-line source string
- [ ] Unknown/invalid character produces TokenType.Unknown — tested
- [ ] Bare `=` produces TokenType.SingleEqual, not Unknown — tested

**Parser**
- [ ] Every non-terminal production rule has at least one parser test
- [ ] Every `|` alternative in every production has its own parser test
- [ ] Every optional element (`?`) has a test with the element and without it
- [ ] Multiple top-level function definitions parse without error
- [ ] Empty function body (`function foo() end`) parses without error
- [ ] Empty class body (`class Foo { }`) parses without error
- [ ] Empty array literal (`[]`) parses without error
- [ ] Switch with no default (cases only) parses without error
- [ ] Zero-argument pattern `Foo()` parses without error
- [ ] Empty field pattern `Foo {}` parses without error
- [ ] Empty annotation parentheses `@ann()` parses without error
- [ ] do-while with a multi-statement body parses without error
- [ ] Bare `return` (no expression) parses without error
- [ ] Array element assignment (`id[expr] := expr`) produces ArrayElementAssignNode
- [ ] Object instantiation uses `<assign_stmt>` + `<new_expr>` (no separate object_declare_stmt)
- [ ] `<call_stmt>` parses plain calls, method calls, and chained method calls
- [ ] Module import list parses WITHOUT double "import" keyword
- [ ] `export_stmt` parses correctly as a statement
- [ ] `pattern_match` parses correctly as a statement
- [ ] `expr is type` produces a node at comparison level (NOT postfix)
- [ ] `expr as type` produces a node at comparison level (NOT postfix)
- [ ] `a + b is int` parses as `(a + b) is int` — verify tree structure
- [ ] At least five deliberate parser error cases are tested and throw

**AST Pretty Printer**
- [ ] Every AST node type has at least one Pretty Printer test
- [ ] Nested nodes are indented one level deeper than their parent
- [ ] Pretty Printer output is a deterministic, non-empty string for all valid ASTs
- [ ] Pretty Printer does not crash on any valid AST

**Interpreter**
- [ ] Every fully-implemented production has at least one interpreter test
- [ ] Scope isolation: variable declared in a loop body does not leak to parent
- [ ] Scope isolation: variable declared in an if body does not leak to parent
- [ ] Scope isolation: function cannot read call-site variables
- [ ] Undefined variable reference throws a descriptive exception
- [ ] Call stack depth limit (500) is enforced — tested with infinite recursion
- [ ] Integer `//` Integer returns Integer, not Float
- [ ] Integer `**` Integer (non-negative) returns Integer, not Float
- [ ] `%` on Float operands throws a type error
- [ ] Division by zero throws for `/`, `//`, and `%`
- [ ] Array element assignment mutates the list in-place
- [ ] foreach over string iterates characters
- [ ] foreach over null throws
- [ ] All four built-in functions (`len`, `str`, `int`, `bool`) are tested
- [ ] `print` keyword statement writes to Console.Out — tested
- [ ] Empty array `[]` can be created and assigned
- [ ] Every "not yet implemented" feature throws the correct message at runtime
- [ ] Silent no-ops are absent — every parsed node either executes or throws

**Integration**
- [ ] Every integration scenario produces fully asserted, deterministic output
- [ ] At least one integration test combines functions + loops + arrays
- [ ] At least one integration test exercises try/catch/finally with throw
- [ ] The do-while multi-statement accumulator integration test passes
- [ ] At least one integration test exercises pattern matching (match/when)
- [ ] At least one integration test exercises the module system

**Code quality**
- [ ] Interpreter uses the Visitor pattern (INodeVisitor) — no large if-else chain
- [ ] Variable resolution uses a scope chain — no flat Dictionary copy
- [ ] All tests use `[TestClass]` and `[TestMethod]` — no XUnit or NUnit
- [ ] All tests pass with 0 failures
- [ ] No test uses `Assert.IsTrue(true)` or other vacuous assertions
- [ ] Demo programs run successfully via their `.cmd` scripts
- [ ] Solution builds with 0 errors and 0 warnings

---

# Build Instructions

## Build the .NET Application

After generating all source files, build and verify the solution using the following steps.

### 1. Restore NuGet Packages
```
dotnet restore TinyLanguage.sln
```

### 2. Build the Solution
```
dotnet build TinyLanguage.sln --configuration Release
```
- The build must complete with **0 errors and 0 warnings**.
- If there are build errors, fix them before proceeding.

### 3. Run Unit Tests
```
dotnet test TinyLanguage.UnitTests.dll --configuration Release --logger "console;verbosity=normal"
```
- All tests must pass with **0 failures**.

### 4. Run Integration Tests
```
dotnet test TinyLanguage.IntegrationTests.dll --configuration Release --logger "console;verbosity=normal"
```
- All tests must pass with **0 failures**.

### 5. Run the Demo Suite
```
dotnet run --project TinyLanguage --configuration Release
```
- With no arguments the application must run the built-in demonstration suite and print:
  `All demos completed successfully.`
- Exit code must be **0**.

### 6. Verify File-Processor Mode
```
dotnet run --project TinyLanguage --configuration Release -- <input-file.tlg> <output-file.txt>
```
- The output file must contain the program output only.
- Errors must be written to stderr, not the output file.
- Exit code must be **0** on success, **1** on any error.

## Build Success Criteria

- `dotnet restore` completes without error.
- `dotnet build` produces **0 errors, 0 warnings**.
- All unit tests pass.
- All integration tests pass.
- Demo suite prints `All demos completed successfully.` and exits with code 0.

## Implementation Requirements for Building

To ensure the solution builds correctly according to specifications:

1. **Solution Structure**:
   - Create a solution file `TinyLanguage.sln`
   - Create project files for:
     * TinyLanguage (main application)
     * TinyLanguage.Core (language implementation)
     * TinyLanguage.UnitTests
     * TinyLanguage.IntegrationTests

2. **Project Dependencies**:
   - TinyLanguage depends on TinyLanguage.Core
   - Unit tests depend on TinyLanguage.Core and TinyLanguage
   - Integration tests depend on TinyLanguage.Core and TinyLanguage

3. **Build Configuration**:
   - Use .NET 10.0 as specified in the requirements
   - Configure all projects to target .NET 10.0
   - Set build configuration to Release

4. **Compiler Settings**:
   - Enable all warnings as errors
   - Disable nullable reference types (as per specifications)
   - Disable implicit usings (as per specifications)

5. **Testing Requirements**:
   - All unit tests must use `[TestClass]` and `[TestMethod]`
   - All integration tests must pass with 0 failures
   - Test output must be clean with no unexpected warnings or errors

Never mark the task complete while any test is failing.
