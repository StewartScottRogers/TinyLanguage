# TinyLanguage — Claude Code Orchestration Plan

This file drives a **Claude Code multi-agent build** of the TinyLanguage solution.
The canonical specification lives in `Build.Solution.md` — treat it as READ-ONLY.

> ## Output location — non-negotiable
>
> The generated .NET solution folder is delivered to a **fixed canonical absolute path**
> that is a **sibling of this orchestrator repo**, never inside it and never inside a
> git worktree. Concretely:
>
> - Orchestrator repo:  `Z:\repos\TinyLanguage\`            *(this checkout)*
> - Final solution:     `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`   *(its sibling)*
>
> The solution folder is **not tracked** by this orchestrator repo's git history. It
> is a standalone, self-contained .NET solution that an end user can `dotnet build`,
> `dotnet test`, and ship without any reference back to this orchestrator. You can
> verify by running `git status` from inside the orchestrator repo after delivery —
> nothing under `..\TinyLanguage.YYYY.MM.DD.HH\` should appear, because that path is
> outside the repo's working tree entirely.
>
> ### .NET solution structural standards (enforced)
>
> The solution folder MUST conform to the following layout — these are standard .NET
> conventions and the build, test, and publish commands assume them. Any layout
> deviation is a phase failure:
>
> 1. `TinyLanguage.slnx` lives at the **root** of the solution folder. Solution-level
>    files (`Directory.Build.props`, `.vscode/launch.json`) sit alongside it.
> 2. Every project lives in its **own subdirectory** at the solution root, named the
>    same as the project. Each project's `.csproj` is inside its own subdirectory.
> 3. **`TinyLanguage.DemoFiles/` is a project subdirectory of the solution folder** —
>    NOT a sibling of the solution, NOT inside one of the source projects, NOT in
>    `bin/`, NOT in any user temp/AppData path. Its absolute path is exactly
>    `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DemoFiles\`. The `.tlg` files,
>    matching `.cmd` files, the published `TinyLanguage.exe`, and the
>    `TinyLanguage.DemoFiles.csproj` are all inside that one directory.
> 4. Project-to-project `<ProjectReference>` paths use **solution-relative paths**
>    (e.g. `..\TinyLanguage.Lexer\TinyLanguage.Lexer.csproj`), never absolute paths
>    and never paths that escape the solution folder.
> 5. Per-project `bin/` and `obj/` directories are the default .NET convention — do
>    not redirect output to a centralised `artifacts/` folder unless the spec requires
>    it. Build artefacts are gitignore-able; nothing under `bin/` or `obj/` ever needs
>    to be hand-edited.
> 6. The solution is **self-contained**: every reference, every Content item, every
>    `<Copy>` in MSBuild targets resolves to a path under the solution root. No
>    target may reach into the orchestrator repo or into any agent worktree.
>
> ### Why this matters (why prior runs got it wrong)
>
> Worktree-isolated agents ran with their CWD inside a git worktree
> (`Z:\repos\TinyLanguage\.claude\worktrees\agent-<id>\`) and naively interpreted
> "sibling of the repo" relative to the worktree, producing a solution at
> `Z:\repos\TinyLanguage\.claude\worktrees\agent-<id>\TinyLanguage.YYYY.MM.DD.HH\`.
> That path is buried inside the orchestrator repo, gets cleaned up on worktree
> removal, and is invisible to a user looking at `Z:\repos\`. The fix has two parts,
> both required:
>
> a) Every agent prompt that mentions the solution path uses the **absolute canonical
>    path** `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` — never a relative `..\` form,
>    never "sibling of the repo" without naming the path.
> b) Phase 5 ends with an explicit **delivery step** that copies the assembled
>    solution from the merge worktree to the canonical absolute path and verifies
>    the canonical path contains a buildable solution. The orchestration is NOT
>    complete until that delivery step passes.

---

## Deliberate deviations from Build.Solution.md

`Build.Solution.md` is the locked specification, but a small number of points
have been deliberately overridden by the project owner. Phase 1D, Phase 4B, and
Phase 5 prompts below already incorporate these overrides. They are recorded
here so that if a future run consults Build.Solution.md directly, the deltas
are not silently re-introduced.

### 1. `TinyLanguage.exe` has no demo mode

`Build.Solution.md` §"Console Application (TinyLanguage.exe)" describes a
"Demo mode (no arguments)" that walks `TinyLanguage.DemoFiles\` and prints
`All demos completed successfully.` That is a test/demo harness, not part of
the language interpreter — the project owner removed it. The exe has exactly
two modes:

  - `TinyLanguage.exe`                         → read source from stdin, write to stdout
  - `TinyLanguage.exe <input.tlg> <output.txt>` → read input file, write output file

Anything else prints a usage line to stderr and exits 1. There is no
`Console.IsInputRedirected` branching — zero args is unconditionally stdin
mode (this avoids the non-TTY shell trap where `IsInputRedirected` returns
true and the exe falls into a piped-from-empty-stdin path).

### 2. The demo-walking job lives in `run-all-demos.cmd`

The "iterate every `.tlg`, print banners, finish with `All demos completed
successfully.`" behavior moves into a stand-alone aggregator script:

  `TinyLanguage.DemoFiles\run-all-demos.cmd`

Phase 1D produces this file alongside the per-demo `.cmd` files. Phase 5's
validation calls it instead of `dotnet run --project TinyLanguage` for the
end-to-end demo check.

### 3. Phase 5 acceptance shifts accordingly

Phase 5 Step 3 (formerly: `dotnet run --project TinyLanguage` prints
"All demos completed successfully.") becomes:
`TinyLanguage.DemoFiles\run-all-demos.cmd` prints "All demos completed
successfully." and exits 0. The per-`.cmd` validation loop in Step 5 is
unchanged — those scripts use file mode (two args).

### 4. Interactive debugger via Debug Adapter Protocol (additive)

`Build.Solution.md` does not describe a debugger. The project owner asked
for one targeted at VS Code today and growing toward a Turbo-Pascal-grade
experience over time. The adopted architecture is **Debug Adapter Protocol
(DAP)** so that the same engine drives VS Code, Visual Studio, JetBrains,
Neovim, etc.

Three layers (built by Phases 4C and 4D below):

  1. **Engine** — `IDebuggerHost` interface + `DebuggerControl` enum +
     statement-boundary callback in `TinyLanguage.Interpreter.Interpreter`.
     Zero overhead when no host is attached. Reusable for a future CLI REPL.
  2. **DAP adapter** — new project `TinyLanguage.DebugAdapter` that speaks
     DAP over stdin/stdout. Worker thread runs the interpreter; server
     thread handles VS Code requests; cooperative pause/resume via
     `BlockingCollection<DebuggerControl>`.
  3. **VS Code extension** — `vscode-extension\` directory at the solution
     root. Tiny CommonJS shim that registers debug type `tinylanguage`
     and points it at the published `TinyLanguage.exe --dap`.

Activation flag: `TinyLanguage.exe --dap` (single arg). Source comes from
the DAP `launch` request, not the command line. All other modes
(zero-args stdin / two-args file) are unchanged.

Future agents must NOT remove the debugger on the assumption "spec doesn't
mention it." It is a deliberate addition.

---

## How to Run This Plan

Open Claude Code in this directory and paste:

```
Read Build.md and execute the full orchestration plan using parallel agents.
```

Claude Code will decompose `Build.Solution.md` into the work units below, spin up
specialised sub-agents (some in parallel git worktrees), track progress with Tasks,
and assemble the final solution at the canonical sibling path defined above.

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

Output target: the **canonical absolute path** `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`,
which is a sibling of the orchestrator repo at `Z:\repos\TinyLanguage\`. Substitute
the actual UTC year/month/day/hour. Use this absolute path verbatim in every file
operation; do NOT use relative `..\` forms — those resolve to whatever the agent's
CWD happens to be (inside a git worktree, this gives the wrong location).

If you are running inside a git worktree (your CWD is something like
`Z:\repos\TinyLanguage\.claude\worktrees\agent-<id>\`), still write to the
canonical absolute path. The orchestrator repo's `.gitignore` excludes the
canonical path explicitly so you do not pollute the worktree's git index.

Create the full solution skeleton at `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` with
this layout (all paths relative to that solution root):

  TinyLanguage.slnx                                    ← solution file at the root
  Directory.Build.props                                ← solution-wide MSBuild props
  .vscode/launch.json                                  ← IDE debugger config
  TinyLanguage/TinyLanguage.csproj                     ← console (net10.0, self-contained single-file win-x64 exe)
  TinyLanguage.Lexer/TinyLanguage.Lexer.csproj         ← classlib (net10.0)
  TinyLanguage.Interpreter/TinyLanguage.Interpreter.csproj  ← classlib (net10.0)
  TinyLanguage.UnitTests/TinyLanguage.UnitTests.csproj      ← MSTest
  TinyLanguage.IntegrationTests/TinyLanguage.IntegrationTests.csproj  ← MSTest
  TinyLanguage.DemoFiles/TinyLanguage.DemoFiles.csproj      ← SDK-style content-only project,
                                                              **lives INSIDE the solution folder**
                                                              as a peer of the source projects.
                                                              Never outside, never under bin/,
                                                              never under any user temp path.

All `<ProjectReference>` entries in csproj files use solution-relative paths (e.g.
`..\TinyLanguage.Lexer\TinyLanguage.Lexer.csproj`). No absolute paths. No paths
that escape the solution folder.

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

The demo suite must be split into TWO TIERS, both produced by this phase:

  TIER A — FEATURE-COVERAGE DEMOS (00001..00399)
    Short, focused .tlg files that exercise individual grammar features in
    isolation (one feature per file). Goal: prove every BNF production and
    every Implementation Note is reachable from real source. These are the
    classic per-feature demos (e.g. 00001.fizzbuzz, 00010.if_statement,
    00026.function_no_params) — keep them small (≤ ~15 lines each) so a
    failure points at a single grammar feature.

  TIER B — ADVANCED DATA STRUCTURE & ALGORITHM DEMOS (00400..00499+)
    Substantial, multi-subroutine .tlg programs that BUILD an advanced data
    structure and EXERCISE it with a meaningful workload. These prove the
    interpreter holds up under real, idiomatic programs — not just one-liners.
    See the "Tier B requirements" block below for the full specification.

Both tiers together must total 300+ .tlg files in TinyLanguage.DemoFiles/,
zero-padded numeric prefix (00001.fizzbuzz.tlg … etc.). Each .tlg file must
have a matching .cmd runner using the template below.

### Tier B requirements — advanced data structures + subroutines

Each Tier B demo MUST satisfy ALL of the following:

1. **Defines a data structure as a class** (or, where a class is unsuitable,
   as a set of cooperating top-level functions over an array). The data
   structure must be the focus of the program, not a side-effect.
2. **Exposes at least three named subroutines** (functions or methods) that
   operate on the structure — e.g. `Insert`, `Remove`, `Find`, `Traverse`,
   `Size`. Inline procedural code with no functions is NOT acceptable for
   Tier B.
3. **Exercises the structure with a non-trivial workload** — at least 8
   distinct operations against the structure, mixing mutation and query.
4. **Prints a deterministic, asserted-against-able output sequence** that
   reflects each operation's result. The output must be reproducible byte-
   for-byte across runs (no clocks, no random numbers, no hash-order
   leakage). Phase 5's .cmd validation only checks "non-empty + exit 0";
   non-determinism would slip through and rot later.
5. **Uses at least four distinct grammar features beyond `let`/`print`** —
   pick from: classes, methods, `this`, recursion, `for`/`while`/`foreach`,
   arrays, array element assignment, exception handling, pattern matching,
   ternary, switch, lambdas, modules, default parameters, `break`/`continue`.
6. **File length: 30–200 lines.** Below 30 it is not "complex"; above 200
   becomes a maintenance liability and slows down `dotnet run` demo mode.
7. **Header comment** at the top of every Tier B .tlg file: one line naming
   the data structure, one line listing the subroutines defined, one line
   describing the workload. Example:
     # Data structure: singly linked list
     # Subroutines: PushFront, PushBack, Remove, Find, Length, Print
     # Workload: build list of 10 ints, remove odd values, print survivors

REQUIRED Tier B coverage — every item below must have at least one demo.
Pick filenames in the 00400+ range; names must be descriptive
(e.g. `00410.linked_list_singly.tlg`, `00425.bst_inorder_traversal.tlg`):

  Linear structures
  - Singly linked list (PushFront, PushBack, Remove, Find, Length, Print)
  - Doubly linked list (insert before/after, remove, forward+reverse traversal)
  - Stack (Push, Pop, Peek, IsEmpty) — exercised with balanced-bracket check
  - Queue (Enqueue, Dequeue, Peek) — exercised with FIFO ordering proof
  - Deque (PushFront, PushBack, PopFront, PopBack)
  - Ring / circular buffer (fixed capacity, wrap-around, overwrite policy)
  - Dynamic array / vector (Append, Get, Set, Remove, Resize)
  - LRU cache (doubly linked list + lookup index, Get/Put with eviction)

  Trees
  - Binary search tree (Insert, Find, InOrder traversal — sorted output)
  - BST deletion (the three cases: leaf, one child, two children)
  - AVL or red-black tree (self-balancing — show heights stay O(log n))
  - Min-heap / max-heap (Insert, ExtractMin/Max, Peek, Heapify)
  - Priority queue using the heap (proves item ordering by priority)
  - Trie (Insert, Contains, StartsWith over a small word list)
  - Segment tree or Fenwick tree (range sum / range update)

  Hashing & sets
  - Hash map with chaining (Put, Get, Remove, collision handling)
  - Hash set built on the hash map (Add, Contains, Remove)
  - Open-addressing hash table (linear or quadratic probing)

  Graphs
  - Graph as adjacency list (AddVertex, AddEdge, Neighbors)
  - BFS traversal (level-order from a source vertex)
  - DFS traversal (preorder + postorder)
  - Topological sort over a DAG
  - Connected components (undirected graph)
  - Dijkstra shortest path (small weighted graph, deterministic output)
  - Union-find / disjoint-set with path compression (Find, Union, Connected)

  Algorithms exercising the above
  - Quicksort, mergesort, heapsort (each as its own demo)
  - Binary search over a sorted array (iterative + recursive variants)
  - Sieve of Eratosthenes (returns primes ≤ N as a list)
  - Longest-common-subsequence DP (table-based, prints length and one LCS)
  - 0/1 knapsack DP
  - Edit distance (Levenshtein) DP
  - Reverse a string in-place using a stack
  - Palindrome check using a deque
  - Postfix evaluator (operator stack)
  - Infix → postfix (shunting-yard) using stacks
  - JSON-ish pretty-printer over nested arrays + maps (recursive)

A recommended Tier B file count is ≥ 50 (out of the 300+ total). Anything
on this list that the language cannot express should be flagged with a
comment in the demo file (`# NOT IMPLEMENTABLE: <reason>`) AND the demo
omitted — do not invent a watered-down replacement that no longer
exercises the data structure.

