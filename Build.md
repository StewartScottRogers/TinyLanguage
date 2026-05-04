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
  - TinyLanguage.slnx
  - TinyLanguage.Lexer/TinyLanguage.Lexer.csproj  (net10.0 classlib)
  - TinyLanguage.Interpreter/TinyLanguage.Interpreter.csproj (net10.0 classlib)
  - TinyLanguage.UnitTests/TinyLanguage.UnitTests.csproj (MSTest)
  - TinyLanguage.IntegrationTests/TinyLanguage.IntegrationTests.csproj (MSTest)
  - TinyLanguage/TinyLanguage.csproj (net10.0 console, self-contained single-file win-x64 exe)
  - TinyLanguage.DemoFiles/TinyLanguage.DemoFiles.csproj  (SDK-style, content-only, no output assembly)
  - .vscode/launch.json  (VS Code debugger — targets TinyLanguage console project)

Apply every coding-style rule from the spec. Do not generate any C# source yet —
scaffold files, project references, Directory.Build.props, and .vscode/launch.json only.

TinyLanguage.slnx must list TinyLanguage/TinyLanguage.csproj FIRST so that Visual
Studio recognises it as the default startup project (the .slnx format has no explicit
startup-project field; VS defaults to the first executable project in the list):
  <Solution>
    <Project Path="TinyLanguage\TinyLanguage.csproj" />
    <Project Path="TinyLanguage.Lexer\TinyLanguage.Lexer.csproj" />
    <Project Path="TinyLanguage.Interpreter\TinyLanguage.Interpreter.csproj" />
    <Project Path="TinyLanguage.UnitTests\TinyLanguage.UnitTests.csproj" />
    <Project Path="TinyLanguage.IntegrationTests\TinyLanguage.IntegrationTests.csproj" />
    <Project Path="TinyLanguage.DemoFiles\TinyLanguage.DemoFiles.csproj" />
  </Solution>

.vscode/launch.json must target the TinyLanguage console project so VS Code F5
launches it directly:
  {
    "version": "0.2.0",
    "configurations": [
      {
        "name": "TinyLanguage (demo mode)",
        "type": "coreclr",
        "request": "launch",
        "preLaunchTask": "build",
        "program": "${workspaceFolder}/TinyLanguage/bin/Debug/net10.0/win-x64/TinyLanguage.exe",
        "args": [],
        "cwd": "${workspaceFolder}",
        "console": "internalConsole",
        "stopAtEntry": false
      },
      {
        "name": "TinyLanguage (file mode)",
        "type": "coreclr",
        "request": "launch",
        "preLaunchTask": "build",
        "program": "${workspaceFolder}/TinyLanguage/bin/Debug/net10.0/win-x64/TinyLanguage.exe",
        "args": ["${input:inputFile}", "${input:outputFile}"],
        "cwd": "${workspaceFolder}",
        "console": "internalConsole",
        "stopAtEntry": false
      }
    ],
    "inputs": [
      { "id": "inputFile",  "type": "promptString", "description": "Input .tlg file"  },
      { "id": "outputFile", "type": "promptString", "description": "Output file path" }
    ]
  }

