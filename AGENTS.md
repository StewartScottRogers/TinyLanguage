# AGENTS.md

This file provides guidance to AI coding agents (Claude Code, OpenAI Codex, Cursor, GitHub Copilot, Aider, goose, etc.) when working with code in this repository. It follows the open [AGENTS.md](https://agents.md) convention. A one-line `CLAUDE.md` shim at the repo root imports this file via Claude Code's `@AGENTS.md` directive.

## Project Overview

TinyLanguage is a complete .NET 10.0 implementation of a small programming language (`.tlg` files) with Lexer, Parser, AST, and tree-walking Interpreter. The specification is locked in `Build.Solution.md` (read-only — never modify it). `Build.md` describes the multi-phase Claude Code orchestration plan. `Build.Plan.md` has the dependency graph and work unit breakdown. The user-facing reference is `TinyLanguage.wiki.md` inside each generated solution folder — produced by Phase 4E of the orchestration; cross-references the spec without duplicating it.

## SDK pin (`global.json`) — required so VS and CLI agree

The solution root contains `global.json` pinning the .NET SDK to the **newest
STABLE (non-preview, non-rc) SDK installed on the build machine** — determine it
with `dotnet --list-sdks`:

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestPatch"
  }
}
```

The literal version is a moving target, not a constant. On the current machine
only `10.0.300` (stable) and `10.0.100-rc.1` are installed — there is **no
`10.0.2xx` band** — so the pin is `10.0.300`. (Earlier docs pinned `10.0.203`;
that band is not installed here, so a `10.0.203` pin would fail outright.)

This pin exists because VS18's NuGet integration must resolve the SAME SDK the
CLI publish used, or lockfiles mismatch and VS NRE's `ResolvePackageAssets`
(`MSB4018`). The invariants: **(a)** a STABLE build VS18 understands — never a
`-preview`/`-rc` SDK; **(b)** `rollForward: latestPatch`, NEVER `latestFeature`
(which rolls UP across feature bands and can land on a `-preview` build that
defeats the pin). When a newer STABLE band ships (10.0.4xx, 11.x GA), bump the
version to it. Do NOT revert to a frozen `10.0.203`. Do not "simplify" this away — see deviation D7 in `Build.md` and `Build.Plan.md`.

## Long paths (machine prereq for VS Batch Rebuild)

The canonical solution path `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` plus the deepest single-file publish intermediate paths (`TinyLanguage.IntegrationTests\obj\Release\net10.0\win-x64\PubTmp\Out\runtimes\win-x64\native\<asset>`) crosses Windows' classic 260-char MAX_PATH. `dotnet build` from the .NET 10 SDK is long-path-aware and tolerates this; **Visual Studio's Batch Rebuild dialog is not** unless three layers cooperate (one machine, two in-solution):

1. **Registry** (one-time, admin): `reg add "HKLM\SYSTEM\CurrentControlSet\Control\FileSystem" /v LongPathsEnabled /t REG_DWORD /d 1 /f`. Restart Visual Studio after enabling.
2. **`Directory.Build.props`** at solution root contains `<_LongPathsEnabled>true</_LongPathsEnabled>` (authored by Phase 1A; see Build.md preamble "Long-path support — non-negotiable").
3. **`TinyLanguage\app.manifest`** declares `<longPathAware xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">true</longPathAware>` and `TinyLanguage.csproj` references it via `<ApplicationManifest>app.manifest</ApplicationManifest>` (also Phase 1A).

If a future agent "simplifies" any of (2) or (3) away, VS Batch Rebuild fails with `The fully qualified file name must be less than 260 characters` on the integration-tests project — **do not remove these on the assumption "the spec doesn't mention them."** They are deliberate additions; see deviation D6 in `Build.md` and `Build.Plan.md`.

## Build & Test Commands

```bash
dotnet build                                    # Must succeed with 0 errors, 0 warnings
dotnet test --verbosity normal                  # Must report Failed: 0
dotnet publish TinyLanguage -c Release          # Produces single-file self-contained TinyLanguage.exe (must run before run-all-demos)
TinyLanguage.DemoFiles\run-all-demos.cmd        # Walks every .tlg, prints "All demos completed successfully.", exit 0
echo print "hi" | dotnet run --project TinyLanguage   # stdin mode (no args): read source from stdin, write to stdout
dotnet run --project TinyLanguage -- input.tlg output.txt  # File-processor mode
TinyLanguage.DemoFiles\TinyLanguage.exe --dap   # Debug Adapter Protocol mode (used by VS Code; not run by hand)
```