Anti-patterns specific to Tier B that have shipped before and must not recur:
- A "linked list" demo that is just `arr := [1,2,3]; print arr` with no
  Node class — that is a Tier A array-literal demo, not a Tier B demo.
- A "BST" demo whose only operation is `Insert` — query and traversal
  must also be exercised, otherwise correctness is unobservable.
- Subroutines defined but never called — Tier B is about exercise, not
  definition.
- Non-deterministic output (e.g. iterating a hash map by insertion order
  on one platform but by bucket order on another). If the structure is
  inherently unordered, sort the keys before printing.
- A demo that prints "ok" with no values — the printed output must let
  a reader reconstruct what the structure did, not just claim success.

Create a matching .cmd runner for each demo file (both tiers).

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

Also produce ONE additional aggregator script in TinyLanguage.DemoFiles/:

  run-all-demos.cmd

This replaces the demo-mode behaviour that USED to live inside TinyLanguage.exe
(see "Deliberate deviations from Build.Solution.md" at the top of this file —
the exe is now interpreter-only). The aggregator must:

- Walk every *.tlg in its own directory in alphabetic order (use
  `dir /b /a-d /on "%DEMO_DIR%*.tlg"` — the bare `for %%F in (...)` form does
  NOT guarantee alphabetic order on Windows).