TinyLanguage/TinyLanguage.csproj must include:
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <PublishSingleFile>true</PublishSingleFile>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  <!-- ONLY a publish-time copy. Do NOT add an AfterTargets="Build" copy target — -->
  <!-- prior builds shipped one and it silently broke every .cmd demo every time  -->
  <!-- a user ran dotnet build after dotnet publish. Background:                  -->
  <!--   * Build-output exe (~160 KB) is the framework-dependent apphost. It needs -->
  <!--     TinyLanguage.dll beside it. The .dll lives in bin\Debug\..\, NOT in    -->
  <!--     DemoFiles\, so the apphost there cannot start.                          -->
  <!--   * The apphost does NOT exit non-zero on missing .dll — it prints         -->
  <!--     "The application to execute does not exist" to stdout and exits 0.     -->
  <!--   * .cmd files use `if errorlevel 1` to detect failure. errorlevel 0       -->
  <!--     passes the check. `type "%OUTPUT%"` then silently fails on the missing -->
  <!--     output file, the .cmd exits 0, and the user sees the apphost error    -->
  <!--     flash by but no real output. Phase 5 validation that only checks       -->
  <!--     "exit 0 + non-empty file + non-empty stdout" cannot detect this state. -->
  <!-- Empirical (SDK 10.0.300-preview): you cannot discriminate build vs publish  -->
  <!-- via Condition. With PublishSingleFile=true in PropertyGroup, $(PublishDir)  -->
  <!-- is non-empty even on plain dotnet build; $(IsPublishing) is empty in BOTH;  -->
  <!-- $(PublishSingleFile) is true in BOTH. So an outer Condition on a Build-time -->
  <!-- target will either always fire or never fire — never selectively. The only -->
  <!-- safe approach is to drop the build-time copy entirely.                      -->
  <!-- Trade-off: users who want to run .cmd demos must run dotnet publish first.  -->
  <!-- For fast dev iteration without publishing, use `dotnet run` on the          -->
  <!-- TinyLanguage project (demo mode runs the .tlg files in-process). NOTE: XML  -->
  <!-- comments forbid the `--` digraph, so write `dotnet run` rather than the     -->
  <!-- equivalent flag spelling that uses two dashes — paste this comment block    -->
  <!-- verbatim into the csproj or MSBuild rejects it with MSB4025.                -->
  <Target Name="CopySingleFileExeToDemoFiles" AfterTargets="Publish"
          Condition="'$(Configuration)' == 'Release'">
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

