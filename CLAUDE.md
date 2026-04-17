# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TinyLanguage is a complete .NET 10.0 implementation of a small programming language (`.tlg` files) with Lexer, Parser, AST, and tree-walking Interpreter. The specification is locked in `Build.Solution.md` (read-only — never modify it). `Build.md` describes the multi-phase Claude Code orchestration plan. `Plan.md` has the dependency graph and work unit breakdown.

## Build & Test Commands

```bash
dotnet build                                    # Must succeed with 0 errors, 0 warnings
dotnet test --verbosity normal                  # Must report Failed: 0
dotnet run --project TinyLanguage               # Demo mode: runs all demo files; exit 0, prints "All demos completed successfully."
dotnet run --project TinyLanguage -- input.tlg output.txt  # File-processor mode
dotnet publish TinyLanguage -c Release          # Produces single-file self-contained TinyLanguage.exe
```

Run a single test class:
```bash
dotnet test --filter "FullyQualifiedName~LexerUnitTests"
```

**Acceptance criteria:** `dotnet build` → 0 errors/warnings; `dotnet test` → 0 failures; `dotnet run` demo mode → exit 0.

### Single-file exe
`TinyLanguage.csproj` is configured with `SelfContained=true`, `RuntimeIdentifier=win-x64`, and `PublishSingleFile=true`.

- **After every `dotnet build`:** the build-output exe is automatically copied to `TinyLanguage.DemoFiles/` via the `CopyExeToDemoFiles` MSBuild target (conditioned on `'$(PublishDir)' == ''` so it fires on builds but not during publish).
- **After `dotnet publish TinyLanguage -c Release`:** the single-file, self-contained exe (~36 MB, no runtime required) is copied to `TinyLanguage.DemoFiles/` via the `CopySingleFileExeToDemoFiles` MSBuild target.

> **Important:** The `CopyExeToDemoFiles` condition must be `'$(PublishDir)' == ''`, **not** `'$(PublishSingleFile)' != 'true'`. Because `PublishSingleFile=true` is declared in the PropertyGroup it is always true at both build and publish time, so the latter condition never fires and the exe is never copied on a plain `dotnet build`.

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
