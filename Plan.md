# TinyLanguage — Plan.md (Phase 0 Architect Output)

Working architect document for the Claude Code orchestration build defined in
`Z:\repos\TinyLanguage\Build.md`. Specification source of truth:
`Z:\repos\TinyLanguage\Build.Solution.md` (READ-ONLY).

Canonical solution root: **`Z:\repos\TinyLanguage.2026.05.08.12\`**
(sibling of the orchestrator repo; UTC 2026-05-08, hour 12).

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
  - Tiny Language Demonstration Suite `TinyLanguage.DemoFiles` (SDK-style csproj, 300+ `.tlg` + matching `.cmd`)
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

These come from Build.md's "Deliberate deviations from Build.Solution.md" section
and the additive Phase 4E. The locked spec is overridden by the project owner on
exactly these points; no other deviations are permitted.

| ID | Title                                  | Build.Solution.md says                                                                  | Build.md override                                                                                                    | Affected phases |
|----|----------------------------------------|------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------|-----------------|
| D1 | `TinyLanguage.exe` has no demo mode    | Zero-arg invocation walks `TinyLanguage.DemoFiles\` and prints "All demos completed successfully." | Two modes only — zero-arg = stdin pipe, two-arg = file mode. Anything else → usage to stderr + exit 1. No `Console.IsInputRedirected` branching. | Phase 4B, Phase 5 |
| D2 | Demo aggregator script                 | Demo-walking lives inside `TinyLanguage.exe`                                              | Demo-walking lives in `TinyLanguage.DemoFiles\run-all-demos.cmd`, generated alongside per-demo `.cmd` files.         | Phase 1D, Phase 5 |
| D3 | Phase 5 acceptance shift               | `dotnet run --project TinyLanguage` prints "All demos completed successfully."            | `TinyLanguage.DemoFiles\run-all-demos.cmd` prints it instead. Per-`.cmd` validation loop unchanged.                  | Phase 5 (Step 3) |
| D4 | Interactive debugger via DAP (additive)| Spec is silent on debugging                                                              | Three-layer DAP design: engine `IDebuggerHost` in Interpreter, new `TinyLanguage.DebugAdapter` project, VS Code extension at `vscode-extension\`. Activation flag `--dap`. | Phase 4C, Phase 4D |
| D5 | User-facing wiki (additive)            | Spec is silent on a wiki                                                                  | Single self-contained `TinyLanguage.wiki.md` at the solution root, generated AFTER Phase 4D and BEFORE Phase 5. Pre-declared in slnx "Solution Items" by Phase 1A. | Phase 4E |
| D6 | Long-path support (additive)            | Spec is silent on long paths                                                              | Three-layer requirement so VS Batch Rebuild succeeds on the timestamped canonical path: (1) `HKLM\...\FileSystem\LongPathsEnabled`=1 (machine prereq, admin), (2) `Directory.Build.props` with `<_LongPathsEnabled>true</_LongPathsEnabled>`, (3) `TinyLanguage\app.manifest` with `<longPathAware>true</longPathAware>` referenced via `<ApplicationManifest>` in `TinyLanguage.csproj`. Phase 5 Step 6c verifies all three; Step 6i drives `devenv.com /Rebuild` to prove VS-buildability. | Phase 1A, Phase 4D, Phase 5 |
| D7 | SDK pin via global.json (additive)      | Spec is silent on SDK selection                                                            | `global.json` at the canonical solution root pins to a stable .NET SDK (`10.0.203`, `rollForward: latestPatch`) so VS's bundled NuGet and the dotnet-CLI publish use the same SDK and the same lockfile format. Without this, VS can NRE on `ResolvePackageAssets` if dotnet-on-PATH is a newer feature band than what VS resolves by default. Phase 5 Step 6c verifies. | Phase 1A, Phase 5 |
| D8 | TestLog helper (additive)               | Spec says "every test prints input + result via Console.WriteLine"                          | Tests route through a `TestLog.Input/Section/Result` helper instead of raw `Console.WriteLine`, with sectioned `--- Input ---` / `--- Result ---` blocks and real-newline indentation. Replaces the older `Visible(s)` pattern that escaped `\n`→`\\n` and collapsed multi-line stdout onto one unreadable line. Phase 3B authors `TinyLanguage.UnitTests/TestLog.cs`; Phase 4A authors `TinyLanguage.IntegrationTests/TestLog.cs`; Phase 4C reuses both. Exact source for the helper is in the "Test output formatting — non-negotiable" preamble of Build.md. | Phase 3B, Phase 4A, Phase 4C |

---

## 3. Work-Unit List

Output paths use the canonical solution root `Z:\repos\TinyLanguage.2026.05.08.12\`.

| ID    | Title                              | Outputs (under canonical root unless noted)                                                                                                                                                                                                  | Dependencies          | Phase |
|-------|------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------|-------|
| WU-0  | Architect Analysis                 | `Z:\repos\TinyLanguage\Plan.md` (this file — orchestrator-repo working doc, not committed)                                                                                                                                                     | none                  | 0     |
| WU-1A | Solution Scaffold                  | `TinyLanguage.slnx`, `Directory.Build.props` (incl. long-path props per D6), `global.json` (SDK pin per D7), `.gitignore`, `.vscode\launch.json`, `TinyLanguage\app.manifest` (longPathAware=true), six project subdirs each with empty csproj (`TinyLanguage` (csproj references app.manifest via `<ApplicationManifest>`), `TinyLanguage.Lexer`, `TinyLanguage.Interpreter`, `TinyLanguage.UnitTests`, `TinyLanguage.IntegrationTests`, `TinyLanguage.DemoFiles`); slnx pre-declares Solution Items entries for `.gitignore`, `install-vscode-debugger.cmd`, `TinyLanguage.wiki.md` | WU-0                  | 1A    |
| WU-1B | Token & Lexer                      | `TinyLanguage.Lexer\TokenType.cs`, `Token.cs`, `Lexer.cs`, `LexerException.cs` (incl. `Pipe`, `This`, all kw)                                                                                                                                  | WU-0                  | 1B    |
| WU-1C | AST Node Types + Pretty Printer    | One `*Node.cs` per AST node class in `TinyLanguage.Lexer\`; `INodeVisitor.cs`; `AstPrettyPrinter.cs`                                                                                                                                          | WU-0                  | 1C    |
| WU-1D | Demo Files (Tier A + Tier B)       | `TinyLanguage.DemoFiles\*.tlg` (≥300, Tier A 00001–00399 + Tier B 00400+); matching `*.cmd` per demo (CWD-independent template); `TinyLanguage.DemoFiles\run-all-demos.cmd`                                                                    | WU-0                  | 1D    |
| WU-2  | Parser                             | `TinyLanguage.Lexer\Parser.cs` (recursive-descent over full BNF, all 23 notes), `ParserException.cs`                                                                                                                                          | WU-1A, WU-1B, WU-1C   | 2     |
| WU-3A | Interpreter                        | `TinyLanguage.Interpreter\IInterpreter.cs`, `Interpreter.cs` (visitor + scope chain + operator semantics + ArrayElementAssign mutation + foreach-over-string), `InterpreterException.cs`, `Scope.cs`                                          | WU-2                  | 3A    |
| WU-3B | Unit Tests (Lexer + Parser)        | `TinyLanguage.UnitTests\TestLog.cs` (pretty-print helper per D8), `LexerUnitTests.cs`, `ParserUnitTests.cs` (MSTest, `Subject_Action_ExpectedOutcome` naming, every test routes through `TestLog.Input/Result`)                              | WU-2                  | 3B    |
| WU-4A | Integration Tests                  | `TinyLanguage.IntegrationTests\TestLog.cs` (pretty-print helper per D8), `InterpreterIntegrationTests.cs` (uses Phase 1D `.tlg` inputs; routes output through `TestLog`)                                                                       | WU-3A, WU-3B          | 4A    |
| WU-4B | Console Application                | `TinyLanguage\Program.cs` (two modes only — see D1); single-file self-contained publish copies `TinyLanguage.exe` into `TinyLanguage.DemoFiles\` via `AfterTargets="Publish"`                                                                  | WU-3A, WU-3B          | 4B    |
| WU-4C | Debugger Engine + DAP Adapter      | Engine (`TinyLanguage.Interpreter\IDebuggerHost.cs`, `DebuggerControl.cs`, `StatementContext.cs`, `DebuggerStackFrame.cs`, `DebuggerRestartException.cs`, `DebuggerQuitException.cs`, `AssemblyInfo.cs`, modified `Interpreter.cs` + `Scope.cs`); new project `TinyLanguage.DebugAdapter\` (`DebugAdapterServer.cs`, `DapMessageReader.cs`, `DapMessageWriter.cs`, `DapHost.cs`, `BreakpointInfo.cs`, `VariableHandle.cs`); slnx insertion between Interpreter and UnitTests; `--dap` flag in `TinyLanguage\Program.cs`; `DebuggerEngineUnitTests.cs` + `DebugAdapterIntegrationTests.cs` | WU-3A, WU-4B          | 4C    |
| WU-4D | VS Code Extension + Installer      | `vscode-extension\package.json`, `extension.js`, `README.md`, `.vscodeignore`, `LICENSE.txt`; `install-vscode-debugger.cmd` at solution root; `.vscode\launch.json` extended with `tinylanguage` config; `.vscode\tasks.json` with `publish` task | WU-4C                 | 4D    |
| WU-4E | Wiki Generation                    | `TinyLanguage.wiki.md` at the solution root (>200 lines, single self-contained markdown covering language tour, demo suite, debugger, architecture, regeneration steps); slnx Solution Items entry already present from WU-1A                  | WU-4D                 | 4E    |
| WU-5  | Final Validation & Delivery        | Validated solution at `Z:\repos\TinyLanguage.2026.05.08.12\`; `dotnet publish` produced ~36 MB single-file exe at `TinyLanguage.DemoFiles\TinyLanguage.exe`; full `.cmd` validation loop green; `run-all-demos.cmd` exits 0; DAP smoke test green | WU-4A, WU-4B, WU-4C, WU-4D, WU-4E | 5 |

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
                     |
                   WU-4E
                     |
                    WU-5  <- joins WU-4A leaf as well
```