- For each demo: print `=== <filename> ===`, run it through
  %~dp0TinyLanguage.exe in file-processor mode (writing to a single reusable
  temp output path), `type` the output, then print `--- end <filename> ---`.
- After the loop, print exactly: `All demos completed successfully.` and exit 0.
- On any non-zero exe exit during the loop, print a `run-all-demos: <demo>
  failed with exit code N` diagnostic to stderr and exit with the same code.
- Pre-flight that %~dp0TinyLanguage.exe exists; if missing, print a stderr
  message instructing `dotnet publish TinyLanguage -c Release` and exit 1.
- CWD-independent — must work from any directory or by double-click. Use
  %~dp0 and %TEMP% throughout.
- Use `setlocal enabledelayedexpansion` so `!errorlevel!` works inside the
  for-loop body.

Correct template:

  @echo off
  setlocal enabledelayedexpansion
  set "DEMO_DIR=%~dp0"
  set "EXE=%DEMO_DIR%TinyLanguage.exe"
  set "TMPOUT=%TEMP%\tinylanguage_run_all_demos.out.txt"

  if not exist "%EXE%" (
      echo TinyLanguage.exe not found at %EXE% 1>&2
      echo Run: dotnet publish TinyLanguage -c Release 1>&2
      exit /b 1
  )

  for /f "usebackq delims=" %%F in (`dir /b /a-d /on "%DEMO_DIR%*.tlg"`) do (
      echo === %%F ===
      "%EXE%" "%DEMO_DIR%%%F" "%TMPOUT%"
      if errorlevel 1 (
          echo run-all-demos: %%F failed with exit code !errorlevel! 1>&2
          exit /b 1
      )
      type "%TMPOUT%"
      echo --- end %%F ---
  )

  echo All demos completed successfully.
  exit /b 0

