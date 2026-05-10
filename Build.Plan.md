# TinyLanguage — Build.Plan.md (architect plan)

Working architect document for the Claude Code orchestration build defined in
`Z:\repos\TinyLanguage\Build.md`. Specification source of truth:
`Z:\repos\TinyLanguage\Build.Solution.md` (READ-ONLY).

Canonical solution root: `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` (sibling of the
orchestrator repo; UTC year/month/day/hour substituted at orchestration start).

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

From Build.md "Deliberate deviations from Build.Solution.md" plus the additive Phase 4E (wiki) and Phase 4F (VS18 + Shared Project). The locked spec is overridden on exactly these points.

| ID | Title | Spec says | Override | Phases |
|----|-------|-----------|----------|--------|
| D1 | `TinyLanguage.exe` has no demo mode | Zero-arg walks `DemoFiles\` and prints "All demos completed successfully." | Two modes: zero-arg stdin pipe, two-arg file mode. Anything else → usage to stderr + exit 1. No `Console.IsInputRedirected` branching. | 4B, 5 |
| D2 | Demo aggregator script | Demo-walking lives in the exe | Lives in `TinyLanguage.DemoFiles\run-all-demos.cmd` alongside per-demo `.cmd` files. | 1D, 5 |
| D3 | Phase 5 acceptance shift | `dotnet run` prints the success banner | `run-all-demos.cmd` prints it. Per-`.cmd` validation loop unchanged. | 5 |
| D4 | Interactive debugger via DAP (additive) | Silent on debugging | Three-layer DAP: engine `IDebuggerHost` in Interpreter, new `TinyLanguage.DebugAdapter`, editor shims at `extensions\vscode\` and `extensions\vs\` (see D9). Activation flag `--dap`. | 4C, 4D, 4F |
| D5 | User-facing wiki (additive) | Silent on a wiki | Single self-contained `TinyLanguage.wiki.md` at solution root, generated after 4D and before 5. Pre-declared in slnx Solution Items by 1A. | 4E |
| D6 | Long-path support (additive) | Silent | Three layers so VS Batch Rebuild handles the timestamped path: (1) HKLM `LongPathsEnabled`=1 (admin, machine prereq), (2) `Directory.Build.props` with `<_LongPathsEnabled>true`, (3) `TinyLanguage\app.manifest` `<longPathAware>true` via `<ApplicationManifest>`. Phase 5 §6c verifies; §6i drives `devenv.com /Rebuild`. | 1A, 5 |
| D7 | SDK pin via global.json (additive) | Silent on SDK selection | `global.json` pins `10.0.203` `rollForward: latestPatch` so VS and `dotnet`-on-PATH produce compatible lockfiles. `latestFeature` would roll up to `10.0.300-preview` and NRE VS18's `ResolvePackageAssets`. Phase 5 §6c verifies. | 1A, 5 |
| D8 | TestLog helper (additive) | Tests print via raw `Console.WriteLine` | Tests route through `TestLog.Input/Section/Result` with `--- Input ---` / `--- Result ---` framing and real-newline indentation. Replaces the older `Visible(s)` pattern that escaped `\n` and collapsed multi-line stdout onto one line. Exact source in Build.md preamble. | 3B, 4A, 4C |
| D9 | Editor shims under a Shared Project (additive) | Silent on editor integration | Both shims live under `extensions\`. `extensions.shproj` + `extensions.projitems` list every shim file as `<None>` items, giving Solution Explorer one navigable tree. The Shared Project produces no assembly — `dotnet build` silently skips it, `devenv.com /Rebuild` loads it via `Microsoft.CodeSharing.CSharp.targets`. The VS18 VSIX (`extensions\vs\TinyLanguage.VsTools.csproj`, net472) is built outside the slnx by `install-vs-debugger.cmd`. GUIDs in §6. | 1A, 4D, 4F |

---

## 3. Work-Unit List

Paths under canonical root `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`.

| ID | Title | Outputs | Dependencies | Phase |
|----|-------|---------|--------------|-------|
| WU-0 | Architect Analysis | This file (orchestrator working doc) | none | 0 |
| WU-1A | Solution Scaffold | `TinyLanguage.slnx` (Solution Items pre-declares `.gitignore`, both installers, wiki); `Directory.Build.props` (D6); `global.json` (D7); `.gitignore`; `.vscode\launch.json`; `TinyLanguage\app.manifest` (D6); six project subdirs with empty csproj | WU-0 | 1A |
| WU-1B | Token & Lexer | `TinyLanguage.Lexer\` — `TokenType.cs`, `Token.cs`, `Lexer.cs`, `LexerException.cs` (incl. `Pipe`, `This`, every kw) | WU-0 | 1B |
| WU-1C | AST Nodes + Pretty Printer | `TinyLanguage.Lexer\` — one `*Node.cs` per AST class, `INodeVisitor.cs`, `AstPrettyPrinter.cs` | WU-0 | 1C |
| WU-1D | Demo Files | `TinyLanguage.DemoFiles\` — ≥300 `.tlg` (Tier A 00001–00399, Tier B 00400+), matching `.cmd` per demo, `run-all-demos.cmd` | WU-0 | 1D |
| WU-2 | Parser | `TinyLanguage.Lexer\Parser.cs` (recursive-descent, full BNF, 23 notes), `ParserException.cs` | WU-1A, WU-1B, WU-1C | 2 |
| WU-3A | Interpreter | `TinyLanguage.Interpreter\` — `IInterpreter.cs`, `Interpreter.cs`, `Scope.cs`, `InterpreterException.cs` | WU-2 | 3A |
| WU-3B | Unit Tests | `TinyLanguage.UnitTests\` — `TestLog.cs` (D8), `LexerUnitTests.cs`, `ParserUnitTests.cs` | WU-2 | 3B |
| WU-4A | Integration Tests | `TinyLanguage.IntegrationTests\` — `TestLog.cs` (D8), `InterpreterIntegrationTests.cs` | WU-3A, WU-3B | 4A |
| WU-4B | Console App | `TinyLanguage\Program.cs` (two modes, D1); single-file Release publish copies `TinyLanguage.exe` into `DemoFiles\` via `AfterTargets="Publish"` | WU-3A, WU-3B | 4B |
| WU-4C | Debugger Engine + DAP | `TinyLanguage.Interpreter\` debug additions (`IDebuggerHost.cs`, `DebuggerControl.cs`, `StatementContext.cs`, `DebuggerStackFrame.cs`, restart/quit exception files, `AssemblyInfo.cs`); new project `TinyLanguage.DebugAdapter\`; `--dap` flag in `Program.cs`; `DebuggerEngineUnitTests.cs` + `DebugAdapterIntegrationTests.cs` | WU-3A, WU-4B | 4C |
| WU-4D | VS Code Editor Shim | `extensions\vscode\` (`package.json`, `extension.js`, `README.md`, `.vscodeignore`, `LICENSE.txt`); `install-vscode-debugger.cmd`; `.vscode\launch.json` adds `tinylanguage` config; `.vscode\tasks.json` adds `publish` task | WU-4C | 4D |
| WU-4E | Wiki | `TinyLanguage.wiki.md` (>200 lines, language tour / demo suite / debugger / architecture / regen steps) | WU-4D | 4E |
| WU-4F | VS18 Shim + Shared Project | `extensions\extensions.shproj` + `extensions.projitems`; `extensions\vs\` VSIX (`TinyLanguage.VsTools.csproj` net472, `source.extension.vsixmanifest`, `TinyLanguagePackage.cs` AsyncPackage, `TinyLanguageAdapterLauncher.cs` `IAdapterLauncher`, `TinyLanguageTargetHostProcess.cs`, `Resources\PackageRegistration.pkgdef`, `Resources\icon.png`, `launch.vs.json.template`, `README.md`, `LICENSE.txt`); `install-vs-debugger.cmd` (paren-safe `:check_tool` + `:resolve_pf86` for the `(x86)` literal-paren bug); slnx gains `<Project Path="extensions\extensions.shproj" />`. GUIDs in §6. | WU-4D | 4F |
| WU-5 | Final Validation & Delivery | `dotnet build` 0/0; `dotnet test` Failed: 0; `dotnet publish` produces ~36 MB exe in `DemoFiles\`; `run-all-demos.cmd` exits 0; `.cmd` validation loop a-d all pass; `devenv.com /Rebuild` reports "succeeded, 0 failed, 0 skipped" with the Shared Project loaded cleanly | WU-4A, WU-4B, WU-4C, WU-4D, WU-4E, WU-4F | 5 |

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
                    WU-5  <- joins WU-4A and WU-1D leaves as well
```

