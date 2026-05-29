# TinyLanguage — Phase 0 Architect Plan

Phase 0 deliverable for the Claude Code multi-agent build defined in
`Z:\repos\TinyLanguage\Build.md`. Specification source of truth:
`Z:\repos\TinyLanguage\Build.Solution.md` (READ-ONLY locked spec).
Companion architect doc: `Z:\repos\TinyLanguage\Build.Plan.md` (allocated
identifiers in §6 are PINNED — do NOT reroll).

This document is the single artefact downstream agents (1A–1D, 2, 3A–3B,
4A–4F, 5) consult before reading any of the source materials in full. Treat
the outline in §1 and the work-unit table in §2 as authoritative.

---

## Hard constraints — read before doing anything

1. **Canonical solution path.** The deliverable lives at
   `Z:\repos\TinyLanguage.2026.05.28.22\` (sibling of the orchestrator
   repo at `Z:\repos\TinyLanguage\`; UTC hour 15 today, 2026-05-13). Every
   absolute path in this document substitutes that exact string. The
   solution folder is NOT inside this repo, NOT inside any git worktree
   (`Z:\repos\TinyLanguage\.claude\worktrees\...`), and NOT inside any
   user temp/AppData path. If your CWD is a worktree, still write to the
   canonical absolute path verbatim — never relative `..\` forms.

2. **SDK pin.** `global.json` at the canonical solution root pins SDK
   `10.0.203` with `rollForward: latestPatch`. NEVER `latestFeature`
   (rolls UP to 10.0.300-preview and NRE's VS18's ResolvePackageAssets).
   See `Build.md` "SDK pinning" preamble.

3. **Long-path support — all three layers required.**
   (a) HKLM registry `LongPathsEnabled` DWORD = 1 (one-time, admin; reboot
       VS after enabling).
   (b) `Directory.Build.props` at solution root contains
       `<_LongPathsEnabled>true</_LongPathsEnabled>`.
   (c) `TinyLanguage\app.manifest` declares
       `<longPathAware xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">true</longPathAware>`
       AND `TinyLanguage.csproj` references it via
       `<ApplicationManifest>app.manifest</ApplicationManifest>`.
   Missing any layer makes VS Batch Rebuild fail with MAX_PATH on the
   integration-tests `obj\Release\net10.0\win-x64\PubTmp\Out\runtimes\win-x64\native\`
   chain even though `dotnet build` succeeds. Phase 5 §6c verifies all three.

4. **`TinyLanguage.exe` has NO demo mode.** Spec §"Console Application"
   describes a zero-args demo walker — REMOVED. Two modes only: zero-args
   = stdin pipe → stdout; two-args = file processor; `--dap` = Debug
   Adapter Protocol server. Anything else → usage to stderr + exit 1. No
   `Console.IsInputRedirected` branching. The demo-walking job lives in
   `TinyLanguage.DemoFiles\run-all-demos.cmd` (Phase 1D).

5. **Test output via `TestLog` helper.** Every test routes through
   `TestLog.Input(...)`, `TestLog.Section(...)`, `TestLog.Result(...)`.
   NEVER raw `Console.WriteLine($"Input: ...")` or escape `\n`→`\\n`.
   Exact `TestLog.cs` source lives in `Build.md` "Test output formatting"
   preamble. Authored once in each test project (UnitTests by Phase 3B,
   IntegrationTests by Phase 4A).

6. **Pinned GUIDs from `Build.Plan.md` §6 — DO NOT REROLL.**

   | Identifier              | Value                                      |
   |-------------------------|--------------------------------------------|
   | Shared Project          | `{5157DF2E-637E-49A6-AE37-F7E6F9D53055}`   |
   | VS18 package            | `{8D86897A-2C9B-4A38-BFE2-2CA687B82AC3}`   |
   | VS18 engine             | `{BAFF8877-1B8C-4D5C-ABB2-A4918F45F244}`   |
   | VS18 launcher CLSID     | `{A08C993B-F229-4376-B2D4-994E825527A1}`   |
   | VS18 stock DAP-host CLSID | `{DAB324E9-7B35-454C-ACA8-F6BB0D5C8673}` (REUSED — VS-owned, NOT generated) |

   These are baked into installer caches, the VS extension registry, and the
   slnx project reference. Future regenerations MUST use these exact values.

---

## 1. Hierarchical Outline of `Build.Solution.md`

Section numbers below match the headings as they appear in the spec file.
Each line is one sentence; an agent should be able to jump straight to the
right section by reading this outline. Line counts approximate; consult
the spec for exact text.

1. **`.NET Standards`** (spec lines ~7–71)
   1.1 **`.NET Version Requirements`** — net10.0, C#, no implicit usings, no nullable, VS 2026+ usable.
   1.2 **`Coding Style`** — lowerCamelCase locals; UpperCamelCase members/types; `I`-prefix interfaces; named Tuples with `Tuple` postfix using `var`; `readonly` everywhere possible; complete-noun naming.
   1.3 **`Library Usage`** — BCL value & reference types only; no NuGet.
   1.4 **`Programming Constructs`** — Tuples preferred for multi-return; AST nodes are Classes (Visitor pattern); stream-read source files.
   1.5 **`File System Structure`** — one type per file (class / interface / enum / record).
   1.6 **`Code Documentation`** — comments target business analysts / entry-level programmers.

2. **`Application Description`** (spec lines ~73–148)
   2.1 **`What to Build`** — solution folder `..\TinyLanguage.YYYY.MM.DD.HH`, name `TinyLanguage`, apply BNF Grammar Verification Strategy.
   2.2 **`Class Library (TinyLanguage.Lexer.dll)`** — Lexer + AST nodes + Parser + AstPrettyPrinter.
   2.3 **`Class Library (TinyLanguage.Interpreter.dll)`** — Interpreter; depends on Lexer.
   2.4 **`UnitTests (TinyLanguage.UnitTests.dll)`** — Lexer + Parser tests.
   2.5 **`IntegrationTests (TinyLanguage.IntegrationTests.dll)`** — Interpreter + end-to-end.
   2.6 **`Console Application (TinyLanguage.exe)`** — file-processor mode + demo mode (deviation D1 removes demo mode; see Hard Constraint §4).
   2.7 **`Tiny Language Demonstration Suite (TinyLanguage.DemoFiles)`** — SDK-style content-only csproj; ≥300 `.tlg` files (we deliver ≥500 across Tiers A/B/C); matching `.cmd` per demo.

3. **`Unit Testing Strategy & Requirements`** (spec lines ~151–206)
   3.1 **`Unit Testing Requirements`** — MSTest only; `UnitTests`/`IntegrationTests` suffix; method names `Subject_Action_ExpectedOutcome`.
   3.2 **`Test Validation Protocol`** — `dotnet build`, `dotnet run`, `dotnet test` commands + 0/0/Failed:0 acceptance + fix-and-retry loop.

4. **`TinyLanguage Syntax`** (spec lines ~209–854)
   4.1 **`Implementation Notes`** (notes 1–23) — comment char; `:=`; statement separators; bare return; keywords-as-identifiers; foreach-over-string; ArrayElementAssignNode; scope chain; function scope; operator types; expression precedence (lowest → highest: ternary → or → and → not → comparison/typeop → additive → multiplicative → power → unary → postfix → primary); conditional-expr disambiguation; cast disambiguation; lambda disambiguation; object instantiation via `:=` + `new`; `static` modifier; empty arg lists; top-level decls; for-loop variable scope; generic-type disambiguation; separator strictness; call-stmt form; pattern alternation associativity.
   4.2 **`BNF Grammar`** —
       * Comments (`#`, line-end; `//` is FLOOR DIVISION, never a comment).
       * Program Structure (`<program> ::= <stmt_list>`; flat).
       * Statements (`<assign_stmt>`, `<var_declare_stmt>`, `<const_declare_stmt>`, `<array_assign_stmt>`, `<if_stmt>`, `<while_stmt>`, `<for_stmt>`, `<foreach_stmt>`, `<do_while_stmt>`, `<switch_stmt>`, `<break_stmt>`, `<continue_stmt>`, `<print_stmt>`, `<input_stmt>`, `<function_def>`, `<call_stmt>`, `<return_stmt>`, `<class_def>`, `<module_def>`, `<import_stmt>`, `<export_stmt>`, `<try_stmt>`, `<throw_stmt>`, `<pattern_match>`, `<annotated_stmt>`).
       * Assignment & Declaration (`let`/`var`/`const`/`enum`; enum body uses `=` not `:=`).
       * Control Flow (`if/then/else/end`, `while/do/end`, `for ... to ... step ... do ... end`, `foreach ... in ... do ... end`, `do ... while`, `switch { case: ... default: ... }`, `break`, `continue`).
       * I/O (`print <expr>`, `input <id>`).
       * Functions (`<function_def>`, `<method_def>` with optional `static`, `<param>` four forms, `<call_stmt>` for plain/method/chained calls, `<return_stmt>` with optional expr).
       * Lambda Expressions (`function(params) <expr>` or `function(params) <stmt_list> end`).
       * Classes & Objects (`<class_def>` with optional `static`/`extends`/`implements`, `<member_list>` of `<method_def>`/`<field_declare_stmt>`/`<const_declare_stmt>`/`<constructor_def>`).
       * Module System (`<module_def>` with optional `import` list using `<module_import>`, `<import_stmt>`, `<export_stmt>`).
       * Expressions — full precedence ladder: `<expr>` (ternary) → `<or_expr>` → `<and_expr>` → `<not_expr>` → `<comparison_expr>` (incl. `is`/`as`) → `<additive_expr>` (`+`, `-`, `&`) → `<multiplicative_expr>` (`*`, `/`, `%`, `//`) → `<power_expr>` (right-assoc) → `<unary_expr>` (prefix `-`) → `<postfix_expr>` (member access, index, call) → `<primary>`.
       * Inline Conditional Expression (`if ... then ... else ...` valid only in expression context).
       * Cast Expression (`(<type>) <unary_expr>`).
       * Arrays (`<array_assign_stmt>` `id[expr] := expr` produces `ArrayElementAssignNode`).
       * Exception Handling (`<try_stmt>`/`<catch_clause>` bare or typed/`<throw_stmt>`).
       * Pattern Matching (`<pattern_match>`/`<pattern_case>` with optional `when` guard; `<pattern>` literal/id/wildcard/constructor/list/field/alternation).
       * Annotations (`@<id>` or `@<id>(<param_list>)`; param list uses `=`).
       * Type System (`<type> ::= <base_type> { <type_suffix> }`; suffixes `[]` and `?`; `<base_type>` includes `int`/`float`/`string`/`bool`/`array`/`object`/`null`/`void`/`<map_type>`/`<generic_type>`/`<id>`).
       * Literals & Identifiers (`<boolean>`, `<string>` with both delimiters + 5 escape sequences, `<number>` including `0b`/`0o`/`0x`, `<id>`).
       * Terminal Symbols.