Do NOT run any of them yet — the interpreter is not built.
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
Read Build.Solution.md "Console Application (TinyLanguage.exe)" for context, but
follow the override at the top of Build.md ("Deliberate deviations from
Build.Solution.md") for the actual behaviour. Build.Solution.md's "Demo mode
(no arguments)" is REMOVED in this project — the exe is interpreter-only and
the demo-walking job lives in TinyLanguage.DemoFiles\run-all-demos.cmd.

Implement TinyLanguage/Program.cs with exactly two modes:

  TinyLanguage.exe                            → read source from stdin,
                                                 write program output to stdout
  TinyLanguage.exe <input.tlg> <output.txt>   → read source from input file,
                                                 write program output to output file

Anything else (one arg that isn't part of a recognised pattern, or 3+ args)
prints a usage line to stderr and exits 1. Errors → stderr, exit 1; success → 0.

Do NOT branch on Console.IsInputRedirected for the no-args case. Empirically
that returns true in non-TTY shells (CI runners, IDE consoles, the
orchestration's Bash harness), surrendering the no-args slot whenever the
host doesn't give the exe a real terminal. Zero args is unconditionally
"read from stdin"; piped input via shell redirection (`exe < file.tlg`)
falls into that path naturally.

Do NOT add a `LocateDemoDirectory`, do NOT walk TinyLanguage.DemoFiles\, do
NOT print `=== filename ===` banners, do NOT print "All demos completed
successfully." Those concerns belong to run-all-demos.cmd, not Program.cs.

Run: dotnet build
Accept: 0 errors, 0 warnings.

Then publish the single-file exe and verify it was copied to DemoFiles:
  dotnet publish TinyLanguage -c Release
  (TinyLanguage.exe must appear in TinyLanguage.DemoFiles/ — self-contained,
   ~36 MB, no runtime required)

Smoke-test:
  echo print "hi" | TinyLanguage.exe                     → prints "hi", exit 0
  TinyLanguage.exe TinyLanguage.DemoFiles/00001.hello_world.tlg out.txt
                                                          → out.txt contains "Hello, World!", exit 0
  TinyLanguage.exe foo                                    → usage on stderr, exit 1
```

---

## Phase 4C — Debugger Engine + DAP Adapter  *(starts after Phase 3A + 4B)*

**Agent:** `general-purpose`
**Isolation:** worktree
**Model:** `claude-opus-4-6`  *(cooperative threading + DAP wire protocol)*
**Prompt:**
```
Read Build.md "Deliberate deviations from Build.Solution.md" item 4 — the
debugger is an additive feature. Build.Solution.md says nothing about
debugging; do NOT take its silence as a reason to skip the work.

Architecture: TinyLanguage.exe speaks the Debug Adapter Protocol (DAP) so
VS Code (and other DAP clients) can drive an interactive debugger. Three
layers: engine (IDebuggerHost in TinyLanguage.Interpreter), DAP adapter
(new TinyLanguage.DebugAdapter project), console flag (--dap in
TinyLanguage\Program.cs). The VS Code extension is Phase 4D.

# Six confirmed design decisions (do NOT relitigate)

1. VS Code extension lives at the solution root: vscode-extension\.
2. Activation flag: --dap (single arg). Source comes from the DAP launch
   request, not the command line.
3. Stop on entry by default. launch.json's stopOnEntry wins if specified.
4. Conditional breakpoints AND logpoints both ship in v1.
5. Pause-while-running ships in v1 (single check in OnStatementBefore).
6. No CLI REPL in v1. Engine API must be reusable for a future CLI front-end.

# WU-A — Engine additions to TinyLanguage.Interpreter

Add files in Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.Interpreter\:

- IDebuggerHost.cs (public interface):
    DebuggerControl OnStatementBefore(StatementContext context);
    void OnFunctionEnter(string functionName, int line);
    void OnFunctionExit(string functionName);
    DebuggerControl OnUnhandledException(InterpreterException ex, StatementContext context);

- DebuggerControl.cs (public enum):
    Continue, StepIn, StepOver, StepOut, Pause, Restart, Quit

- StatementContext.cs (public sealed class):
    AstNode Node, int Line, Scope Scope, IReadOnlyList<DebuggerStackFrame> CallStack

- DebuggerStackFrame.cs (public sealed class):
    string FunctionName, int Line, Scope LocalScope

- DebuggerRestartException.cs, DebuggerQuitException.cs (internal sealed):
    Used to unwind the recursive interpreter when host returns Restart/Quit.

- AssemblyInfo.cs:
    [assembly: InternalsVisibleTo("TinyLanguage.DebugAdapter")]
    The DAP host needs to type-test against InstanceValue/FunctionValue and
    catch DebuggerRestartException/DebuggerQuitException without expanding
    the interpreter's public API surface.

Modify Interpreter.cs:

- Add public property `IDebuggerHost DebuggerHost { get; set; }` (default null).
  When null, the hot path is unchanged: ONE null-check per statement, no
  StatementContext/StackFrame allocations, no behavioural difference. The
  existing 274 unit + 72 integration tests must continue to pass.
- Maintain List<DebuggerStackFrame> _debugCallStack (only when host != null).
  Push on function/method/lambda enter (inside InvokeFunction); pop on exit.
- Push a synthetic <global> frame in RunWithDebugger so stackTrace at a
  program-root pause always returns at least one frame.
- Before EVERY statement-level Visit (ProgramNode body, StatementListNode body,
  each individual statement Visit), if DebuggerHost != null, build
  StatementContext and call host.OnStatementBefore. Process the returned
  DebuggerControl: Continue → run; Restart → throw DebuggerRestartException;
  Quit → throw DebuggerQuitException; StepIn/StepOver/StepOut/Pause are
  decisions the HOST makes per-statement (engine just reports — the host
  owns the entire step-mode state machine). Engine does NOT track step state.
- Add public method `object Evaluate(string expression, Scope scope)`:
  lex, parse-as-expression, evaluate in the given scope without mutating
  the interpreter's stdout/stdin. Used by DAP evaluate, conditional
  breakpoints, logpoint message interpolation.
- Add public method `void RunWithDebugger(AstNode program, IDebuggerHost host,
  TextWriter stdout, TextReader stdin)` as the debugger entry point.

Modify Scope.cs:

- Add `IReadOnlyDictionary<string, object> LocalBindings { get; }` (or
  IEnumerable<KeyValuePair<string,object>>) so DAP variables requests can
  list a scope's own bindings without walking the parent chain. (Walking
  parents into a function frame would dump globals into every locals view.)

# WU-B — TinyLanguage.DebugAdapter (new project)

Create Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DebugAdapter\TinyLanguage.DebugAdapter.csproj:
- net10.0 classlib, no implicit usings, no nullable
- ProjectReferences: TinyLanguage.Interpreter, TinyLanguage.Lexer

Add to TinyLanguage.slnx between TinyLanguage.Interpreter and
TinyLanguage.UnitTests.

Files:

- DebugAdapterServer.cs — public static class with:
    int RunOnStdInOut() — convenience overload reading Console.OpenStandardInput / OpenStandardOutput
    int Run(Stream input, Stream output) — testable overload
  Returns 0 on clean disconnect, 1 on unrecoverable error.

- DapMessageReader.cs / DapMessageWriter.cs — Content-Length\r\n\r\n<JSON>
  framing over UTF-8. Use System.Text.Json (BCL).

- DapHost.cs — implements IDebuggerHost. Cross-thread bridge:
    Owns Dictionary<int, BreakpointInfo> for breakpoints (line → condition + logMessage).
    OnStatementBefore decides pause iff: pause-flag set, stopOnEntry pending,
    step state matches (host owns this state), or breakpoint at this line
    AND condition truthy (evaluate via Interpreter.Evaluate). For logpoints,
    interpolate {expr} substrings, emit Output event, do NOT pause.
    Uses BlockingCollection<DebuggerControl> for command queue from server thread.
    volatile bool _pauseRequested for the pause-while-running flag.

- BreakpointInfo.cs — { line, condition?, logMessage? }
- VariableHandle.cs — variablesReference allocation + lookup

DAP requests handled (dispatch by command):
  initialize, launch, setBreakpoints, configurationDone, threads,
  stackTrace, scopes, variables, evaluate, continue, next, stepIn,
  stepOut, pause, restart, disconnect

DAP events emitted:
  initialized, stopped (reasons: entry, step, breakpoint, pause, exception),
  continued, output (categories: stdout, stderr), terminated, exited

DAP capabilities advertised in initialize response:
  supportsConfigurationDoneRequest, supportsConditionalBreakpoints,
  supportsLogPoints, supportsEvaluateForHovers, supportsRestartRequest,
  supportsTerminateRequest

The launch request accepts BOTH program (path to .tlg, the production
contract VS Code uses) AND source (inline source string, used by
integration tests so they don't need a temp file). Document this.

Threading model: caller of RunOnStdInOut runs the DAP server on its own
thread. A worker thread runs Interpreter.RunWithDebugger. They communicate
via BlockingCollection<DebuggerControl> (server → worker) and DapHost's
internal locks (worker → server when emitting stopped events). Server
thread synchronously queries DapHost state for stackTrace/scopes/variables/
evaluate — safe because the worker is paused at this point.

Filter built-in sentinels (len, str, int, bool, float) from the variables
view at the program-root scope; otherwise the user's locals are buried in
noise.

# WU-C — Console flag

Modify Program.cs:
- ProjectReference TinyLanguage → TinyLanguage.DebugAdapter (csproj edit).
- Add --dap flag handling AT THE TOP of Main, before the existing zero-args
  / two-args branches:
    if (args.Length == 1 && args[0] == "--dap") {
        return DebugAdapterServer.RunOnStdInOut();
    }
- Update the usage message to mention --dap.
- Do NOT regress the existing modes (stdin / file).

# Acceptance for Phase 4C

From the canonical solution path:
  dotnet build           → 0 errors, 0 warnings
  dotnet test            → Failed: 0 (numbers > 346 — the existing tests
                                       plus debugger tests added in WU-E
                                       below)
  dotnet publish TinyLanguage -c Release    → ~36 MB exe in DemoFiles\
  TinyLanguage.DemoFiles\run-all-demos.cmd  → still prints
                                               "All demos completed
                                               successfully.", exit 0

DAP smoke test (from solution root):
  Pipe a Content-Length-framed JSON `initialize` request into
  TinyLanguage.exe --dap; verify the response is a valid JSON object with
  "type":"response", "command":"initialize", "success":true, body containing
  capabilities; verify an `initialized` event is emitted.

# WU-E — Tests (delivered with this phase)

In TinyLanguage.UnitTests:
- DebuggerEngineUnitTests.cs (~9 tests):
  Breakpoint on line N pauses; StepIn pauses every statement; StepOver
  skips function bodies; StepOut runs until depth decreases;
  Evaluate(expr, scope) reads locals + arithmetic + member access;
  DebuggerControl.Restart unwinds cleanly; null host = zero overhead.
  Use a RecordingHost test helper (IDebuggerHost) that records calls and
  returns scripted DebuggerControl values from a queue.

In TinyLanguage.IntegrationTests:
- DebugAdapterIntegrationTests.cs (~7 tests): in-process DAP via Stream
  pairs (don't spawn the exe). Test cases: initialize handshake; launch +
  configurationDone with stopOnEntry; continue runs to completion;
  setBreakpoints + continue stops at breakpoint; stackTrace; variables;
  evaluate. Every test uses the launch.source overload to embed source
  inline (no temp .tlg files).

Every test prints input/result via Console.WriteLine.

# Reporting
Report:
- The exact dotnet build summary line
- The exact dotnet test summary lines
- Files added (counts + key paths)
- Files modified with one-line justifications
- DAP requests + events implemented (lists)
- Any decisions you made that weren't explicit in this prompt
- Known limitations / TODOs
```

---

## Phase 4D — VS Code Extension  *(starts after Phase 4C; small)*

**Agent:** `general-purpose`
**Isolation:** worktree
**Prompt:**
```
Phase 4C is done — TinyLanguage.exe --dap speaks DAP correctly. Your job
is the thin VS Code extension that lets users F5-debug a .tlg file.

Output target: Z:\repos\TinyLanguage.YYYY.MM.DD.HH\vscode-extension\
This is a peer of the source projects at the solution root. It ships with
every regenerated solution.

Files:

- package.json — the extension manifest. Required keys:
    name: "tinylanguage-debug"
    displayName: "TinyLanguage Debugger"
    version: "0.1.0"
    publisher: "tinylanguage-local"
    engines.vscode: "^1.70.0"
    categories: ["Debuggers"]
    main: "./extension.js"
    activationEvents: ["onDebug"]
    contributes:
      languages: [{ id:"tinylanguage", extensions:[".tlg"], aliases:["TinyLanguage"] }]
      debuggers: [{
        type: "tinylanguage",
        label: "TinyLanguage",
        languages: ["tinylanguage"],
        configurationAttributes.launch.required: ["program"],
        configurationAttributes.launch.properties.program: { type:"string", default:"${file}" },
        configurationAttributes.launch.properties.stopOnEntry: { type:"boolean", default:true },
        initialConfigurations: [
          { type:"tinylanguage", request:"launch", name:"Debug TinyLanguage program", program:"${file}", stopOnEntry:true }
        ]
      }]

- extension.js — minimal CommonJS. Activates on debug. Registers a
  DebugAdapterDescriptorFactory for type "tinylanguage" that resolves to
  TinyLanguage.exe at ${workspaceFolder}/TinyLanguage.DemoFiles/TinyLanguage.exe
  with arg ["--dap"]. (Falls back to "TinyLanguage.exe" on PATH if no
  workspace folder.) Exports activate / deactivate.

- README.md — install instructions in 5 commands or fewer:
    cd vscode-extension
    npm install -g vsce
    vsce package
    code --install-extension tinylanguage-debug-0.1.0.vsix
  Plus a launch.json template the user can paste.

- .vscodeignore — minimal, just exclude .vscode/ and node_modules/

Also update .vscode/launch.json at the SOLUTION root (NOT the
vscode-extension's). Keep the existing two configurations for the
TinyLanguage console (created by Phase 1A); ADD a third configuration
for the TinyLanguage debug type:
  {
    "type": "tinylanguage",
    "request": "launch",
    "name": "Debug current .tlg file",
    "program": "${file}",
    "stopOnEntry": true,
    "preLaunchTask": "publish"
  }

Create .vscode/tasks.json (or add to existing) with a "publish" task that
runs `dotnet publish TinyLanguage -c Release` so the preLaunchTask resolves.

# Additional output: install-vscode-debugger.cmd at the solution root

Also create `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\install-vscode-debugger.cmd`
— a Windows .cmd installer the user can double-click from Explorer (or run
from any shell). It must be idempotent and run five steps:

  1. Verify prerequisites: `where dotnet`, `where node`, `where npm`, `where code`.
     Any missing → print which one + the install URL → pause + exit 1.
  2. `dotnet publish "%SCRIPT_DIR%TinyLanguage\TinyLanguage.csproj" -c Release`.
  3. If `where vsce` fails, `npm install -g vsce`. After install, re-check;
     fall back to `%APPDATA%\npm\vsce.cmd` if PATH hasn't picked up the new
     install in this shell session.
  4. `pushd "%SCRIPT_DIR%vscode-extension" && call "%VSCE%" package`. Captures
     errorlevel into a saved RC, popd's, then checks the saved RC.
  5. `call code --install-extension "%SCRIPT_DIR%vscode-extension\tinylanguage-debug-0.1.0.vsix" --force`.

Pause at the start (after printing the five-step plan) so a double-click user
can read it before committing. Pause on every error path so the cmd window
doesn't vanish when run from Explorer. Print a final "Done. Open a .tlg file
and press F5." message on success.

## CMD parser gotcha to avoid (this bit a prior run; do NOT repeat it)

When you write the prerequisite-check subroutine, do NOT use the natural

    :check_tool
    where %~1 >nul 2>&1
    if errorlevel 1 (
        echo ERROR: %~1 not found on PATH.
        echo   %~2
        exit /b 1
    )
    exit /b 0

structure with hint strings that contain parentheses. cmd substitutes `%~2`
at parse time, BEFORE counting block-delimiting parens. If `%~2` expands to
`https://nodejs.org (npm ships with Node)`, the literal `(` and `)` inside
make cmd's parser close the if-block early, leaving `exit /b 1` OUTSIDE the
if. Result: errorlevel 1 is returned even when the tool is on PATH, and the
"ERROR: ..." line never prints (because the if-block body never ran).

Two-part fix (apply both):

  (a) Refactor :check_tool to early-return so there's no if-block at all:

        :check_tool
        where %~1 >nul 2>&1
        if not errorlevel 1 exit /b 0
        echo.
        echo ERROR: %~1 not found on PATH.
        echo   %~2
        exit /b 1

  (b) Avoid parentheses in any string substituted into an if-block elsewhere
      in the script. Two specific spots that previously had this latent bug:
      the "(not on PATH yet in this shell)" diagnostic and the "(exit code N)"
      error. Use plain prose with hyphens or em-dashes instead. The bug is
      dormant in error-only paths but surfaces unpredictably; eliminate it
      by construction.

# Acceptance
- vscode-extension/package.json validates as JSON (jq . package.json works)
- README.md describes install in ≤ 5 commands
- .vscode/launch.json keeps existing dotnet F5 configs AND adds the new tinylanguage one
- .vscode/tasks.json has a "publish" task
- install-vscode-debugger.cmd exists at the solution root, is paren-safe per the gotcha above
- The installer end-to-end path runs to exit 0 on a machine that already has dotnet/node/npm/code installed (test it with `cmd /c install-vscode-debugger.cmd < NUL > log 2>&1` from a clean shell; expect exit 0 on first run AND on second run — idempotent)

# Reporting
- Files created (paths + line counts)
- Confirmation that JSON parses
- Confirmation that the installer ran end-to-end exit 0 on first AND second run
- Note any decisions that diverge from the spec above
```

---

## Phase 4E — Wiki Generation  *(starts after Phase 4D; small, parallel-safe with Phase 5 step 1)*

**Agent:** `general-purpose`
**Prompt:**
```
Generate the user-facing wiki for the TinyLanguage solution.

Read for context:
  Z:\repos\TinyLanguage\Build.Solution.md  (locked spec — sections 1–7)
  Z:\repos\TinyLanguage\Build.md           ("Deliberate deviations from Build.Solution.md")
  Z:\repos\TinyLanguage\Plan.md            (work-unit breakdown + final layout)
  Z:\repos\TinyLanguage\CLAUDE.md          (project conventions; cross-link from the wiki)

Output target: Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.wiki.md
(Substitute the actual canonical solution path. The file may already exist as a
0-byte stub — overwrite it.)

The wiki is a SINGLE self-contained markdown document covering:

  1. What is TinyLanguage — one-paragraph elevator pitch + the three execution
     modes (stdin / file / --dap).
  2. Language tour — runnable snippets covering: Hello World, variables (let/var/const),
     arithmetic + types, control flow (if/while/for/foreach/do-while), functions,
     lambdas, classes (incl. extends + Constructor + this), arrays, modules,
     exceptions (try/catch/finally/throw), pattern matching (every kind), built-in
     functions (len/str/int/bool/float), and truthiness rules.
  3. Quick reference card — a one-screen cheat sheet of operators and keywords.
  4. Demo suite — describe Tier A (00001..00399, feature coverage) and Tier B
     (00400..00499, advanced data structures) with a category table for Tier B,
     plus how to run them (per-demo .cmd and run-all-demos.cmd).
  5. Building and running — the four canonical commands; explain that .cmd
     demos require a prior dotnet publish.
  6. Debugging in VS Code — install vsce, package the extension, F5.
  7. Architecture — three-layer DAP design (engine / DAP adapter / editor shim),
     project layout, data-flow diagram, key design decisions (visitor pattern,
     linked-list scope chain, BCL-only).
  8. Implementation notes reference — table of the most-load-bearing notes
     (1, 2, 4, 5, 7, 8–9, 10, 11, 12, 13, 14, 16, 19, 20, 23) with a one-line
     gloss each. Cross-reference Build.Solution.md §4.1 for the full list.
  9. Test suite — table of test projects and counts; how to run a single class.
 10. Known limitations — the deliberate design decisions baked into the spec
     (no closure capture of caller locals; no bitwise ops; no map literals;
     single-threaded; no super() call). Frame these as design choices, not bugs.
 11. Regenerating the solution — point at Build.md and list every phase
     (0, 1A–1D parallel, 2, 3A–3B parallel, 4A–4B parallel, 4C, 4D, **4E (this
     wiki)**, 5).
 12. References — Build.Solution.md, Build.md, CLAUDE.md, Plan.md, the DAP spec.

Constraints:

- Style: factual, terse, code-heavy. No marketing language. No emoji.
- The wiki is read by humans, not by future Claude Code sessions — do NOT add
  agent prompts, work-unit IDs, or phase numbers inside the language tour.
- Use markdown tables for reference matter (operators, demo categories, test
  counts). Use fenced code blocks (```tinylanguage / ```bash / ```powershell)
  for runnable examples. Verify the snippets actually parse (lex them through
  the freshly-built TinyLanguage.exe — if a snippet fails to lex/parse, fix
  the snippet, do not "loosen" the example to hide a real bug).
- Keep the wiki under ~600 lines. It is a reference document, not a tutorial.
- When the wiki and Build.Solution.md disagree, the spec wins — say so
  explicitly at the top of the wiki.
- Cross-reference but do NOT duplicate Build.Solution.md content verbatim.
  The spec is the source of truth; the wiki is the user's on-ramp.

Acceptance:

- Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.wiki.md exists, > 200 lines,
  < 800 lines.
- Every fenced code block tagged ```tinylanguage parses cleanly through the
  built parser (run the lexer + parser smoke harness used in Phase 2 against
  each block, or write a quick ad-hoc check).
- The Demo Suite section's Tier B category table is consistent with the
  actual filenames in TinyLanguage.DemoFiles/.

Reporting:
- Final line count of TinyLanguage.wiki.md
- Number of code blocks (split by language tag)
- Confirmation that ```tinylanguage blocks all parse
- Any decisions that diverge from this prompt
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

### Step 3 — Run all demos (interpreter end-to-end)
  Build.Solution.md's "Demo mode (no arguments)" is removed in this project
  (see "Deliberate deviations from Build.Solution.md" at the top of Build.md).
  The end-to-end demo walk lives in TinyLanguage.DemoFiles\run-all-demos.cmd
  produced by Phase 1D.

  Step 3 requires the published exe from Step 4 — interleave Step 4 first if
  this is a fresh build:
    dotnet publish TinyLanguage -c Release      # populates DemoFiles\TinyLanguage.exe
    TinyLanguage.DemoFiles\run-all-demos.cmd

  Accept: prints "All demos completed successfully.", exit code 0.
  If any demo fails: read the error from stderr (run-all-demos prints
  `run-all-demos: <demo> failed with exit code N`), fix the interpreter or
  demo file, rebuild + republish, re-run.

### Step 4 — Release publish (single-file exe)
  dotnet publish TinyLanguage -c Release
  Accept: TinyLanguage.DemoFiles/TinyLanguage.exe is the freshly-published self-contained
  single-file build (~36 MB). Verify size > 30 MB:
    PowerShell: (Get-Item TinyLanguage.DemoFiles\TinyLanguage.exe).Length -gt 30000000
  Bash:        [ "$(stat -c%s TinyLanguage.DemoFiles/TinyLanguage.exe)" -gt 30000000 ]
  If size is ~160 KB instead, that is the framework-dependent apphost — check
  TinyLanguage.csproj has NOT regrown an AfterTargets="Build" copy target (see the
  csproj comments for the long history). The .cmd demos cannot work with the apphost.

### Step 4b — DAP smoke test
  Quick check that the published exe responds to a DAP `initialize` request.
  Skip this step if Phase 4C wasn't run (older solutions without a debugger).

  PowerShell:
    $body = '{"seq":1,"type":"request","command":"initialize","arguments":{"clientID":"smoke","adapterID":"tinylanguage","linesStartAt1":true,"columnsStartAt1":true,"pathFormat":"path"}}'
    $msg  = "Content-Length: $($body.Length)`r`n`r`n$body"
    $proc = Start-Process -FilePath "TinyLanguage.DemoFiles\TinyLanguage.exe" -ArgumentList "--dap" `
              -RedirectStandardInput "stdin.tmp" -RedirectStandardOutput "stdout.tmp" -PassThru -NoNewWindow
    # (Or use a more proper Stream approach — the integration tests in Phase 4C exercise
    # the full protocol, this is just a "does the exe answer" check.)

  Accept: stdout contains a JSON response with "command":"initialize","success":true
  followed by an "event":"initialized" event.

  If this fails: the --dap flag is not wired (Program.cs regression) or
  TinyLanguage.DebugAdapter is missing from the publish (csproj reference
  regression). Fix and republish.

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

### Step 6 — DELIVER to the canonical sibling path  *(non-negotiable final step)*

Up to this point the assembled solution may live inside a merge worktree at
`Z:\repos\TinyLanguage\.claude\worktrees\agent-<phase5-id>\TinyLanguage.YYYY.MM.DD.HH\`.
A worktree is **ephemeral orchestration scratch space**, not a deliverable. The
plan is NOT complete until the solution is at the canonical path
`Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` (sibling of the orchestrator repo) AND
that canonical copy has been re-validated end-to-end.

  6a. If the solution at the canonical sibling path already exists from prior
      work, REMOVE IT FIRST so the delivery is a clean copy. Use Remove-Item
      with -Recurse and -Force; do NOT delete anything outside that exact path.
        $canonical = "Z:\repos\TinyLanguage.YYYY.MM.DD.HH"
        if (Test-Path $canonical) { Remove-Item -Recurse -Force $canonical }

  6b. Copy the worktree's solution folder to the canonical path. Use robocopy
      because it is the standard Windows tool for reliable directory mirroring,
      it preserves timestamps, and it skips bin/ and obj/ cleanly:
        robocopy `
          "Z:\repos\TinyLanguage\.claude\worktrees\agent-<phase5-id>\TinyLanguage.YYYY.MM.DD.HH" `
          "Z:\repos\TinyLanguage.YYYY.MM.DD.HH" `
          /MIR /XD bin obj .vs /XF *.user
      robocopy returns 0–7 for success (1 = files copied, 0 = nothing to do).
      Treat exit code >= 8 as failure.

  6c. Verify the structural standards from the preamble hold at the canonical path:
        - $canonical\TinyLanguage.slnx                     exists
        - $canonical\Directory.Build.props                  exists
        - $canonical\TinyLanguage\TinyLanguage.csproj       exists
        - $canonical\TinyLanguage.Lexer\...csproj           exists
        - $canonical\TinyLanguage.Interpreter\...csproj     exists
        - $canonical\TinyLanguage.UnitTests\...csproj       exists
        - $canonical\TinyLanguage.IntegrationTests\...csproj exists
        - $canonical\TinyLanguage.DemoFiles\TinyLanguage.DemoFiles.csproj exists
        - $canonical\TinyLanguage.DemoFiles\*.tlg           ≥ 300 files
        - $canonical\TinyLanguage.DemoFiles\*.cmd           one per .tlg
        - $canonical\TinyLanguage.DebugAdapter\TinyLanguage.DebugAdapter.csproj exists (Phase 4C)
        - $canonical\vscode-extension\package.json          exists (Phase 4D)
        - $canonical\install-vscode-debugger.cmd            exists (Phase 4D)
        - $canonical\TinyLanguage.wiki.md                   exists, > 200 lines (Phase 4E)
      Fail loudly with a specific path if any of these is missing.

  6d. Re-run the full build, test, publish, and run-all-demos chain at the
      canonical path from a CLEAN shell (do not rely on any state still warm
      in the worktree). Note: publish must run BEFORE run-all-demos.cmd —
      the aggregator needs the published TinyLanguage.exe sitting inside
      DemoFiles\.
        cd Z:\repos\TinyLanguage.YYYY.MM.DD.HH
        dotnet build              # Accept: 0 errors, 0 warnings
        dotnet test               # Accept: Failed: 0
        dotnet publish TinyLanguage -c Release
        # Verify TinyLanguage.DemoFiles\TinyLanguage.exe is the ~36 MB self-contained build
        # at the canonical location — NOT inside any worktree.
        TinyLanguage.DemoFiles\run-all-demos.cmd
        # Accept: prints "All demos completed successfully.", exit 0

  6e. Re-run the .cmd validation loop from Step 5, this time pointed at
      `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DemoFiles\` (the canonical
      DemoFiles path). All N .cmd files must still pass criteria (a)–(d).

  6f. Confirm the orchestrator repo is unchanged: `git status` from
      `Z:\repos\TinyLanguage\` reports a clean working tree (modulo any
      orchestration metadata commits). The canonical solution path is OUTSIDE
      this repo's working tree, so it must not appear in `git status` output.

  6g. Sanity check: list the absolute paths so the user can see exactly where
      the deliverable lives. The expected output:
        Solution root:  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\
        Demo files:     Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DemoFiles\
        Published exe:  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.DemoFiles\TinyLanguage.exe
        User wiki:      Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.wiki.md

  6h. Confirm the wiki is present and non-trivial: `TinyLanguage.wiki.md` exists
      at the canonical solution root, is more than 200 lines, and references
      Build.Solution.md as the source of truth. If the file is missing or empty,
      Phase 4E was skipped — re-run Phase 4E before declaring complete.

All six steps must be green. Do not declare the plan complete until Step 6h
confirms the canonical path holds the buildable, fully-validated solution
plus the user wiki. Do not refactor unrelated code. Do not alter
Build.Solution.md.
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

// Phase 4E — single call, after 4D, before Phase 5 (wiki generation)
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 4E

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
| 4C — Debugger + DAP | `claude-opus-4-6` | Cooperative threading + DAP wire protocol |
| 4D — VS Code Extension | `claude-sonnet-4-6` | Small JSON + CommonJS shim |
| 4E — Wiki Generation | `claude-sonnet-4-6` | Synthesis + cross-referencing, no novel design |
| 5 — Validation | `claude-sonnet-4-6` | Fix-and-retry loop, targeted edits |

---

## Constraints (from Build.Solution.md — never override)

- `Build.Solution.md` is READ-ONLY. No agent may modify it.
- No external NuGet packages. BCL only.
- No nullable, no implicit usings.
- MSTest only (no XUnit, no NUnit).
- One type per file.
- All acceptance criteria must be green before the plan is complete.