TokenType MUST include EVERY keyword referenced anywhere in the BNF or in the demo
files (TinyLanguage.DemoFiles/*.tlg). It is easy to silently miss a keyword by
reading the BNF productions only — production names like <class_def> may not enumerate
every contextual keyword. The non-negotiable list (keep this complete or you will
break the parser/interpreter downstream):

  Declarations:  let, var, const, enum, function, return, class, extends, implements,
                 Constructor (capital C — exact spelling per BNF), new, static,
                 module, import, export, as
  Control flow:  if, then, else, end, while, do, for, to, step, foreach, in, break,
                 continue, switch, case, default
  I/O:           print, input
  Exceptions:    try, catch, finally, throw
  Patterns:      match, when
  Logical:       and, or, not, is
  Self-ref:      this  ← REQUIRED. Class methods reference fields via `this.field`.
                       Demo 00158.class_this_reference.tlg and many others depend on
                       it. Add TokenType.This and a kw-table entry for "this".
  Type names:    int, float, string, bool, array, object, map, void
  Literals:      null, true, false

Operator/punctuation tokens MUST include `Pipe` (the `|` character) as its OWN token
type — bare `|` outside `||` is pattern alternation in `match`, NOT a lexer Unknown
token. The parser uses TokenType.Pipe to detect alternation in <pattern> productions.
Demo 00247.match_alternation.tlg parses `1 | 2 | 3 => ...` and will fail with
"Expected '=>' after pattern but found '|' (token type Pipe)" if the parser predicate
checks TokenType.Unknown instead of TokenType.Pipe — and the predicate uses whichever
token type the lexer emits, so emitting bare `|` as anything other than Pipe propagates
the bug.

Critical Implementation Notes (from Build.Solution.md, repeated here so they don't
get missed):
  Note 1  — `#` is a line comment. `//` is ALWAYS the floor-division operator and
            NEVER a comment under any circumstance.
  Note 2  — `:=` is the only assignment operator for variables/declarations.
            Bare `=` is SingleEqual (valid only inside enum value lists and
            annotation parameter lists). `=` must NOT lex as Unknown anywhere.
  Note 5  — Reserved words always emit their keyword TokenType, never Identifier.
  Note 16 — `static` is its own token (TokenType.Static), not fused into
            static-function or static-class.

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

Each .cmd file must work correctly regardless of the directory it is run from
(arbitrary CWD, the script's own directory, or by double-click in Explorer).
The script must:
- Use %~dp0TinyLanguage.exe to locate the exe (same folder as the .cmd)
- Use %~dp0<filename>.tlg to locate the input file (same folder as the .cmd)
- Default the output path to %~dp0output.txt (next to the script) — a CWD-relative
  default fails silently when invoked from a directory the user can't write to,
  scatters output across random directories, and leaves no visible trace.
- Accept an optional output-path override as %~1 (the natural first argument).
  Also honour %~2 to satisfy the Build.Solution.md contract that says
  "if an output file path is provided as a second argument to the .cmd script,
  pass it through to TinyLanguage.exe".
- Quote every path so directory names with spaces work.
- Print the produced output to the console (`type "%OUTPUT%"`) so the user can
  see results. Without this the .cmd appears to do nothing — the .exe in
  file-processor mode writes only to the output file and prints nothing to stdout.
- Propagate TinyLanguage.exe's exit code; on non-zero exit, write a diagnostic
  to stderr and exit with the same code.
- Do NOT cd or pushd into the .cmd file's own directory.

Correct template (each demo substitutes its own zero-padded prefix and tlg name):
  @echo off
  setlocal
  set "OUTPUT=%~1"
  if "%OUTPUT%"=="" set "OUTPUT=%~2"
  if "%OUTPUT%"=="" set "OUTPUT=%~dp0output.txt"
  "%~dp0TinyLanguage.exe" "%~dp000001.fizzbuzz.tlg" "%OUTPUT%"
  if errorlevel 1 (
      echo TinyLanguage.exe exited with code %ERRORLEVEL% 1>&2
      exit /b %ERRORLEVEL%
  )
  type "%OUTPUT%"
  exit /b 0

Anti-patterns that have shipped before and must not recur:
- `if "%2"==""` as the sole argument check — silently ignores %1, so passing
  one argument has no effect.
- `output.txt` as a bare CWD-relative default — output appears in whatever
  directory the user happened to be in (or in C:\Windows when double-clicked
  by some shells), and the .cmd looks like it produced nothing.
- No `type` of the output file — the .cmd appears to do nothing because the
  exe is silent in file-processor mode, leading users to report "they don't
  execute" even though every demo exits 0.
- No `errorlevel` propagation — a parse/runtime error in the .tlg leaves the
  .cmd exiting 0, masking real failures from any batch validation step.

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
Merge all worktree branches. From the solution root run the full Batch Build & Test
Protocol below. Every step must pass before you may declare the plan complete.

### Step 1 — Clean build (whole solution)
  dotnet build
  Accept: 0 errors, 0 warnings.
  Note: the build does NOT copy any exe to TinyLanguage.DemoFiles\. That happens
  only on `dotnet publish` (Step 4). After a clean checkout, DemoFiles\TinyLanguage.exe
  may not exist yet — that is expected at this point.

### Step 2 — Unit + integration tests
  dotnet test --verbosity normal
  Accept: Failed: 0.
  If any test fails: read full output, identify root cause, make minimal fix, re-run
  dotnet build → dotnet test. Repeat until Failed: 0.

### Step 3 — Demo mode (interpreter end-to-end)
  dotnet run --project TinyLanguage
  Accept: prints "All demos completed successfully.", exit code 0.
  If any demo fails: read the error, fix the interpreter or demo file, re-run.

### Step 4 — Release publish (single-file exe)
  dotnet publish TinyLanguage -c Release
  Accept: TinyLanguage.DemoFiles/TinyLanguage.exe is the freshly-published self-contained
  single-file build (~36 MB). Verify size > 30 MB:
    PowerShell: (Get-Item TinyLanguage.DemoFiles\TinyLanguage.exe).Length -gt 30000000
  Bash:        [ "$(stat -c%s TinyLanguage.DemoFiles/TinyLanguage.exe)" -gt 30000000 ]
  If size is ~160 KB instead, that is the framework-dependent apphost — check
  TinyLanguage.csproj has NOT regrown an AfterTargets="Build" copy target (see the
  csproj comments for the long history). The .cmd demos cannot work with the apphost.

### Step 5 — Verify ALL .cmd files via the published exe
  *** GUARD FIRST: re-verify that DemoFiles\TinyLanguage.exe is still ~36 MB        ***
  *** (not the 160 KB apphost) immediately before the loop. If anything between     ***
  *** Step 4 and Step 5 ran `dotnet build` (e.g. test phase that re-triggered       ***
  *** dependency build, IDE auto-build, an agent cleanup step), the publish exe    ***
  *** could have been clobbered. Re-publish if so before proceeding.                ***

  Run every .cmd file in TinyLanguage.DemoFiles/ — not just the first 10 — and
  for each one verify ALL of:
    a) exit code = 0
    b) the output file written by the .cmd is non-empty
    c) the .cmd printed the output to stdout (so users see something happen)
    d) stdout does NOT contain the apphost-failure signature
       "The application to execute does not exist" — see the csproj comments for
       why this can pass criteria (a)-(c) yet still be broken: the apphost prints
       this error and exits with code 0, the .cmd's `type` then silently emits an
       empty file, and naive validation accepts it.

  History of validation false-positives (none of these recur if (a)-(d) are all
  checked, but the loop must check all four):
  - 320 .cmd files exited 0 but produced no visible output because the default
    `output.txt` was CWD-relative and there was no `type` of the output file.
    Caught by criterion (c).
  - 476 .cmd files exited 0, produced an output file, AND printed to stdout, but
    the stdout was the apphost-not-finding-its-dll error from a build-clobbered
    DemoFiles\TinyLanguage.exe. Caught by criterion (d).

  Suggested PowerShell loop (run from the solution root):
    $dir = "TinyLanguage.DemoFiles"
    if ((Get-Item "$dir\TinyLanguage.exe").Length -lt 30000000) {
      throw "DemoFiles\TinyLanguage.exe is too small ($([math]::Round((Get-Item "$dir\TinyLanguage.exe").Length/1MB, 2)) MB) — re-run dotnet publish TinyLanguage -c Release"
    }
    $cmds = Get-ChildItem $dir -Filter *.cmd | Sort-Object Name
    $outDir = New-Item -ItemType Directory "$env:TEMP\tlg_validate" -Force
    $failed = @()
    foreach ($c in $cmds) {
      $out = Join-Path $outDir ($c.BaseName + ".out.txt")
      $stdout = ($(& cmd /c $c.FullName $out 2>&1) -join "`n")
      if ($LASTEXITCODE -ne 0 -or -not (Test-Path $out) -or `
          (Get-Item $out).Length -eq 0 -or [string]::IsNullOrWhiteSpace($stdout) -or `
          $stdout -match 'The application to execute does not exist') {
        $failed += $c.Name
      }
    }
    if ($failed.Count -gt 0) { throw "Failed: $($failed -join ', ')" }
    "All $($cmds.Count) demo .cmd files passed."

  Accept only when this loop reports success for every file.

  Optional but recommended: spot-check the OUTPUT CORRECTNESS of well-known demos
  (none of the criteria above catch wrong-but-non-empty output). At minimum:
    00001.hello_world      → "Hello, World!"
    00036.fizzbuzz         → 1, 2, Fizz, ..., FizzBuzz at 15
    00075.function_iterative_factorial → 1, 1, 120, 3628800
    00079.function_gcd     → 6, 1, 25
    00104.class_extends    → "Animal Rex", "Rex says woof"
    00226.module_basic     → 7

All five steps must be green. Do not declare the plan complete until they are.
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