Notes:
- Phase 1 (WU-1A..1D) is fully parallel — four worktrees launch simultaneously.
- WU-2 joins all three foundation projects (1A, 1B, 1C); WU-1D is independent of WU-2 but WU-5 joins it.
- Phase 3 (WU-3A, WU-3B) is parallel after WU-2.
- Phase 4 split: WU-4A and WU-4B parallel after WU-3A/3B; WU-4C sequential after WU-3A + WU-4B; WU-4D after WU-4C; WU-4E after WU-4D.
- Phase 5 (WU-5) joins all leaves: WU-4A and WU-4E.

**Critical path** (longest chain): WU-0 -> WU-1B/1C -> WU-2 -> WU-3A -> WU-4B -> WU-4C -> WU-4D -> WU-4E -> WU-5.

---

## 5. Final Project Directory Layout

All paths under `Z:\repos\TinyLanguage.2026.05.08.12\`. Annotation `(Pn)` indicates the producing phase.

```
Z:\repos\TinyLanguage.2026.05.08.12\
├── TinyLanguage.slnx                                       (1A, edited 4C to add DebugAdapter project)
├── Directory.Build.props                                   (1A)
├── global.json                                             (1A — SDK pin; see D7)
├── .gitignore                                              (1A)
├── install-vscode-debugger.cmd                             (4D)
├── TinyLanguage.wiki.md                                    (4E)
├── .vscode\
│   ├── launch.json                                         (1A; extended 4D with tinylanguage debug config)
│   └── tasks.json                                          (4D — publish preLaunchTask)
├── TinyLanguage\                                           (1A scaffold; 4B Program.cs; 4C --dap flag)
│   ├── TinyLanguage.csproj                                 (1A; ProjectReference to DebugAdapter added 4C)
│   ├── app.manifest                                        (1A — longPathAware=true; see D6)
│   └── Program.cs                                          (4B; modified 4C)
├── TinyLanguage.Lexer\                                     (1A scaffold; 1B+1C+2 source)
│   ├── TinyLanguage.Lexer.csproj                           (1A)
│   ├── TokenType.cs / Token.cs / Lexer.cs / LexerException.cs   (1B)
│   ├── *Node.cs (one file per AST node class)              (1C)
│   ├── INodeVisitor.cs                                     (1C)
│   ├── AstPrettyPrinter.cs                                 (1C)
│   ├── Parser.cs                                           (2)
│   └── ParserException.cs                                  (2)
├── TinyLanguage.Interpreter\                               (1A scaffold; 3A source; 4C debugger additions)
│   ├── TinyLanguage.Interpreter.csproj                     (1A)
│   ├── IInterpreter.cs / Interpreter.cs / Scope.cs         (3A; Interpreter + Scope modified 4C)
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
│   ├── BreakpointInfo.cs                                   (4C)
│   └── VariableHandle.cs                                   (4C)
├── TinyLanguage.UnitTests\                                 (1A scaffold; 3B source; 4C debugger tests)
│   ├── TinyLanguage.UnitTests.csproj                       (1A)
│   ├── TestLog.cs                                          (3B — pretty-print helper; see D8)
│   ├── LexerUnitTests.cs / ParserUnitTests.cs              (3B)
│   └── DebuggerEngineUnitTests.cs                          (4C)
├── TinyLanguage.IntegrationTests\                          (1A scaffold; 4A source; 4C DAP integration)
│   ├── TinyLanguage.IntegrationTests.csproj                (1A)
│   ├── TestLog.cs                                          (4A — pretty-print helper; see D8)
│   ├── InterpreterIntegrationTests.cs                      (4A)
│   └── DebugAdapterIntegrationTests.cs                     (4C)
├── TinyLanguage.DemoFiles\                                 (1A scaffold; 1D content; 4B publish target)
│   ├── TinyLanguage.DemoFiles.csproj                       (1A — content-only, copies *.tlg + *.cmd)
│   ├── 00001..00399.*.tlg                                  (1D — Tier A feature-coverage demos)
│   ├── 00400..00499.*.tlg                                  (1D — Tier B advanced data structures)
│   ├── *.cmd (one per .tlg, CWD-independent)               (1D)
│   ├── run-all-demos.cmd                                   (1D — replaces removed exe demo mode, see D2)
│   └── TinyLanguage.exe                                    (4B — placed by AfterTargets="Publish"; ~36 MB self-contained)
└── vscode-extension\                                        (4D — peer of source projects)
    ├── package.json                                        (4D)
    ├── extension.js                                        (4D)
    ├── README.md                                           (4D)
    ├── .vscodeignore                                       (4D)
    └── LICENSE.txt                                         (4D)
```

---

*End of Plan.md — architect-level outline only. No code generated. Build.Solution.md
and Build.md are unchanged.*
