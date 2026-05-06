# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TinyLanguage is a complete .NET 10.0 implementation of a small programming language (`.tlg` files) with Lexer, Parser, AST, and tree-walking Interpreter. The specification is locked in `Build.Solution.md` (read-only — never modify it). `Build.md` describes the multi-phase Claude Code orchestration plan. `Plan.md` has the dependency graph and work unit breakdown.

## Build & Test Commands

```bash
dotnet build                                    # Must succeed with 0 errors, 0 warnings
dotnet test --verbosity normal                  # Must report Failed: 0
dotnet publish TinyLanguage -c Release          # Produces single-file self-contained TinyLanguage.exe (must run before run-all-demos)
TinyLanguage.DemoFiles\run-all-demos.cmd        # Walks every .tlg, prints "All demos completed successfully.", exit 0
echo print "hi" | dotnet run --project TinyLanguage   # stdin mode (no args): read source from stdin, write to stdout
dotnet run --project TinyLanguage -- input.tlg output.txt  # File-processor mode
```

Run a single test class:
```bash
dotnet test --filter "FullyQualifiedName~LexerUnitTests"
```

**Acceptance criteria:** `dotnet build` → 0 errors/warnings; `dotnet test` → 0 failures; `TinyLanguage.DemoFiles\run-all-demos.cmd` → exit 0.

> **Console exe override (deviation from Build.Solution.md):** Build.Solution.md describes a "Demo mode (no arguments)" baked into the exe. The project owner removed it — `TinyLanguage.exe` is now interpreter-only. Two modes: zero args = read source from stdin and write to stdout; two args = file-processor mode. The demo-walking job moved to `TinyLanguage.DemoFiles\run-all-demos.cmd` (an aggregator script alongside the per-demo `.cmd` files). See "Deliberate deviations from Build.Solution.md" at the top of `Build.md` for the full record. Do NOT add `LocateDemoDirectory`, demo-walking, or banner-printing back into `Program.cs`.

### Single-file exe
`TinyLanguage.csproj` is configured with `SelfContained=true`, `RuntimeIdentifier=win-x64`, and `PublishSingleFile=true`.

- **After `dotnet publish TinyLanguage -c Release`:** the single-file, self-contained exe (~36 MB, no runtime required) is copied to `TinyLanguage.DemoFiles/TinyLanguage.exe` via the `CopySingleFileExeToDemoFiles` MSBuild target (`AfterTargets="Publish"`, `Condition="'$(Configuration)' == 'Release'"`). This is the **only** exe-copy step. Plain `dotnet build` does NOT copy any exe to `DemoFiles/`.
- **Working with `.cmd` demos requires a prior publish.** The `.cmd` files in `TinyLanguage.DemoFiles/` invoke `%~dp0TinyLanguage.exe`, so they need the self-contained exe sitting beside them. After cloning or after a clean, run `dotnet publish TinyLanguage -c Release` once to populate it. After that, `dotnet build` is safe — the publish-time copy is left untouched.
- **For fast dev iteration without publishing:** pipe a `.tlg` file into `dotnet run --project TinyLanguage` (zero-args = stdin mode), or pass two args (`dotnet run --project TinyLanguage -- input.tlg output.txt`). The demo aggregator (`run-all-demos.cmd`) requires the published exe and so requires a prior `dotnet publish`.

> **History — why this is structured this way (do not "fix" it back):** an earlier version of `TinyLanguage.csproj` also had a `CopyExeToDemoFiles` target with `AfterTargets="Build"` that copied the framework-dependent build-output exe (~160 KB) to `DemoFiles/`. That looked harmless but was a silent foot-gun: the apphost there had no companion `TinyLanguage.dll` (the .dll stayed in `bin\Debug\..\`), so when `.cmd` demos ran, the apphost printed `"The application to execute does not exist: '...\TinyLanguage.dll'"` to stdout AND **exited with code 0**. The `.cmd`'s `if errorlevel 1` check did not fire, `type "%OUTPUT%"` silently failed on the missing output file, and the `.cmd` exited 0. Result: every `dotnet build` run AFTER a `dotnet publish` silently broke every `.cmd` demo, while exit codes still claimed success. Phase 5's "exit 0 + non-empty file + non-empty stdout" validation passed at the moment it ran (publish was the last operation) but any subsequent build broke things. The fix is structural: only copy on publish, and require users to publish at least once. Empirical MSBuild props on SDK 10.0.300-preview made it impossible to discriminate build-vs-publish from a `Condition` (`$(PublishDir)` is non-empty even on plain build with `PublishSingleFile=true`; `$(IsPublishing)` is empty in BOTH cases; `$(PublishSingleFile)` is true in both), so adding an outer condition could not save the build-time target — removing it was the only safe move.

## Architecture

Six projects in the solution (`TinyLanguage.slnx`). `TinyLanguage` is listed first so Visual Studio treats it as the startup project (the `.slnx` format has no explicit startup-project field; VS defaults to the first executable project). A `.vscode/launch.json` provides the same default for VS Code.

| Project | Role |
|---|---|
| `TinyLanguage.Lexer` | Lexer, Parser, AST nodes, `AstPrettyPrinter` |
| `TinyLanguage.Interpreter` | Tree-walking interpreter, `Scope` chain |
| `TinyLanguage` | Console app (demo mode + file-processor mode) |
| `TinyLanguage.UnitTests` | MSTest unit tests (lexer + parser) |
| `TinyLanguage.IntegrationTests` | MSTest integration tests (interpreter + end-to-end) |

**Data flow:** source text → `Lexer` → `Token[]` → `Parser` → AST → `Interpreter` (visitor) → output/side effects.

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
- **Test output:** Every test must print its input and result using `Console.WriteLine` so the test log shows what was exercised. Example:
  ```csharp
  Console.WriteLine($"Input: {source}");
  Console.WriteLine($"Result: {actual}");
  ```

## Disambiguation Rules

The parser has 23 documented disambiguation rules (see `Build.Solution.md` §1.5). The critical ones:

- `(expr)` vs cast: look-ahead determines cast vs. grouped expression.
- Lambda detection: `(params) =>` vs. parenthesized expression.
- `if` as expression vs. statement: determined by context (right-hand side of assignment/return/call arg → expression; otherwise → statement).
- `do { ... } while` vs `do` block: `while` keyword after `}` closes a do-while; otherwise it's a do-block.

When implementing parser rules, consult the BNF in `Build.Solution.md` §1.4 and the notes in §1.5 before writing any code.

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
