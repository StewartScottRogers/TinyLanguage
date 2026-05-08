# TinyLanguage — Phase 0 Plan.md

Working document for the Claude Code orchestration build of TinyLanguage.
Canonical solution root: `Z:\repos\TinyLanguage.2026.05.07.20\`
Spec source-of-truth: `Build.Solution.md` (locked, read-only).
Deviations layer:    `Build.md` (no exe demo-mode; `run-all-demos.cmd`; DAP debugger; `vscode-extension/`).

---

## 1. Hierarchical Outline of `Build.Solution.md`

```
1. .NET Standards
   1.1 .NET Version Requirements ............. .NET 10.0, C#, VS 2026, no Implicit Using, no Nullable
   1.2 Coding Style .......................... naming, I-prefix, var-only-for-Tuples, readonly default
   1.3 Library Usage ......................... BCL only, no third-party
   1.4 Programming Constructs ................ Tuples for multi-return, Records over Classes (except Visitor AST)
   1.5 File System Structure ................. one type per file
   1.6 Code Documentation .................... business-analyst-readable comments

2. Application Description
   2.1 What to Build ......................... TinyLanguage.YYYY.MM.DD.HH; BNF-driven test generation
   2.2 Class Library: TinyLanguage.Lexer.dll . Lexer + AST nodes + Parser + AstPrettyPrinter
   2.3 Class Library: TinyLanguage.Interpreter.dll
   2.4 UnitTests: TinyLanguage.UnitTests.dll
   2.5 IntegrationTests: TinyLanguage.IntegrationTests.dll
   2.6 Console App: TinyLanguage.exe ......... [SPEC: file-mode + demo-mode] (DEVIATION: demo-mode REMOVED)
   2.7 Demonstration Suite: TinyLanguage.DemoFiles

3. Unit Testing Strategy & Requirements
   3.1 Unit Testing Requirements
   3.2 Test Validation Protocol

4. TinyLanguage Syntax
   4.1 Implementation Notes (1–23)
   4.2 BNF Grammar

5. TinyLanguage Semantics
   5.1 Type System
   5.2 Built-in Functions .................... len/str/int/bool; print is a keyword
   5.3 Scope Rules ........................... linked-list scope chain
   5.4 Runtime Error Policy
   5.5 Feature Implementation Status
   5.6 Interpreter Architecture .............. Visitor pattern via INodeVisitor

6. BNF Grammar Verification Strategy
   6.1 Layer 1 — Lexer Coverage
   6.2 Layer 2 — Parser Coverage
   6.3 Layer 3 — Pretty Printer Coverage
   6.4 Layer 4 — Interpreter Coverage
   6.5 Layer 5 — Integration Coverage
   6.6 Verification Checklist

7. Build Instructions
   7.1 Build the .NET Application
   7.2 Build Success Criteria ................ 0/0; Failed: 0; "All demos completed successfully."; exit 0
   7.3 Implementation Requirements
