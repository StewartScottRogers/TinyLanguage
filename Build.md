# TinyLanguage — Claude Code Orchestration Plan

This file drives a **Claude Code multi-agent build** of the TinyLanguage solution.
The canonical specification lives in `Build.Solution.md` — treat it as READ-ONLY.

> **Output location:** The generated solution folder (`TinyLanguage.YYYY.MM.DD.HH/`) is
> created as a **sibling of this repo**, not inside it. For example, if this repo is at
> `Z:\repos\TinyLanguage\`, the output goes to `Z:\repos\TinyLanguage.2026.04.16.19\`.

---

## How to Run This Plan

Open Claude Code in this directory and paste:

```
Read Build.md and execute the full orchestration plan using parallel agents.
```

Claude Code will decompose `Build.Solution.md` into the work units below, spin up
specialised sub-agents (some in parallel git worktrees), track progress with Tasks,
and assemble the final solution.

---

## Phase 0 — Architect Analysis  *(sequential, blocks everything)*

**Agent:** `Plan` subagent
**Model:** `claude-opus-4-6`
**Prompt:**

```
Read Build.Solution.md in full. Produce:
1. A hierarchical outline of every section.
2. A Work Unit list (title, inputs, outputs, dependencies, assignee role).
3. A dependency graph identifying the critical path.
4. The final project directory layout under TinyLanguage.YYYY.MM.DD.HH/.
Return structured markdown — do not generate any code.
```

**Output:** `Plan.md` in the repo root (working document, not committed).
**Blocks:** Phases 1–4.

---

## Phase 1 — Parallel Foundation  *(all four agents launch simultaneously)*

Each agent runs in an **isolated git worktree** (`isolation: "worktree"`).

### 1A — Solution Scaffold
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md sections: ".NET Standards", "Application Description",
"File System Structure".

Create the full solution skeleton ONE LEVEL ABOVE the repo root (i.e. a sibling
of the TinyLanguage repo directory, not inside it):
- ../TinyLanguage.YYYY.MM.DD.HH/          ← sibling of Z:\repos\TinyLanguage\
  - TinyLanguage.slnx (or .sln for SDK-style)
  - TinyLanguage.Lexer/TinyLanguage.Lexer.csproj  (net10.0 classlib)
  - TinyLanguage.Interpreter/TinyLanguage.Interpreter.csproj (net10.0 classlib)
  - TinyLanguage.UnitTests/TinyLanguage.UnitTests.csproj (MSTest)
  - TinyLanguage.IntegrationTests/TinyLanguage.IntegrationTests.csproj (MSTest)
  - TinyLanguage/TinyLanguage.csproj (net10.0 console, self-contained single-file win-x64 exe)
  - TinyLanguage.DemoFiles/TinyLanguage.DemoFiles.csproj  (SDK-style, content-only, no output assembly)

Apply every coding-style rule from the spec. Do not generate any C# source yet —
scaffold files, project references, and Directory.Build.props only.

TinyLanguage/TinyLanguage.csproj must include:
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <PublishSingleFile>true</PublishSingleFile>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  <!-- copy exe to DemoFiles after every build -->
  <Target Name="CopyExeToDemoFiles" AfterTargets="Build">
    <Copy SourceFiles="$(OutputPath)TinyLanguage.exe"
          DestinationFolder="$(MSBuildProjectDirectory)\..\TinyLanguage.DemoFiles\"
          SkipUnchangedFiles="true" />
  </Target>
  <!-- copy single-file exe to DemoFiles after publish -->
  <Target Name="CopySingleFileExeToDemoFiles" AfterTargets="Publish">
    <Copy SourceFiles="$(PublishDir)TinyLanguage.exe"
          DestinationFolder="$(MSBuildProjectDirectory)\..\TinyLanguage.DemoFiles\"
          SkipUnchangedFiles="true" />
  </Target>

Run: dotnet build
Accept only: 0 errors, 0 warnings.
```

### 1B — Token & Lexer
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md sections: "BNF Grammar", "Implementation Notes" (all 23 rules),
".NET Standards", "Coding Style".

Implement inside TinyLanguage.Lexer/:
- TokenType.cs   — every token type derived from the BNF
- Token.cs       — immutable record (Value, Type, Line)
- Lexer.cs       — streaming lexer (prefer streams over string loads)
- LexerException.cs

Apply every naming convention from the spec. Use only BCL. No nullable, no implicit
usings. One type per file.