5. **`TinyLanguage Semantics`** (spec lines ~857–977)
   5.1 **`Type System`** — Integer (64-bit signed long), Float (64-bit double), Bool; arithmetic promotion table; operator-specific rules (`//`, `**`, `%`, `&`, `+`); truthiness table.
   5.2 **`Built-in Functions`** — `len`, `str`, `int`, `bool`. `print` is a STATEMENT keyword, not a callable function.
   5.3 **`Scope Rules`** — linked-list scope chain (not flat dict); function scope (parent = global only); closures out-of-scope; lambdas see only global + own params.
   5.4 **`Runtime Error Policy`** — descriptive message + source line number; stack depth limit 500; div-by-zero; type mismatch; unimplemented features throw `"Feature not yet implemented: <name>"`.
   5.5 **`Feature Implementation Status`** — table of categories that must be fully implemented (declarations, control flow, functions, arrays, exceptions, annotations, expressions, classes, modules, pattern matching, list comprehension, lambdas, type operations).
   5.6 **`Interpreter Architecture`** — Visitor pattern via `INodeVisitor`; no large if-else dispatch.

6. **`BNF Grammar Verification Strategy`** (spec lines ~980–1418)
   6.1 **`Layer 1 — Lexer Coverage`** — every keyword/operator/delimiter/literal form; line+column tracking; Unknown for invalid; comment text excluded from tokens.
   6.2 **`Layer 2 — Parser Coverage`** — every non-terminal; every `|` alternative; every `?` optional present and absent; multiple top-level fn defs; deliberate error cases (≥5).
   6.3 **`Layer 3 — AST Pretty Printer Coverage`** — every node type; indentation one level deeper than parent; deterministic non-empty output.
   6.4 **`Layer 4 — Interpreter Coverage`** — every executable production; scope isolation; arithmetic correctness; built-ins; truthiness.
   6.5 **`Layer 5 — Integration Coverage`** — FizzBuzz, Fibonacci, bubble sort, string builder, calculator, exception safety, accumulator do-while, mixed bases, annotation passthrough, nested control flow, pattern matching, module isolation.
   6.6 **`Verification Checklist`** — Lexer / Parser / Pretty Printer / Interpreter / Integration / Code quality (one box per item, every box must be checked).

7. **`Build Instructions`** (spec lines ~1421–1507)
   7.1 Restore (`dotnet restore`).
   7.2 Build (`dotnet build ... --configuration Release`; 0 errors, 0 warnings).
   7.3 Unit Tests.
   7.4 Integration Tests.
   7.5 Demo Suite (NOTE: D1 — exe demo mode removed; D3 — `run-all-demos.cmd` prints success banner).
   7.6 File-Processor Mode verification.
   7.7 Build Success Criteria.
   7.8 Implementation Requirements (solution structure, project deps, build config, compiler settings, testing reqs). NOTE: spec §7.8 names `TinyLanguage.Core`; this orchestration splits into `TinyLanguage.Lexer` + `TinyLanguage.Interpreter` per §2.2/§2.3 above.

---

## 2. Work-Unit List

