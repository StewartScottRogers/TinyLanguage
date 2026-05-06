# Plan.md — TinyLanguage Build Plan

## 1. Hierarchical Outline of Build.Solution.md

### 1.1 .NET Standards
- 1.1.1 .NET Version Requirements (.NET 10.0, C#, VS 2026+, no implicit using, no nullable)
- 1.1.2 Coding Style (naming conventions, readonly, tuples, exceptions)
- 1.1.3 Library Usage (BCL only)
- 1.1.4 Programming Constructs (tuples, records vs classes, streams)
- 1.1.5 File System Structure (one file per type)
- 1.1.6 Code Documentation (comments for analysts/juniors)

### 1.2 Application Description
- 1.2.1 What to Build (solution folder `TinyLanguage.YYYY.MM.DD.HH`, BNF verification strategy)
- 1.2.2 Class Library: TinyLanguage.Lexer.dll (Lexer, AST nodes, Parser, Pretty Printer)
- 1.2.3 Class Library: TinyLanguage.Interpreter.dll (Interpreter, depends on Lexer)
- 1.2.4 Unit Tests: TinyLanguage.UnitTests.dll (Lexer + Parser tests)
- 1.2.5 Integration Tests: TinyLanguage.IntegrationTests.dll (Interpreter + E2E)
- 1.2.6 Console Application: TinyLanguage.exe (file-processor mode, demo mode)
- 1.2.7 Demonstration Suite: TinyLanguage.DemoFiles (SharedProject, 300+ .tlg files, .cmd scripts)

### 1.3 Unit Testing Strategy & Requirements
- 1.3.1 Unit Testing Requirements (MSTest, naming conventions)
- 1.3.2 Test Validation Protocol (build, run, test commands)
- 1.3.3 Acceptance Criteria (0 errors, 0 warnings, 0 test failures, demo success)
- 1.3.4 Fix-and-Retry Loop

### 1.4 TinyLanguage Syntax
- 1.4.1 Implementation Notes (23 disambiguation and semantic rules)
- 1.4.2 BNF Grammar
  - 1.4.2.1 Comments
  - 1.4.2.2 Program Structure (`<program>`, `<stmt_list>`)
  - 1.4.2.3 Statements (24 statement types)
  - 1.4.2.4 Assignment and Declaration
  - 1.4.2.5 Control Flow
  - 1.4.2.6 I/O Statements
  - 1.4.2.7 Functions
  - 1.4.2.8 Lambda Expressions
  - 1.4.2.9 Classes and Objects
  - 1.4.2.10 Module System
  - 1.4.2.11 Expressions
  - 1.4.2.12 Inline Conditional Expression
  - 1.4.2.13 Cast Expression
  - 1.4.2.14 Arrays
  - 1.4.2.15 Exception Handling
  - 1.4.2.16 Pattern Matching
  - 1.4.2.17 Annotations
  - 1.4.2.18 Type System
  - 1.4.2.19 Literals and Identifiers
  - 1.4.2.20 Terminal Symbols

### 1.5 TinyLanguage Semantics
- 1.5.1 Type System (Integer, Float, Bool)
- 1.5.2 Arithmetic Promotion rules
- 1.5.3 Operator-Specific Rules (`//`, `**`, `%`, `&`, `+`)
- 1.5.4 Truthiness rules
- 1.5.5 Built-in Functions (`len`, `str`, `int`, `bool`)
- 1.5.6 Scope Rules (scope chain, function scope, closures/lambdas)
- 1.5.7 Runtime Error Policy
- 1.5.8 Feature Implementation Status
- 1.5.9 Interpreter Architecture (Visitor pattern)

### 1.6 BNF Grammar Verification Strategy
- 1.6.1 Layer 1 — Lexer Coverage
- 1.6.2 Layer 2 — Parser Coverage
- 1.6.3 Layer 3 — AST Pretty Printer Coverage
- 1.6.4 Layer 4 — Interpreter Coverage
- 1.6.5 Layer 5 — Integration Coverage
- 1.6.6 Verification Checklist

### 1.7 Build Instructions

---

## 2. Work Unit List

### WU-01: Solution Scaffold
| Field | Value |
|---|---|
| **Title** | Create .NET 10.0 Solution and Project Files |
| **Inputs** | Sec 1.1.1, 1.2.1–1.2.7 |
| **Outputs** | `TinyLanguage.slnx`, all six `.csproj`, `Directory.Build.props`, `.vscode/launch.json` — all created at the **canonical absolute path** `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` (sibling of the orchestrator repo). NOT inside any worktree, NOT inside the orchestrator repo. See Build.md "Output location — non-negotiable". |
| **Dependencies** | None |
| **Assignee Role** | Build Engineer |
| **Path discipline** | Use the absolute canonical path verbatim in every file operation. Project subdirectories (`TinyLanguage/`, `TinyLanguage.Lexer/`, `TinyLanguage.Interpreter/`, `TinyLanguage.UnitTests/`, `TinyLanguage.IntegrationTests/`, `TinyLanguage.DemoFiles/`) are direct children of the solution root. `<ProjectReference>` paths are solution-relative (e.g. `..\TinyLanguage.Lexer\...csproj`). |
| **Exe requirement** | `TinyLanguage.csproj` must set `SelfContained=true`, `RuntimeIdentifier=win-x64`, `PublishSingleFile=true`, `EnableCompressionInSingleFile=true`, and include only an `AfterTargets="Publish"` copy target that places the self-contained exe inside `TinyLanguage.DemoFiles/` (a peer subdirectory of the solution root). NO `AfterTargets="Build"` copy target — that has historically silently broken every `.cmd` demo (see Build.md Phase 1A history block). |

### WU-02: Token Types and Lexer
| Field | Value |
|---|---|
| **Title** | Implement TokenType Enum and Lexer |
| **Inputs** | Sec 1.4.2.19, 1.4.2.20, 1.4.1 Notes 1–5/16, 1.1.2 |
| **Outputs** | `TokenType.cs`, `Token.cs`, `Lexer.cs`, `LexerException.cs` |
| **Dependencies** | WU-01 |
| **Assignee Role** | Language Engineer |

### WU-03: AST Node Types
| Field | Value |
|---|---|
| **Title** | Implement All AST Node Classes and Visitor Interface |
| **Inputs** | Sec 1.4.2.3–1.4.2.17, 1.5.9, 1.4.1 Notes 7/15, 1.1.4 |
| **Outputs** | All `Nodes/*.cs`, `INodeVisitor.cs`, `AstNode.cs` |
| **Dependencies** | WU-01 |
| **Assignee Role** | Language Engineer |

### WU-04: Parser
| Field | Value |
|---|---|
| **Title** | Implement Recursive-Descent Parser |
| **Inputs** | Sec 1.4.2 (full BNF), 1.4.1 (all 23 notes), 1.5.8 |
| **Outputs** | `Parser.cs`, `ParserException.cs` |
| **Dependencies** | WU-02, WU-03 |
| **Assignee Role** | Language Engineer |

### WU-05: AST Pretty Printer
| Field | Value |
|---|---|
| **Title** | Implement AST Pretty Printer using Visitor Pattern |
| **Inputs** | Sec 1.2.2, 1.6.3, 1.5.9 |
| **Outputs** | `AstPrettyPrinter.cs` |
| **Dependencies** | WU-03 |
| **Assignee Role** | Language Engineer |

### WU-06: Interpreter
| Field | Value |
|---|---|
| **Title** | Implement Interpreter with Visitor Pattern, Scope Chain, Built-ins |
| **Inputs** | Sec 1.5 (full semantics), 1.4.1 Notes 6–10/19, 1.2.3 |
| **Outputs** | `Interpreter.cs`, `IInterpreter.cs`, `Scope.cs`, `InterpreterException.cs` |
| **Dependencies** | WU-03, WU-04 |
| **Assignee Role** | Language Engineer |

### WU-07: Unit Tests (Lexer + Parser)
| Field | Value |
|---|---|
| **Title** | Implement Layers 1–3 Test Classes |
| **Inputs** | Sec 1.6.1–1.6.3, 1.3.1 |
| **Outputs** | `LexerUnitTests.cs`, `ParserUnitTests.cs` |
| **Dependencies** | WU-02, WU-04 |
| **Assignee Role** | QA Engineer |
| **Test output rule** | Every test prints its input and result via `Console.WriteLine` |

### WU-08: Integration Tests
| Field | Value |
|---|---|
| **Title** | Implement Layers 4–5 Test Classes |
| **Inputs** | Sec 1.6.4–1.6.5, 1.3.1 |
| **Outputs** | `InterpreterIntegrationTests.cs` |
| **Dependencies** | WU-06 |
| **Assignee Role** | QA Engineer |
| **Test output rule** | Every test prints its input and result via `Console.WriteLine` |

### WU-09: Console Application
| Field | Value |
|---|---|
| **Title** | Implement TinyLanguage.exe with File-Processor and Demo Modes |
| **Inputs** | Sec 1.2.6 |
| **Outputs** | `Program.cs` |
| **Dependencies** | WU-06 |
| **Assignee Role** | Application Engineer |
| **Verification** | `dotnet publish TinyLanguage -c Release` → single-file `TinyLanguage.exe` appears in `TinyLanguage.DemoFiles/` (self-contained, no runtime required) |

### WU-10: Demo File Suite
| Field | Value |
|---|---|
| **Title** | Generate 300+ Demo .tlg Files and Matching .cmd Scripts |
| **Inputs** | Sec 1.2.7, 1.5.8, full BNF |
| **Outputs** | `TinyLanguage.DemoFiles/00001.*.tlg` … `004xx.*.tlg` + matching `.cmd` |
| **Dependencies** | WU-04 (grammar finalized) |
| **Assignee Role** | Content Engineer |
| **.cmd rule** | Use `%~dp0TinyLanguage.exe` and `%~dp0<file>.tlg` so the cmd works from any directory. Default output to `%~dp0output.txt` (next to the script — never to a CWD-relative path, which fails silently). Accept `%~1` (or `%~2` per Build.Solution.md §1.2.7) as an output override. Quote every path. After running, `type "%OUTPUT%"` so the user sees results. Propagate `errorlevel`. Never `cd` or `pushd` into `%~dp0`. See Build.md Phase 1D for the full template. |
| **Tier split** | Two tiers, both produced in this WU: **Tier A — Feature-coverage demos (00001..00399)**: short single-feature .tlg files (≤ ~15 lines) that exercise one BNF production each. **Tier B — Advanced data-structure demos (00400..00499+, ≥ 50 files)**: 30–200-line programs that define an advanced data structure (singly/doubly linked list, stack, queue, deque, ring buffer, LRU cache, BST, AVL/RB tree, heap/priority queue, trie, segment/Fenwick tree, hash map/set, open-addressing table, graph adjacency list, union-find) and exercise it via at least three named subroutines and ≥ 8 mixed mutation/query operations. Tier B also covers algorithms over those structures (BFS, DFS, Dijkstra, topological sort, quicksort, mergesort, heapsort, binary search, Sieve, LCS, knapsack, edit distance, postfix eval, shunting-yard). Output must be deterministic; subroutines must be called, not just defined; each Tier B file starts with a 3-line header comment naming the data structure, the subroutines, and the workload. See Build.md Phase 1D "Tier B requirements" for the full specification. |

### WU-11: Validation and Assembly
| Field | Value |
|---|---|
| **Title** | Execute Build, Test, Demo Validation, Deliver, and Fix-and-Retry Loop |
| **Inputs** | Sec 1.3.2–1.3.4, 1.6.6, 1.7 |
| **Outputs** | Green build, green tests, green demo, **and a buildable solution at the canonical absolute path `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`** (sibling of the orchestrator repo). The work unit is NOT complete while the solution lives only inside a worktree at `Z:\repos\TinyLanguage\.claude\worktrees\agent-<id>\TinyLanguage.YYYY.MM.DD.HH\`. |
| **Dependencies** | WU-07, WU-08, WU-09, WU-10 |
| **Assignee Role** | Build Engineer |
| **Delivery requirement** | After Steps 1–5 are green inside the merge worktree, run Build.md Phase 5 Step 6 (robocopy from worktree to canonical path, structural verification, full re-build/re-test/re-publish at the canonical path, .cmd validation against the canonical DemoFiles, and `git status` confirmation that the orchestrator repo is unaffected). Only when Step 6 reports the canonical path holds a clean buildable solution may this WU be marked complete. |

---

## 3. Dependency Graph and Critical Path

```
WU-01 (Scaffold)
  |
  +---> WU-02 (Lexer) ──────────────────┐
  |                                      │
  +---> WU-03 (AST Nodes) ──┬──────────►WU-04 (Parser) ──►WU-06 (Interpreter) ──►WU-08 (Integration Tests)
                             │                              │                       │
                             └──►WU-05 (Printer)           └──►WU-10 (Demo Files)  └──►WU-09 (Console App)
                             │
                             └──►WU-07 (Unit Tests) ◄──────WU-04
                                                                    │
                                                         WU-11 (Validation) ◄── all above
```

**Critical Path:** WU-01 → WU-02 + WU-03 → WU-04 → WU-06 → WU-08 → WU-11

---

## 4. Project Directory Layout

The generated solution folder is delivered to the **canonical absolute path**
`Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` — a sibling of the orchestrator repo at
`Z:\repos\TinyLanguage\`. It is NOT inside the orchestrator repo, NOT inside any
git worktree under `.claude/worktrees/`, and NOT tracked by the orchestrator's
git history. See Build.md "Output location — non-negotiable" for the full
contract, including the delivery step in Phase 5 that copies from the merge
worktree to the canonical path and re-validates end-to-end.

**Standard .NET solution structure** (enforced — see Build.md preamble):
- The solution file (`TinyLanguage.slnx`) sits at the **root** of the solution
  folder, alongside `Directory.Build.props` and `.vscode/`.
- Every project lives in its **own subdirectory** named after the project, with
  its `.csproj` file inside that subdirectory.
- **`TinyLanguage.DemoFiles/` is a project subdirectory of the solution folder**
  at `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DemoFiles\`. The `.tlg`
  files, the matching `.cmd` runners, the `TinyLanguage.DemoFiles.csproj`, and
  (after `dotnet publish`) the self-contained `TinyLanguage.exe` all live in
  that one directory. Demo files MUST NOT be placed outside the solution folder,
  inside any other project, inside `bin/`, or in any user temp/AppData path.
- All `<ProjectReference>` paths are solution-relative
  (e.g. `..\TinyLanguage.Lexer\TinyLanguage.Lexer.csproj`) — never absolute,
  never escaping the solution root.

```
Z:\repos\TinyLanguage.YYYY.MM.DD.HH\        (canonical absolute path; sibling of the repo)

├── TinyLanguage.slnx                       (solution file at the root)
├── Directory.Build.props
│
├── TinyLanguage/
│   ├── TinyLanguage.csproj
│   └── Program.cs
│
├── TinyLanguage.Lexer/
│   ├── TinyLanguage.Lexer.csproj
│   ├── TokenType.cs
│   ├── Token.cs
│   ├── Lexer.cs
│   ├── LexerException.cs
│   ├── AstNode.cs
│   ├── INodeVisitor.cs
│   ├── Parser.cs
│   ├── ParserException.cs
│   ├── AstPrettyPrinter.cs
│   └── Nodes/
│       ├── ProgramNode.cs
│       ├── AssignStatementNode.cs
│       ├── LetDeclareNode.cs
│       ├── VarDeclareNode.cs
│       ├── ConstDeclareNode.cs
│       ├── EnumDefNode.cs
│       ├── IfStatementNode.cs
│       ├── WhileStatementNode.cs
│       ├── ForStatementNode.cs
│       ├── ForeachStatementNode.cs
│       ├── DoWhileStatementNode.cs
│       ├── SwitchStatementNode.cs
│       ├── BreakStatementNode.cs
│       ├── ContinueStatementNode.cs
│       ├── PrintStatementNode.cs
│       ├── InputStatementNode.cs
│       ├── FunctionDefNode.cs
│       ├── MethodDefNode.cs
│       ├── CallStatementNode.cs
│       ├── ReturnStatementNode.cs
│       ├── ClassDefNode.cs
│       ├── ConstructorDefNode.cs
│       ├── FieldDeclareNode.cs
│       ├── ModuleDefNode.cs
│       ├── ImportStatementNode.cs
│       ├── ExportStatementNode.cs
│       ├── TryStatementNode.cs
│       ├── ThrowStatementNode.cs
│       ├── PatternMatchNode.cs
│       ├── PatternCaseNode.cs
│       ├── PatternNode.cs
│       ├── AnnotatedStatementNode.cs
│       ├── AnnotationNode.cs
│       ├── ArrayElementAssignNode.cs
│       ├── BinaryOpNode.cs
│       ├── UnaryOpNode.cs
│       ├── TernaryNode.cs
│       ├── IdentifierNode.cs
│       ├── IntegerLiteralNode.cs
│       ├── FloatLiteralNode.cs
│       ├── StringLiteralNode.cs
│       ├── BoolLiteralNode.cs
│       ├── NullLiteralNode.cs
│       ├── ArrayLiteralNode.cs
│       ├── IndexAccessNode.cs
│       ├── MemberAccessNode.cs
│       ├── MethodCallNode.cs
│       ├── FunctionCallNode.cs
│       ├── NewExprNode.cs
│       ├── CastExprNode.cs
│       ├── TypeCheckNode.cs
│       ├── TypeAssertNode.cs
│       ├── ConditionalExprNode.cs
│       ├── LambdaExprNode.cs
│       ├── ListComprehensionNode.cs
│       ├── ParameterNode.cs
│       └── TypeNode.cs
│
├── TinyLanguage.Interpreter/
│   ├── TinyLanguage.Interpreter.csproj
│   ├── IInterpreter.cs
│   ├── Interpreter.cs
│   ├── Scope.cs
│   └── InterpreterException.cs
│
├── TinyLanguage.UnitTests/
│   ├── TinyLanguage.UnitTests.csproj
│   ├── LexerUnitTests.cs
│   └── ParserUnitTests.cs
│
├── TinyLanguage.IntegrationTests/
│   ├── TinyLanguage.IntegrationTests.csproj
│   └── InterpreterIntegrationTests.cs
│
└── TinyLanguage.DemoFiles/                 # Project subdirectory of the solution.
    │                                        # Absolute path:
    │                                        #   Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DemoFiles\
    │                                        # NEVER outside the solution folder.
    ├── TinyLanguage.DemoFiles.csproj        # SDK-style content-only csproj per Build.Solution.md §1.2.7
    │                                        # (NOT a SharedProject .shproj — that fragility was retired.)
    │
    ├── TinyLanguage.exe                     # ~36 MB self-contained single-file build,
    │                                        # placed here by the AfterTargets="Publish" copy target
    │                                        # in TinyLanguage/TinyLanguage.csproj. Present after
    │                                        # `dotnet publish TinyLanguage -c Release`.
    │
    ├── 00001.fizzbuzz.tlg            # Tier A — feature-coverage demos
    ├── 00001.fizzbuzz.cmd            #          (00001..00399, one feature each, ≤ ~15 lines)
    ├── 00002.fibonacci.tlg
    ├── 00002.fibonacci.cmd
    ├── ... (≥ 250 Tier A pairs)
    │
    ├── 00410.linked_list_singly.tlg  # Tier B — advanced data structure demos
    ├── 00410.linked_list_singly.cmd  #          (00400..00499+, ≥ 50 pairs, 30–200 lines each)
    ├── 00420.stack_balanced_brackets.tlg
    ├── 00420.stack_balanced_brackets.cmd
    ├── 00425.bst_inorder_traversal.tlg
    ├── 00425.bst_inorder_traversal.cmd
    ├── 00440.heap_priority_queue.tlg
    ├── 00440.heap_priority_queue.cmd
    ├── 00450.hash_map_chaining.tlg
    ├── 00450.hash_map_chaining.cmd
    ├── 00460.graph_bfs_dfs.tlg
    ├── 00460.graph_bfs_dfs.cmd
    └── ... (≥ 50 Tier B pairs — see Build.md Phase 1D for required coverage)
```

> **Path discipline** — the orchestration agents must use the canonical absolute
> path `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` for every file operation, never a
> relative `..\` form. Inside a git worktree, a relative `..\` resolves to the
> wrong location (typically a sibling of the worktree, not a sibling of the
> orchestrator repo). The Phase 5 delivery step (Build.md Phase 5 Step 6) is the
> only place a robocopy from the merge worktree to the canonical path is
> permitted — and even there, the source and destination are both spelled out
> as full absolute paths, with explicit verification that the canonical path
> ends up holding a buildable solution.