Run: dotnet build TinyLanguage.Lexer
Accept only: 0 errors, 0 warnings.
```

### 1C — AST Node Types
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md sections: "BNF Grammar" (all statement/expression productions),
"Implementation Notes", "Coding Style".

Implement inside TinyLanguage.Lexer/ (AST lives here per spec):
- One file per AST node class (Classes not Records — they use the Visitor pattern).
- INodeVisitor.cs interface with Visit overloads for every node type.
- AstPrettyPrinter.cs implementing INodeVisitor.

Do NOT implement the Parser yet. Nodes only.

Run: dotnet build TinyLanguage.Lexer
Accept only: 0 errors, 0 warnings.
```

### 1D — Demo File Stubs
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md sections: "Tiny Language Demonstration Suite",
"TinyLanguage Syntax", the full BNF Grammar, and all 23 Implementation Notes.

Create 300+ .tlg demo files in TinyLanguage.DemoFiles/, zero-padded numeric prefix
(00001.fizzbuzz.tlg … etc.), covering every grammar feature exhaustively.
Create a matching .cmd runner for each demo file.

Each .cmd file must work correctly regardless of the directory it is run from:
- Use %~dp0TinyLanguage.exe to locate the exe (same folder as the .cmd)
- Use %~dp0<filename>.tlg to locate the input file (same folder as the .cmd)
- Output path defaults to the CURRENT WORKING DIRECTORY (not %~dp0)
- Do NOT cd or pushd into the .cmd file's own directory

Correct template:
  @echo off
  if "%2"=="" (
      %~dp0TinyLanguage.exe %~dp000001.fizzbuzz.tlg output.txt
  ) else (
      %~dp0TinyLanguage.exe %~dp000001.fizzbuzz.tlg %2
  )

Do NOT run them yet — the interpreter is not built.
```

---

## Phase 2 — Parser  *(starts after 1A + 1B + 1C complete)*

**Agent:** `general-purpose`
**Isolation:** worktree
**Model:** `claude-opus-4-6`  *(complex recursive-descent work)*
**Prompt:**
```
Merge worktree outputs from Phase 1A, 1B, 1C into a single working branch.

Read Build.Solution.md: "BNF Grammar" (full), all 23 Implementation Notes.

Implement TinyLanguage.Lexer/Parser.cs:
- Recursive-descent parser matching every BNF production exactly.
- ParseStatementList accepts optional stop-token sets (notes 3, 12).
- Disambiguation rules for cast, lambda, conditional, generic type (notes 12–14, 20).
- Bare return support (note 4).
- Correct operator precedence table (note 11).
- ParserException.cs with source line number.

Run: dotnet build TinyLanguage.Lexer
Accept only: 0 errors, 0 warnings.
```

---

## Phase 3 — Parallel: Interpreter + Unit Tests  *(starts after Phase 2)*

### 3A — Interpreter
**Agent:** `general-purpose`
**Isolation:** worktree
**Model:** `claude-opus-4-6`
**Prompt:**
```
Read Build.Solution.md: "Class Library (TinyLanguage.Interpreter.dll)",
all 23 Implementation Notes, BNF Grammar.

Implement TinyLanguage.Interpreter/:
- IInterpreter.cs
- Interpreter.cs   — tree-walking visitor implementing INodeVisitor
- Scope chain with rules from notes 8 and 9.
- All operator semantics from note 10.
- Foreach-over-string (note 6), ArrayElementAssign mutation (note 7).
- InterpreterException.cs with source line.

Run: dotnet build TinyLanguage.Interpreter
Accept only: 0 errors, 0 warnings.
```

### 3B — Unit Tests (Lexer + Parser)
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md: "Unit Testing Strategy & Requirements",
"BNF Grammar Verification Strategy", "Test Validation Protocol",
"Coding Style", "Implementation Notes".

Implement TinyLanguage.UnitTests/:
- LexerUnitTests.cs  — every token type, boundary conditions, error cases.
- ParserUnitTests.cs — every BNF production, all 23 disambiguation rules.

Use MSTest only. Test class suffix: UnitTests. No "Test" in method names.
Name pattern: Subject_Action_ExpectedOutcome.

Every test must print its input and result via Console.WriteLine so the test
log shows what was exercised, e.g.:
  Console.WriteLine($"Input: {source}");
  Console.WriteLine($"Result: {actual}");

Run: dotnet test TinyLanguage.UnitTests
Accept only: Failed: 0.
```

---

## Phase 4 — Integration Tests + Console App  *(starts after Phase 3A + 3B)*