- Phases 1A-1D parallel (four worktrees).
- WU-2 joins 1A + 1B + 1C; WU-1D feeds WU-5 directly.
- WU-3A and WU-3B parallel after WU-2.
- WU-4A and WU-4B parallel after WU-3A + WU-3B; WU-4C sequential after WU-3A + WU-4B; WU-4D after WU-4C; WU-4E and WU-4F parallel after WU-4D (neither overlaps the other's outputs).
- WU-5 joins WU-4A, WU-4E, WU-4F, WU-1D.

**Critical path** (longest chain): WU-0 → WU-1B/1C → WU-2 → WU-3A → WU-4B → WU-4C → WU-4D → WU-4F → WU-5.

---

## 5. Final Project Directory Layout

Paths under `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`. Annotation `(Pn)` = producing phase.

```
TinyLanguage.YYYY.MM.DD.HH\
├── TinyLanguage.slnx                                       (1A; +DebugAdapter 4C; +extensions.shproj 4F)
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
├── TinyLanguage.DemoFiles\
│   ├── TinyLanguage.DemoFiles.csproj                       (1A)
│   ├── 00001..00399.*.tlg                                  (1D — Tier A)
│   ├── 00400..00499.*.tlg                                  (1D — Tier B)
│   ├── *.cmd (one per .tlg, CWD-independent)               (1D)
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
| .NET SDK pin | `10.0.203` / `rollForward: latestPatch` | `global.json` | 1A (D7) |
| Extension version | `0.1.0` | `source.extension.vsixmanifest` + `extensions/vscode/package.json` | 4D, 4F |
| VS Code extension ID | `tinylanguage-local.tinylanguage-debug` | `code --list-extensions`; `code --install-extension` arg | 4D |
| VS18 install hive | `%LOCALAPPDATA%\Microsoft\VisualStudio\18.0_*\Extensions\` | per-user (no admin); `vswhere -find` against `Common7\IDE\Extensions\` returns empty | 4F |

---

*End of Build.Plan.md — architect-level outline. No code generated. Build.Solution.md and Build.md are unchanged.*