Every output path is absolute and rooted at
`Z:\repos\TinyLanguage.2026.05.28.22\`. WU-IDs (WU-1A through WU-5) and
allocated GUIDs come from `Build.Plan.md` §3 and §6 — PINNED.

### Phase 1A — Solution Scaffold *(WU-1A)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:**
  * `Build.Solution.md` §`.NET Standards`, §`Application Description`, §`File System Structure`
  * `Build.md` preambles: "Output location", "Long-path support", "SDK pinning"
* **Outputs (absolute paths):**
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.slnx` (lists TinyLanguage csproj FIRST; Solution Items folder pre-declares `.gitignore`, `install-vscode-debugger.cmd`, `install-vs-debugger.cmd`, `TinyLanguage.wiki.md`)
  * `Z:\repos\TinyLanguage.2026.05.28.22\Directory.Build.props` (incl. `<_LongPathsEnabled>true`)
  * `Z:\repos\TinyLanguage.2026.05.28.22\global.json` (SDK 10.0.203, rollForward latestPatch)
  * `Z:\repos\TinyLanguage.2026.05.28.22\.gitignore`
  * `Z:\repos\TinyLanguage.2026.05.28.22\.vscode\launch.json` (two configs for TinyLanguage console)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage\app.manifest` (`<longPathAware>true</longPathAware>`)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage\TinyLanguage.csproj` (SelfContained, RuntimeIdentifier win-x64, PublishSingleFile, ApplicationManifest, CopySingleFileExeToDemoFiles target `AfterTargets="Publish"` Configuration=Release ONLY — no Build-time copy)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\TinyLanguage.Lexer.csproj` (net10.0 classlib, no implicit usings, no nullable)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Interpreter\TinyLanguage.Interpreter.csproj` (net10.0 classlib; ProjectRef Lexer)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.UnitTests\TinyLanguage.UnitTests.csproj` (MSTest)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.IntegrationTests\TinyLanguage.IntegrationTests.csproj` (MSTest)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.DemoFiles\TinyLanguage.DemoFiles.csproj` (SDK-style content-only)
* **Dependencies:** WU-0 (Phase 0).
* **Acceptance:** `dotnet build` from `Z:\repos\TinyLanguage.2026.05.28.22\` returns 0 errors, 0 warnings (no C# source yet — empty projects compile). All `<ProjectReference>` paths solution-relative.

### Phase 1B — Token & Lexer *(WU-1B)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:**
  * `Build.Solution.md` §`BNF Grammar`, §`Implementation Notes` (all 23), §`.NET Standards`/`Coding Style`
* **Outputs:**
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\TokenType.cs` (every kw from the BNF + `this`, `Pipe` for `|` outside `||`, `Constructor` capital-C, every type kw, every operator)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\Token.cs` (immutable record: Value, Type, Line)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\Lexer.cs` (streaming over file streams; `#` line comment; `//` is FloorDiv; `:=` only assignment, bare `=` lexes as SingleEqual)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\LexerException.cs`
* **Dependencies:** WU-0. Runs in parallel with 1A/1C/1D.
* **Acceptance:** `dotnet build TinyLanguage.Lexer` → 0/0.

### Phase 1C — AST Nodes + Pretty Printer *(WU-1C)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:** `Build.Solution.md` §`BNF Grammar` (all statement/expression productions), §`Implementation Notes`, §`Coding Style`
* **Outputs (all under `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\`):**
  * One `*Node.cs` file per AST node (Classes, not Records — Visitor pattern needs inheritance). Roughly 60+ node files covering statements, expressions, declarations, control flow, pattern matching.
  * `INodeVisitor.cs` (one `Visit` overload per node type)
  * `AstPrettyPrinter.cs` implementing `INodeVisitor`
  * Every node stores its source line number (per `Build.Solution.md` §5.4 AST requirement).
* **Dependencies:** WU-0. Parallel with 1A/1B/1D. Does NOT implement Parser.
* **Acceptance:** `dotnet build TinyLanguage.Lexer` → 0/0.

### Phase 1D — Demo Files (Tier A + B + C) *(WU-1D)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6` (may split Tier C across parallel sub-agents)
* **Isolation:** worktree
* **Inputs:** `Build.Solution.md` §`Tiny Language Demonstration Suite`, full BNF, all 23 Implementation Notes; `Build.md` Phase 1D prompt (Tier A/B/C catalogues).
* **Outputs (under `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.DemoFiles\`):**
  * **Tier A** `00001.*..00399.*.tlg` — feature-coverage demos, one feature per file, ≤15 lines each. ~300-330 files.
  * **Tier B** `00400.*..00499.*.tlg` — advanced data structures + ≥3 named subroutines + ≥8 distinct ops + deterministic output + 30–200 lines + header comment. ≥50 files (linear, trees, hashing+sets, graphs, algorithms).
  * **Tier C** `00500.*..00514.*` (linear lists, 15), `00520.*..00539.*` (trees, 20), `00560.*..00579.*` (tries+B-trees, 20), `00580.*..00599.*` (heaps+hash, 20), `00600.*..00629.*` (graphs+space part, 30), `00640.*..00659.*` (ADTs+composites, 20). Exactly 125 demos covering the catalogued data structures with Wikipedia URL in header.
  * One `<prefix>.<name>.cmd` per `.tlg` using the CWD-independent template (`%~dp0`, `%~1`/`%~2` for output path, `%~dp0output.txt` default, `type "%OUTPUT%"` to print, errorlevel propagation; NO `cd` or `pushd`).
  * `run-all-demos.cmd` (D2): walks `*.tlg` in alphabetic order via `dir /b /a-d /on`, pre-flights `%~dp0TinyLanguage.exe`, prints `=== <file> ===` / output / `--- end <file> ---`, ends with `All demos completed successfully.` exit 0; on any non-zero exe exit prints diagnostic to stderr and exits with that code.
* **Dependencies:** WU-0. Parallel with 1A/1B/1C. Demos are NOT executed in this phase (no interpreter yet).
* **Acceptance:** ≥500 `.tlg` files at the canonical DemoFiles path; one `.cmd` per `.tlg`; `run-all-demos.cmd` present. No execution.

### Phase 2 — Parser *(WU-2)*

* **Agent type / model:** `general-purpose` / `claude-opus-4-6` (complex recursive descent + 23 disambiguation rules)
* **Isolation:** worktree (merges outputs from 1A+1B+1C first)
* **Inputs:** `Build.Solution.md` full §`BNF Grammar` and all 23 §`Implementation Notes`.
* **Outputs:**
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\Parser.cs` (recursive-descent matching every BNF production; `ParseStatementList` accepts optional stop-token sets per notes 3/12; cast/lambda/conditional/generic disambiguation per notes 12–14, 20; bare-return per note 4; operator precedence per note 11; pattern alternation via `TokenType.Pipe`)
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Lexer\ParserException.cs` (carries source line number)
* **Dependencies:** WU-1A + WU-1B + WU-1C.
* **Acceptance:** `dotnet build TinyLanguage.Lexer` → 0/0.

### Phase 3A — Interpreter *(WU-3A)*

* **Agent type / model:** `general-purpose` / `claude-opus-4-6`
* **Isolation:** worktree
* **Inputs:** `Build.Solution.md` §`Class Library (TinyLanguage.Interpreter)`, §`TinyLanguage Semantics`, all 23 §`Implementation Notes`, full BNF.
* **Outputs (under `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Interpreter\`):**
  * `IInterpreter.cs`
  * `Interpreter.cs` — tree-walking `INodeVisitor`; operator semantics per note 10 (`Integer ** Integer` non-neg → Integer; `Integer // Integer` → Integer; `% Float` → type error); foreach-over-string per note 6 yields each char as 1-char string; `ArrayElementAssignNode` mutates in-place per note 7; scope chain per notes 8–9 (linked list, function frame parent = global only)
  * `Scope.cs` — linked-list scope; `LocalBindings` for DAP later
  * `InterpreterException.cs` with source line
* **Dependencies:** WU-2.
* **Acceptance:** `dotnet build TinyLanguage.Interpreter` → 0/0.

### Phase 3B — Unit Tests (Lexer + Parser) *(WU-3B)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:** `Build.Solution.md` §`Unit Testing Strategy & Requirements`, §`BNF Grammar Verification Strategy` (Layers 1 + 2), `Build.md` "Test output formatting" preamble (exact `TestLog.cs` source — D8).
* **Outputs (under `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.UnitTests\`):**
  * `TestLog.cs` (namespace `TinyLanguage.UnitTests`, verbatim from Build.md preamble)
  * `LexerUnitTests.cs` — every keyword/operator/delimiter/literal form; line+column tracking; Unknown handling; `=` lexes SingleEqual not Unknown; multi-token sequences; bare `|` lexes Pipe
  * `ParserUnitTests.cs` — every non-terminal production; every `|` alternative; every `?` present-and-absent; multiple top-level fn defs; empty bodies (`function foo() end`, `class Foo { }`, `[]`, `switch x {default:...}`, `Foo()`, `Foo{}`, `@ann()`); bare return; ArrayElementAssignNode shape; `is`/`as` at comparison level (NOT postfix); `a + b is int` parses as `(a + b) is int`; ≥5 deliberate parse-error cases
  * Every test calls `TestLog.Input(...)` / `TestLog.Result(...)`. NO raw `Console.WriteLine($"Input: ...")`. NO `Visible()` helper escaping `\n`.
* **Dependencies:** WU-2.
* **Acceptance:** `dotnet test TinyLanguage.UnitTests` → Failed: 0.

### Phase 4A — Integration Tests *(WU-4A)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:** `Build.Solution.md` §`IntegrationTests`, §`BNF Grammar Verification Strategy` (Layers 4 + 5), `Build.md` "Test output formatting" preamble (D8).
* **Outputs (under `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.IntegrationTests\`):**
  * `TestLog.cs` (namespace `TinyLanguage.IntegrationTests`, verbatim from Build.md preamble — same class, only namespace differs from 3B's copy)
  * `InterpreterIntegrationTests.cs` — end-to-end programs for every Verification-Checklist row (FizzBuzz, Fibonacci, bubble sort, string builder, calculator, exception safety with nested try/catch/finally, accumulator do-while, mixed-base literals, annotation passthrough, nested control flow, match/when, module import/export). May load Tier A/B demos as test inputs.
  * Every test calls `TestLog.Input(label, source)` / `TestLog.Result(actual)`. Multi-line content uses real `\n`.
* **Dependencies:** WU-3A + WU-3B.
* **Acceptance:** `dotnet test TinyLanguage.IntegrationTests` → Failed: 0.

### Phase 4B — Console Application *(WU-4B)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:** `Build.Solution.md` §`Console Application` (for context only); `Build.md` "Deliberate deviations" item D1 (NO demo mode).
* **Outputs:**
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage\Program.cs` — exactly two execution modes:
    * zero args → read from stdin, write program output to stdout (unconditional; NO `Console.IsInputRedirected` branching)
    * two args → read input file, write output file
    * anything else → usage line to stderr, exit 1
  * NO `LocateDemoDirectory`, NO `DemoFiles\` walking, NO `=== filename ===` banners, NO `All demos completed successfully.` print.
* **Dependencies:** WU-3A + WU-3B.
* **Acceptance:**
  * `dotnet build` from solution root → 0/0
  * `dotnet publish TinyLanguage -c Release` produces ~36 MB self-contained single-file exe; `CopySingleFileExeToDemoFiles` target (`AfterTargets="Publish"`, Configuration=Release) copies it to `TinyLanguage.DemoFiles\TinyLanguage.exe`
  * Smoke: `echo print "hi" | TinyLanguage.exe` → prints `hi`, exit 0
  * Smoke: `TinyLanguage.exe TinyLanguage.DemoFiles\00001.hello_world.tlg out.txt` → exit 0, `out.txt` non-empty
  * Smoke: `TinyLanguage.exe foo` → usage on stderr, exit 1

### Phase 4C — Debugger Engine + DAP Adapter *(WU-4C)*

* **Agent type / model:** `general-purpose` / `claude-opus-4-6` (cooperative threading + DAP wire protocol)
* **Isolation:** worktree
* **Inputs:** `Build.md` "Deliberate deviations" item D4; existing Interpreter from WU-3A; existing Program.cs from WU-4B.
* **Outputs:**
  * **Engine additions** under `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.Interpreter\`:
    * `IDebuggerHost.cs` — `OnStatementBefore`, `OnFunctionEnter`, `OnFunctionExit`, `OnUnhandledException`
    * `DebuggerControl.cs` — enum Continue/StepIn/StepOver/StepOut/Pause/Restart/Quit
    * `StatementContext.cs` — sealed class with `AstNode`, `Line`, `Scope`, `IReadOnlyList<DebuggerStackFrame> CallStack`
    * `DebuggerStackFrame.cs`
    * `DebuggerRestartException.cs` / `DebuggerQuitException.cs` (internal sealed)
    * `AssemblyInfo.cs` with `[assembly: InternalsVisibleTo("TinyLanguage.DebugAdapter")]`
    * Modifications to `Interpreter.cs`: `IDebuggerHost DebuggerHost { get; set; }` (null = zero overhead, one null-check per stmt); `_debugCallStack` maintained when host set; per-statement `host.OnStatementBefore` call; `Evaluate(expression, scope)` for DAP evaluate/conditional bps/logpoints; `RunWithDebugger(program, host, stdout, stdin)` entry point
    * Modifications to `Scope.cs`: `LocalBindings` exposing this scope's bindings only (no parent walk)
  * **New project** `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.DebugAdapter\` (net10.0 classlib, no nullable, no implicit usings; ProjectRefs Interpreter+Lexer):
    * `TinyLanguage.DebugAdapter.csproj`
    * `DebugAdapterServer.cs` — `RunOnStdInOut()` / `Run(Stream, Stream)`
    * `DapMessageReader.cs` / `DapMessageWriter.cs` — `Content-Length\r\n\r\n<JSON>` framing UTF-8; `System.Text.Json` only
    * `DapHost.cs` — implements `IDebuggerHost`; cross-thread bridge; `BlockingCollection<DebuggerControl>` for server→worker; `volatile bool _pauseRequested`; breakpoints dict line→`BreakpointInfo`; conditional eval via `Interpreter.Evaluate`; logpoint `{expr}` interpolation emits `output` event without pausing
    * `BreakpointInfo.cs`, `VariableHandle.cs`
    * Handles DAP requests: initialize, launch (accepts `program` AND `source`), setBreakpoints, configurationDone, threads, stackTrace, scopes, variables, evaluate, continue, next, stepIn, stepOut, pause, restart, disconnect
    * Emits DAP events: initialized, stopped (entry/step/breakpoint/pause/exception), continued, output (stdout/stderr), terminated, exited
    * Capabilities advertised: supportsConfigurationDoneRequest, supportsConditionalBreakpoints, supportsLogPoints, supportsEvaluateForHovers, supportsRestartRequest, supportsTerminateRequest
  * **Console flag** — `Program.cs` adds `if (args.Length == 1 && args[0] == "--dap") return DebugAdapterServer.RunOnStdInOut();` BEFORE existing zero/two-args branches; updates usage line; ProjectReference TinyLanguage → TinyLanguage.DebugAdapter added to `TinyLanguage.csproj`.
  * **slnx edit** — inserts `<Project Path="TinyLanguage.DebugAdapter\TinyLanguage.DebugAdapter.csproj" />` between Interpreter and UnitTests.
  * **Tests**:
    * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.UnitTests\DebuggerEngineUnitTests.cs` — ~9 tests with `RecordingHost` (records calls, returns scripted DebuggerControl from queue). Covers: breakpoint on line N pauses; StepIn pauses every stmt; StepOver skips fn bodies; StepOut runs until depth decreases; Evaluate reads locals + arithmetic + member access; Restart unwinds cleanly; null host = zero overhead.
    * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.IntegrationTests\DebugAdapterIntegrationTests.cs` — ~7 tests via in-process Stream pairs (no exe spawn): initialize handshake; launch+configurationDone+stopOnEntry; continue runs to completion; setBreakpoints+continue stops at bp; stackTrace; variables; evaluate. All via `launch.source` inline overload.
    * Every debug test uses `TestLog.Input` / `TestLog.Result`; composite results print each field on its own line with `\n`.
* **Dependencies:** WU-3A + WU-4B.
* **Acceptance:**
  * `dotnet build` → 0/0
  * `dotnet test` → Failed: 0 (numbers exceed the pre-debugger count by 9+7)
  * `dotnet publish TinyLanguage -c Release` → ~36 MB self-contained exe in `DemoFiles\`
  * `TinyLanguage.DemoFiles\run-all-demos.cmd` → `All demos completed successfully.` exit 0 (regression check)
  * DAP smoke: piping a Content-Length-framed `initialize` JSON request into `TinyLanguage.exe --dap` yields a JSON response `command:initialize, success:true` plus an `initialized` event.

### Phase 4D — VS Code Extension *(WU-4D)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree
* **Inputs:** `Build.md` Phase 4D prompt; existing DAP server from WU-4C.
* **Outputs:**
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vscode\package.json` — name `tinylanguage-debug`, publisher `tinylanguage-local`, version 0.1.0, engines.vscode `^1.70.0`, main `./extension.js`. activationEvents MUST include `onLanguage:tinylanguage` (and `onDebug`, `onDebugResolve:tinylanguage`, `onDebugDynamicConfigurations:tinylanguage`). contributes.languages registers `.tlg`; contributes.debuggers registers type `tinylanguage` with configurationAttributes.launch.required `["program"]`.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vscode\extension.js` — CommonJS module; registers BOTH a `DebugConfigurationProvider` (synthesises launch config from active editor when no `.vscode/launch.json` exists) AND a `DebugAdapterDescriptorFactory` (returns `DebugAdapterExecutable(exePath, ["--dap"])`). Exe resolution chain: (a) `session.configuration.exe` override; (b) walk UP from `path.dirname(program)` for `TinyLanguage.exe` or `TinyLanguage.DemoFiles/TinyLanguage.exe`; (c) `${workspaceFolder}/TinyLanguage.DemoFiles/TinyLanguage.exe`; (d) bare `"TinyLanguage.exe"` PATH fallback. Use `fs.existsSync` for (a)-(c).
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vscode\README.md` — ≤5 install commands
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vscode\.vscodeignore`
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vscode\LICENSE.txt` — required to keep `vsce package` unattended (no `--allow-missing-license` flag exists)
  * Modifies `Z:\repos\TinyLanguage.2026.05.28.22\.vscode\launch.json` — keeps existing two console configs; adds third `{ type:"tinylanguage", request:"launch", name:"Debug current .tlg file", program:"${file}", stopOnEntry:true, preLaunchTask:"publish" }`
  * Creates `Z:\repos\TinyLanguage.2026.05.28.22\.vscode\tasks.json` with `publish` task running `dotnet publish TinyLanguage -c Release`
  * `Z:\repos\TinyLanguage.2026.05.28.22\install-vscode-debugger.cmd` — idempotent five-step installer (prereq check via early-return `:check_tool` to avoid the cmd-parser paren-substitution bug; `dotnet publish`; `npm install -g vsce` fallback to `%APPDATA%\npm\vsce.cmd`; `pushd extensions\vscode && vsce package --allow-missing-repository`; `code --install-extension ... --force`). Pause at start; pause on every error path; no parentheses in any string substituted into if-blocks.
* **Dependencies:** WU-4C.
* **Acceptance:**
  * `extensions\vscode\package.json` parses as valid JSON
  * `install-vscode-debugger.cmd` exits 0 on FIRST run AND on SECOND run (idempotent) when invoked as `cmd /c install-vscode-debugger.cmd < NUL > log 2>&1` from a clean shell
  * `.vscode\launch.json` retains existing configs AND adds the new `tinylanguage` one
  * `.vscode\tasks.json` has a `publish` task

### Phase 4E — Wiki Generation *(WU-4E)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6`
* **Isolation:** worktree (parallel-safe with 4F)
* **Inputs:** `Build.Solution.md` §1–§7 (locked spec); `Build.md` "Deliberate deviations" section; `Build.Plan.md` work-unit table; `CLAUDE.md` / `AGENTS.md` conventions; the freshly-built `TinyLanguage.exe` for snippet validation.
* **Outputs:**
  * `Z:\repos\TinyLanguage.2026.05.28.22\TinyLanguage.wiki.md` — single self-contained markdown file:
    1. What is TinyLanguage (+ three execution modes)
    2. Language tour (runnable snippets for hello/let-var-const/arithmetic/control flow/functions/lambdas/classes/arrays/modules/exceptions/match/built-ins/truthiness)
    3. Quick reference card (operators + keywords)
    4. Demo suite (Tier A/B/C category tables + how to run)
    5. Building and running (4 canonical commands; note `.cmd` demos require prior `dotnet publish`)
    6. Debugging in VS Code (install vsce + F5)
    7. Architecture (three-layer DAP; project layout; data-flow; visitor pattern; linked-list scope chain; BCL-only)
    8. Implementation notes reference (1, 2, 4, 5, 7, 8–9, 10, 11, 12, 13, 14, 16, 19, 20, 23 with one-line gloss; cross-ref spec §4.1)
    9. Test suite table
    10. Known limitations (no closure capture of caller locals; no bitwise ops; no map literals; single-threaded; no super())
    11. Regenerating the solution (point at Build.md; list phases 0, 1A-1D, 2, 3A-3B, 4A-4B, 4C, 4D, 4E this wiki, 4F, 5)
    12. References
  * Style: factual, terse, code-heavy, no marketing, no emoji. Markdown tables for reference matter; fenced code blocks tagged ```tinylanguage / ```bash / ```powershell.
* **Dependencies:** WU-4D.
* **Acceptance:**
  * File exists at canonical path, >200 lines and <800 lines.
  * Every ```tinylanguage fenced block parses cleanly through the lexer+parser.
  * Tier B + Tier C tables match actual filenames in `TinyLanguage.DemoFiles\`.

### Phase 4F — VS18 Editor Shim + Shared Project *(WU-4F)*

* **Agent type / model:** `general-purpose` / `claude-opus-4-6` (VS Debug Adapter Host wiring + pkgdef registry shape + .shproj import chain)
* **Isolation:** worktree (parallel-safe with 4E)
* **Inputs:** `Build.md` "Deliberate deviations" item D5/D9; existing DAP server from WU-4C; **PINNED GUIDs from §Hard Constraints above (Build.Plan.md §6)**.
* **Outputs:**
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\extensions.shproj` — ToolsVersion 15.0, ProjectGuid `{5157DF2E-637E-49A6-AE37-F7E6F9D53055}`, import chain: `Microsoft.Common.props` FIRST, then `CodeSharing\Microsoft.CodeSharing.Common.Default.props`, then `CodeSharing\Microsoft.CodeSharing.Common.props`, then `extensions.projitems`, then `CodeSharing\Microsoft.CodeSharing.CSharp.targets`. (Earlier runs that used ToolsVersion 14.0 without the `Common.props` import logged "The project file cannot be opened by the project system" — this exact chain fixes it.)
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\extensions.projitems` — `SharedGUID` matches shproj; lists every file under `extensions\vs\` and `extensions\vscode\` as `<None>` items.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\TinyLanguage.VsTools.csproj` — legacy MSBuild (NOT SDK-style), `TargetFrameworkVersion v4.7.2`, `CreateVsixContainer=true`, `DeployExtension=false`, `GeneratePkgDefFile=true`, PackageReferences: `Microsoft.VSSDK.BuildTools`, `Microsoft.VisualStudio.SDK`, `Microsoft.VisualStudio.Debugger.DebugAdapterHost.Interfaces 16.6.40406.1`, `Microsoft.VisualStudio.Shared.VSCodeDebugProtocol`. (Do NOT use the prompt-suggested `Microsoft.VisualStudio.VSCodeDebugAdapterHost` — it does not exist on nuget.org.)
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\source.extension.vsixmanifest` — Identity Id `TinyLanguage.VsTools.{8D86897A-2C9B-4A38-BFE2-2CA687B82AC3}`, Version 0.1.0, Publisher `tinylanguage-local`, InstallationTarget for VS Community/Pro/Enterprise `[18.0,)`, prerequisite `Microsoft.VisualStudio.Component.CoreEditor [18.0,)`.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\TinyLanguagePackage.cs` — `[Guid("8D86897A-2C9B-4A38-BFE2-2CA687B82AC3")]` AsyncPackage, empty `InitializeAsync`.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\TinyLanguageAdapterLauncher.cs` — `[Guid("A08C993B-F229-4376-B2D4-994E825527A1")]` `IAdapterLauncher` implementation. Notes: `IAdapterLauncher` extends `IDebugAdapterHostComponent` (no-op `Initialize`); `LaunchAdapter(LaunchAdapterRequest, ITargetHostInterop)` returns `ITargetHostProcess`; `UpdateLaunchOptions` pass-through. Exe resolution mirrors the VS Code factory (override → walk-up → solution-dir → PATH).
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\TinyLanguageTargetHostProcess.cs` — `ITargetHostProcess` wrapper around `System.Diagnostics.Process`; `ErrorDataReceived` is BCL `DataReceivedEventHandler`.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\Resources\PackageRegistration.pkgdef` — registers AD7Metrics engine `{BAFF8877-1B8C-4D5C-ABB2-A4918F45F244}` (Name "TinyLanguage", CLSID `{DAB324E9-7B35-454C-ACA8-F6BB0D5C8673}` — the VS-owned stock DAP-host CLSID, REUSED not generated; AdapterLauncher `{A08C993B-F229-4376-B2D4-994E825527A1}`); registers launcher CLSID; registers `.tlg` file extension association.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\Resources\icon.png` — 90x90 PNG.
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\launch.vs.json.template`
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\README.md`
  * `Z:\repos\TinyLanguage.2026.05.28.22\extensions\vs\LICENSE.txt`
  * `Z:\repos\TinyLanguage.2026.05.28.22\install-vs-debugger.cmd` — idempotent five-step installer. Prereqs (`dotnet`, `vswhere`, MSBuild + VSIXInstaller via vswhere) checked through early-return `:check_tool` pattern. Includes `:resolve_pf86` subroutine to bind `%ProgramFiles(x86)%` value via setlocal/endlocal BEFORE any if-block sees the literal `(x86)` parens. Builds VSIX via MSBuild, installs per-user via `VSIXInstaller /quiet`.
  * **slnx edit** — appends `<Project Path="extensions\extensions.shproj" />` AFTER the seven .NET projects (Lexer, Interpreter, DebugAdapter, TinyLanguage console, UnitTests, IntegrationTests, DemoFiles). Solution Items folder already references `install-vs-debugger.cmd` from Phase 1A — do NOT re-add.
* **Dependencies:** WU-4D.
* **Acceptance:**
  * `dotnet build` from canonical path → 0/0 (Shared Project is silently skipped by `dotnet`)
  * `dotnet test --no-build` → Failed: 0 (4F adds NO tests)
  * `MSBuild extensions\vs\TinyLanguage.VsTools.csproj /restore /p:Configuration=Release /p:DeployExtension=false` → 0 errors, produces `.vsix`
  * `devenv.com TinyLanguage.slnx /Rebuild "Debug|Any CPU" /Out ...rebuild.log` → "succeeded, 0 failed, 0 skipped"; `/Out` log does NOT contain `extensions.shproj : error : The project file cannot be opened by the project system`
  * `install-vs-debugger.cmd` exits 0 on first AND second run

### Phase 5 — Final Validation & Delivery *(WU-5)*

* **Agent type / model:** `general-purpose` / `claude-sonnet-4-6` (fix-and-retry loop, targeted edits)
* **Isolation:** worktree (the merge worktree — collects all prior branches)
* **Inputs:** all prior phase outputs; `Build.md` Phase 5 instructions; PINNED canonical absolute path `Z:\repos\TinyLanguage.2026.05.28.22\`.
* **Outputs:** none new (this phase verifies + delivers). Performs:
  * **Step 1** `dotnet build` at solution root → 0 errors / 0 warnings.
  * **Step 2** `dotnet test --verbosity normal` → Failed: 0. Fix-and-retry loop on failure.
  * **Step 3 + 4** `dotnet publish TinyLanguage -c Release` (interleaved before Step 3 if fresh build) → ~36 MB self-contained exe inside `TinyLanguage.DemoFiles\`. Verify via `(Get-Item TinyLanguage.DemoFiles\TinyLanguage.exe).Length -gt 30000000`. If ~160 KB → csproj regrew an `AfterTargets="Build"` copy target — remove and rerun.
  * **Step 4b** DAP smoke test — pipe a `Content-Length`-framed `initialize` request into `TinyLanguage.exe --dap`; expect `command:initialize, success:true` plus `initialized` event.
  * **Step 5** `.cmd` validation loop over every `.cmd` in `TinyLanguage.DemoFiles\`:
    * (a) exit 0
    * (b) output file non-empty
    * (c) stdout non-empty
    * (d) stdout does NOT contain `The application to execute does not exist` (apphost-not-finding-dll signature)
    Guard FIRST: re-verify `DemoFiles\TinyLanguage.exe` is still ~36 MB before the loop (re-publish if not).
    Spot-check correctness of `00001.hello_world`, `00036.fizzbuzz`, `00075.function_iterative_factorial`, `00079.function_gcd`, `00104.class_extends`, `00226.module_basic`.
  * **Step 6 — Deliver to canonical sibling path:**
    * 6a. `Remove-Item -Recurse -Force Z:\repos\TinyLanguage.2026.05.28.22\` if it exists from prior work.
    * 6b. `robocopy <worktree-solution-path> Z:\repos\TinyLanguage.2026.05.28.22\ /MIR /XD bin obj .vs /XF *.user`. Treat exit code ≥8 as failure.
    * 6c. Structural verification at the canonical path:
      * `TinyLanguage.slnx` exists
      * `global.json` exists, pins to 10.0.203 with `latestPatch`; `dotnet --version` from `$canonical` reports a 10.0.2xx SDK
      * `Directory.Build.props` contains `<_LongPathsEnabled>true`
      * `TinyLanguage\TinyLanguage.csproj` contains `<ApplicationManifest>`
      * `TinyLanguage\app.manifest` contains `<longPathAware>true`
      * All seven project csproj files present (Lexer, Interpreter, DebugAdapter, TinyLanguage console, UnitTests, IntegrationTests, DemoFiles)
      * `TinyLanguage.DemoFiles\*.tlg` ≥ 500 files; one `.cmd` per `.tlg`
      * `extensions\vscode\package.json` present (4D)
      * `extensions\vs\TinyLanguage.VsTools.csproj` present (4F)
      * `extensions\extensions.shproj` + `extensions.projitems` present (4F)
      * `install-vscode-debugger.cmd` (4D), `install-vs-debugger.cmd` (4F), `TinyLanguage.wiki.md` (4E) present at solution root
      * `HKLM\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled` DWORD = 1 (abort with `reg add` instructions if missing)
    * 6d. From a CLEAN shell at the canonical path: `dotnet build` 0/0; `dotnet test` Failed: 0; `dotnet publish TinyLanguage -c Release`; `TinyLanguage.DemoFiles\run-all-demos.cmd` → `All demos completed successfully.` exit 0.
    * 6e. Re-run Step 5 `.cmd` validation loop pointed at canonical `DemoFiles\`.
    * 6f. Confirm orchestrator repo is clean: `git status` from `Z:\repos\TinyLanguage\` shows nothing under the canonical path (it's outside the working tree).
    * 6g. Print absolute paths for the user (solution root, DemoFiles, published exe, wiki).
    * 6h. Confirm `TinyLanguage.wiki.md` >200 lines and references `Build.Solution.md`.
    * 6i. **Visual Studio Batch Rebuild smoke test (blocks plan completion):** resolve `devenv.com` (PATH → known VS2026 install paths → `vswhere -property productPath`); run `devenv.com $canonical\TinyLanguage.slnx /Rebuild "Debug|Any CPU" /Out $env:TEMP\tlg_vs_batch_rebuild.log`. Accept `$LASTEXITCODE -eq 0` AND log contains "Rebuild All succeeded" (or `0 failed`) AND log does NOT contain `must be less than 260 characters`, `too long`, `MSB6005`, or `MSB3491`.
* **Dependencies:** WU-4A + WU-4B + WU-4C + WU-4D + WU-4E + WU-4F (and transitively WU-1D for the demos).
* **Acceptance:** Every step above is green. The plan is NOT complete until 6i confirms `devenv.com /Rebuild` exits 0 with no MAX_PATH errors.

---

## 3. Dependency Graph & Critical Path

```
                              WU-0  (this Plan)
                                |
        +----------+------------+-----------+
        |          |            |           |
      WU-1A      WU-1B        WU-1C       WU-1D                   (Phase 1: 4 parallel worktrees)
        \          \           /            |
         \          \         /             |
          +------- WU-2 ------+             |                      (Phase 2: parser, joins 1A+1B+1C)
                   |                        |
             +-----+-----+                  |
             |           |                  |
           WU-3A       WU-3B                |                      (Phase 3: 2 parallel)
             \          /                   |
              \        /                    |
               +-+----+                     |
                 |    |                     |
               WU-4A WU-4B                  |                      (Phase 4A+4B parallel after 3A+3B)
                       |                    |
                     WU-4C  --- D4 splice (debugger additive)      (Phase 4C after 3A+4B)
                       |                    |
                     WU-4D  --- D4 splice (VS Code shim)           (Phase 4D after 4C)
                      / \                   |
                     /   \                  |
                  WU-4E  WU-4F  --- D5 wiki / D9 VS18 splice       (Phase 4E+4F parallel after 4D)
                     \   /                  |
                      \ /                   |
                      WU-5  <-------- WU-1D demos feed in here     (Phase 5)
```

**Where the deliberate deviations splice in:**

* **D1 — no demo mode in exe** — Phase 4B writes `Program.cs` without `LocateDemoDirectory` or `=== filename ===` banners.
* **D2 — `run-all-demos.cmd`** — Phase 1D authors the aggregator alongside the per-demo `.cmd`s.
* **D3 — Phase 5 acceptance** — Phase 5 Step 3/4 use `run-all-demos.cmd` instead of `dotnet run`.
* **D4 — DAP debugger** — Phase 4C adds engine hooks + `TinyLanguage.DebugAdapter` + `--dap` flag; Phase 4D adds VS Code shim at `extensions\vscode\`.
* **D5 — wiki** — Phase 4E generates `TinyLanguage.wiki.md` after 4D, in parallel with 4F. Pre-declared in slnx Solution Items by 1A.
* **D6 — long-path support** — Phase 1A authors all three layers (Directory.Build.props, app.manifest, csproj `<ApplicationManifest>`). Phase 5 §6c verifies all three plus HKLM registry.
* **D7 — SDK pin** — Phase 1A authors `global.json` (10.0.203 / `latestPatch`). Phase 5 §6c verifies.
* **D8 — `TestLog` helper** — Phase 3B authors `TestLog.cs` in UnitTests; Phase 4A authors the same class in IntegrationTests (same source, different namespace). Phase 4C tests use it too.
* **D9 — Editor shims under Shared Project** — Phase 4F authors `extensions\extensions.shproj` + `extensions.projitems` and adds the VS18 shim under `extensions\vs\`, parallel to the existing `extensions\vscode\` from 4D.

**Critical path (longest sequential chain to delivery):**

```
WU-0 -> WU-1B (or 1C) -> WU-2 -> WU-3A -> WU-4B -> WU-4C -> WU-4D -> WU-4F -> WU-5
```

That is nine sequential nodes. 4E joins WU-4D in parallel with 4F so it is on a co-critical path but never serialises ahead of 4F (4E is wiki text + snippet validation; 4F is VSIX + Shared Project + `devenv.com /Rebuild` validation, which dominates). 1D feeds WU-5 directly but does not extend the critical path because it runs in parallel with the entire 1B/1C/2/3/4 chain.

**Parallelism budget:**

| Phase window | Parallel worktrees |
|--------------|--------------------|
| Phase 1 | 4 (1A, 1B, 1C, 1D) |
| Phase 3 | 2 (3A, 3B) |
| Phase 4A+4B | 2 |
| Phase 4E+4F | 2 |
| Everywhere else | 1 |

---

## 4. Final Project Directory Layout

Two levels deep, rooted at `Z:\repos\TinyLanguage.2026.05.28.22\`. Annotation `(Pn)` = producing phase. Items added later by another phase are noted with `+Pn`.

```
TinyLanguage.2026.05.28.22\
├── TinyLanguage.slnx                                  (1A; +DebugAdapter project entry 4C; +extensions.shproj 4F)
├── Directory.Build.props                              (1A — D6: _LongPathsEnabled)
├── global.json                                        (1A — D7: SDK 10.0.203 latestPatch)
├── .gitignore                                         (1A)
├── install-vscode-debugger.cmd                        (4D — D4 shim installer)
├── install-vs-debugger.cmd                            (4F — D9 VS18 installer)
├── TinyLanguage.wiki.md                               (4E — D5 user reference)
│
├── .vscode\
│   ├── launch.json                                    (1A; +"Debug current .tlg file" config 4D)
│   └── tasks.json                                     (4D — "publish" task)
│
├── TinyLanguage\                                       (console exe project)
│   ├── TinyLanguage.csproj                            (1A: SelfContained, RuntimeIdentifier=win-x64, PublishSingleFile, ApplicationManifest, CopySingleFileExeToDemoFiles AfterTargets=Publish; +DebugAdapter ProjectReference 4C)
│   ├── app.manifest                                   (1A — D6: longPathAware=true)
│   └── Program.cs                                     (4B: stdin / file modes; +--dap branch 4C)
│
├── TinyLanguage.Lexer\                                 (Lexer + AST nodes + Parser + Pretty Printer)
│   ├── TinyLanguage.Lexer.csproj                      (1A)
│   ├── TokenType.cs                                   (1B)
│   ├── Token.cs                                       (1B)
│   ├── Lexer.cs                                       (1B)
│   ├── LexerException.cs                              (1B)
│   ├── (60+ *Node.cs files, one per AST class)        (1C)
│   ├── INodeVisitor.cs                                (1C)
│   ├── AstPrettyPrinter.cs                            (1C)
│   ├── Parser.cs                                      (2)
│   └── ParserException.cs                             (2)
│
├── TinyLanguage.Interpreter\                           (tree-walking interpreter + debugger engine hooks)
│   ├── TinyLanguage.Interpreter.csproj                (1A; ProjectRef Lexer)
│   ├── IInterpreter.cs                                (3A)
│   ├── Interpreter.cs                                 (3A; +DebuggerHost property / RunWithDebugger / Evaluate 4C)
│   ├── Scope.cs                                       (3A; +LocalBindings 4C)
│   ├── InterpreterException.cs                        (3A)
│   ├── IDebuggerHost.cs                               (4C — D4)
│   ├── DebuggerControl.cs                             (4C)
│   ├── StatementContext.cs                            (4C)
│   ├── DebuggerStackFrame.cs                          (4C)
│   ├── DebuggerRestartException.cs                    (4C)
│   ├── DebuggerQuitException.cs                       (4C)
│   └── AssemblyInfo.cs                                (4C: InternalsVisibleTo TinyLanguage.DebugAdapter)
│
├── TinyLanguage.DebugAdapter\                          (DAP server — D4; created 4C)
│   ├── TinyLanguage.DebugAdapter.csproj               (4C: net10.0 classlib, ProjectRefs Interpreter+Lexer)
│   ├── DebugAdapterServer.cs                          (4C)
│   ├── DapMessageReader.cs                            (4C)
│   ├── DapMessageWriter.cs                            (4C)
│   ├── DapHost.cs                                     (4C — IDebuggerHost implementation; cross-thread bridge)
│   ├── BreakpointInfo.cs                              (4C)
│   └── VariableHandle.cs                              (4C)
│
├── TinyLanguage.UnitTests\                             (MSTest — Lexer + Parser + Debugger Engine)
│   ├── TinyLanguage.UnitTests.csproj                  (1A)
│   ├── TestLog.cs                                     (3B — D8)
│   ├── LexerUnitTests.cs                              (3B)
│   ├── ParserUnitTests.cs                             (3B)
│   └── DebuggerEngineUnitTests.cs                     (4C — ~9 tests)
│
├── TinyLanguage.IntegrationTests\                      (MSTest — Interpreter + DAP)
│   ├── TinyLanguage.IntegrationTests.csproj           (1A)
│   ├── TestLog.cs                                     (4A — D8, same source as UnitTests' copy, different namespace)
│   ├── InterpreterIntegrationTests.cs                 (4A)
│   └── DebugAdapterIntegrationTests.cs                (4C — ~7 tests via in-process Stream pairs)
│
├── TinyLanguage.DemoFiles\                             (regular SDK-style csproj, NOT shproj)
│   ├── TinyLanguage.DemoFiles.csproj                  (1A: content-only `**/*.tlg` + `**/*.cmd` PreserveNewest)
│   ├── 00001.*.tlg .. 00399.*.tlg                     (1D — Tier A feature coverage)
│   ├── 00400.*.tlg .. 00499.*.tlg                     (1D — Tier B advanced data structures)
│   ├── 00500.*.tlg .. 00514.*.tlg                     (1D — Tier C: Linear lists, 15 demos)
│   ├── 00520.*.tlg .. 00539.*.tlg                     (1D — Tier C: Trees, 20 demos)
│   ├── 00560.*.tlg .. 00579.*.tlg                     (1D — Tier C: Tries + B-trees, 20 demos)
│   ├── 00580.*.tlg .. 00599.*.tlg                     (1D — Tier C: Heaps + Hash-based, 20 demos)
│   ├── 00600.*.tlg .. 00629.*.tlg                     (1D — Tier C: Graphs + space partitioning, 30 demos)
│   ├── 00640.*.tlg .. 00659.*.tlg                     (1D — Tier C: ADTs + composites, 20 demos)
│   ├── 00001.*.cmd .. (one per .tlg, CWD-independent) (1D)
│   ├── run-all-demos.cmd                              (1D — D2 aggregator)
│   └── TinyLanguage.exe                               (4B — ~36 MB self-contained single-file build, populated by CopySingleFileExeToDemoFiles AfterTargets=Publish)
│
└── extensions\                                         (Shared Project container — D9; created 4F)
    ├── extensions.shproj                              (4F — ProjectGuid {5157DF2E-...}; ToolsVersion 15.0; Microsoft.Common.props FIRST in import chain, then CodeSharing\*.props, then projitems, then CodeSharing\CSharp.targets)
    ├── extensions.projitems                           (4F — SharedGUID matches shproj; enumerates every file in vs\ and vscode\ as <None> items)
    │
    ├── vscode\                                         (VS Code shim — D4; created 4D)
    │   ├── package.json                               (4D)
    │   ├── extension.js                               (4D — DebugConfigurationProvider + DebugAdapterDescriptorFactory)
    │   ├── README.md                                  (4D)
    │   ├── .vscodeignore                              (4D)
    │   └── LICENSE.txt                                (4D — required to keep vsce package unattended)
    │
    └── vs\                                             (VS18 shim — D9; created 4F; net472 VSIX, built OUTSIDE the slnx by install-vs-debugger.cmd)
        ├── TinyLanguage.VsTools.csproj                (4F — legacy MSBuild, net472)
        ├── source.extension.vsixmanifest              (4F — VS18 InstallationTarget [18.0,))
        ├── TinyLanguagePackage.cs                     (4F — AsyncPackage, [Guid 8D86897A-...])
        ├── TinyLanguageAdapterLauncher.cs             (4F — IAdapterLauncher, [Guid A08C993B-...])
        ├── TinyLanguageTargetHostProcess.cs           (4F — ITargetHostProcess wrapper)
        ├── launch.vs.json.template                    (4F)
        ├── README.md                                  (4F)
        ├── LICENSE.txt                                (4F)
        └── Resources\
            ├── PackageRegistration.pkgdef             (4F — AD7Metrics engine {BAFF8877-...} reusing stock DAP-host CLSID {DAB324E9-...}; launcher CLSID {A08C993B-...}; .tlg extension assoc)
            └── icon.png                               (4F — 90x90)
```

---

## Closing notes for downstream agents

* Read `Build.md` preambles ("Output location", "Long-path support", "SDK pinning", "Test output formatting", "Deliberate deviations from Build.Solution.md") BEFORE starting your phase's work. The single most common failure mode is an agent reading the spec section directly and missing that the project owner has deliberately overridden it.
* All paths in agent outputs must be ABSOLUTE and rooted at `Z:\repos\TinyLanguage.2026.05.28.22\` — never `..\TinyLanguage.YYYY.MM.DD.HH\`, never relative to your worktree's CWD.
* If you're tempted to "simplify away" `global.json`, `Directory.Build.props`, `app.manifest`, the `--dap` branch, the VS18 shim, or the `TestLog` helper, STOP and re-read the corresponding deviation entry. Every one of them shipped because removing it caused a specific, reproducible failure documented in `Build.md`.

*End of Plan.md — Phase 0 architect deliverable. No code generated. `Build.Solution.md` unchanged.*