```

---

## 0. Deliberate Deviations (apply on top of §1–§7)

| # | Spec point | Deviation | Effect |
|---|-----------|-----------|--------|
| D1 | §2.6 Demo mode | exe has NO demo mode; zero args = stdin source; ignore `Console.IsInputRedirected` | WU-4B |
| D2 | §2.6 "All demos completed successfully." | Behavior moves to `TinyLanguage.DemoFiles\run-all-demos.cmd` | WU-1D |
| D3 | §7.2 acceptance | Phase 5 Step 3 calls `run-all-demos.cmd` | WU-5 |
| D4 | (no spec entry) | DAP debugger added: engine in `TinyLanguage.Interpreter`, new `TinyLanguage.DebugAdapter` project, `vscode-extension/`, `--dap` flag | WU-4C, WU-4D |
| D5 | §2.2 split | Lexer + AST + Parser all in `TinyLanguage.Lexer.dll` | WU-1B, WU-1C, WU-2 |

---

## 2. Work Unit List

| ID | Title | Outputs (under canonical root) | Dependencies | Role |
|----|-------|-------------------------------|--------------|------|
| WU-0 | Architect plan | `Plan.md` (this file) | — | Phase 0 |
| WU-1A | Solution scaffold + csproj skeletons + slnx + Directory.Build.props + .vscode/launch.json | scaffold files | — | Phase 1A |
| WU-1B | Lexer | `TinyLanguage.Lexer/TokenType.cs`, `Token.cs`, `Lexer.cs`, `LexerException.cs` | — | Phase 1B |
| WU-1C | AST node classes + visitor + pretty printer | one file per AST node, `INodeVisitor.cs`, `AstPrettyPrinter.cs` | — | Phase 1C |
| WU-1D | Demo .tlg + .cmd files + run-all-demos.cmd | `TinyLanguage.DemoFiles/*.tlg` (300+) + `*.cmd` + aggregator | — | Phase 1D |
| WU-2 | Recursive-descent Parser + ParserException | `Parser.cs`, `ParserException.cs` | WU-1A, WU-1B, WU-1C | Phase 2 |
| WU-3A | Tree-walking interpreter + Scope | `IInterpreter.cs`, `Interpreter.cs`, `Scope.cs`, value-types | WU-2 | Phase 3A |
| WU-3B | Lexer + Parser unit tests | `TinyLanguage.UnitTests/*.cs` | WU-2 | Phase 3B |
| WU-4A | Interpreter integration tests | `TinyLanguage.IntegrationTests/*.cs` | WU-3A, WU-3B | Phase 4A |
| WU-4B | Console app Program.cs (stdin + file modes — NO demo mode) | `TinyLanguage/Program.cs` | WU-3A | Phase 4B |
| WU-4C | Debugger engine + DebugAdapter project + --dap + tests | engine files, new project, modified Program.cs, tests | WU-3A, WU-4B | Phase 4C |
| WU-4D | VS Code extension + .vscode updates + double-click installer | `vscode-extension/*`, updated `.vscode/launch.json`, `tasks.json`, `install-vscode-debugger.cmd` (paren-safe, idempotent) | WU-4C | Phase 4D |
| WU-5 | Final assembly, validate, deliver to canonical sibling path | published exe + all green | WU-1D, WU-3B, WU-4A, WU-4B, WU-4C, WU-4D | Phase 5 |

---

## 3. Dependency Graph + Critical Path

- WU-0 → unblocks Phase 1
- WU-1A, WU-1B, WU-1C, WU-1D run in parallel
- WU-2 = join(WU-1A, WU-1B, WU-1C)
- WU-3A and WU-3B run in parallel after WU-2
- WU-4A = join(WU-3A, WU-3B)
- WU-4B depends only on WU-3A (parallel with WU-4A)
- WU-4C depends on WU-3A AND WU-4B
- WU-4D depends on WU-4C
- WU-5 = join(WU-1D, WU-3B, WU-4A, WU-4B, WU-4C, WU-4D)

### Critical Path

> **WU-0 → WU-1B/1C → WU-2 → WU-3A → WU-4B → WU-4C → WU-4D → WU-5**

Highest-risk nodes: **WU-2** (parser disambiguation: notes 12, 13, 14, 20, 22, 23) and **WU-4C** (cooperative-threaded DAP host with conditional breakpoints + logpoints + restart unwind).

---

## 4. Final Project Directory Layout

```
Z:\repos\TinyLanguage.2026.05.07.20\
├── TinyLanguage.slnx                    ← lists TinyLanguage FIRST
├── Directory.Build.props                ← TargetFramework=net10.0, Nullable=disable, ImplicitUsings=disable, TreatWarningsAsErrors=true
├── install-vscode-debugger.cmd          ← double-click installer (Phase 4D); paren-safe by structure
├── TinyLanguage.wiki.md                 ← user-facing wiki (Phase 4E)
├── .gitignore
│
├── .vscode\
│   ├── launch.json                      ← stdin / file / Debug current .tlg
│   └── tasks.json                       ← build + publish
│
├── TinyLanguage\
│   ├── TinyLanguage.csproj              ← Exe; SelfContained; PublishSingleFile; AfterTargets=Publish copy
│   └── Program.cs                       ← --dap | stdin | file modes
│
├── TinyLanguage.Lexer\                  ← classlib: Lexer + AST + Parser + PrettyPrinter
│   ├── TinyLanguage.Lexer.csproj
│   ├── TokenType.cs / Token.cs / Lexer.cs / LexerException.cs
│   ├── Parser.cs / ParserException.cs
│   ├── INodeVisitor.cs / AstPrettyPrinter.cs / AstNode.cs
│   └── (Statements, Expressions, Types subfolders — one file per node class)
│
├── TinyLanguage.Interpreter\
│   ├── TinyLanguage.Interpreter.csproj
│   ├── AssemblyInfo.cs                  ← [InternalsVisibleTo("TinyLanguage.DebugAdapter")]
│   ├── IInterpreter.cs / Interpreter.cs / InterpreterException.cs / Scope.cs / BuiltInFunctions.cs
│   ├── Values\ (Instance/Function/Lambda/Class/Module/List/Map)
│   ├── IDebuggerHost.cs / DebuggerControl.cs / StatementContext.cs / DebuggerStackFrame.cs
│   └── DebuggerRestartException.cs / DebuggerQuitException.cs
│
├── TinyLanguage.DebugAdapter\           ← new project per D4
│   ├── TinyLanguage.DebugAdapter.csproj
│   ├── DebugAdapterServer.cs / DapMessageReader.cs / DapMessageWriter.cs
│   ├── DapHost.cs / BreakpointInfo.cs / VariableHandle.cs
│
├── TinyLanguage.UnitTests\              ← MSTest
│   └── LexerUnitTests / ParserUnitTests / AstPrettyPrinterUnitTests / DebuggerEngineUnitTests
│
├── TinyLanguage.IntegrationTests\       ← MSTest
│   └── InterpreterIntegrationTests / ScopeIsolation / Exception / PatternMatching / ModuleSystem / DebugAdapterIntegrationTests
│
├── TinyLanguage.DemoFiles\              ← INSIDE solution folder
│   ├── TinyLanguage.DemoFiles.csproj    ← Content-only
│   ├── run-all-demos.cmd                ← aggregator (D2)
│   ├── 00001..00399.*.tlg + .cmd        ← Tier A
│   ├── 00400..00499+.*.tlg + .cmd       ← Tier B
│   └── TinyLanguage.exe                 ← AFTER `dotnet publish` (~36 MB)
│
└── vscode-extension\                    ← per D4
    ├── package.json / extension.js / README.md / .vscodeignore
```

### Conformance to BD §0 structural standards

- **Std 1**: `.slnx` at root + `Directory.Build.props` + `.vscode/` siblings ✓
- **Std 2**: every project in own subdir ✓
- **Std 3**: DemoFiles inside solution root ✓
- **Std 4**: project refs use `..\<Project>\<Project>.csproj` ✓
- **Std 5**: default per-project bin/obj ✓
- **Std 6**: every path resolves under canonical root ✓