### 4A — Integration Tests
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md: "IntegrationTests", "Test Validation Protocol",
"Unit Testing Requirements".

Implement TinyLanguage.IntegrationTests/:
- InterpreterIntegrationTests.cs — end-to-end programs for every language feature.
- Use the .tlg demo files from Phase 1D as test inputs where appropriate.

Every test must print its input and result via Console.WriteLine so the test
log shows what was exercised, e.g.:
  Console.WriteLine($"Input: {source}");
  Console.WriteLine($"Result: {actual}");

Run: dotnet test TinyLanguage.IntegrationTests
Accept only: Failed: 0.
```

### 4B — Console Application
**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Read Build.Solution.md: "Console Application (TinyLanguage.exe)".

Implement TinyLanguage/Program.cs:
- File-processor mode (two args: input path, output path).
- Piped I/O support (stdin / stdout).
- Demo mode (no args): run all .tlg files from TinyLanguage.DemoFiles,
  print headers + output + separators, exit 0 printing
  "All demos completed successfully." or exit 1 on any failure.
- Errors → stderr, exit code 1. Success → exit code 0.

Run: dotnet run --project TinyLanguage
Accept only: "All demos completed successfully." printed, exit code 0.

Then publish the single-file exe and verify it was copied to DemoFiles:
  dotnet publish TinyLanguage -c Release
  (TinyLanguage.exe must appear in TinyLanguage.DemoFiles/ — self-contained, ~36 MB, no runtime required)
```

---

## Phase 5 — Final Validation & Assembly  *(sequential, all branches merged)*

**Agent:** `general-purpose`
**Prompt:**
```
Merge all worktree branches. From the solution root run the Test Validation Protocol:

  dotnet build
  dotnet run --project TinyLanguage
  dotnet test --verbosity normal
  dotnet publish TinyLanguage -c Release

Acceptance criteria (from Build.Solution.md "Acceptance Criteria"):
  - dotnet build   → 0 errors, 0 warnings; TinyLanguage.exe copied to TinyLanguage.DemoFiles/
  - dotnet run     → "All demos completed successfully.", exit 0
  - dotnet test    → Failed: 0
  - dotnet publish → single-file self-contained TinyLanguage.exe (~36 MB) in TinyLanguage.DemoFiles/

If any criterion fails, apply the Fix-and-Retry Loop from the spec:
  1. Read full error output.
  2. Identify root cause.
  3. Edit only minimal code needed.
  4. Re-run dotnet build → dotnet test.
  5. Repeat until Failed: 0.

Do not refactor unrelated code. Do not alter Build.Solution.md.
```

---

## Agent Tool Invocation Pattern

When Claude Code executes this plan, each phase maps to an `Agent` tool call:

```jsonc
// Phase 1 — four calls in a SINGLE message (parallel)
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1A
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1B
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1C
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1D

// Phase 2 — single call, awaited (sequential)
Agent({ subagent_type: "general-purpose", isolation: "worktree",
        model: "opus", prompt: "..." })

// Phase 3 — two calls in a SINGLE message (parallel)
Agent({ subagent_type: "general-purpose", isolation: "worktree",
        model: "opus", prompt: "..." })  // 3A
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 3B
```

Progress for each work unit is tracked with `TaskCreate` / `TaskUpdate` so the
orchestrating session always has a live view of build state.

---

## Model Selection Rationale

| Phase | Model | Reason |
|-------|-------|--------|
| 0 — Architect Analysis | `claude-opus-4-6` | Deep spec comprehension, full outline |
| 1A–1D — Scaffolding | `claude-sonnet-4-6` | Mechanical, file-heavy, fast |
| 2 — Parser | `claude-opus-4-6` | Complex recursive-descent + 23 disambiguation rules |
| 3A — Interpreter | `claude-opus-4-6` | Scope chain, operator semantics, tree-walking |
| 3B — Unit Tests | `claude-sonnet-4-6` | Systematic coverage, lower complexity |
| 4A–4B — Integration + CLI | `claude-sonnet-4-6` | Straightforward integration work |
| 5 — Validation | `claude-sonnet-4-6` | Fix-and-retry loop, targeted edits |

---

## Constraints (from Build.Solution.md — never override)

- `Build.Solution.md` is READ-ONLY. No agent may modify it.
- No external NuGet packages. BCL only.
- No nullable, no implicit usings.
- MSTest only (no XUnit, no NUnit).
- One type per file.
- All acceptance criteria must be green before the plan is complete.