Run a single test class:
```bash
dotnet test --filter "FullyQualifiedName~LexerUnitTests"
dotnet test --filter "FullyQualifiedName~DebuggerEngineUnitTests"
dotnet test --filter "FullyQualifiedName~DebugAdapterIntegrationTests"
```

**Acceptance criteria:** `dotnet build` → 0 errors/warnings; `dotnet test` → 0 failures; `TinyLanguage.DemoFiles\run-all-demos.cmd` → exit 0.

**Validating the per-demo `.cmd` files from PowerShell:** invoke each as `& cmd /c $c.FullName $out` (let PowerShell quote args). Do NOT manually double-quote the whole command (`cmd /c "<cmd>" "<out>"`) — when the line both begins and ends with a quote, `cmd.exe` strips the outer quotes and mangles it, making EVERY `.cmd` spuriously exit 1. If all/most `.cmd` "fail" while `run-all-demos.cmd` passes, suspect the harness invocation, not the demos.

**PowerShell stdin pipes prepend a UTF-8 BOM.** Piping a PowerShell string into `TinyLanguage.exe` (zero-arg stdin mode) or `TinyLanguage.exe --dap` makes the lexer reject a leading BOM / corrupts DAP Content-Length framing. Smoke-test stdin via `cmd /c` (e.g. `echo print "hi" | cmd /c TinyLanguage.exe`) or a BOM-less byte write, not a raw PowerShell string pipe. File mode auto-strips a BOM, so `.cmd` demos are unaffected.

> **Console exe override (deviation from Build.Solution.md):** Build.Solution.md describes a "Demo mode (no arguments)" baked into the exe. The project owner removed it — `TinyLanguage.exe` is now interpreter-only. Three modes: zero args = read source from stdin and write to stdout; two args = file-processor mode; `--dap` = Debug Adapter Protocol server (used by VS Code). The demo-walking job moved to `TinyLanguage.DemoFiles\run-all-demos.cmd` (an aggregator script alongside the per-demo `.cmd` files). See "Deliberate deviations from Build.Solution.md" at the top of `Build.md` for the full record. Do NOT add `LocateDemoDirectory`, demo-walking, or banner-printing back into `Program.cs`.

## Debugging in VS Code

The interpreter has a built-in DAP server. To debug a `.tlg` program in VS Code:

1. **One-time setup** — double-click `install-vscode-debugger.cmd` at the solution root (or run it from any shell). It is idempotent and runs five steps: prereq check (dotnet, node, npm, code), `dotnet publish TinyLanguage -c Release`, `npm install -g vsce` if missing, `vsce package`, `code --install-extension ... --force`. Manual fallback if you'd rather drive it yourself: `cd extensions\vscode && vsce package --allow-missing-repository && code --install-extension tinylanguage-debug-0.1.0.vsix --force`, plus a separate `dotnet publish TinyLanguage -c Release`. (`--allow-missing-repository` and the `LICENSE.txt` next to `package.json` together silence vsce's two y/N prompts so the install runs unattended.)
2. **Open a `.tlg` file**, click in the gutter to set a breakpoint, press `F5`. F5 works whether or not the workspace is the canonical solution root — the extension's `DebugConfigurationProvider` synthesises a launch config when no `.vscode/launch.json` is present, and its `DebugAdapterDescriptorFactory` walks up from the .tlg's directory to locate `TinyLanguage.exe` (also checking `${workspaceFolder}\TinyLanguage.DemoFiles\` and finally PATH). If the canonical workspace IS open, the "Debug current .tlg file" launch config the extension contributed is also picked up automatically.

Available in the debugger: gutter breakpoints, conditional breakpoints (right-click → Edit Breakpoint), logpoints, Step Over (F10) / Step In (F11) / Step Out (Shift+F11), Continue (F5), Pause, Restart, Variables panel, Watch panel, Call Stack panel, hover-to-evaluate, and the Debug Console (REPL) for ad-hoc expression evaluation in the current scope.

The architecture is three layers:

| Layer | Project | What it does |
|---|---|---|
| Engine | `TinyLanguage.Interpreter` | `IDebuggerHost` interface; per-statement callback; breakpoint match; step-mode state; `Evaluate(expr, scope)`. Zero overhead when no host attached. |
| DAP adapter | `TinyLanguage.DebugAdapter` | Speaks DAP over stdin/stdout. Worker thread runs interpreter; server thread handles VS Code requests; cooperative pause/resume via `BlockingCollection<DebuggerControl>`. |
| Editor shim (VS Code) | `extensions/vscode/` | ~30 lines of CommonJS. Registers debug type `tinylanguage`, points VS Code at `TinyLanguage.exe --dap`. |
| Editor shim (VS18) | `extensions/vs/` | Small `IAdapterLauncher` VSIX. Plugs into VS's stock Debug Adapter Host engine and spawns the same `TinyLanguage.exe --dap`. |

Future direction: a CLI REPL front-end (consuming the same engine) and a richer Turbo-Pascal-style integrated experience. Adding new debugger features should happen in the engine first; the DAP adapter and any future front-ends benefit equally.

> **Debugger override (additive — not in Build.Solution.md):** Build.Solution.md does not describe a debugger. The project owner explicitly added one. Future agents must not delete it on the assumption that "the spec doesn't mention it." See "Deliberate deviations from Build.Solution.md" item 4 in `Build.md`.

## Debugging in Visual Studio 2026

Same DAP server as VS Code, different editor shim. To debug a `.tlg` program in VS18:

1. **One-time setup** — double-click `install-vs-debugger.cmd` at the solution root (or run it from any shell). Idempotent. Five steps: prereq check (dotnet, vswhere, plus VS18's MSBuild and VSIXInstaller resolved via vswhere), `dotnet publish TinyLanguage -c Release`, build `extensions/vs/TinyLanguage.VsTools.csproj` against the VS SDK, install the resulting `TinyLanguage.VsTools.vsix` per-user via `VSIXInstaller.exe /quiet` (admin not required).
2. **Restart Visual Studio**, open the `.slnx` (or any folder containing `.tlg` files via *Open Folder*), open a `.tlg` file, click in the gutter to set a breakpoint, press `F5`.

The shim is a tiny `IAdapterLauncher` implementation in `extensions/vs/` that plugs into VS's stock Debug Adapter Host engine and spawns the same `TinyLanguage.exe --dap` the VS Code extension uses. Feature parity (gutter / conditional / logpoints / step / continue / pause / restart / variables / watch / call stack / hover / Immediate Window) is by construction. See section 7 of `TinyLanguage.wiki.md` for the user-facing details and the "Deliberate deviations from Build.Solution.md" item 5 in `Build.md` for the deliberate-addition record.

> **VS18 extension override (additive — not in Build.Solution.md):** parallel to the VS Code item above. Future agents must not delete `extensions/vs/` on the assumption that the spec or item 4 already covers it.

### Single-file exe
`TinyLanguage.csproj` is configured with `SelfContained=true`, `RuntimeIdentifier=win-x64`, and `PublishSingleFile=true`.

- **After `dotnet publish TinyLanguage -c Release`:** the single-file, self-contained exe (~36 MB, no runtime required) is copied to `TinyLanguage.DemoFiles/TinyLanguage.exe` via the `CopySingleFileExeToDemoFiles` MSBuild target (`AfterTargets="Publish"`, `Condition="'$(Configuration)' == 'Release'"`). This is the **only** exe-copy step. Plain `dotnet build` does NOT copy any exe to `DemoFiles/`.
- **Working with `.cmd` demos requires a prior publish.** The `.cmd` files in `TinyLanguage.DemoFiles/` invoke `%~dp0TinyLanguage.exe`, so they need the self-contained exe sitting beside them. After cloning or after a clean, run `dotnet publish TinyLanguage -c Release` once to populate it. After that, `dotnet build` is safe — the publish-time copy is left untouched.
- **For fast dev iteration without publishing:** pipe a `.tlg` file into `dotnet run --project TinyLanguage` (zero-args = stdin mode), or pass two args (`dotnet run --project TinyLanguage -- input.tlg output.txt`). The demo aggregator (`run-all-demos.cmd`) requires the published exe and so requires a prior `dotnet publish`.

> **History — why this is structured this way (do not "fix" it back):** an earlier version of `TinyLanguage.csproj` also had a `CopyExeToDemoFiles` target with `AfterTargets="Build"` that copied the framework-dependent build-output exe (~160 KB) to `DemoFiles/`. That looked harmless but was a silent foot-gun: the apphost there had no companion `TinyLanguage.dll` (the .dll stayed in `bin\Debug\..\`), so when `.cmd` demos ran, the apphost printed `"The application to execute does not exist: '...\TinyLanguage.dll'"` to stdout AND **exited with code 0**. The `.cmd`'s `if errorlevel 1` check did not fire, `type "%OUTPUT%"` silently failed on the missing output file, and the `.cmd` exited 0. Result: every `dotnet build` run AFTER a `dotnet publish` silently broke every `.cmd` demo, while exit codes still claimed success. Phase 5's "exit 0 + non-empty file + non-empty stdout" validation passed at the moment it ran (publish was the last operation) but any subsequent build broke things. The fix is structural: only copy on publish, and require users to publish at least once. Empirical MSBuild props on SDK 10.0.300-preview made it impossible to discriminate build-vs-publish from a `Condition` (`$(PublishDir)` is non-empty even on plain build with `PublishSingleFile=true`; `$(IsPublishing)` is empty in BOTH cases; `$(PublishSingleFile)` is true in both), so adding an outer condition could not save the build-time target — removing it was the only safe move.

## Architecture

Seven projects in the solution (`TinyLanguage.slnx`). `TinyLanguage` is listed first so Visual Studio treats it as the startup project (the `.slnx` format has no explicit startup-project field; VS defaults to the first executable project). A `.vscode/launch.json` provides the same default for VS Code.

| Project | Role |
|---|---|
| `TinyLanguage.Lexer` | Lexer, Parser, AST nodes, `AstPrettyPrinter` |
| `TinyLanguage.Interpreter` | Tree-walking interpreter, `Scope` chain, `IDebuggerHost` engine hooks |
| `TinyLanguage.DebugAdapter` | Debug Adapter Protocol server (used by the VS Code extension via `TinyLanguage.exe --dap`) |
| `TinyLanguage` | Console app (stdin / file / DAP modes) |
| `TinyLanguage.UnitTests` | MSTest unit tests (lexer, parser, debugger engine) |
| `TinyLanguage.IntegrationTests` | MSTest integration tests (interpreter end-to-end + DAP) |
| `TinyLanguage.DemoFiles` | 520+ `.tlg` demos (Tier A feature coverage 00001–00399, Tier B data structures 00400–00499, Tier C comprehensive DS catalogue 00500–00699) + matching `.cmd` runners + `run-all-demos.cmd` |

Plus `extensions/vscode/` and `extensions/vs/` at the solution root — small editor shims for VS Code (CommonJS extension) and Visual Studio 2026 (managed VSIX with an `IAdapterLauncher`). Both register the `tinylanguage` debug type and spawn the same `TinyLanguage.exe --dap`.

**Data flow:** source text → `Lexer` → `Token[]` → `Parser` → AST → `Interpreter` (visitor) → output/side effects. With `--dap`, an `IDebuggerHost` (the DAP adapter's `DapHost`) is attached to the interpreter and observes statement boundaries, function enter/exit, and unhandled exceptions.

### Key design decisions

- **Visitor pattern:** All AST nodes implement `Accept(INodeVisitor)`. The interpreter and pretty-printer both implement `INodeVisitor`.
- **Scope chain:** `Scope` is a linked list with parent delegation — not a flat dictionary. Functions create a fresh scope (no closure capture from call site).
- **No nullable, no implicit usings** throughout — explicit types everywhere.
- **Records over classes** except AST nodes (which need the visitor pattern and inheritance).
- **BCL only** — no NuGet packages.

### AST node hierarchy

Base class `AstNode`. Roughly 60+ node types covering statements, expressions, declarations, control flow, and pattern matching. Each node is in its own file (one type per file rule).

## Coding Standards

From `Build.Solution.md` — follow exactly:

- **Naming:** `UpperCamelCase` for types/methods/members; `lowerCamelCase` for locals; `I`-prefix for interfaces; exceptions use same case as variable name; tuple types suffixed `Tuple`.
- **One file per type** (class, interface, enum, record).
- **`readonly`** on all variables whenever possible.
- **Streams over string loads** for file I/O.
- **Test method naming:** `Subject_Action_ExpectedOutcome` (no "Test" in the name). Test class names end in `UnitTests` or `IntegrationTests`.
- **Test framework:** MSTest only (no XUnit, no NUnit).
- **MSTest 4.x:** The pinned SDK's `dotnet new mstest` template uses the **MSTest 4.0.2 meta-package** (it bundles the Microsoft.Testing.Platform runner — no separate `Microsoft.NET.Test.Sdk`) and emits a `MSTestSettings.cs` with an assembly-level `[Parallelize]` attribute. Omitting it trips analyzer **MSTEST0001** (warning → fails the 0-warning gate). Set it to `[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.MethodLevel)]` (fully qualified; no implicit usings) to also avoid flaky console-redirection races across test projects. MSTest 4.x analyzers (**MSTEST0037**) reject several classic asserts — use the 4.x forms: `Assert.HasCount`, `Assert.Contains`/`Assert.DoesNotContain`, `Assert.IsGreaterThan`/`Assert.IsGreaterThanOrEqualTo`, `Assert.Throws<T>`.
- **Test output:** Every test must print its input and result via the `TestLog` helper (NOT raw `Console.WriteLine`). Each test project (`TinyLanguage.UnitTests`, `TinyLanguage.IntegrationTests`) owns its own `TestLog.cs` with `Input(label, content)`, `Section(label, content)`, `Result(content)` static methods. The helper prints `--- Input: <label> ---` / `--- Result ---` / `---` framing with real-newline-indented bodies, so multi-line content (program source, multi-line stdout, token lists) stays readable in the test runner log.

  ```csharp
  TestLog.Input(demoFileName, source);
  TestLog.Result(actual);   // multi-line content is fine — uses real \n
  ```

  Do NOT introduce a `Visible(s)` helper that escapes `\n`→`\\n` — it collapses multi-line stdout onto one unreadable line. See deviation D8 in `Build.md` and `Build.Plan.md`. The exact `TestLog.cs` source lives in the "Test output formatting — non-negotiable" preamble of `Build.md`.

## Disambiguation Rules

The parser has 23 documented disambiguation rules (see `Build.Solution.md` §1.5). The critical ones:

- `(expr)` vs cast: look-ahead determines cast vs. grouped expression.
- Lambda detection: `(params) =>` vs. parenthesized expression.
- `if` as expression vs. statement: determined by context (right-hand side of assignment/return/call arg → expression; otherwise → statement).
- `do { ... } while` vs `do` block: `while` keyword after `}` closes a do-while; otherwise it's a do-block.

When implementing parser rules, consult the BNF in `Build.Solution.md` §1.4 and the notes in §1.5 before writing any code.

## Parser leniencies beyond the BNF (D10–D21)

In addition to D1–D9 in `Build.md`, the parser deliberately relaxes the BNF in ten places (D10–D21) to accept the demo corpus's idiomatic style. Future regenerations MUST preserve all ten — reverting any one will produce widespread demo failures:

- **D10** Newlines act as implicit `;` separators between statements (Note 3 relaxation). `ParseStatementList` accepts a strictly-later upcoming-token line as separator when no explicit `;` was consumed. Corollary: the postfix parser must NOT consume a `[` (index) or `(` (call) that BEGINS on a strictly-later source line as part of the previous statement's expression — otherwise a `print "x"` followed by a bracket-led next statement (e.g. a `[a, b] => ...` match case) is wrongly swallowed. A leading `.` member access may still continue across lines (`.` cannot start a statement). (Discovered fixing 00306.)
- **D11** Trailing `;` before block-enders (`end`, `else`, `}`, EOF, …) and leading `;` before a statement are silently tolerated (Note 21 relaxation). Multiple consecutive `;`s = zero-or-more empty statements.
- **D12** `else if X then ... end` chains supported — `ParseIfStatement` recurses into a nested `if_stmt` after `else if`; the inner `end` closes the whole chain. FizzBuzz parses with one `end`. The fold fires ONLY when the `if` is on the SAME source line as `else`; an `if` on a later line is an ordinary nested if owning its own `end` (the naive "any if after else" rule orphans the outer `end` and breaks nested-if-in-else demos).
- **D13** Member assignment and postfix-LHS assignment/call: new AST nodes `MemberAssignStmtNode`, `PostfixAssignStmtNode`, `PostfixCallStmtNode`. `ParseStatement` dispatches both `Identifier` and `This` to a shared `ParsePostfixLedAssignOrCall` helper that parses a full postfix expression then specialises on `:=` or `(args)`. Supports `this.X := v`, `this.Items[i] := v`, `obj.a.b.c := v`, `this.Nodes[i].AddNeighbor(...)`.
- **D14** `var x := 1` (type-inferred `var`) — BNF requires type annotation; relaxed to optional like `let`.
- **D15** `new Foo.Bar(args)` — dotted type names accepted in `new`. The final segment resolves against the class table.
- **D16** Built-in conversion call-syntax: `int(x)`, `float(x)`, `bool(x)`, `str(x)`. int/str/bool/float are spec built-in functions but lex as type-name keyword tokens; `ParsePrimary` treats a type-name keyword immediately followed by `(` as a conversion call (FunctionCallNode → same conversion as the `(int)x` cast). Casts, `: int` annotations, and `is`/`as int` are unaffected.
- **D17** Lambda body by first token: a lambda body is a single expression if its first token starts an expression, else a statement BLOCK (optionally closed by `end`), decided by the first token (NOT by scanning for a matching `end`, which latches onto the enclosing function's `end`). Enables `function(n) print n`. The block-body decision uses the FULL statement-starting keyword set — return/print/if/while/for/foreach/let/var/const/throw/try/switch/match/break/continue/do — not a subset; in particular a body beginning with `return` is a statement block (the original implementation omitted `return`).
- **D20** `export <definition>` — the strict BNF is `export <id>`, but the parser also accepts `export function|class|static|let|var|const <definition>`: it parses the inner definition (which lands in module/global scope, since modules promote exports to global with no qualified `M.f` access) and records the export marker (a runtime no-op). The bare `export <id>` form still works. Required by module demos 00291–00295. Lives in `ParseExportStatement`.
- **D21** `static` instance-style fields (`static let X := 0`, `static var Y := 0`) — extends D19 (class `const` is static): the class-member parser accepts `static` before `let`/`var`/`const` field declarations and routes them to the class's static members, reachable and assignable as `ClassName.Field`. `FieldDeclareNode` carries `IsStatic`. Implementation Note 16 ("static modifies a field"). Required by 00216, 00237.

Full details (BNF citation + implementation pointer + reasoning) in `Build.md` "Deliberate deviations" §6 and the deviations table in `Build.Plan.md` §2.

## Language behaviours demo authors rely on (learned from the corpus)

- **Built-in conversions work as BOTH casts and calls:** `(int)x` and `int(x)` are equivalent; same for float/bool/str. `len`/`str` are ordinary calls too.
- **`as` is a CHECKED type assertion, not a converter.** `x as int` throws when the runtime type does not match (e.g. `3.7 as int` fails). To CONVERT use `int(3.7)` or `(int)3.7`. Demo authors must not write `x as T` expecting coercion.
- **No map/object literal exists.** There is no `{ key: value }` literal — maps and objects are modeled as classes (e.g. a Dictionary class over parallel arrays). `let m := { ... }` does not parse. (`map` is also a reserved type-name keyword, so it cannot be a variable name.)
- **`match` and `switch` use BRACE form.** `match X { pattern => body; ... }` — no `case`/`end` inside a match; `_` is the wildcard, `when cond` is a guard, `a | b | c =>` is alternation. `switch X { case v: ...; default: ... }`.
- **Lambda EXPRESSION bodies are bare** — `function(n) n * n` (NO `=>` arrow). A statement-BLOCK body is `function(n) ... end` and is chosen by the first body token (see D17).
- **`+` concatenates arrays** (`arr := arr + [x]`) as well as numbers/strings — this is the corpus's standard list-append idiom (no list-grow builtin exists).
- **Class `const` fields are static** — reachable as `ClassName.CONST`.
- **Modules have NO qualified access** — `module M { export function f() }` promotes `f` to global scope; call it `f(...)`, never `M.f(...)`.
- **`try` requires `catch`** — no catch-less `try/finally`.
- **Out of the BNF (avoid / mark `# NOT IMPLEMENTABLE`):** scientific-notation float literals (`6.6e-34`), field-level annotations (`@Foo` on a class field).
- **Reserved words can't be parameter/variable names** — includes `step`, `to`, `in`, `do`, `match`, the type-name keywords, etc.

## Lexer trap fixed in-place: float `PeekIsDigit()`

The `PeekIsDigit()` helper inside `Lexer.cs` decides whether `.` starts a float fractional part. The natural-looking implementation that checks `CurrentChar` is WRONG — at the call site `CurrentChar` IS the `.`, never a digit, so `1.5` lexes as Integer Dot Integer instead of Float. Use `Source.Peek()`:

```csharp
private bool PeekIsDigit()
{
    int nextChar = Source.Peek();
    return nextChar != -1 && char.IsDigit((char)nextChar);
}
```

Phase 1B agents must implement this correctly from the start.

---

## Coding Guidelines

# Karpathy Guidelines

Behavioral guidelines to reduce common LLM coding mistakes, derived from [Andrej Karpathy's observations](https://x.com/karpathy/status/2015883857489522876) on LLM coding pitfalls.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.
