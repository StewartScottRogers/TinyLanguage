# TinyLanguage — Build.Plan.md (architect plan)

Working architect document for the Claude Code orchestration build defined in
`Z:\repos\TinyLanguage\Build.md`. Specification source of truth:
`Z:\repos\TinyLanguage\Build.Solution.md` (READ-ONLY).

Canonical solution root: `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` (sibling of the
orchestrator repo; UTC year/month/day/hour substituted at orchestration start).

> **Execution model.** Subagents (even worktree-isolated) write directly to the
> canonical absolute path, which is OUTSIDE any repo/worktree. So the solution
> accumulates in one place — Phase 2's "merge worktree outputs" and Phase 5 Step
> 6a/6b robocopy are NO-OPS; validate the solution in place. (Robocopy remains a
> fallback only for an alternate build-inside-worktree model.)

---

## 1. Hierarchical Outline of Build.Solution.md

- **.NET Standards**
  - .NET Version Requirements (net10.0, C#, no implicit usings, no nullable, VS 2026+)
  - Coding Style (lowerCamelCase locals, UpperCamelCase members/types, `I`-prefix interfaces, Tuples named + `Tuple` postfix + `var`, `readonly` everywhere possible, complete-noun naming)
  - Library Usage (BCL only — value & reference types)
  - Programming Constructs (favor Tuples over Records/Classes/Structs; AST nodes are Classes for Visitor; stream-read source files)
  - File System Structure (one type per file: class / interface / enum / record)
  - Code Documentation (comments target business analysts / entry-level)
- **Application Description**
  - What to Build (`..\TinyLanguage.YYYY.MM.DD.HH`, BNF Grammar Verification Strategy)
  - Class Library `TinyLanguage.Lexer.dll` (lexer + AST node types + parser + pretty printer)
  - Class Library `TinyLanguage.Interpreter.dll` (depends on Lexer)
  - UnitTests `TinyLanguage.UnitTests.dll`
  - IntegrationTests `TinyLanguage.IntegrationTests.dll`
  - Console Application `TinyLanguage.exe` (file-processor mode, demo mode — see deviation D1)
  - Tiny Language Demonstration Suite `TinyLanguage.DemoFiles` (SDK-style csproj, 500+ `.tlg` + matching `.cmd` — Tier A feature coverage 00001-00399, Tier B advanced DS 00400-00499, Tier C Wikipedia-style DS catalogue 00500-00699)
- **Unit Testing Strategy & Requirements**
  - Unit Testing Requirements (MSTest only; `UnitTests` / `IntegrationTests` suffix)
  - Test Validation Protocol (commands, acceptance criteria, fix-and-retry loop)
- **TinyLanguage Syntax**
  - Implementation Notes 1–23 (comment char, `:=`, separators, bare return, keywords, foreach-string, ArrayElementAssign, scope chain, function scope, operator types, precedence, conditional expr, cast, lambda, `new`, `static`, empty arg lists, top-level decls, for-loop scope, generic-type, separator strictness, call-stmt, pattern alternation)
  - BNF Grammar (Comments, Program Structure, Statements, Assignment/Declaration, Control Flow, I/O, Functions, Lambdas, Classes/Objects, Module System, Expressions with full precedence ladder, Conditional Expression, Cast, Arrays, Exceptions, Pattern Matching, Annotations, Type System, Literals/Identifiers, Terminals)
- **TinyLanguage Semantics**
  - Type System (Integer/Float/Bool, arithmetic promotion table, operator-specific rules, truthiness)
  - Built-in Functions (`len`, `str`, `int`, `bool`; `print` is a statement keyword)
  - Scope Rules (linked-list scope chain, function scope, closures out-of-scope)
  - Runtime Error Policy (descriptive + line number, stack depth 500, div-by-zero, type mismatch, unimplemented features)
  - Feature Implementation Status (categories that must be fully implemented)
  - Interpreter Architecture (Visitor pattern via `INodeVisitor`)
- **BNF Grammar Verification Strategy**
  - Layer 1 — Lexer Coverage (every keyword, operator, delimiter, literal form)
  - Layer 2 — Parser Coverage (every non-terminal, every alternative, every optional)
  - Layer 3 — AST Pretty Printer Coverage (every node type, indentation, determinism)
  - Layer 4 — Interpreter Coverage (every executable production)
  - Layer 5 — Integration Coverage (FizzBuzz, Fibonacci, sort, calculator, etc.)
  - Verification Checklist (Lexer, Parser, Pretty Printer, Interpreter, Integration, Code quality)
- **Build Instructions**
  - Restore, Build, Unit Tests, Integration Tests, Demo Suite, File-Processor Mode
  - Build Success Criteria
  - Implementation Requirements (solution structure, project deps, build config, compiler settings, testing reqs)

---

## 2. Deliberate Deviations from Build.Solution.md

From Build.md "Deliberate deviations from Build.Solution.md" plus the additive Phase 4E (wiki) and Phase 4F (VS18 + Shared Project). The locked spec is overridden on exactly these points.

> The 2026.05.29.02 run also surfaced non-deviation build lessons (MSTest 4.0.2 specifics, the `DemoFiles.csproj` non-recursive-glob requirement, and the correct demo-sweep harness invocation) now captured in `Build.md` and `AGENTS.md` — so this table stays focused on grammar/semantics deviations.

> **Spec compliance, not a deviation (2026.05.29.23 run):** the interpreter was made spec-compliant on array out-of-bounds READ — Build.Solution.md Layer 4 ("Out-of-bounds index returns null without crashing") requires a list/string OOB read to return `null` (not throw); map missing-key still throws. This aligns the implementation with the locked spec and so is NOT recorded as a deviation below.

> The 2026.06.01 run surfaced further non-deviation lessons (now in `Build.md`/`AGENTS.md`, not the deviation table): class bodies are mandatorily brace-delimited `class X { ... }` per BNF §505 (127 brace-less Tier C demos were the run's dominant convergence failure); the parser must accept the `do { ... } while` brace-block form (spec §1.5); the interpreter must unwrap an `EnumValue` bound in `for i := A to B` loops; the **MSB3270** AnyCPU-vs-win-x64 warning on `TinyLanguage.IntegrationTests` (it references the win-x64 console exe) breaks the full-solution 0-warning gate unless `<ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch>None</...>` is set in Phase 1A; and the `run-all-demos.cmd` finalizer must emit `%TEMP%\tinylanguage_...` correctly. Delivered 631 demos / 396 tests, all gates green incl. `devenv /Rebuild` 8/0/0.

| ID | Title | Spec says | Override | Phases |
|----|-------|-----------|----------|--------|
| D1 | `TinyLanguage.exe` has no demo mode | Zero-arg walks `DemoFiles\` and prints "All demos completed successfully." | Two modes: zero-arg stdin pipe, two-arg file mode. Anything else → usage to stderr + exit 1. No `Console.IsInputRedirected` branching. | 4B, 5 |
| D2 | Demo aggregator script | Demo-walking lives in the exe | Lives in `TinyLanguage.DemoFiles\run-all-demos.cmd` alongside per-demo `.cmd` files. | 1D, 5 |
| D3 | Phase 5 acceptance shift | `dotnet run` prints the success banner | `run-all-demos.cmd` prints it. Per-`.cmd` validation loop unchanged. | 5 |
| D4 | Interactive debugger via DAP (additive) | Silent on debugging | Three-layer DAP: engine `IDebuggerHost` in Interpreter, new `TinyLanguage.DebugAdapter`, editor shims at `extensions\vscode\` and `extensions\vs\` (see D9). Activation flag `--dap`. | 4C, 4D, 4F |
| D5 | User-facing wiki (additive) | Silent on a wiki | Single self-contained `TinyLanguage.wiki.md` at solution root, generated after 4D and before 5. Pre-declared in slnx Solution Items by 1A. | 4E |
| D6 | Long-path support (additive) | Silent | Three layers so VS Batch Rebuild handles the timestamped path: (1) HKLM `LongPathsEnabled`=1 (admin, machine prereq), (2) `Directory.Build.props` with `<_LongPathsEnabled>true`, (3) `TinyLanguage\app.manifest` `<longPathAware>true` via `<ApplicationManifest>`. Phase 5 §6c verifies; §6i drives `devenv.com /Rebuild`. | 1A, 5 |
| D7 | SDK pin via global.json (additive) | Silent on SDK selection | `global.json` pins the NEWEST STABLE (non-preview/-rc) SDK installed (via `dotnet --list-sdks`), with `rollForward: latestPatch` so VS and `dotnet`-on-PATH produce compatible lockfiles. The literal version is a moving target — the reference run pinned `10.0.300` (no 10.0.2xx band was installed; a `10.0.203` pin would have failed). NEVER `latestFeature` (rolls up to a -preview and NRE's VS18 ResolvePackageAssets). Phase 5 §6c verifies (accepts any stable build; rejects only -preview/-rc). | 1A, 5 |
| D8 | TestLog helper (additive) | Tests print via raw `Console.WriteLine` | Tests route through `TestLog.Input/Section/Result` with `--- Input ---` / `--- Result ---` framing and real-newline indentation. Replaces the older `Visible(s)` pattern that escaped `\n` and collapsed multi-line stdout onto one line. Exact source in Build.md preamble. | 3B, 4A, 4C |
| D9 | Editor shims under a Shared Project (additive) | Silent on editor integration | Both shims live under `extensions\`. `extensions.shproj` + `extensions.projitems` list every shim file as `<None>` items, giving Solution Explorer one navigable tree. The Shared Project produces no assembly — `dotnet build` silently skips it, `devenv.com /Rebuild` loads it via `Microsoft.CodeSharing.CSharp.targets`. The VS18 VSIX (`extensions\vs\TinyLanguage.VsTools.csproj`, net472) is built outside the slnx by `install-vs-debugger.cmd`. GUIDs in §6. | 1A, 4D, 4F |
| D10 | Newlines as implicit `;` separators | Note 3: "Newlines are whitespace; they do not insert implicit semicolons." | `ParseStatementList` accepts a strictly-later upcoming-token line as an implicit separator when no explicit `;` was consumed. Required because Tier A demos use one-statement-per-line convention. REFINEMENT: as a corollary of newline-as-separator, the postfix parser must not consume a `[` (index) or `(` (call) beginning on a strictly-later source line as part of the previous statement's expression — otherwise a bracket-led next statement (e.g. a `[a,b] => ...` match case after `print "x"`) is wrongly swallowed; a leading `.` member access may still cross lines. (Fixed 00306.) | 2 |
| D11 | Trailing `;` before block-enders tolerated | Note 21: `;` before `end`/`else`/`catch`/`finally`/`while`/`}`/EOF is a parse error. | Trailing `;` is silently accepted as an empty separator; leading `;`s before a statement also tolerated. `function foo();` (with stray `;` after `()` closing paren) parses cleanly. Demo-corpus convention. | 2 |
| D12 | `else if` chains | BNF: `<if_stmt> ::= "if" <expr> "then" <stmt_list> ("else" <stmt_list>)? "end"` (nested ifs require their own `end`). | `ParseIfStatement` special-cases `else if` — recurses into a complete inner if-statement and uses its `end` to close the whole chain. FizzBuzz-style `if A then ... else if B then ... else ... end` parses with ONE `end`. REFINEMENT: the fold fires ONLY when `if` is on the SAME source line as `else`; an `if` on a later line is a normal nested if that owns its own `end` (the naive "any if after else" rule orphans the outer `end` and breaks ~17 nested-if-in-else demos). | 2 |
| D13 | Member assignment + postfix-LHS forms | BNF allows only `<id> := <expr>` and `<id>[expr] := <expr>`. No `obj.field := value`. | New AST nodes `MemberAssignStmtNode`, `PostfixAssignStmtNode`, `PostfixCallStmtNode`. `ParseStatement` dispatches `Identifier` AND `This` to a shared `ParsePostfixLedAssignOrCall` that parses a full postfix expression then specialises on `:=` or `(args)`. Supports `this.X := v`, `this.Items[i] := v`, `this.Data[i][j] := v`, `this.Nodes[i].AddNeighbor(...)`. | 1C (new node files), 2, 3A |
| D14 | `var x := 1` without type annotation | BNF: `<var_declare_stmt> ::= "var" <id> ":" <type> ":=" <expr>`. | Type annotation made optional, parallels `let`. Demo corpus uses both forms. | 2 |
| D15 | Dotted type names in `new` | BNF: `<new_expr> ::= "new" <id> "(" <arg_list>? ")"` (single identifier). | `ParseNewExpression` accepts a dotted chain (`new Shapes.Circle(5)`); final segment resolves against the class table. Module-qualified instantiation in Tier C demos. | 2 |
| D16 | Built-in conversion call-syntax `int(x)`/`float(x)`/`bool(x)`/`str(x)` | Spec lists int/str/bool/float as built-in functions but lexer makes them keyword tokens | ParsePrimary parses a type-name keyword immediately followed by `(` as a builtin conversion CALL (FunctionCallNode, canonical name int/float/bool/str). Casts/annotations/is/as unaffected. ~32 demos. | 2, 3A |
| D17 | Lambda body by first token | Note 14 (expr body vs block body) | A lambda body is a single expression if its first token starts an expression, else a statement BLOCK (optional `end`). Decided by first token, NOT by scanning for a matching `end` (which latches onto the enclosing function's `end`). Fixes `function(n) print n`. REFINEMENT: the block-vs-expression decision uses the FULL statement-starting keyword set (return/print/if/while/for/foreach/let/var/const/throw/try/switch/match/break/continue/do); in particular a body starting with `return` is a statement block (the first implementation omitted `return`, breaking 00171). | 2 |
| D18 | Array/list `+` concatenation | Note 10 silent on list operands | Interpreter `+` concatenates two arrays into a new list (corpus append idiom `arr := arr + [x]`; no in-spec list-grow builtin). | 3A |
| D19 | Class `const` → static member | Class body allows `const` | Interpreter routes class-body `const` to StaticMembers (reachable via `ClassName.CONST`), not per-instance Fields. | 3A |
| D20 | `export <definition>` leniency | BNF: `<export_stmt> ::= "export" <id>` (bare identifier only). | `ParseExportStatement` also accepts `export function|class|static|let|var|const <definition>`: parses the inner definition (it lands in module/global scope — modules promote exports to global, no qualified `M.f` access) and records the export marker (runtime no-op). Bare `export <id>` still works. Required by module demos 00291–00295. | 2 |
| D21 | `static` instance-style fields (`static let`/`static var`) | BNF `static` appears only on `<method_def>`; fields are non-static. | Extends D19: the class-member parser accepts `static` before `let`/`var`/`const` field declarations and routes them to the class's StaticMembers (reachable/assignable as `ClassName.Field`). `FieldDeclareNode` gains `IsStatic`. Note 16 ("static modifies a field"). Required by 00216, 00237. | 1C, 2, 3A |
| D22 | Indexed-assign append at `index == Length` | BNF/Note 7: `<id>[expr] := <expr>` mutates an existing element. | Interpreter: an indexed assignment where `index == array.Length` APPENDS (grow-by-one); `index < Length` mutates in place; `index > Length`/`< 0` still errors. Applies to both `arr[i] := v` (ArrayElementAssignNode) and `this.data[i] := v` (PostfixAssignStmtNode) via one shared `StoreIndexed`. Required by Tier B algorithm demos that build arrays by indexed assignment from empty. Safe: only converts a former error to success. | 3A |
| D23 | "Data Structures" catalogue test project (additive) | Silent on a per-data-structure test suite. | New 8th .NET project `TinyLanguage.DataStructures.Tests` with EXACTLY 220 numbered `[TestMethod]`s — one per structure on Wikipedia's *List of data structures* — each running a `.tlg` implementation in-process and asserting its golden `.expected`. The full catalogue, per-entry feasibility (yes/approx/primitive/no), demo-coverage rule, and generation/acceptance spec live in `Build.DataStructures.md` (machine-readable `tools\data-structures.catalogue.json`). Extends Tier C with catalogue-completion demos in reserved band `00700–00899` plus a `catalogue.manifest.tsv`. | 1A, 1D, 4.5, 4G, 5 |

### 2b. Conformance corrections carried forward (NOT deviations)

The 2026-08-10 Phase 4.7 adversarial review found five places where the implementation diverged
from `Build.Solution.md` in ways **no demo exercised** — a green golden sweep proves the corpus
works, it does not prove spec conformance. These are restorations of the spec, not deviations,
so they carry no `D` number; implement them from the start in the phases noted.

| # | Correction | Spec basis | Phase |
|---|---|---|---|
| C1 | `const` bindings are immutable everywhere, not just as class fields (an inner `let` may still shadow) | "`const` declares an immutable binding" + Runtime Error Policy "silent failures are forbidden" | 3A |
| C2 | Built-in functions (`len`, `str`, `int`, `bool`, `float`) may not be redefined by user code | Built-in Functions section | 3A |
| C3 | `switch X { }` with no `case` and no `default` is a PARSE error | `<case_list>` requires ≥1 clause; named in the Layer-2 error list | 2 |
| C4 | A class `<member_list>` takes no separator token — two members may sit on one line | `<member_list> ::= <member> <member_list>` | 2 |
| C5 | Comparison operators are non-associative — `1 < 2 < 3` is a PARSE error, not a deferred runtime type error | `<comparison_expr>` permits one operator; "all forms are non-associative" | 2 |

Plus one correctness rule that is neither deviation nor spec text, but an implementation trap
that silently produces wrong output (see AGENTS.md "Interpreter trap"): **evaluate the
assignment RHS before resolving the target container** in `ArrayElementAssignNode`,
`MemberAssignStmtNode` and `PostfixAssignStmtNode`. ~18 demos in Phase 3A.

### 2c. Ambiguities in Build.Solution.md — resolved by corpus precedent, do NOT thrash

Re-litigated every run; the pinned answers are:

| Ambiguity | Resolution |
|---|---|
| `Integer / Integer` — table says "Float (when the result has a fractional part)" | `/` is **always** float division (`6 / 3` → `2.0`); `//` is integer division |
| List comprehension — "defined but not yet implemented" vs Feature Status "must be fully implemented" | It **is** implemented (`[x * 2 for x in a]`). There is no `if` filter clause. |
| Note 13 cast-vs-grouping — a strict reading makes `(a)` a cast to type `a` | Treated as grouping; a strict reading would break ordinary parenthesised expressions |
| Note 2 vs Layer-1 checklist on a bare `=` | Follow the checklist: lexes as `SingleEqual` (not `Unknown`); the parser rejects it |
| Type annotations at runtime | Syntax only — not runtime-enforced. Enforcing would be invention, not conformance. |
| Layer-2 "trailing `;` before `end` → parse exception" vs D11 | D11 wins: it is tolerated |

---

## 3. Work-Unit List

Paths under canonical root `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`.

| ID | Title | Outputs | Dependencies | Phase |
|----|-------|---------|--------------|-------|
| WU-0 | Architect Analysis | This file (orchestrator working doc) | none | 0 |
| WU-1A | Solution Scaffold | `TinyLanguage.slnx` (Solution Items pre-declares `.gitignore`, both installers, wiki); `Directory.Build.props` (D6); `global.json` (D7); `.gitignore`; `.vscode\launch.json`; `TinyLanguage\app.manifest` (D6); seven project subdirs with empty csproj (incl. `TinyLanguage.DataStructures.Tests`, D23) | WU-0 | 1A |
| WU-1B | Token & Lexer | `TinyLanguage.Lexer\` — `TokenType.cs`, `Token.cs`, `Lexer.cs`, `LexerException.cs` (incl. `Pipe`, `This`, every kw) | WU-0 | 1B |
| WU-1C | AST Nodes + Pretty Printer | `TinyLanguage.Lexer\` — one `*Node.cs` per AST class, `INodeVisitor.cs`, `AstPrettyPrinter.cs` | WU-0 | 1C |
| WU-1D | Demo Files | `TinyLanguage.DemoFiles\` — ≥500 `.tlg` (delivered set: Tier A 00001–00399 feature coverage 399 demos, Tier B 00400–00499 advanced DS+algorithms 100 demos, Tier C 00500–00659 Wikipedia-style DS catalogue 125 demos across 6 categories; 624 `.tlg` total) + Tier C catalogue-completion 00700–00899 (per `Build.DataStructures.md`, D23) so all 220 Wikipedia structures have a backing demo, matching `.cmd` per demo, a matching golden `<name>.expected` file per demo (the EXACT predicted stdout), `run-all-demos.cmd`, `catalogue.manifest.tsv` (220 rows) | WU-0 | 1D |
| WU-2 | Parser | `TinyLanguage.Lexer\Parser.cs` (recursive-descent, full BNF, 23 notes), `ParserException.cs` | WU-1A, WU-1B, WU-1C | 2 |
| WU-3A | Interpreter | `TinyLanguage.Interpreter\` — `IInterpreter.cs`, `Interpreter.cs`, `Scope.cs`, `InterpreterException.cs` | WU-2 | 3A |
| WU-3B | Unit Tests | `TinyLanguage.UnitTests\` — `TestLog.cs` (D8), `LexerUnitTests.cs`, `ParserUnitTests.cs` | WU-2 | 3B |
| WU-4A | Integration Tests | `TinyLanguage.IntegrationTests\` — `TestLog.cs` (D8), `InterpreterIntegrationTests.cs` | WU-3A, WU-3B | 4A |
| WU-4B | Console App | `TinyLanguage\Program.cs` (two modes, D1); single-file Release publish copies `TinyLanguage.exe` into `DemoFiles\` via `AfterTargets="Publish"` | WU-3A, WU-3B | 4B |
| WU-4C | Debugger Engine + DAP | `TinyLanguage.Interpreter\` debug additions (`IDebuggerHost.cs`, `DebuggerControl.cs`, `StatementContext.cs`, `DebuggerStackFrame.cs`, restart/quit exception files, `AssemblyInfo.cs`); new project `TinyLanguage.DebugAdapter\`; `--dap` flag in `Program.cs`; `DebuggerEngineUnitTests.cs` + `DebugAdapterIntegrationTests.cs` | WU-3A, WU-4B | 4C |
| WU-4D | VS Code Editor Shim | `extensions\vscode\` (`package.json`, `extension.js`, `README.md`, `.vscodeignore`, `LICENSE.txt`); `install-vscode-debugger.cmd`; `.vscode\launch.json` adds `tinylanguage` config; `.vscode\tasks.json` adds `publish` task | WU-4C | 4D |
| WU-4E | Wiki | `TinyLanguage.wiki.md` (>200 lines, language tour / demo suite / debugger / architecture / regen steps) | WU-4D | 4E |
| WU-4F | VS18 Shim + Shared Project | `extensions\extensions.shproj` + `extensions.projitems`; `extensions\vs\` VSIX (`TinyLanguage.VsTools.csproj` net472, `source.extension.vsixmanifest`, `TinyLanguagePackage.cs` AsyncPackage, `TinyLanguageAdapterLauncher.cs` `IAdapterLauncher`, `TinyLanguageTargetHostProcess.cs`, `Resources\PackageRegistration.pkgdef`, `Resources\icon.png`, `launch.vs.json.template`, `README.md`, `LICENSE.txt`); `install-vs-debugger.cmd` (paren-safe `:check_tool` + `:resolve_pf86` for the `(x86)` literal-paren bug); slnx gains `<Project Path="extensions\extensions.shproj" />`. GUIDs in §6. | WU-4D | 4F |
| WU-4.7 | Adversarial Review (parser+interpreter) | Edge-case probing of `Parser.cs`/`Interpreter.cs`/`Scope.cs`/`Lexer.cs` via throwaway programs; fix genuine spec-divergences; keep the golden sweep at CRASH=0 TIMEOUT=0 GOLD=0 | WU-4.5 | 4.7 |
| WU-4G | Data Structures catalogue tests | `TinyLanguage.DataStructures.Tests\` — `MSTestSettings.cs`, `TestLog.cs`, `DataStructureGoldenRunner.cs`, `DataStructureCatalogueIntegrationTests.cs` (EXACTLY 220 `[TestMethod]`s generated from `tools\data-structures.catalogue.json` joined with `catalogue.manifest.tsv`); see `Build.DataStructures.md` (D23) | WU-3A, WU-1D, WU-4.5 | 4G |
| WU-5 | Final Validation & Delivery | `dotnet build` 0/0; `dotnet test` Failed: 0; `dotnet publish` produces ~36 MB exe in `DemoFiles\`; `run-all-demos.cmd` exits 0; `.cmd` validation loop a-d all pass; `devenv.com /Rebuild` reports "succeeded, 0 failed, 0 skipped" with the Shared Project loaded cleanly | WU-4A, WU-4B, WU-4C, WU-4D, WU-4E, WU-4F, WU-4.7 | 5 |

---

## 4. Dependency Graph & Critical Path

```
                              WU-0
                               |
        +----------+-----------+-----------+
        |          |           |           |
      WU-1A      WU-1B       WU-1C       WU-1D
        \          \           /
         \          \         /
          +----- WU-2 -------+
                  |
            +-----+-----+
            |           |
          WU-3A       WU-3B
            \           /
             \         /
              +---+---+
              |       |
            WU-4A   WU-4B
                     |
                   WU-4C
                     |
                   WU-4D
                    / \
                   /   \
                WU-4E  WU-4F
                   \   /
                    \ /
              demo-convergence gate
                     |
                  WU-4.7  <- adversarial review (parser+interpreter)
                     |
                  WU-4G   <- Data Structures catalogue tests (needs 3A + 1D + 4.5; D23)
                     |
                    WU-5  <- joins WU-4A, WU-4G and WU-1D leaves as well
```

- Phases 1A-1D parallel (four worktrees).
- WU-2 joins 1A + 1B + 1C; WU-1D feeds WU-5 directly.
- WU-3A and WU-3B parallel after WU-2.
- WU-4A and WU-4B parallel after WU-3A + WU-3B; WU-4C sequential after WU-3A + WU-4B; WU-4D after WU-4C; WU-4E and WU-4F parallel after WU-4D (neither overlaps the other's outputs).
- WU-4G (Data Structures catalogue tests, D23) runs after WU-3A + WU-1D + the demo-convergence gate (WU-4.5), in the test wave alongside WU-3B/WU-4A; serialized with their builds.
- WU-5 joins WU-4A, WU-4E, WU-4F, WU-4G, WU-1D.

> **Project-reference serializations (override the "parallel" labels).** Although
> the graph fans out, these edges force ordering: 1A → 1B/1C (Lexer csproj);
> 3A → 3B (UnitTests → Interpreter); 4B → 4A (IntegrationTests → console project);
> 4E and 4F must not run concurrent MSBuild on the solution (obj-lock races). 1D
> (demo authoring) is independent of all. Insert a **Demo-convergence** gate after
> 3A/3B/4A/4B (sweep all demos to 0 failures — each demo's actual stdout must
> byte-compare equal to its golden `<name>.expected`, not merely exit 0) before
> final validation. The **WU-4.7 adversarial review** (edge-case probing of
> `Parser.cs`/`Interpreter.cs`/`Scope.cs`/`Lexer.cs`, fixing genuine spec
> divergences) runs AFTER the convergence gate and BEFORE final validation,
> keeping the golden sweep at CRASH=0 TIMEOUT=0 GOLD=0.

**Critical path** (longest chain): WU-0 → WU-1B/1C → WU-2 → WU-3A → WU-4B → WU-4C → WU-4D → WU-4F → demo-convergence gate → WU-4.7 → WU-4G → WU-5.

---

## 5. Final Project Directory Layout

Paths under `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`. Annotation `(Pn)` = producing phase.

```
TinyLanguage.YYYY.MM.DD.HH\
├── TinyLanguage.slnx                                       (1A, incl. DataStructures.Tests D23; +DebugAdapter 4C; +extensions.shproj 4F)
├── Directory.Build.props                                   (1A — D6)
├── global.json                                             (1A — D7)
├── .gitignore                                              (1A)
├── install-vscode-debugger.cmd                             (4D)
├── install-vs-debugger.cmd                                 (4F)
├── TinyLanguage.wiki.md                                    (4E)
├── .vscode\
│   ├── launch.json                                         (1A; +tinylanguage 4D)
│   └── tasks.json                                          (4D)
├── TinyLanguage\
│   ├── TinyLanguage.csproj                                 (1A; +DebugAdapter ref 4C)
│   ├── app.manifest                                        (1A — D6)
│   └── Program.cs                                          (4B; +--dap 4C)
├── TinyLanguage.Lexer\
│   ├── TinyLanguage.Lexer.csproj                           (1A)
│   ├── TokenType.cs / Token.cs / Lexer.cs / LexerException.cs    (1B)
│   ├── *Node.cs (one per AST type)                         (1C)
│   ├── INodeVisitor.cs / AstPrettyPrinter.cs               (1C)
│   └── Parser.cs / ParserException.cs                      (2)
├── TinyLanguage.Interpreter\
│   ├── TinyLanguage.Interpreter.csproj                     (1A)
│   ├── IInterpreter.cs / Interpreter.cs / Scope.cs         (3A; +debug hooks 4C)
│   ├── InterpreterException.cs                             (3A)
│   ├── IDebuggerHost.cs / DebuggerControl.cs               (4C)
│   ├── StatementContext.cs / DebuggerStackFrame.cs         (4C)
│   ├── DebuggerRestartException.cs / DebuggerQuitException.cs   (4C)
│   └── AssemblyInfo.cs                                     (4C)
├── TinyLanguage.DebugAdapter\                              (4C — new project)
│   ├── TinyLanguage.DebugAdapter.csproj                    (4C)
│   ├── DebugAdapterServer.cs                               (4C)
│   ├── DapMessageReader.cs / DapMessageWriter.cs           (4C)
│   ├── DapHost.cs                                          (4C)
│   └── BreakpointInfo.cs / VariableHandle.cs               (4C)
├── TinyLanguage.UnitTests\
│   ├── TinyLanguage.UnitTests.csproj                       (1A)
│   ├── TestLog.cs                                          (3B — D8)
│   ├── LexerUnitTests.cs / ParserUnitTests.cs              (3B)
│   └── DebuggerEngineUnitTests.cs                          (4C)
├── TinyLanguage.IntegrationTests\
│   ├── TinyLanguage.IntegrationTests.csproj                (1A)
│   ├── TestLog.cs                                          (4A — D8)
│   ├── InterpreterIntegrationTests.cs                      (4A)
│   └── DebugAdapterIntegrationTests.cs                     (4C)
├── TinyLanguage.DataStructures.Tests\                      (csproj 1A; bodies 4G — D23)
│   ├── TinyLanguage.DataStructures.Tests.csproj            (1A)
│   ├── MSTestSettings.cs                                   (4G)
│   ├── TestLog.cs                                          (4G — D8)
│   ├── DataStructureGoldenRunner.cs                        (4G)
│   └── DataStructureCatalogueIntegrationTests.cs           (4G — 220 [TestMethod]s)
├── TinyLanguage.DemoFiles\
│   ├── TinyLanguage.DemoFiles.csproj                       (1A)
│   ├── 00001..00399.*.tlg                                  (1D — Tier A, 399 demos)
│   ├── 00400..00499.*.tlg                                  (1D — Tier B advanced DS, 100 demos)
│   ├── 00500..00514.*.tlg                                  (1D — Tier C: Linear lists)
│   ├── 00520..00539.*.tlg                                  (1D — Tier C: Trees)
│   ├── 00560..00579.*.tlg                                  (1D — Tier C: Tries + B-trees)
│   ├── 00580..00599.*.tlg                                  (1D — Tier C: Heaps + Hash-based)
│   ├── 00600..00629.*.tlg                                  (1D — Tier C: Graphs + Space partitioning)
│   ├── 00640..00659.*.tlg                                  (1D — Tier C: ADTs + Composites)
│   ├── 00700..00899.*.tlg                                  (1D — Tier C catalogue-completion, D23)
│   ├── catalogue.manifest.tsv                              (1D — 220 entry→demo rows, D23)
│   ├── *.cmd (one per .tlg, CWD-independent)               (1D)
│   ├── *.expected (golden stdout, one per .tlg)            (1D)
│   ├── run-all-demos.cmd                                   (1D — D2)
│   └── TinyLanguage.exe                                    (4B — ~36 MB, AfterTargets=Publish)
└── extensions\                                              (Shared Project container — D9)
    ├── extensions.shproj                                   (4F)
    ├── extensions.projitems                                (4F)
    ├── vscode\                                              (VS Code shim)
    │   ├── package.json / extension.js / README.md         (4D)
    │   └── .vscodeignore / LICENSE.txt                     (4D)
    └── vs\                                                  (VS18 shim — net472 VSIX, built outside slnx)
        ├── TinyLanguage.VsTools.csproj                     (4F)
        ├── source.extension.vsixmanifest                   (4F)
        ├── TinyLanguagePackage.cs                          (4F — AsyncPackage)
        ├── TinyLanguageAdapterLauncher.cs                  (4F — IAdapterLauncher)
        ├── TinyLanguageTargetHostProcess.cs                (4F)
        ├── launch.vs.json.template                         (4F)
        ├── README.md / LICENSE.txt                         (4F)
        └── Resources\
            ├── PackageRegistration.pkgdef                  (4F)
            └── icon.png                                    (4F)
```

---

## 6. Allocated Identifiers

Pin these. Future regenerations must NOT reroll values that are baked into the VS extension cache, installer scripts, slnx project references, or test fixtures.

| Identifier | Value | Used by | Phase |
|---|---|---|---|
| Shared Project GUID | `{5157DF2E-637E-49A6-AE37-F7E6F9D53055}` | `extensions.shproj` `<ProjectGuid>` and `extensions.projitems` `<SharedGUID>` (must match) | 4F |
| VS18 package GUID | `{8D86897A-2C9B-4A38-BFE2-2CA687B82AC3}` | `TinyLanguagePackage.cs` `[Guid(...)]`; Identity Id in `source.extension.vsixmanifest` | 4F |
| VS18 engine GUID | `{BAFF8877-1B8C-4D5C-ABB2-A4918F45F244}` | pkgdef `AD7Metrics\Engine\{...}` key | 4F |
| VS18 launcher CLSID | `{A08C993B-F229-4376-B2D4-994E825527A1}` | pkgdef `CLSID\{...}`; AD7Metrics `AdapterLauncher` field; `[Guid(...)]` on launcher class | 4F |
| VS18 stock DAP-host CLSID | `{DAB324E9-7B35-454C-ACA8-F6BB0D5C8673}` | pkgdef AD7Metrics `CLSID` field — REUSED from VS, not generated | 4F |
| .NET SDK pin | newest STABLE installed band (ref run: `10.0.300`) / `rollForward: latestPatch` | `global.json` | 1A (D7) |
| Extension version | `0.1.0` | `source.extension.vsixmanifest` + `extensions/vscode/package.json` | 4D, 4F |
| VS Code extension ID | `tinylanguage-local.tinylanguage-debug` | `code --list-extensions`; `code --install-extension` arg | 4D |
| VS18 install hive | `%LOCALAPPDATA%\Microsoft\VisualStudio\18.0_*\Extensions\` | per-user (no admin); `vswhere -find` against `Common7\IDE\Extensions\` returns empty | 4F |
| Data Structures catalogue | 220 entries (page order, pinned), new-demo band `00700–00899`, test names `Ds001..Ds220` (do not re-roll for existing entries — append new) | `Build.DataStructures.md`; `tools\data-structures.catalogue.json`; `TinyLanguage.DemoFiles\catalogue.manifest.tsv`; `TinyLanguage.DataStructures.Tests` | 1A/1D/4G (D23) |

---

*End of Build.Plan.md — architect-level outline. No code generated. Build.Solution.md and Build.md are unchanged.*
