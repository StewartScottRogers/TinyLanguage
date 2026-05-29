# TinyLanguage — Claude Code Orchestration Plan

This file drives a **Claude Code multi-agent build** of the TinyLanguage solution.
The canonical specification lives in `Build.Solution.md` — treat it as READ-ONLY.

> **Last verified full run:** 2026-05-29 → delivered to
> `Z:\repos\TinyLanguage.2026.05.29.02\` — 520 demos (Tier A 00001–00340 / B ~55 / C 125),
> 370 tests (281 unit + 89 integration), all gates green including
> `devenv.com /Rebuild` (7 succeeded, 0 failed, no MAX_PATH). SDK pin was
> `10.0.300` (no 10.0.2xx band installed). For fuller feature coverage, extend
> Tier A to the full `00001–00399` band (the 2026-05-28 run reached 399). The
> deviations and build lessons discovered across runs are folded into the relevant
> phases/preambles below (search D16–D21, the "MSTest 4.x" and "Demo sweep harness"
> preambles, the `tools\sweep-demos.ps1` helper, "Demo convergence", "Execution
> model", and the SDK/long-path preambles).

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

> ## Execution model — subagents write to the canonical path directly
>
> Each phase subagent — **even when worktree-isolated** — writes its outputs to the
> canonical ABSOLUTE path `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\`, which lies OUTSIDE
> any repo or git worktree. Consequence: the assembled solution accumulates in ONE
> place as phases complete; there are no per-phase branches to merge.
>
> Therefore, in this execution model:
> - Phase 2's "Merge worktree outputs from Phase 1A/1B/1C" is a **no-op** — those
>   outputs are already co-located at the canonical path.
> - Phase 5 Step 6a/6b (remove canonical + robocopy from a merge worktree) are
>   **no-ops** — the solution is already AT the canonical path. Skip them and
>   validate in place (Steps 6c–6i).
>
> The merge/robocopy instructions remain below only as a FALLBACK for an alternate
> model where agents build INSIDE their worktree and the result must be copied out.
> If you ran agents in the direct-to-canonical model (the default here), just
> verify the solution in situ.

---

> ## Long-path support — non-negotiable
>
> The canonical solution path `Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` is 37
> characters before any project subdirectory. Combined with the deepest
> intermediate paths produced by a self-contained single-file
> `net10.0/win-x64` publish — for example
> `TinyLanguage.IntegrationTests\obj\Release\net10.0\win-x64\PubTmp\Out\runtimes\win-x64\native\<asset>` —
> the absolute path can exceed Windows' classic MAX_PATH limit (260 chars).
> `dotnet build` from a recent .NET SDK is long-path-aware and tolerates this,
> but **Visual Studio's Batch Rebuild dialog still fails** on these paths
> unless long-path support is explicitly enabled at three layers. All three
> are required; missing any one causes Batch Rebuild to fail with
> `The specified path, file name, or both are too long. The fully qualified
> file name must be less than 260 characters.`
>
> ### Layer 1 — Machine registry (one-time, requires admin)
>
> ```powershell
> reg add "HKLM\SYSTEM\CurrentControlSet\Control\FileSystem" /v LongPathsEnabled /t REG_DWORD /d 1 /f
> ```
>
> No reboot required, but Visual Studio must be restarted to pick it up. The
> install-vscode-debugger.cmd installer (Phase 4D) should verify this key and,
> if 0, WARN with the admin `reg add` command — but treat a clean Step 6i
> `devenv /Rebuild` as authoritative rather than hard-aborting on the registry
> value alone (see Phase 5 §6c).
>
> ### Layer 2 — Solution-level Directory.Build.props
>
> Phase 1A authors `Directory.Build.props` at the solution root. It must
> include:
>
> ```xml
> <Project>
>   <PropertyGroup>
>     <!-- Force MSBuild + tracker to use \\?\ long-path APIs on Windows. -->
>     <MSBuildEnableAllPropertyFunctions>true</MSBuildEnableAllPropertyFunctions>
>     <UseCommonOutputDirectory>false</UseCommonOutputDirectory>
>     <!-- Surface long paths to .NET SDK tasks. -->
>     <_LongPathsEnabled>true</_LongPathsEnabled>
>   </PropertyGroup>
> </Project>
> ```
>
> ### Layer 3 — Per-executable app.manifest
>
> The `TinyLanguage` console project (the only executable in the solution)
> ships with an `app.manifest` declaring long-path awareness so any path the
> exe itself manipulates at runtime — and any path Windows hands it via
> command-line arguments — uses the long-path API. Phase 1A authors:
>
> `TinyLanguage\app.manifest`:
> ```xml
> <?xml version="1.0" encoding="utf-8"?>
> <assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
>   <application xmlns="urn:schemas-microsoft-com:asm.v3">
>     <windowsSettings>
>       <longPathAware xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">true</longPathAware>
>     </windowsSettings>
>   </application>
> </assembly>
> ```
>
> `TinyLanguage\TinyLanguage.csproj` references it via:
> ```xml
> <PropertyGroup>
>   <ApplicationManifest>app.manifest</ApplicationManifest>
> </PropertyGroup>
> ```
>
> ### Why this matters (why prior runs got it wrong)
>
> A previous orchestration produced a buildable solution at the canonical
> path that `dotnet build` happily processed but that Visual Studio's
> Batch Rebuild dialog refused with MAX_PATH errors on the
> `TinyLanguage.IntegrationTests\obj\Release\net10.0\win-x64\PubTmp\...`
> chain. The user could neither single-step build via Batch Build nor
> right-click → Rebuild on the affected projects, blocking IDE-driven
> development entirely. The fix is structural: Phase 1A authors all three
> layers above so every regenerated solution is VS-batch-rebuildable from
> first commit. Phase 5 Step 6c verifies the three artefacts exist in the
> deliverable; if any is missing, delivery fails loudly.

---

> ## SDK pinning — non-negotiable
>
> Visual Studio's MSBuild + NuGet integration uses the .NET SDK that
> resolves at the solution root. If the agent shell publishes the
> deliverable with one SDK and Visual Studio later resolves a *different*
> SDK at the same path, lockfile incompatibility produces
> `MSB4018: ResolvePackageAssets task failed unexpectedly. NullReferenceException`
> on every project — independent of long-path concerns.
>
> Concretely on the development machine: dotnet on PATH was
> `10.0.300-preview.0.26177.108` and Visual Studio 18 resolved
> `10.0.203` by default. The publish wrote `obj/project.assets.json`
> in the preview SDK's format; VS18's older NuGet bits NRE'd reading it.
>
> ### The fix
>
> Phase 1A authors a `global.json` at the canonical solution root pinning the SDK
> to the **newest STABLE (non-preview, non-rc) .NET SDK actually installed on the
> build machine**. Determine it with `dotnet --list-sdks` — the literal version is
> a moving target, NOT a constant:
>
> ```json
> {
>   "sdk": {
>     "version": "<newest-stable-installed, e.g. 10.0.300>",
>     "rollForward": "latestPatch"
>   }
> }
> ```
>
> On the reference machine (last successful run, 2026-05) only `10.0.300` (stable)
> and `10.0.100-rc.1` were installed — there was **no `10.0.2xx` band at all** — so
> the pin was `10.0.300`. Do NOT hard-require the literal `10.0.203`: that band may
> not be installed, in which case a `10.0.203` pin fails outright (latestPatch can
> only roll *within* the 2xx band).
>
> The non-negotiable invariants are: **(a)** a STABLE build VS18 understands —
> never a `-preview`/`-rc` SDK; **(b)** `rollForward: latestPatch`, NEVER
> `latestFeature` (which rolls *up* across feature bands and can land on a
> `-preview` build that NRE's VS18's `ResolvePackageAssets`). If a newer stable
> band (10.0.4xx, 11.x, …) is installed in future, pin that.
>
> Phase 5 Step 6c verifies `global.json` exists at the canonical path, pins the
> newest installed stable band with `rollForward: latestPatch`, and that
> `dotnet --version` from that path reports a STABLE (non-preview, non-rc) SDK.
> Step 6c rejects only `-preview`/`-rc` builds — it must NOT require a specific
> `10.0.2xx` number.

---

> ## Test output formatting — non-negotiable
>
> Build.Solution.md requires every test print its input and result via
> `Console.WriteLine` so the test runner log shows what was exercised.
> The naive form — `Console.WriteLine($"Input: {source}")` followed by
> `Console.WriteLine($"Result: {Visible(actual)}")` with `Visible`
> escaping `\n` to `\\n` — produces output that's unreadable for any
> non-trivial multi-line result:
>
> ```
> Input: 00277.repeat_print.tlg
> Source:
> for i := 1 to 5 do
>     print "tick"
> end
>
> Result: tick\ntick\ntick\ntick\ntick\n
> ```
>
> Every test instead routes through a small `TestLog` helper that prints
> sectioned, real-newline-indented output:
>
> ```
> --- Input: 00277.repeat_print.tlg ---
>   for i := 1 to 5 do
>       print "tick"
>   end
> --- Result ---
>   tick
>   tick
>   tick
>   tick
>   tick
> ---
> ```
>
> ### Required `TestLog.cs` (authored once per test project)
>
> Both `TinyLanguage.UnitTests/TestLog.cs` (authored by Phase 3B when it
> first introduces tests) and `TinyLanguage.IntegrationTests/TestLog.cs`
> (authored by Phase 4A) must contain the same class. Only the
> `namespace` line differs:
>
> ```csharp
> using System;
>
> namespace TinyLanguage.UnitTests   // or TinyLanguage.IntegrationTests
> {
>     public static class TestLog
>     {
>         private const string Indent = "  ";
>
>         public static void Input(string content) { Input(null, content); }
>
>         public static void Input(string label, string content)
>         {
>             Console.WriteLine();
>             if (string.IsNullOrEmpty(label)) Console.WriteLine("--- Input ---");
>             else Console.WriteLine("--- Input: " + label + " ---");
>             WriteIndented(content);
>         }
>
>         public static void Section(string label, string content)
>         {
>             if (string.IsNullOrEmpty(label)) Console.WriteLine("---");
>             else Console.WriteLine("--- " + label + " ---");
>             WriteIndented(content);
>         }
>
>         public static void Result(string content)
>         {
>             Console.WriteLine("--- Result ---");
>             WriteIndented(content);
>             Console.WriteLine("---");
>         }
>
>         private static void WriteIndented(string content)
>         {
>             if (content == null) { Console.WriteLine(Indent + "(null)"); return; }
>             if (content.Length == 0) { Console.WriteLine(Indent + "(empty)"); return; }
>             string normalised = content.Replace("\r\n", "\n");
>             int start = 0;
>             for (int index = 0; index < normalised.Length; index = index + 1)
>             {
>                 if (normalised[index] == '\n')
>                 {
>                     Console.WriteLine(Indent + normalised.Substring(start, index - start));
>                     start = index + 1;
>                 }
>             }
>             if (start < normalised.Length)
>                 Console.WriteLine(Indent + normalised.Substring(start));
>         }
>     }
> }
> ```
>
> ### Test-author rules (apply to Phases 3B, 4A, 4C, and any future test phase)
>
> 1. NEVER write `Console.WriteLine($"Input: {x}")` or `Console.WriteLine($"Result: {y}")`. Always go through `TestLog.Input(...)` / `TestLog.Result(...)`.
> 2. Multi-line content (source code, multi-line stdout, lists of tokens) goes into the helper as a single string with real `\n` separators — do NOT pre-escape `\n` to `\\n`. The helper handles indentation.
> 3. Composite results (e.g. debugger tests with multiple fields) format each field on its own line:
>    ```csharp
>    TestLog.Result("enters = [" + string.Join(",", host.FrameEnters) + "]\n"
>                 + "exits  = [" + string.Join(",", host.FrameExits) + "]");
>    ```
>    NOT a single `field1=...; field2=...; field3=...` blob.
> 4. Token-list helpers (`DescribeTokens` etc.) print one token per line, not a single comma-separated string.
> 5. Single-line inputs/results pass through the helper unchanged. The helper still wraps them in the section header so test rows are visually uniform when scanning a 500+ test log.

---

## MSTest 4.x — non-negotiable

The pinned .NET 10 SDK's `dotnet new mstest` template uses the **MSTest 4.0.2
meta-package** (it bundles the Microsoft.Testing.Platform runner — there is NO
separate `Microsoft.NET.Test.Sdk`) and emits a `MSTestSettings.cs` carrying an
assembly-level `[Parallelize]` attribute. Phases 3B, 4A, 4C (and any future test
phase) must honor three things or the `0-warning` / `Failed:0` gates break:

1. **Keep `MSTestSettings.cs` and pin Workers = 1.** Omitting the `[Parallelize]`
   attribute trips analyzer **MSTEST0001** (a warning → fails the 0-warning gate).
   Set it (fully-qualified, no implicit usings):
   `[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.Parallelize(Workers = 1, Scope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope.MethodLevel)]`
   Workers = 1 also prevents the flaky console-redirection / shared-Stream races the
   3B/4A/4C tests otherwise hit (6–25 intermittent failures depending on worker count).

2. **Use the MSTest 4.x assertion APIs.** Analyzer **MSTEST0037** rejects several
   classic asserts. Replacements: `Assert.HasCount` (not `AreEqual(n, x.Count)`),
   `Assert.Contains` / `Assert.DoesNotContain`, `Assert.IsGreaterThan` /
   `Assert.IsGreaterThanOrEqualTo`, `Assert.Throws<T>` (not `ThrowsException<T>`).

3. **Name clash:** the lexer type is `TinyLanguage.Lexer.Lexer`; alias it
   (`using LexerCore = TinyLanguage.Lexer.Lexer;`) in test files that also open the
   `TinyLanguage.Lexer` namespace.

Recorded so each test phase does not re-discover it — the 2026.05.29.02 run had
three separate phases independently hit MSTEST0001/0037.

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
  3. **VS Code extension** — `extensions\vscode\` directory under the
     Shared Project container (see item 6 / Phase 4F). Tiny CommonJS shim
     that registers debug type `tinylanguage` and points it at the
     published `TinyLanguage.exe --dap`.

Activation flag: `TinyLanguage.exe --dap` (single arg). Source comes from
the DAP `launch` request, not the command line. All other modes
(zero-args stdin / two-args file) are unchanged.

Future agents must NOT remove the debugger on the assumption "spec doesn't
mention it." It is a deliberate addition.

### 5. Visual Studio 2026 debugger extension (additive)

Parallel to the VS Code extension (item 4), the project owner asked for a
Visual Studio 2026 (VS18) editor shim that hosts the same `TinyLanguage.exe
--dap` server. It lives at `extensions/vs/`, a sibling of `extensions/vscode/`
inside the Shared Project container (see item 6 / Phase 4F). The VS18 shim
is a tiny `IAdapterLauncher` implementation
packaged as a `.vsix`; it plugs into VS's stock Debug Adapter Host engine
(`Microsoft.VisualStudio.Debugger.DebugAdapterHost.Interfaces`, ships with
VS18 at `Common7\IDE\Extensions\Microsoft\DebugAdapterHost\`). The pkgdef
registers a TinyLanguage-specific AD7Metrics engine GUID whose
`AdapterLauncher` field points at our launcher class CLSID, while reusing
the stock DAP-host engine CLSID. Build via `install-vs-debugger.cmd` at the
solution root; same five-step idempotent shape as
`install-vscode-debugger.cmd`. Same DAP server, same feature set
&mdash; feature parity with VS Code is by construction.

Future agents must NOT delete `extensions/vs/` on the assumption that "the
spec doesn't mention it" or that "the debugger is already covered by item
4". Both editors are deliberate.

### 6. Parser leniencies (D10–D17)

The 500+ demo corpus uses common scripting conventions that the strict
BNF in `Build.Solution.md` does not accept. Rather than mass-rewriting
the demos, the parser has been deliberately relaxed in six places. A
future regeneration that reverts any of these will see widespread demo
failures.

  **D10 — Newlines as implicit `;` separators.** Implementation Note 3
  says "Newlines are whitespace; they do not insert implicit semicolons."
  Relaxed: inside any `ParseStatementList`, if no explicit `;` was
  consumed AND the upcoming token sits on a strictly later source line
  than the last token of the just-parsed statement, the parser accepts
  it as the start of the next statement. Required because Phase 1D
  authors the Tier A demos with the one-statement-per-line convention.
  REFINEMENT (2026.05.29.02): as a corollary, the postfix parser must NOT consume a
  `[` (index) or `(` (call) that BEGINS on a strictly-later source line as part of
  the previous statement's expression — otherwise a bracket-led next statement
  (e.g. a `[a, b] => ...` match case after `print "x"`) gets swallowed, yielding
  "Expected ']' but found ','". A leading `.` member access MAY still continue
  across lines (`.` cannot start a statement). Fixed demo 00306.

  **D11 — Trailing `;` before block-enders tolerated.** Implementation
  Note 21 says `;` before `end`, `else`, `catch`, `finally`, `while`,
  `}`, or EOF is a parse error. Relaxed: trailing `;` is silently
  accepted as an empty separator (any number of consecutive `;`s acts
  as zero-or-more empty statements). Also tolerated at the START of a
  statement list — so `function foo();` (with stray `;` right after
  the closing paren of the param list) parses cleanly. Required for
  the demo convention and the mechanical statement-separator fix-up.

  **D12 — `else if` chains.** The BNF defines
  `<if_stmt> ::= "if" <expr> "then" <stmt_list> ("else" <stmt_list>)? "end"`
  which strictly requires nested ifs to have their own `end`. Relaxed:
  inside `ParseIfStatement`, if the token after `else` is `if`, recurse
  into a complete `if_stmt` and use it as the entire else-branch — the
  inner `if`'s terminating `end` closes the whole chain. So FizzBuzz's
  `if A then ... else if B then ... else ... end` parses with ONE `end`.
  CRITICAL refinement: the `else if` fold fires ONLY when the `if` token is on the
  SAME source line as `else`. An `if` on a LATER line after `else` is an ordinary
  nested if-statement that owns its own `end` — do NOT fold it into the chain. The
  naive "any `if` after `else`" rule consumes the nested if's `end` and orphans the
  outer `end`, breaking ~17 nested-if-in-else demos (DP tables, tree methods).
  FizzBuzz uses the same-line idiom and still parses with ONE `end`.

  **D13 — Member assignment and postfix-LHS assignment.** The BNF only
  defines `<id> := <expr>` and `<id>[expr] := <expr>`. There is no
  `obj.field := value` form. Three additive AST nodes fix this:

    * `MemberAssignStmtNode(receiver: List<string>, value, line)` —
      flat dotted-identifier LHS like `this.X := 5` or `a.b.c := 7`.
    * `PostfixAssignStmtNode(targetExpr, value, line)` — arbitrary
      postfix-expression LHS like `this.Items[i] := v` or
      `this.Data[i][j] := v`.
    * `PostfixCallStmtNode(callExpr, line)` — method-chain call
      statements like `this.Nodes[i].AddNeighbor(...)`.

    Implementation: `ParseStatement` dispatches both `Identifier` and
    `This` to a shared `ParsePostfixLedAssignOrCall`. It parses a full
    postfix expression, then on `:=` specialises down to
    `AssignStmtNode` / `ArrayElementAssignNode` / `MemberAssignStmtNode`
    / `PostfixAssignStmtNode` based on the LHS shape, or wraps a call
    expression in `CallStmtNode` / `PostfixCallStmtNode`. The
    interpreter walks the receiver chain through `InstanceValue.Fields`
    and `Dictionary<object,object>` for the simple `MemberAssign` form
    and uses generic expression evaluation for the postfix forms.

  **D14 — `var x := 1` (type-inferred `var`).** The BNF
  `<var_declare_stmt> ::= "var" <id> ":" <type> ":=" <expr>` requires
  the type annotation. Relaxed: type is optional (parallels `let`).
  Required because Tier A demos like `00030.var_declaration_typed.tlg`
  ship in both forms.

  **D15 — Dotted type names in `new` expressions.** The BNF
  `<new_expr> ::= "new" <id> "(" <arg_list>? ")"` accepts only a bare
  identifier. Relaxed: `ParseNewExpression` accepts a dotted chain
  (`new Shapes.Circle(5)`) and resolves the final segment against the
  class table. Required for module-qualified instantiation patterns
  that appear in several Tier C demos.

  **D16 — Built-in conversion call-syntax.** Build.Solution.md lists `int`/`str`/
  `bool`/`float` (and `len`) as built-in FUNCTIONS, but the lexer emits keyword
  tokens (IntType/FloatType/BoolType/StringType) for those names, so `int("7")`,
  `float(n)`, `bool(x)`, `str(v)` would not parse in expression position. Relaxed:
  in `ParsePrimary`, a type-name keyword immediately followed by `(` is parsed as a
  conversion call (canonical builtin name int/float/bool/str → FunctionCallNode →
  same conversion logic as the `(int)x` cast). Casts, annotations, and `is`/`as`
  are unaffected. ~32 demos depend on this.

  **D17 — Lambda body by first token.** A lambda/anonymous-function body is a
  single-expression body when its first token starts an expression, or a statement
  BLOCK (optionally closed by `end`) when its first token is a statement-only
  keyword. Decided by the first token, NOT by scanning for a matching `end` (which
  latches onto the enclosing function's `end`). Fixes `function(n) print n`.
  REFINEMENT (2026.05.29.02): the block-vs-expression decision uses the FULL
  statement-starting keyword set — return/print/if/while/for/foreach/let/var/const/
  throw/try/switch/match/break/continue/do — NOT a subset. In particular a body
  beginning with `return` is a statement block; the first implementation omitted
  `return` and broke `function() return 1 end` (demo 00171).

  **D20 — `export <definition>`.** The BNF is `<export_stmt> ::= "export" <id>`
  (bare identifier only). Relaxed: `ParseExportStatement` ALSO accepts a definition
  after `export` — `export function|class|static|let|var|const <definition>` — by
  parsing the inner definition (it lands in module/global scope, since modules
  promote exports to global with no qualified `M.f` access) and recording the
  export marker (a runtime no-op). The bare `export <id>` form still works.
  Required by module demos 00291–00295. Lives in `ParseExportStatement`.

  **D21 — `static` instance-style fields (`static let`/`static var`).** Extends D19
  (class `const` is static). The BNF only puts `static` on `<method_def>`. Relaxed:
  the class-member parser accepts `static` before `let`/`var`/`const` field
  declarations and routes them to the class's static members (reachable AND
  assignable as `ClassName.Field`); `FieldDeclareNode` gains an `IsStatic` flag.
  Implementation Note 16 ("static modifies a field"). Required by 00216, 00237.

  These ten relaxations (D10–D21) + the existing D1–D9 are the COMPLETE set of
  documented deviations from the BNF. Anything else in the parser
  matches the spec.

### 7. Lexer bug repaired in-place: `PeekIsDigit()` look-ahead

Phase 1B's initial implementation of `PeekIsDigit()` (the helper that
decides whether `.` starts a float fractional part) reads `CurrentChar`
— which AT THE POINT of the call IS the `.` itself, never a digit.
Decimal-point floats like `1.5` therefore lexed as `Integer('1')`,
`Dot('.')`, `Integer('5')` instead of `Float('1.5')`. Both Phase 3A
and Phase 3B agents independently flagged this from their respective
test work.

The fix is one line — use `Source.Peek()` which returns the character
AFTER `CurrentChar` without consuming it:

```csharp
private bool PeekIsDigit()
{
    int nextChar = Source.Peek();
    return nextChar != -1 && char.IsDigit((char)nextChar);
}
```

Phase 1B agents must implement `PeekIsDigit()` correctly from the
start so Phase 3B doesn't have to invent "documents the bug" tests
that later need to be rewritten when the bug is fixed.

---

## How to Run This Plan

Open Claude Code in this directory and paste:

```
Read Build.md and execute the full orchestration plan using parallel agents.
```

Claude Code will decompose `Build.Solution.md` into the work units below, spin up
specialised sub-agents (some in parallel git worktrees), track progress with Tasks,
and assemble the final solution at the canonical sibling path defined above.

### Phase sequencing — project references override the "parallel" labels

The diagrams show parallel fan-out, but `<ProjectReference>` edges and shared
MSBuild output directories make these orderings MANDATORY:
- **1A before 1B and 1C** — 1B/1C build the `TinyLanguage.Lexer` csproj that 1A
  scaffolds. (1A and 1D may run together; 1D is pure text authoring.)
- **3A before 3B** — `TinyLanguage.UnitTests` references `TinyLanguage.Interpreter`,
  so testing it requires the interpreter to compile.
- **4B before 4A** — `TinyLanguage.IntegrationTests` references the
  `TinyLanguage` console project; build/test of it requires the real Program.cs.
- **4E and 4F must not run concurrent `dotnet build`/`publish`/`devenv`** — two
  MSBuild processes on the same solution race on `obj/` locks. Run them
  sequentially (4E is light and read-only against the published exe; 4F is heavy).
- **1D is independent of everything** (authoring `.tlg`/`.cmd` text) — run it
  alongside any phase.

Launch a parallel pair in one message ONLY when they touch disjoint projects and
neither builds the other's project.

### Orchestration execution recipe (what worked — 2026.05.29.02 run)

The dependency graph is mostly sequential (project-reference edges + shared `obj/`
locks), but ONE big unit parallelizes cleanly and should overlap the whole chain:

- **Run Phase 1D (demos) as a BACKGROUND fan-out concurrent with the build chain.**
  Demos are pure text authoring with no build dependency until Phase 4.5, so start
  them immediately (they create the canonical DemoFiles dir themselves). Fan out:
  Tier A by FEATURE AREA (~7 agents over the FULL `00001–00399` band —
  basics/operators/control-flow/functions/classes/arrays+exceptions/modules+patterns),
  Tier B by 2 agents (00400–00449 structures, 00450–00499 algorithms), Tier C by the
  6 enumerated categories. Authors write **`.tlg` ONLY**; a single final deterministic
  step generates every `.cmd` from the `.tlg` set + `run-all-demos.cmd` (uniform, not
  500 hand-written runners). Aim Tier A at the full 00001–00399 band — the 2026.05.29.02
  run stopped at ~00340 and was lighter than it should have been.
- **Serialize every build-running phase** (1A → 1B → 1C → 2 → 3A → 3B → 4B → 4A →
  4.5 → 4C → 4D → {4E then 4F} → 5). Two `dotnet build`/`test`/`publish` processes on
  the same solution race on `obj/` locks and produce spurious failures — do NOT run
  two at once even when phase labels say "parallel". 4E (wiki; read-only against the
  published exe) and 4F (heavy MSBuild + devenv) must especially not build
  concurrently: run 4E first, then 4F.
- **Gate between phases** on the exact acceptance (`build 0/0`, `test Failed:0`, the
  sweep `FAILED=0 TIMEOUT=0`) so a cascading parser/interpreter bug is caught at the
  phase that introduced it, not three phases later.
- **Size the convergence problem with `tools\sweep-demos.ps1` FIRST** — it yields the
  true failure list. The 2026.05.29.02 run's real failure set was only ~13 demos (a
  few parser features + a few demo anti-patterns), trivial once the harness
  false-negatives (null `ExitCode`, shared output file) were eliminated.

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
  Directory.Build.props                                ← solution-wide MSBuild props (incl. long-path props)
  global.json                                          ← SDK pin (see "SDK pinning" preamble)
  .vscode/launch.json                                  ← IDE debugger config
  TinyLanguage/TinyLanguage.csproj                     ← console (net10.0, self-contained single-file win-x64 exe)
  TinyLanguage/app.manifest                            ← <longPathAware>true</longPathAware> manifest
  TinyLanguage.Lexer/TinyLanguage.Lexer.csproj         ← classlib (net10.0)
  TinyLanguage.Interpreter/TinyLanguage.Interpreter.csproj  ← classlib (net10.0)
  TinyLanguage.UnitTests/TinyLanguage.UnitTests.csproj      ← MSTest
  TinyLanguage.IntegrationTests/TinyLanguage.IntegrationTests.csproj  ← MSTest
  TinyLanguage.DemoFiles/TinyLanguage.DemoFiles.csproj      ← SDK-style content-only project,
                                                              **lives INSIDE the solution folder**
                                                              as a peer of the source projects.
                                                              Never outside, never under bin/,
                                                              never under any user temp path.

The DemoFiles content-only csproj MUST glob the demos with NON-RECURSIVE flat
patterns and NO copy-to-output (EnableDefaultCompileItems=false so it compiles
no C#):

  <Content Include="*.tlg" CopyToOutputDirectory="Never" />
  <Content Include="*.cmd" CopyToOutputDirectory="Never" />

Do NOT use `**/*.tlg` and do NOT set CopyToOutputDirectory="PreserveNewest". A
manually-authored <Content Include> does NOT inherit DefaultItemExcludes, so a
`**/` glob re-globs the bin\ copies on every build and nests them one level
deeper each rebuild — unbounded growth that eventually exceeds MAX_PATH and
breaks the Phase 5 §6i `devenv /Rebuild` gate. The demos run from the DemoFiles
ROOT (the .cmd files use %~dp0) and the published exe is copied to that root,
never to bin\, so CopyToOutputDirectory is pointless here. (Discovered in the
2026.05.29.02 run after the recursion had already begun.)

All `<ProjectReference>` entries in csproj files use solution-relative paths (e.g.
`..\TinyLanguage.Lexer\TinyLanguage.Lexer.csproj`). No absolute paths. No paths
that escape the solution folder.

Apply every coding-style rule from the spec. Do not generate any C# source yet —
scaffold files, project references, Directory.Build.props, and .vscode/launch.json only.

TinyLanguage.slnx must list TinyLanguage/TinyLanguage.csproj FIRST so that Visual
Studio recognises it as the default startup project (the .slnx format has no explicit
startup-project field; VS defaults to the first executable project in the list).

The slnx must also include a "Solution Items" folder that pre-declares the four
non-project files end-users open from the solution: `.gitignore` (created here
in Phase 1A), `install-vscode-debugger.cmd` (created in Phase 4D),
`install-vs-debugger.cmd` (created in Phase 4F), and `TinyLanguage.wiki.md`
(created in Phase 4E). `dotnet build` IGNORES `<File>` entries that don't yet
exist on disk — verified empirically — so it is safe to pre-declare all four
here. The benefit: the slnx is authored exactly once, and Phases 4D / 4E / 4F
don't need to edit it for Solution Items. The folder makes the four files
appear under the solution node in Visual Studio / Rider so users discover the
wiki and both installers without spelunking the filesystem.

Exact slnx contents:
  <Solution>
    <Folder Name="/Solution Items/">
      <File Path=".gitignore" />
      <File Path="install-vscode-debugger.cmd" />
      <File Path="install-vs-debugger.cmd" />
      <File Path="TinyLanguage.wiki.md" />
    </Folder>
    <Project Path="TinyLanguage\TinyLanguage.csproj" />
    <Project Path="TinyLanguage.Lexer\TinyLanguage.Lexer.csproj" />
    <Project Path="TinyLanguage.Interpreter\TinyLanguage.Interpreter.csproj" />
    <Project Path="TinyLanguage.UnitTests\TinyLanguage.UnitTests.csproj" />
    <Project Path="TinyLanguage.IntegrationTests\TinyLanguage.IntegrationTests.csproj" />
    <Project Path="TinyLanguage.DemoFiles\TinyLanguage.DemoFiles.csproj" />
  </Solution>

(Phase 4C inserts `<Project Path="TinyLanguage.DebugAdapter\TinyLanguage.DebugAdapter.csproj" />`
between Interpreter and UnitTests when it adds the DAP project — see Phase 4C below.
Phase 4F appends `<Project Path="extensions\extensions.shproj" />` after the seven
.NET projects when it adds the Shared Project container — see Phase 4F below.
Both projects are real files at the time they're added to the slnx; do not
pre-declare them in Phase 1A.)

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

Long-path support — non-negotiable. Read the "Long-path support" section at
the top of Build.md before scaffolding. Phase 1A is responsible for authoring
ALL of:

  (a) The solution-root `Directory.Build.props` (exact contents specified in
      that section — long-path properties).
  (b) `TinyLanguage\app.manifest` declaring <longPathAware>true</longPathAware>
      with the exact contents specified in that section.
  (c) `<ApplicationManifest>app.manifest</ApplicationManifest>` inside the
      <PropertyGroup> of `TinyLanguage\TinyLanguage.csproj`.

These three together make the deliverable VS-Batch-Rebuild-capable; missing
any one causes Visual Studio to fail with MAX_PATH errors on the deepest
publish intermediate paths even though `dotnet build` succeeds. Phase 5
Step 6c will verify all three exist in the deliverable.

TinyLanguage/TinyLanguage.csproj must include:
  <ApplicationManifest>app.manifest</ApplicationManifest>
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
  <!-- comments forbid the double-hyphen digraph, so write `dotnet run` not the    -->
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

Decimal-point float trap (do NOT repeat this bug from an earlier run):

  When the number reader sees `.` it must decide between "fractional part
  of a float" and "member-access dot after an integer". The natural way
  to write the helper is wrong:

      private bool PeekIsDigit() {                         // BROKEN
          return CurrentChar != -1 && char.IsDigit((char)CurrentChar);
      }

  At the call site, CurrentChar IS the `.` — it is never a digit, so the
  float branch is never taken and `1.5` lexes as Integer('1') Dot('.')
  Integer('5'). Phase 3A and Phase 3B independently flagged this on the
  previous run. Use Source.Peek() to look ONE character past the `.`:

      private bool PeekIsDigit() {                         // CORRECT
          int nextChar = Source.Peek();
          return nextChar != -1 && char.IsDigit((char)nextChar);
      }

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

  TIER B — ADVANCED DATA STRUCTURE & ALGORITHM DEMOS (00400..00499)
    Substantial, multi-subroutine .tlg programs that BUILD an advanced data
    structure and EXERCISE it with a meaningful workload. These prove the
    interpreter holds up under real, idiomatic programs — not just one-liners.
    See the "Tier B requirements" block below for the full specification.

  TIER C — COMPREHENSIVE DATA-STRUCTURE CATALOGUE (00500..00699)
    Wikipedia-style reference implementations covering 125 canonical data
    structures across 6 categories (linear lists, trees, tries+B-trees,
    heaps+hash, graphs+space partitioning, ADTs+composites). Each demo cites
    its Wikipedia article in the header. See "Tier C" block below for the
    full catalogue + per-demo contract.

All three tiers together must total 500+ .tlg files in TinyLanguage.DemoFiles/,
zero-padded numeric prefix (00001.fizzbuzz.tlg … etc.). Each .tlg file must
have a matching .cmd runner using the template below. Tier A targets ~300-330
demos, Tier B targets ~50 (out of 100 numeric slots reserved), and Tier C
targets exactly 125 demos per the catalogue below.

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

A recommended Tier B file count is ≥ 50 (out of the 500+ total across Tier A, B, C). Anything
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

GLOBAL anti-patterns (apply to all tiers; each one has shipped before and
broken the demo sweep):

- **Keywords used as parameter names.** TinyLanguage reserves these tokens:
  `to`, `from` (not actually a kw, OK), `step`, `in`, `do`, `then`, `else`,
  `end`, `as`, `is`, `new`, `static`, `base` (not a kw, OK), `int`, `float`,
  `string`, `bool`, `array`, `object`, `map`, `void`, `null`, `true`,
  `false`, plus all control-flow / declaration keywords. The Tower-of-Hanoi
  pattern `function hanoi(n, from, to, via)` fails because `to` lexes as
  TokenType.To, not Identifier (Note 5). Rename clashing params (`src`,
  `dst`, `mid`, etc.).
- **Closure-using lambdas.** The spec is explicit: "lambdas see only global
  + own params" (Build.Solution.md "Scope Rules"). Demos like
  `function(x) function(y) x + y` won't run — `x` is invisible to the
  inner lambda. Use a class to hold the captured value instead:
  `class Adder { let Base := 0; Constructor(b); this.Base := b end;
   function Apply(y); return this.Base + y end }`.
- **Bitwise operators.** TinyLanguage has NO bitwise operators. The BNF
  has logical `and`/`or`/`not` only. `|` is `TokenType.Pipe` (pattern
  alternation), `&` is `TokenType.Amp` (STRING CONCAT, per Note 10),
  and `~`/`^` are not in the lexer at all. Demos that need bitwise ops
  (Fenwick tree, XOR linked list, bitwise AND/OR/XOR/NOT) must either
  be omitted with `# NOT IMPLEMENTABLE` or rewritten to the closest
  in-spec analog (the bitwise-AND-based array indexing in Fenwick →
  plain prefix-sum array; the XOR-pointer linked list → explicit
  prev/next index arrays).
- **`map` as a variable name.** `map` lexes as `TokenType.MapType`, so
  `let map := new HashMap()` fails. Use `hmap`, `dict`, `kv`, etc.
- **Module qualified access is NOT supported.** `module M { export function f()
  ... }` promotes `f` to GLOBAL scope; there is NO `M.f()` dotted-access form.
  Call exported members UNQUALIFIED (`MathOps.Abs(x)` fails "Undefined variable
  MathOps").
- **`try` requires a `catch`.** There is no catch-less `try/finally`; always
  write `try ... catch ... finally ... end` (finally optional, catch mandatory).
- **Scientific-notation float literals (`6.6e-34`) are OUT of the BNF.** Avoid
  them; if a structure truly needs one, mark `# NOT IMPLEMENTABLE` and use a plain
  decimal so the demo still runs.
- **Field-level annotations are OUT of the BNF.** Annotations are statement/
  member-level only — do NOT put `@Foo` on a class field.
- Add **`step`** to the reserved-words-that-cannot-be-parameter-names list
  (alongside `to`, `in`, `do`, `then`, `else`, `end`, `as`, `is`, `new`, `static`,
  the type-name keywords, etc.).
- **`match` is a keyword** — never use it as a variable or loop-variable name.

REQUIRED `;` separator hygiene:

- Every consecutive pair of top-level statements must be separated by `;`
  in source even though the parser's D10 leniency would forgive newlines
  in this build. Authoring with explicit `;` future-proofs the demos
  against a stricter parser regenerated by a future agent.
- Do NOT add `;` after a `function NAME(...)` or `Constructor(...)` line
  (the body follows; `;` there starts an "empty statement" inside the
  function body which the parser's D11 leniency forgives — but ugly).
- Do NOT add `;` before a closing `}`, `end`, `else`, `catch`, `finally`,
  `while` (in do-while), or before EOF.

CLEANUP rule:

- Do NOT leave generator scripts (`_gen_*.py`, `_fix_*.py`, `generate_*.ps1`,
  etc.) in `TinyLanguage.DemoFiles\`. The DemoFiles project's content glob
  picks up `.tlg` and `.cmd`, so leftover scripts don't break the build,
  but they pollute the deliverable. Delete them before reporting done.

TERMINATION rule:

- Every demo must terminate within ~5 seconds when run through the
  published exe in file-processor mode. Demos with infinite loops
  (e.g. mis-indexed traversal in the XOR-linked-list catalogue demo)
  must be fixed or omitted with `# NOT IMPLEMENTABLE`. The Phase 5
  acceptance step times each demo at 5s; anything slower fails the
  sweep.

### Tier C — comprehensive data-structure catalogue (00500..00699)

Tier C is a Wikipedia-style reference suite covering the canonical data
structures from https://en.wikipedia.org/wiki/List_of_data_structures.
Every demo is self-contained, didactic, deterministic, and exercised by a
small workload. Each .tlg follows the same shape as Tier B (header comment,
class-based implementation, named subroutines, state snapshots after each
operation) but with an explicit Wikipedia reference in the header.

Six numeric subranges, one per category. Each agent assigned a subrange
authors every demo in its range:

  Linear lists (00500..00514) — 15 demos:
    00500.assoc_list                   Association list (key-value pairs)
    00501.self_organizing_list         Move-to-front on Find
    00502.skip_list                    Skip list, DETERMINISTIC level rule (no RNG)
    00503.unrolled_linked_list         Multiple values per node
    00504.vlist                        Bagwell's persistent VList
    00505.conc_tree                    Concatenation tree (Scala parallel collections)
    00506.xor_linked_list              XOR-encoded prev/next (simulated)
    00507.zipper_list                  List zipper at a focus
    00508.dcel                         Doubly Connected Edge List (planar subdivision)
    00509.difference_list              Difference list (O(1) concat, flatten once)
    00510.free_list                    Free-list memory pool
    00511.array_list_growable          Dynamic array with explicit growth policy
    00512.singly_linked_list_advanced  Reverse + Middle + Floyd cycle detection
    00513.persistent_list              Immutable cons-list with structural sharing
    00514.intrusive_list               Intrusive doubly-linked list

  Trees, general-purpose (00520..00539) — 20 demos:
    00520.aa_tree                      AA tree (Andersson's right-leaning RB)
    00521.binary_tree_traversals       Pre/In/Post/Level-order
    00522.cartesian_tree               From sequence via stack-based linear build
    00523.left_child_right_sibling     LCRS general-tree representation
    00524.order_statistic_tree         Size-augmented BST with KthSmallest/RankOf
    00525.randomized_bst               Treap-style with SEEDED LCG priorities
    00526.rope                         Tree of string fragments
    00527.scapegoat_tree               Rebuild on alpha-imbalance (alpha=0.7)
    00528.splay_tree                   Self-adjusting via splay
    00529.threaded_binary_tree         InOrder pred/succ threads, no stack
    00530.treap                        DETERMINISTIC priorities (hash key)
    00531.wavl_tree                    Weak AVL (rank-based)
    00532.weight_balanced_tree         BB[alpha] tree
    00533.zip_tree                     Tarjan zip tree (geometric ranks)
    00534.binary_search_tree_iterative Iterative insert/search/delete
    00535.morris_traversal             O(1) extra space via threading
    00536.full_binary_tree             Full-tree property check
    00537.complete_binary_tree_check   Completeness property check
    00538.bst_floor_ceiling            Floor/Ceiling queries
    00539.bst_range_count              Range count via subtree-size augmentation

  Tries and B-trees (00560..00579) — 20 demos:
    00560.radix_tree                   Compressed trie with split-on-insert
    00561.suffix_tree                  Brute-force build (Ukkonen optional)
    00562.ternary_search_tree          3-way trie (lt/eq/gt)
    00563.patricia_trie                Binary radix trie
    00564.bit_trie                     8-bit binary trie + FindMaxXOR
    00565.trie_autocomplete            Prefix → sorted completions
    00566.compressed_trie_path         Side-by-side node count vs uncompressed
    00567.dawg                         Shared-suffix DAWG (vs trie node count)
    00568.suffix_array                 Suffix array + LCP construction
    00569.aho_corasick                 Multi-pattern with failure links
    00570.btree                        B-tree order m=3
    00571.bplus_tree                   B+ tree (linked leaves)
    00572.btree_two_three              2-3 tree
    00573.btree_two_three_four         2-3-4 tree (RBT equivalence)
    00574.btree_split_merge            B-tree (m=4) with delete merges
    00575.bplus_range_scan             B+ tree range scan via leaf chain
    00576.btree_bulk_load              Bottom-up bulk construction
    00577.btree_split_visualization    Cascading splits printed step-by-step
    00578.btree_iterator               Cursor-based in-order iterator
    00579.btree_delete_cases           Three deletion cases enumerated

  Heaps and hash-based (00580..00599) — 20 demos:
    00580.binomial_heap                Forest of binomial trees
    00581.fibonacci_heap               Lazy merge + cascading-cut
    00582.pairing_heap                 Two-pass pairing on ExtractMin
    00583.leftist_heap                 Rank (s-value) annotated
    00584.skew_heap                    Always-swap merge variant
    00585.d_ary_heap                   General arity (d=4)
    00586.binary_heap_in_array         Array-backed with index math
    00587.indexed_priority_queue       Reverse index for external-key DecreaseKey
    00588.double_ended_priority_queue  Min-max heap
    00589.median_heap                  Two heaps for running median
    00590.hash_table_chained_resize    Chaining + load-factor rehash
    00591.hamt                         Hash Array Mapped Trie (5-bit chunks)
    00592.count_min_sketch             Probabilistic frequency estimator
    00593.cuckoo_hashing               Two-table kick-out
    00594.hopscotch_hashing            Neighbourhood-bitmap probing
    00595.consistent_hashing           Sorted ring with virtual nodes
    00596.linear_probing               Tombstones explicit
    00597.quadratic_probing            i² probe sequence
    00598.robin_hood_hashing           Distance-from-ideal swaps
    00599.invertible_bloom_filter      Listable IBLT (count + keySum + valSum)

  Graphs and space partitioning (00600..00629) — 30 demos:
    00600.graph_adjacency_matrix       2-D bool/weight matrix
    00601.graph_edge_list              Edge tuples
    00602.graph_incidence_matrix       V×E incidence matrix
    00603.graph_csr                    Compressed Sparse Row
    00604.blockchain                   Linked blocks with prevHash + nonce (deterministic mini-PoW)
    00605.directed_graph               In/Out degree
    00606.undirected_graph_simple      Symmetric edges
    00607.weighted_graph               Edge weights
    00608.bipartite_graph              Two-coloured partition
    00609.multigraph                   Parallel edges
    00610.hypergraph                   Edges as vertex sets
    00611.graph_transpose              Edge-reversed copy
    00612.graph_contraction            Edge contraction merge
    00613.graph_complement             Edge-flipped graph
    00614.graph_isomorphism_check      Degree-sequence + brute-force mapping
    00615.interval_tree                BST-on-start with subtree max-end
    00616.range_tree                   1-D range queries
    00617.bin_grid                     2-D uniform grid bin
    00618.kd_tree_2d                   2-D K-d tree + NN search
    00619.quadtree                     Region quadtree (capacity-based split)
    00620.z_order_curve                Morton interleave/deinterleave
    00621.bk_tree                      Levenshtein BK-tree
    00622.r_tree_2d                    MBR-based R-tree (simplified)
    00623.uniform_grid_3d              3-D uniform grid bin
    00624.vp_tree                      Vantage-Point tree NN
    00625.range_search_1d              Sorted array + bounds
    00626.priority_search_tree         Heap-by-y + BST-by-x
    00627.skip_quadtree                Multi-level quadtree, deterministic promotion
    00628.point_region_quadtree        PR-quadtree (points in leaves only)
    00629.morton_order_sort            Sort 2-D points by Z-order

  ADTs and composites (00640..00659) — 20 demos:
    00640.multimap                     key → list of values
    00641.multiset                     element → count (bag)
    00642.ordered_set                  BST-backed with range queries
    00643.ordered_map                  BST-backed key→value, range queries
    00644.disjoint_set_union_advanced  Path compression + union-by-rank
    00645.bag                          Unordered multiset (no remove)
    00646.indexed_list                 Random-access list via balanced BST
    00647.persistent_set               Immutable BST set with sharing
    00648.lru_set                      LRU set with eviction
    00649.bidirectional_map            BiMap (O(1) both directions)
    00650.record_struct                Class-backed record (Point, Rectangle)
    00651.tagged_union                 Sum type dispatched via match
    00652.tuple_pair                   Pair class
    00653.tuple_triple                 Triple class + sort by field
    00654.string_view                  Source + start + length wrapper
    00655.optional                     Maybe / Option type
    00656.either                       Either / Result type
    00657.tagged_pointer               Low-bit flag packing (simulated)
    00658.struct_array_of_structs      AoS vs SoA side-by-side
    00659.union_with_discriminator     Discriminated union

Per-demo contract — every Tier C .tlg MUST satisfy ALL of:

1. Header comment: Structure / Category / Operations with Big-O / Reference
   (Wikipedia URL). Example:
     # Structure: Binomial heap
     # Category: Heap
     # Operations: Insert O(log n), Min O(log n), ExtractMin O(log n), Merge O(log n)
     # Reference: https://en.wikipedia.org/wiki/Binomial_heap
2. Implementation as a class (or cooperating classes) — authentic to the
   structure. A "splay tree" must actually splay; a "treap" must actually
   use heap-ordered priorities; a "B+ tree" must actually link its leaves.
3. ≥ 3 named subroutines exercising the canonical operations.
4. ≥ 8 distinct operations against a small instance (10-20 elements).
5. Print labeled state snapshots after each operation. For trees: indented
   ASCII or sideways with `├── ` / `└── ` / `│   ` prefixes. For linear
   structures: horizontal `[a, b, c]` or `a -> b -> c` rendering.
6. Deterministic output. Any randomized structure (Skip list, Treap,
   Randomized BST, Cuckoo) uses a SEEDED LCG — never the system clock.
7. 30-200 lines. Use `;` separator throughout per Note 21.
8. Edge-case tests after the main demo: empty, single-element, one tricky
   case appropriate to the structure.

Coverage discipline: every item in the six lists above must have at least
one demo. If a structure is genuinely infeasible in TinyLanguage (e.g.
genuine closures for a curried difference-list), implement the closest
faithful approximation and note the deviation in the header — do NOT omit
the demo entirely; do NOT substitute a watered-down replacement that
doesn't exercise the structure.

The Phase 1D agent can split Tier C authoring across parallel sub-agents
(one per category) or produce all six categories itself; either is
acceptable as long as the final demo count and per-demo contract hold.

Create a matching .cmd runner for each demo file (Tier A, Tier B, AND Tier C).

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
No merge needed — Phases 1A/1B/1C already wrote to the canonical path (see
'Execution model' preamble). Confirm the scaffold + lexer + AST are present at the
canonical path.

Read Build.Solution.md: "BNF Grammar" (full), all 23 Implementation Notes.

Implement TinyLanguage.Lexer/Parser.cs:
- Recursive-descent parser matching every BNF production exactly.
- ParseStatementList accepts optional stop-token sets (notes 3, 12).
- Disambiguation rules for cast, lambda, conditional, generic type (notes 12–14, 20).
- Bare return support (note 4).
- Correct operator precedence table (note 11).
- ParserException.cs with source line number.
- Built-in conversion CALL-syntax (deviation D16): `int(x)` / `float(x)` / `bool(x)` /
  `str(x)` must parse as builtin calls. The lexer emits IntType/FloatType/BoolType/
  StringType keyword tokens for those names, so in ParsePrimary, when one of these
  type-name keywords is IMMEDIATELY followed by `(`, parse it as a conversion CALL
  (emit the canonical builtin identifier int/float/bool/str so the postfix loop
  builds a FunctionCallNode). Must NOT break casts `(int)x`, annotations `: int`,
  or `is int`/`as int`. ~32 demos need this; the spec lists int/str/bool/float as
  built-in functions.
- Lambda body decided by FIRST token (deviation D17): a lambda body is a single
  expression when its first token starts an expression, or a statement BLOCK
  (optionally closed by `end`) when its first token is a statement-only keyword.
  Decide by the FULL statement-keyword set — print/if/while/for/foreach/return/let/
  var/const/throw/try/switch/match/break/continue/do (do NOT omit `return`, or
  `function() return 1 end` fails). Decide by that first token; do NOT scan ahead
  for a matching `end` (that latches onto the ENCLOSING function's `end` and
  mis-parses `function(n) print n`).
- `export <definition>` (deviation D20): besides the BNF's `export <id>`, accept a
  definition after `export` (export function|class|static|let|var|const ...) — parse
  the inner definition (it lands in module/global scope; modules promote to global
  with no qualified `M.f` access) and record the export marker (runtime no-op).
  Required by demos 00291–00295. Lives in ParseExportStatement.
- `static` instance-style fields (deviation D21): accept `static` before
  `let`/`var`/`const` field declarations in a class body (not just before
  `function`); route them to the class's static members (reachable/assignable as
  `ClassName.Field`). `FieldDeclareNode` carries `IsStatic`. Required by 00216, 00237.
- Newline-gated postfix (D10 corollary): do NOT consume a `[` or `(` that begins on
  a strictly-later source line as an index/call on the previous statement's
  expression (a leading `.` member access may still cross lines). Without this a
  `print "x"` before a `[a, b] => ...` match case swallows the case. Fixed 00306.

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
- Operator `+` is overloaded by operand type: numeric add when both numeric,
  string concat when either is a string, and **array/list concatenation** when
  both are arrays (returns a NEW list). The demo corpus's universal append idiom
  is `arr := arr + [x]` (there is no in-spec list-grow builtin and indexed assign
  cannot extend a list), so list `+` is REQUIRED even though the spec is silent on
  list operands.
- Class `const` fields belong to the CLASS, not the instance: route
  `FieldKind.Const` declarations in a class body into `StaticMembers` so they are
  reachable via `ClassName.CONST` (a const is immutable and instance-independent).
- `static` instance-style fields (deviation D21): also route class-body `static let`
  and `static var` declarations (parser sets `FieldDeclareNode.IsStatic`) into
  `StaticMembers`, and EXCLUDE them from per-instance `Fields`. Unlike `const` these
  are MUTABLE, so `MemberSet` must accept a `ClassInfo` owner (and a static reached
  through an instance) so `ClassName.Field := v` and `static`-counter demos work.
- Built-in conversions int/float/bool/str must be invokable BOTH as casts
  `(int)x` AND as calls `int(x)` (parser D16 routes the call form here) — share
  one conversion code path.

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
- TestLog.cs       — pretty-printing helper authored verbatim from the "Test output formatting — non-negotiable" preamble at the top of Build.md (UnitTests namespace).
- LexerUnitTests.cs  — every token type, boundary conditions, error cases.
- ParserUnitTests.cs — every BNF production, all 23 disambiguation rules.

Use MSTest only. Test class suffix: UnitTests. No "Test" in method names.
Name pattern: Subject_Action_ExpectedOutcome.

Every test must print its input and result via the TestLog helper so the test
log shows what was exercised — see "Test output formatting — non-negotiable"
at the top of Build.md for the exact rules. Summary:

  TestLog.Input(source);                    // for a bare source string
  TestLog.Input("label", source);           // when there's a meaningful label
  TestLog.Result(actual);                   // multi-line content uses real newlines

Do NOT use raw `Console.WriteLine($"Input: ...")` / `Console.WriteLine($"Result: ...")`.
Do NOT escape `\n` to `\\n` in any value passed to TestLog — the helper handles
indentation and uses real newlines so multi-line content stays readable. For
helpers like `DescribeTokens`, emit one token per line (not a comma-joined blob).

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
- TestLog.cs       — pretty-printing helper authored verbatim from the "Test output formatting — non-negotiable" preamble at the top of Build.md (IntegrationTests namespace).
- InterpreterIntegrationTests.cs — end-to-end programs for every language feature.
- Use the .tlg demo files from Phase 1D as test inputs where appropriate.

Every test must print its input and result via the TestLog helper so the test
log shows what was exercised — see "Test output formatting — non-negotiable"
at the top of Build.md for the exact rules. Summary:

  TestLog.Input(demoFileName, source);      // when the input has a label
  TestLog.Section("Stdin", stdin);          // for additional input streams
  TestLog.Result(actual);                   // multi-line stdout uses real newlines

Do NOT use raw `Console.WriteLine($"Input: ...")` / `Console.WriteLine($"Result: ...")`.
Do NOT introduce a `Visible(s)` helper that escapes `\n` to `\\n` — that collapses
multi-line program stdout onto one unreadable line. The TestLog helper handles
indentation and preserves real newlines for any content it prints.

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

## Phase 4.5 — Demo convergence  *(after 3A/3B/4A/4B; before final validation)*

**Agent:** `general-purpose`  **Model:** `claude-opus-4-6` (spec triage)

Publish the exe, then run a FULL sweep — every `*.tlg` through the published exe
in file mode, COLLECTING all non-zero exits (do NOT use `run-all-demos.cmd` for
this; it aborts on the first failure). Triage each failing demo against the spec:
spec-defined feature → fix code (parser/interpreter); out-of-spec anti-pattern →
fix the demo (or `# NOT IMPLEMENTABLE` + keep it running deterministically).
Re-sweep until **0 failures of 595+**, keeping the test suite green and
republishing after any code change.

With the D16/D17 parser relaxations, the same-line-D12 refinement, and the
array-`+`/static-const interpreter fixes folded into Phases 2 and 3A, most
failures will not recur — but run the sweep as a HARD gate. Expected failure
categories on a from-scratch run (each with its resolution): builtin call-syntax
(D16, parser); nested-if-in-else `end` (D12 same-line, parser); module qualified
access (demo → unqualified); array `+` (interpreter); static `const`
(interpreter); lambda statement body (D17, parser); reserved-word params /
`match`-as-name / `while ... then` typo / closures-over-locals / catch-less
`finally` / sci-notation / field-annotation (all demo fixes).

Sweep harness — use the reusable, hardened script in the orchestrator repo
(`tools\sweep-demos.ps1`). It uses `System.Diagnostics.Process` for a RELIABLE
ExitCode, a UNIQUE output file per demo, async stdout/stderr drains, and a 5s
per-demo TIMEOUT so an infinite-loop demo is reported as a TIMEOUT instead of
hanging the run. It exits 0 only when FAILED=0 AND TIMEOUT=0, so callers can gate:

```powershell
powershell -File Z:\repos\TinyLanguage\tools\sweep-demos.ps1 -Canonical "$canonical"
# prints e.g.  TOTAL=520 FAILED=0 TIMEOUT=0  (exe 35.83 MB)  + a categorized failure list
```

Harness FALSE-NEGATIVE traps that wasted a cycle in the 2026.05.29.02 run — do NOT
hand-roll around them: (a) `Start-Process -PassThru` WITHOUT `-Wait` leaves the
returned object's `.ExitCode` null/blank, so `$p.ExitCode -ne 0` is TRUE for EVERY
demo (a false "519 of 520 failed" run); (b) sharing ONE output file across runs
yields false "being used by another process" I/O errors. The script avoids both.
The INVOCATION GOTCHA in Phase 5 Step 5 (`& cmd /c $c.FullName $out`, never the
whole command double-quoted) applies to any per-`.cmd` loop too.

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

1. VS Code extension lives at extensions\vscode\ (under the Shared Project
   container — see Phase 4F).
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

Every test prints input/result via the TestLog helper authored by Phases 3B
and 4A — see "Test output formatting — non-negotiable" at the top of Build.md.
Do NOT use raw `Console.WriteLine($"Input: ...")` / `Console.WriteLine($"Result: ...")`.
For composite results (multiple fields), put each field on its own line via
`\n` inside the result string, e.g.:

  TestLog.Result("enters = [" + string.Join(",", host.FrameEnters) + "]\n"
               + "exits  = [" + string.Join(",", host.FrameExits) + "]");

Test parallelism note (avoid intermittent failures): MSTest defaults to
running test methods in parallel. The pre-existing Phase 3B / 4A tests
share some console-redirection state which is not parallel-safe, and the
DAP integration tests in particular touch shared Stream pairs. Add to
BOTH test projects:

  [assembly: Parallelize(Workers = 1, Scope = ExecutionScope.MethodLevel)]

or set `<RunSettingsFilePath>` to a .runsettings with
`<MSTest.Parallelize.Workers>1</MSTest.Parallelize.Workers>`. Without
this, `dotnet test` exhibits flaky 6-25 failures on a clean checkout
depending on parallel-worker count.

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

Output target: Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vscode\
This sits under the Shared Project container `extensions\` (authored by
Phase 4F) and is a peer of `extensions\vs\` (also Phase 4F). The two shims
are bundled visually in Solution Explorer via `extensions\extensions.shproj`
but built independently — VS Code reads `extensions\vscode\` directly; VS18
builds `extensions\vs\TinyLanguage.VsTools.csproj` via `install-vs-debugger.cmd`.

Files:

- package.json — the extension manifest. Required keys:
    name: "tinylanguage-debug"
    displayName: "TinyLanguage Debugger"
    version: "0.1.0"
    publisher: "tinylanguage-local"
    engines.vscode: "^1.70.0"
    categories: ["Debuggers"]
    main: "./extension.js"
    activationEvents: [
      "onLanguage:tinylanguage",
      "onDebug",
      "onDebugResolve:tinylanguage",
      "onDebugDynamicConfigurations:tinylanguage"
    ]
    (Note: `onDebug` alone is NOT enough. If the user opened VS Code at a folder
    that is not the canonical solution root — and therefore has no
    `.vscode/launch.json` with the "Debug current .tlg file" config — pressing
    F5 on a .tlg file did literally nothing in early versions, because the
    extension never activated and so couldn't resolve a synthetic config.
    Adding `onLanguage:tinylanguage` activates the extension as soon as a .tlg
    file is opened in any folder, making the resolveDebugConfiguration hook
    available in time for F5.)
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

- extension.js — small CommonJS module. MUST register BOTH a
  DebugConfigurationProvider AND a DebugAdapterDescriptorFactory for type
  "tinylanguage". Two separate jobs:

    1. DebugConfigurationProvider.resolveDebugConfiguration(folder, config) —
       called by VS Code on F5. If `config` is empty (no `.vscode/launch.json`
       entry exists) AND the active editor is a `.tlg` file (languageId
       "tinylanguage"), synthesise a config:
         { type: "tinylanguage", request: "launch",
           name: "Debug current .tlg file",
           program: <activeEditor.document.uri.fsPath>,
           stopOnEntry: true }
       This makes "open any folder, open a .tlg file, press F5" work end-to-end
       even when there is no launch.json. Without this provider, F5 with no
       launch.json silently does nothing.

    2. DebugAdapterDescriptorFactory.createDebugAdapterDescriptor(session) —
       returns `new vscode.DebugAdapterExecutable(<exePath>, ["--dap"])`. The
       exe path is resolved by the FOLLOWING precedence chain (first hit wins):
         a. session.configuration.exe — explicit override in launch.json
         b. Walk UP from `path.dirname(session.configuration.program)` looking
            for a `TinyLanguage.exe` directly in that dir, OR a
            `TinyLanguage.DemoFiles/TinyLanguage.exe` inside it. This makes the
            extension find the published interpreter even when the user opened
            VS Code at an arbitrary folder, as long as the .tlg file lives
            somewhere inside (or beside) the canonical solution.
         c. `${workspaceFolder}/TinyLanguage.DemoFiles/TinyLanguage.exe` —
            classic in-workspace lookup.
         d. Bare `"TinyLanguage.exe"` — final fallback, lets PATH resolve it.

  Use `fs.existsSync` to verify each candidate before returning it; only the
  bare-name PATH fallback is unverified.

  Exports `activate(context)` and `deactivate()`. `activate` registers BOTH the
  provider and the factory and pushes both disposables onto `context.subscriptions`.

- README.md — install instructions in 5 commands or fewer:
    cd extensions\vscode
    npm install -g vsce
    vsce package --allow-missing-repository
    code --install-extension tinylanguage-debug-0.1.0.vsix
  Plus a launch.json template the user can paste. The `--allow-missing-repository`
  flag matches what install-vscode-debugger.cmd uses — see the LICENSE.txt and
  Step 4 notes below for why both are required for an unattended package run.

- .vscodeignore — minimal, just exclude .vscode/ and node_modules/

- LICENSE.txt — REQUIRED to keep `vsce package` unattended. Without a LICENSE,
  LICENSE.md, or LICENSE.txt file in the extension root, vsce prints
  ` WARNING  LICENSE.md, LICENSE.txt or LICENSE not found` AND prompts
  `Do you want to continue? [y/N]` — which hangs the installer when run by
  double-click or via stdin redirection. There is no `--allow-missing-license`
  flag in vsce, so the only fix is to ship a LICENSE file. A short prose
  disclaimer is fine — the extension is local-only (publisher
  "tinylanguage-local") and not destined for the Marketplace, so a permissive
  one-paragraph note like "Local-use extension generated by the TinyLanguage
  build orchestration. No warranty." is sufficient.

Also update .vscode/launch.json at the SOLUTION root (NOT the
extensions\vscode\'s). Keep the existing two configurations for the
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
from any shell). The slnx already references this file from the "Solution Items"
folder Phase 1A authored, so do NOT edit `TinyLanguage.slnx` here — just create
the .cmd at the solution root and Visual Studio will pick it up automatically.

It must be idempotent and run five steps:

  1. Verify prerequisites: `where dotnet`, `where node`, `where npm`, `where code`.
     Any missing → print which one + the install URL → pause + exit 1.
  2. `dotnet publish "%SCRIPT_DIR%TinyLanguage\TinyLanguage.csproj" -c Release`.
  3. If `where vsce` fails, `npm install -g vsce`. After install, re-check;
     fall back to `%APPDATA%\npm\vsce.cmd` if PATH hasn't picked up the new
     install in this shell session.
  4. `pushd "%SCRIPT_DIR%extensions\vscode" && call "%VSCE%" package --allow-missing-repository`.
     Captures errorlevel into a saved RC, popd's, then checks the saved RC.
     The `--allow-missing-repository` flag suppresses vsce's
     ` WARNING  A 'repository' field is missing from the 'package.json' manifest file.`
     followed by `Do you want to continue? [y/N]` prompt. The extension's
     publisher is "tinylanguage-local" (not destined for the Marketplace), so
     adding a fake repository URL would be misleading; the flag is the right fix.
     Pair this with the LICENSE.txt requirement above — vsce has TWO
     unattended-install-blocking warnings (missing repo + missing LICENSE) and
     both must be addressed for the installer to run end-to-end without
     keystrokes.
  5. `call code --install-extension "%SCRIPT_DIR%extensions\vscode\tinylanguage-debug-0.1.0.vsix" --force`.

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
- extensions/vscode/package.json validates as JSON (jq . package.json works)
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
  Z:\repos\TinyLanguage\Build.Plan.md      (work-unit breakdown + final layout)
  Z:\repos\TinyLanguage\CLAUDE.md          (project conventions; cross-link from the wiki)

Output target: Z:\repos\TinyLanguage.YYYY.MM.DD.HH\TinyLanguage.wiki.md
(Substitute the actual canonical solution path. The file may already exist as a
0-byte stub — overwrite it.)

The slnx already references this file from the "Solution Items" folder Phase 1A
authored, so do NOT edit `TinyLanguage.slnx` here — just write the wiki at the
canonical solution root and Visual Studio will pick it up automatically.

The wiki is a SINGLE self-contained markdown document covering:

  1. What is TinyLanguage — one-paragraph elevator pitch + the three execution
     modes (stdin / file / --dap).
  2. Language tour — runnable snippets covering: Hello World, variables (let/var/const),
     arithmetic + types, control flow (if/while/for/foreach/do-while), functions,
     lambdas, classes (incl. extends + Constructor + this), arrays, modules,
     exceptions (try/catch/finally/throw), pattern matching (every kind), built-in
     functions (len/str/int/bool/float), and truthiness rules.
  3. Quick reference card — a one-screen cheat sheet of operators and keywords.
  4. Demo suite — describe Tier A (00001..00399, feature coverage), Tier B
     (00400..00499, advanced data structures), and Tier C (00500..00699,
     comprehensive Wikipedia-style data-structure catalogue across 6
     categories) with a category table for Tier B + Tier C, plus how to run
     them (per-demo .cmd and run-all-demos.cmd).
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
 12. References — Build.Solution.md, Build.md, CLAUDE.md, Build.Plan.md, the DAP spec.

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
- The Demo Suite section's Tier B AND Tier C category tables are consistent
  with the actual filenames in TinyLanguage.DemoFiles/.

Reporting:
- Final line count of TinyLanguage.wiki.md
- Number of code blocks (split by language tag)
- Confirmation that ```tinylanguage blocks all parse
- Any decisions that diverge from this prompt
```

---

## Phase 4F — VS18 Editor Shim + Shared Project  *(starts after Phase 4D; parallel-safe with Phase 4E)*

**Agent:** `general-purpose`
**Model:** `claude-opus-4-6`  *(VS Debug Adapter Host wiring + pkgdef registry shape)*
**Prompt:**
```
Read Build.md "Deliberate deviations from Build.Solution.md" item 5 — the VS18
editor shim is an additive feature parallel to the VS Code shim (item 4 / Phase
4D). Build.Solution.md is silent on debugging entirely; do NOT take its silence
as a reason to skip the work.

The same `TinyLanguage.exe --dap` DAP server already powers VS Code. Your job is
the thin editor shim for VS18 plus the Shared Project container that gives both
shims one navigable tree in Solution Explorer.

OUTPUT TARGETS — absolute paths:
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\extensions.shproj
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\extensions.projitems
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\TinyLanguage.VsTools.csproj
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\source.extension.vsixmanifest
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\TinyLanguagePackage.cs
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\TinyLanguageAdapterLauncher.cs
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\TinyLanguageTargetHostProcess.cs
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\Resources\PackageRegistration.pkgdef
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\Resources\icon.png
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\launch.vs.json.template
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\README.md
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\extensions\vs\LICENSE.txt
  Z:\repos\TinyLanguage.YYYY.MM.DD.HH\install-vs-debugger.cmd

PLUS one slnx edit: append <Project Path="extensions\extensions.shproj" /> after
the seven .NET projects. The Solution Items folder already has the
install-vs-debugger.cmd entry from Phase 1A; do NOT re-add it.

# Pinned GUIDs (from Build.Plan.md §6 Allocated Identifiers — DO NOT reroll)

| Identifier              | Value                                      |
|-------------------------|--------------------------------------------|
| Shared Project          | {5157DF2E-637E-49A6-AE37-F7E6F9D53055}     |
| VS18 package            | {8D86897A-2C9B-4A38-BFE2-2CA687B82AC3}     |
| VS18 engine             | {BAFF8877-1B8C-4D5C-ABB2-A4918F45F244}     |
| VS18 launcher CLSID     | {A08C993B-F229-4376-B2D4-994E825527A1}     |
| VS18 stock DAP-host CLSID | {DAB324E9-7B35-454C-ACA8-F6BB0D5C8673}   (REUSED, NOT generated — VS-owned) |

These are baked into installer caches, the VS extension registry, and the slnx
project reference. Future regenerations MUST use these exact values or VS will
refuse to load the previously-installed extension.

# WU-A — Shared Project container (extensions/extensions.shproj + .projitems)

extensions\extensions.shproj — exact contents, matching VS18's own SharedProject
template (the import path is what VS18's CodeSharing targets ACTUALLY ship at;
ToolsVersion 14.0 with `Microsoft.CodeSharing.Common.targets` as final import
will load but VS will not be able to open the project — see the Phase 4F gotcha
note below):

  <?xml version="1.0" encoding="utf-8"?>
  <Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup Label="Globals">
      <ProjectGuid>{5157DF2E-637E-49A6-AE37-F7E6F9D53055}</ProjectGuid>
      <MinimumVisualStudioVersion>14.0</MinimumVisualStudioVersion>
    </PropertyGroup>
    <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
    <Import Project="$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.Common.Default.props" />
    <Import Project="$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.Common.props" />
    <PropertyGroup />
    <Import Project="extensions.projitems" Label="Shared" />
    <Import Project="$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\CodeSharing\Microsoft.CodeSharing.CSharp.targets" />
  </Project>

GOTCHA — early Phase 4F runs used ToolsVersion="14.0" and `Common.targets`
(without the `Microsoft.Common.props` import). devenv.com /Rebuild still built
the other seven projects but logged "extensions.shproj : error : The project
file cannot be opened by the project system, because it is missing some
critical imports or the referenced SDK cannot be found." The fix is the exact
import chain above: Microsoft.Common.props FIRST (initializes VisualStudioVersion),
then CodeSharing\*.props, then projitems, then CodeSharing\CSharp.targets.
Verified: dotnet build 0/0, devenv /Rebuild loads cleanly.

extensions\extensions.projitems — enumerates every file under extensions\vs\
and extensions\vscode\ as <None> items so directory hierarchy survives in
Solution Explorer. SharedGUID MUST match shproj's ProjectGuid:

  <?xml version="1.0" encoding="utf-8"?>
  <Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
      <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
      <HasSharedItems>true</HasSharedItems>
      <SharedGUID>{5157DF2E-637E-49A6-AE37-F7E6F9D53055}</SharedGUID>
    </PropertyGroup>
    <PropertyGroup Label="Configuration">
      <Import_RootNamespace>extensions</Import_RootNamespace>
    </PropertyGroup>
    <ItemGroup>
      <None Include="$(MSBuildThisFileDirectory)vs\LICENSE.txt" />
      <None Include="$(MSBuildThisFileDirectory)vs\README.md" />
      <None Include="$(MSBuildThisFileDirectory)vs\launch.vs.json.template" />
      <None Include="$(MSBuildThisFileDirectory)vs\source.extension.vsixmanifest" />
      <None Include="$(MSBuildThisFileDirectory)vs\TinyLanguage.VsTools.csproj" />
      <None Include="$(MSBuildThisFileDirectory)vs\TinyLanguageAdapterLauncher.cs" />
      <None Include="$(MSBuildThisFileDirectory)vs\TinyLanguagePackage.cs" />
      <None Include="$(MSBuildThisFileDirectory)vs\TinyLanguageTargetHostProcess.cs" />
      <None Include="$(MSBuildThisFileDirectory)vs\Resources\PackageRegistration.pkgdef" />
      <None Include="$(MSBuildThisFileDirectory)vs\Resources\icon.png" />
      <None Include="$(MSBuildThisFileDirectory)vscode\.vscodeignore" />
      <None Include="$(MSBuildThisFileDirectory)vscode\extension.js" />
      <None Include="$(MSBuildThisFileDirectory)vscode\LICENSE.txt" />
      <None Include="$(MSBuildThisFileDirectory)vscode\package.json" />
      <None Include="$(MSBuildThisFileDirectory)vscode\README.md" />
    </ItemGroup>
  </Project>

# WU-B — extensions/vs/ VSIX project (TinyLanguage.VsTools)

Project name TinyLanguage.VsTools (not VsDebugger) so the bundle can grow into
syntax highlighting / project templates / etc. without renaming. Today it ships
only the debugger.

TinyLanguage.VsTools.csproj — legacy MSBuild csproj (NOT SDK-style):
- <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion> — VS18 extensions still
  target .NET Framework; they load in-process to devenv.exe.
- <UseCodebase>true</UseCodebase>
- <CreateVsixContainer>true</CreateVsixContainer>
- <DeployExtension>false</DeployExtension>  — do NOT auto-launch experimental hive
- <GeneratePkgDefFile>true</GeneratePkgDefFile>
- PackageReferences (these ACTUALLY exist on nuget.org — do NOT reach for the
  prompt-suggested Microsoft.VisualStudio.VSCodeDebugAdapterHost, it doesn't exist).
  Use these EXACT versions that built 0/0 in the 2026.05.29.02 run (NU1603 version
  floats otherwise emit warnings that fail the 0-warning gate):
    Microsoft.VSSDK.BuildTools 17.14.2094
    Microsoft.VisualStudio.SDK 17.0.32112.339
    Microsoft.VisualStudio.Debugger.DebugAdapterHost.Interfaces 16.6.40406.1
        (the interface contract; the implementation DLL ships INSIDE VS18 at
         Common7\IDE\Extensions\Microsoft\DebugAdapterHost\ and is not redistributable)
    Microsoft.VisualStudio.Shared.VSCodeDebugProtocol 17.10.10123.1
- REQUIRED legacy-csproj plumbing (the 2026.05.29.02 run had to add ALL of these or
  restore silently no-op'd and the VsSDK import failed MSB4226):
    * <RestoreProjectStyle>PackageReference</RestoreProjectStyle>
    * an <Import ...Microsoft.CSharp.targets> BEFORE the VsSDK import — a legacy
      csproj has no `Restore` target until the common C# targets load, so without
      this `$(VSToolsPath)\VSSDK\Microsoft.VsSDK.targets` fails with MSB4226.
    * guard the VsSDK import with Condition="Exists(...)".
- Add EXPLICIT framework references — <Reference Include="System" />,
  <Reference Include="System.Core" />, <Reference Include="System.Xml" />.
  Add `using Microsoft.VisualStudio;` (for VSConstants) and mark LICENSE.txt as
  Content with IncludeInVSIX=true (VSSDK1310 — the manifest <License> asset).
  The VSSDK metapackage does NOT auto-add them, so System.Diagnostics.Process /
  DataReceivedEventHandler fail with CS1069/CS0012 without them.
- VS18-era VSSDK BuildTools (17.14+) enforce analyzer VSSDK1311: EACH
  <InstallationTarget> in source.extension.vsixmanifest must contain a
  <ProductArchitecture>amd64</ProductArchitecture> child element.

source.extension.vsixmanifest — Identity Id="TinyLanguage.VsTools.{package-GUID}"
Version="0.1.0" Publisher="tinylanguage-local". InstallationTarget for VS18:

  <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[18.0,)" />
  <InstallationTarget Id="Microsoft.VisualStudio.Pro" Version="[18.0,)" />
  <InstallationTarget Id="Microsoft.VisualStudio.Enterprise" Version="[18.0,)" />

Prerequisite: Microsoft.VisualStudio.Component.CoreEditor [18.0,)

Asset Type="Microsoft.VisualStudio.VsPackage" d:Source="Project"
d:ProjectName="%CurrentProject%" Path="|%CurrentProject%;PkgdefProjectOutputGroup|"

TinyLanguagePackage.cs — namespace TinyLanguage.VsTools; public sealed class :
AsyncPackage decorated with:
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [InstalledProductRegistration("TinyLanguage Tools", "Editor + DAP debugger for the TinyLanguage interpreter.", "0.1.0")]
  [Guid(PackageGuidString)]
  [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]

PackageGuidString = "8D86897A-2C9B-4A38-BFE2-2CA687B82AC3"
InitializeAsync override — empty body. The package's only job is to load the
assembly so pkgdef registry entries take effect.

TinyLanguageAdapterLauncher.cs — implements IAdapterLauncher from
Microsoft.VisualStudio.Debugger.DebugAdapterHost.Interfaces. Class decorated with
[Guid("A08C993B-F229-4376-B2D4-994E825527A1")].

API deviations (verified by reflecting the shipped
`Microsoft.VisualStudio.Debugger.DebugAdapterHost.Interfaces.dll` — these are the
REAL signatures; the naive shape does NOT compile):
- `LaunchAdapter` is `ITargetHostProcess LaunchAdapter(IAdapterLaunchInfo launchInfo, ITargetHostInterop interop)`
  — NOT `LaunchAdapterRequest`. The launch config (program/exe) arrives as a JSON
  STRING in `IAdapterLaunchInfo.LaunchJson`, not as typed properties — parse it;
  in a .NET Framework 4.7.2 in-proc extension AVOID a JSON dependency and use a
  tiny flat-JSON string extractor.
- `UpdateLaunchOptions(IAdapterLaunchInfo launchInfo)` returns VOID (pass-through).
- `Initialize(IDebugAdapterHostContext context)` from `IDebugAdapterHostComponent`
  — no-op.
- `ITargetHostProcess`: `Handle` is `IntPtr`; `StandardInput`/`StandardOutput`
  are `System.IO.Stream`; `ErrorDataReceived` is `DataReceivedEventHandler` (BCL);
  `Exited` is `EventHandler`; plus `Terminate()` and `HasExited`. Provide a thin
  wrapper over `System.Diagnostics.Process` forwarding `.StandardInput.BaseStream`
  / `.StandardOutput.BaseStream`.

Exe-resolution chain (mirror the VS Code extension's logic from extensions/vscode/extension.js):
  1. request.LaunchJson.exe (explicit override)
  2. Walk UP from path.dirname(request.LaunchJson.program) looking for
     TinyLanguage.exe directly OR TinyLanguage.DemoFiles\TinyLanguage.exe inside it.
  3. <SolutionDir>\TinyLanguage.DemoFiles\TinyLanguage.exe
  4. Bare "TinyLanguage.exe" (PATH fallback)
Use File.Exists to verify each candidate; only the PATH fallback is unverified.

TinyLanguageTargetHostProcess.cs — thin ITargetHostProcess wrapper around
System.Diagnostics.Process. Forwards StandardInput / StandardOutput streams and
emits ErrorDataReceived / Exited events. Implements IDisposable.

Resources\PackageRegistration.pkgdef — registers the AD7Metrics engine and
launcher CLSID. CRITICAL: reuses VS18's stock DAP-host engine CLSID
{DAB324E9-7B35-454C-ACA8-F6BB0D5C8673} (look it up in
Common7\IDE\Extensions\Microsoft\DebugAdapterHost\EngineRegistration.pkgdef to
confirm; do NOT generate a new one — VS owns this CLSID, our engine plugs INTO
it):

  ; AD7Metrics engine
  [$RootKey$\AD7Metrics\Engine\{BAFF8877-1B8C-4D5C-ABB2-A4918F45F244}]
  "Name"="TinyLanguage"
  "CLSID"="{DAB324E9-7B35-454C-ACA8-F6BB0D5C8673}"        ; stock DAP-host engine
  "AdapterLauncher"="{A08C993B-F229-4376-B2D4-994E825527A1}"
  "Attach"="0"
  "AddressBP"="0"
  "AlwaysLoadLocal"="0"
  "AutoSelectPriority"="4"
  "CallStackBP"="1"
  "Disassembly"="0"
  "DumpWriter"="0"
  "Embedded"="0"
  "Exceptions"="1"
  "HitCountBP"="1"
  "JustMyCodeStepping"="0"

  ; Launcher CLSID — points VS at our managed launcher class
  [$RootKey$\CLSID\{A08C993B-F229-4376-B2D4-994E825527A1}]
  @="TinyLanguage Adapter Launcher"
  "Assembly"="TinyLanguage.VsTools, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null"
  "InprocServer32"="$WinDir$\System32\mscoree.dll"
  "Class"="TinyLanguage.VsTools.TinyLanguageAdapterLauncher"
  "ThreadingModel"="Both"

  ; File extension association — .tlg files are debuggable by this engine
  [$RootKey$\Languages\File Extensions\.tlg]
  @="TinyLanguage"

Resources\icon.png — 90x90 PNG with monogram "TL" or any small icon VS will
accept. Do not block on icon perfection.

launch.vs.json.template — drop into the user's `.vs\` folder for Open Folder
debug:
  {
    "version": "0.2.1",
    "defaults": {},
    "configurations": [
      {
        "type": "tinylanguage",
        "name": "Debug current TinyLanguage program",
        "project": "CMakeLists.txt",
        "program": "${file}",
        "stopOnEntry": true
      }
    ]
  }
(NOTE: `"project": "CMakeLists.txt"` is the VS18 convention for non-project
debug. If users hit "missing project" errors, this may need iteration — flag
in your report.)

README.md — five-step install:
  1. Run install-vs-debugger.cmd from the solution root.
  2. Wait for publish + VSIX build + VSIXInstaller (a few minutes).
  3. Restart Visual Studio.
  4. Open the .slnx (or any folder containing .tlg files via Open Folder mode).
  5. Open a .tlg file, set a gutter breakpoint, press F5.
Supported features: gutter breakpoints, conditional breakpoints, logpoints,
Step Over (F10) / Step In (F11) / Step Out (Shift+F11), Continue (F5), Pause,
Restart, Variables, Watch, Call Stack, hover-to-evaluate, Debug Console
(Immediate Window).

LICENSE.txt — short prose:
"Local-use Visual Studio extension generated by the TinyLanguage build
orchestration. No warranty."

# WU-C — install-vs-debugger.cmd at solution root

Idempotent, double-click-runnable. Same five-step shape as
install-vscode-debugger.cmd:

  1. Verify prereqs: dotnet, vswhere, MSBuild (resolved via vswhere), VSIXInstaller
     (resolved via vswhere). Use the early-return :check_tool pattern from
     install-vscode-debugger.cmd. NO parentheses inside any string substituted
     into an if-block.
  2. dotnet publish "%SCRIPT_DIR%TinyLanguage\TinyLanguage.csproj" -c Release.
  3. Locate MSBuild + VSIXInstaller via vswhere → %MSBUILD% / %VSIXINSTALLER%. NOTE
     (2026.05.29.02): `vswhere -latest` can return a BuildTools-only install that has
     NO IDE and NO VSIXInstaller. Select the install that has the CoreEditor/MSBuild
     workload AND both MSBuild.exe and Common7\IDE\VSIXInstaller.exe present (iterate
     `vswhere -products * -requires Microsoft.Component.MSBuild` and pick the first
     whose Common7\IDE\VSIXInstaller.exe exists) so the Community/Pro/Enterprise IDE
     install wins over BuildTools.
  4. pushd "%SCRIPT_DIR%extensions\vs" && call "%MSBUILD%" TinyLanguage.VsTools.csproj
     /restore /p:Configuration=Release /p:DeployExtension=false
  5. call "%VSIXINSTALLER%" /quiet "%SCRIPT_DIR%extensions\vs\bin\Release\TinyLanguage.VsTools.vsix"
     (per-user install — no admin needed). Treat VSIXInstaller exit 0 AND 1001
     ("already installed") as success so re-runs stay idempotent.

CMD parser gotcha (extra one, on top of the install-vscode-debugger.cmd gotcha) —
the `%ProgramFiles(x86)%` environment variable contains literal `(x86)` parens
that collide with cmd's if-block parsing if substituted inside `if not exist (...)`.
Fix: introduce a `:resolve_pf86` callee subroutine that binds the value via
setlocal/endlocal BEFORE any if-block sees it. Convert if-bodies to
`goto :no_vswhere` style with an early return. Apply this pattern wherever
%ProgramFiles(x86)% appears inside an if-block.

Pause at the start (after printing the five-step plan). Pause on every error path.
Print "Done. Restart VS, open a .tlg file, press F5." on success.

# WU-D — slnx edit

Append after the seven .NET projects (NOT inside Solution Items folder — this is
a real Project entry):

  <Project Path="extensions\extensions.shproj" />

Verify the slnx still parses (load as XML).

# Acceptance

From the canonical solution path, run:
  dotnet build                                                     # 0 errors / 0 warnings
  dotnet test --no-build                                           # Failed: 0 (still 395 — Phase 4F adds NO tests)
  & "${env:ProgramFiles(x86)}\...\vswhere.exe" -latest -property installationPath
  & "<installationPath>\MSBuild\Current\Bin\MSBuild.exe" extensions\vs\TinyLanguage.VsTools.csproj /restore /p:Configuration=Release /p:DeployExtension=false   # builds the VSIX
  devenv.com TinyLanguage.slnx /Rebuild "Debug|Any CPU" /Out $env:TEMP\rebuild.log    # "succeeded, 0 failed, 0 skipped"

The /Out log MUST NOT contain `extensions.shproj : error : The project file
cannot be opened by the project system` — if it does, the import chain in the
.shproj is wrong.

Then run install-vs-debugger.cmd end-to-end:
  cmd /c install-vs-debugger.cmd < NUL > install-vs.log 2>&1   # exit 0 first run
  cmd /c install-vs-debugger.cmd < NUL > install-vs.log 2>&1   # exit 0 second run (idempotent)

Verify the extension installed: check %LOCALAPPDATA%\Microsoft\VisualStudio\18.0_*\Extensions\
for a directory whose extension.vsixmanifest contains
"TinyLanguage.VsTools.{8D86897A-2C9B-4A38-BFE2-2CA687B82AC3}". Per-user installs
do NOT appear under Common7\IDE\Extensions\ — vswhere -find against that path
returns empty even when the extension is installed correctly.

# WU-E — Tests

Phase 4F adds NO tests. The DAP server is already exercised by Phase 4C's 7 DAP
integration tests + 9 debugger engine unit tests. The VS-side wiring (pkgdef
loading, AsyncPackage initialization, IAdapterLauncher launching the exe) can
only be verified by F5 inside a running VS18 — that requires a human and a
running IDE. Phase 5's acceptance covers `devenv.com /Rebuild` (validates the
extension assembly compiles + the Shared Project loads) but cannot validate F5
itself. Document this limitation in your report.

# Reporting

Report:
- The dotnet build summary line.
- The dotnet test summary line.
- The MSBuild VSIX build summary (errors/warnings, .vsix size).
- The devenv /Rebuild summary line + a grep of the /Out log for
  `extensions.shproj : error` (should find nothing).
- The install-vs-debugger.cmd first-run + second-run exit codes.
- The %LOCALAPPDATA%\...Extensions\ path of the installed extension.
- The contents of the .vsix (Expand-Archive into a temp dir): verify pkgdef
  + package DLL are present.
- Any API deviations from the prompt's described shape.
- Manual F5 verification checklist for the user (cannot be done from the agent).
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

  **INVOCATION GOTCHA (caused a total false-negative once).** Invoke each `.cmd`
  from PowerShell as `& cmd /c $c.FullName $out` and let PowerShell quote the
  arguments. Do NOT manually double-quote the whole command as
  `cmd /c "<cmd>" "<out>"` — when the command line both begins and ends with a
  quote, `cmd.exe` strips the outer quotes and mangles the line, making EVERY `.cmd`
  spuriously exit 1. If a validation run reports all/most `.cmd` failing while
  `run-all-demos.cmd` passes, suspect the harness invocation, not the demos.

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
        - $canonical\global.json  exists, pins the newest STABLE (non-preview, non-rc) installed SDK band with rollForward "latestPatch"; verify `dotnet --version` from $canonical reports a STABLE 10.0.x+ SDK. Reject ONLY -preview/-rc builds — do NOT require a literal 10.0.2xx. (Reference run pinned 10.0.300 because no 10.0.2xx band was installed.)
        - $canonical\Directory.Build.props                  exists, contains <_LongPathsEnabled>true
        - $canonical\TinyLanguage\TinyLanguage.csproj       exists, contains <ApplicationManifest>
        - $canonical\TinyLanguage\app.manifest              exists, contains <longPathAware>true
        - $canonical\TinyLanguage.Lexer\...csproj           exists
        - $canonical\TinyLanguage.Interpreter\...csproj     exists
        - $canonical\TinyLanguage.UnitTests\...csproj       exists
        - $canonical\TinyLanguage.IntegrationTests\...csproj exists
        - $canonical\TinyLanguage.DemoFiles\TinyLanguage.DemoFiles.csproj exists
        - $canonical\TinyLanguage.DemoFiles\*.tlg           ≥ 500 files (Tier A + Tier B + Tier C)
        - $canonical\TinyLanguage.DemoFiles\*.cmd           one per .tlg
        - $canonical\TinyLanguage.DebugAdapter\TinyLanguage.DebugAdapter.csproj exists (Phase 4C)
        - $canonical\extensions\vscode\package.json         exists (Phase 4D)
        - $canonical\extensions\vs\TinyLanguage.VsTools.csproj exists (Phase 4F)
        - $canonical\extensions\extensions.shproj           exists (Phase 4F)
        - $canonical\extensions\extensions.projitems        exists (Phase 4F)
        - $canonical\install-vscode-debugger.cmd            exists (Phase 4D)
        - $canonical\install-vs-debugger.cmd                exists (Phase 4F)
        - $canonical\TinyLanguage.wiki.md                   exists, > 200 lines (Phase 4E)
        - HKLM\SYSTEM\CurrentControlSet\Control\FileSystem\LongPathsEnabled DWORD —
          read it (`(Get-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem' -Name LongPathsEnabled -EA SilentlyContinue).LongPathsEnabled`).
          If 1, good. If missing/0, **WARN** and print the admin command
          `reg add "HKLM\SYSTEM\CurrentControlSet\Control\FileSystem" /v LongPathsEnabled /t REG_DWORD /d 1 /f`
          — but do **NOT** abort delivery solely because it is 0. The key only matters
          when the canonical path + deepest publish intermediates exceed 260 chars; with
          the short canonical path (`...YYYY.MM.DD.HH\` ≈ 36 chars) and long-path
          Layers 2-3 (Directory.Build.props `_LongPathsEnabled` + app.manifest
          `longPathAware`) present, VS Batch Rebuild succeeds regardless. Treat a clean
          Step 6i `devenv.com /Rebuild` (exit 0, no MAX_PATH errors) as the AUTHORITATIVE
          proof. On the reference run the registry value was 0 yet Step 6i was fully clean.
          Only escalate to a hard block if Step 6i actually reports a MAX_PATH error.
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

  6i. **Visual Studio Batch Rebuild smoke test** *(blocks plan completion)*

      The fundamental promise of this orchestration: the deliverable is
      Visual-Studio-buildable end to end, not just `dotnet build`-buildable.
      Drive `devenv.com` from the command line so a CI can verify it without a
      human clicking through the IDE.

      First locate `devenv.com`. Try in order:
        1. `Get-Command devenv.com -EA SilentlyContinue` (if PATH has it).
        2. `& "${env:ProgramFiles}\Microsoft Visual Studio\2026\Professional\Common7\IDE\devenv.com"`
        3. `& "${env:ProgramFiles}\Microsoft Visual Studio\2026\Enterprise\Common7\IDE\devenv.com"`
        4. `& "${env:ProgramFiles}\Microsoft Visual Studio\2026\Community\Common7\IDE\devenv.com"`
        5. Fallback to `vswhere.exe`:
             `& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property productPath`
           and switch the trailing exe to devenv.com.
        If none resolves, print a stderr message naming all candidate paths
        searched and exit 1.

      Run:
        & $devenv "$canonical\TinyLanguage.slnx" /Rebuild "Debug|Any CPU" `
          /Out "$env:TEMP\tlg_vs_batch_rebuild.log"

      Accept all of:
        - $LASTEXITCODE -eq 0
        - The /Out log contains "Rebuild All succeeded" (or the localised
          equivalent for non-en-US installs — fall back to checking that
          `Select-String -Path $logPath -Pattern '0 failed'` finds a hit).
        - The /Out log does NOT contain any of:
          "The fully qualified file name must be less than 260 characters"
          "The specified path, file name, or both are too long"
          "MSB6005"  (path-too-long MSBuild error)
          "MSB3491"  (also path-too-long)

      If any check fails: re-verify Layers 1–3 of long-path support, restart
      Visual Studio if Layer 1 was just enabled in this session, and re-run.
      Path-too-long failures here are NOT an acceptable outcome — they mean
      the solution we just delivered is unusable in the IDE.

All seven steps must be green. Do not declare the plan complete until Step 6i
confirms `devenv.com /Rebuild` exits 0 with no MAX_PATH errors. Do not
refactor unrelated code. Do not alter Build.Solution.md.
```

---

## Agent Tool Invocation Pattern

When Claude Code executes this plan, each phase maps to an `Agent` tool call.
Before launching any parallel pair, re-read "Phase sequencing — project references
override the 'parallel' labels" in "How to Run This Plan": project-reference edges
make 1A→1B/1C, 3A→3B, and 4B→4A mandatory, and 4E/4F must not build concurrently.

```jsonc
// Phase 1 — four calls in a SINGLE message (parallel)
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1A
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1B
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1C
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 1D

// Phase 4E + 4F — two calls in a SINGLE message (parallel, both after 4D, both before Phase 5)
Agent({ subagent_type: "general-purpose", isolation: "worktree", prompt: "..." })  // 4E (wiki)
Agent({ subagent_type: "general-purpose", isolation: "worktree",
        model: "opus", prompt: "..." })  // 4F (VS18 shim + Shared Project)

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
| 4F — VS18 Shim + Shared Project | `claude-opus-4-6` | VS Debug Adapter Host wiring, pkgdef registry shape, .shproj import chain |
| 5 — Validation | `claude-sonnet-4-6` | Fix-and-retry loop, targeted edits |

---

## Constraints (from Build.Solution.md — never override)

- `Build.Solution.md` is READ-ONLY. No agent may modify it.
- No external NuGet packages. BCL only.
- No nullable, no implicit usings.
- MSTest only (no XUnit, no NUnit).
- One type per file.
- All acceptance criteria must be green before the plan is complete.
