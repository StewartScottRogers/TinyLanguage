# Build.DataStructures.md — Data-structure catalogue & "Data Structures" test suite

This file is **consumed by `Build.md` on every orchestration run**. It guarantees the
regenerated solution contains a dedicated MSTest project,
**`TinyLanguage.DataStructures.Tests`**, with **one numbered test per data structure**
on Wikipedia's *List of data structures*, each **implemented in the TinyLanguage
language** (a `.tlg` program) and validated against a golden `.expected` output.

It is a companion to `Build.md` (the orchestration plan) and `Build.Plan.md` (the
work-unit breakdown). `Build.Solution.md` remains the **locked, read-only** spec; this
project is an **additive deviation** recorded as D23 in `Build.md` / `Build.Plan.md`.

- **Source list:** <https://en.wikipedia.org/wiki/List_of_data_structures>
- **Machine-readable catalogue (the source of truth for generation):**
  `Z:\repos\TinyLanguage\tools\data-structures.catalogue.json` — 220 objects, one per
  catalogue entry, each `{ n, name, category, section, feasible, existing_demo,
  test_method, duplicate_of, approach }`. The table in §5 below is rendered from it.
- **Scope:** **220 numbered catalogue entries** spanning **206 unique structures**
  (14 entries are explicit duplicates/synonyms of another entry, faithfully mirroring
  the page's cross-section listings — see §6). A completeness audit against the live
  page found **no missing structures**.

> **Why this exists.** The corpus already ships a Tier C "Wikipedia-style data-structure
> catalogue" (demos `00500–00659`), but there was no *test suite* that maps 1:1 to the
> Wikipedia list, and the catalogue covered only part of the page. This file makes the
> coverage **complete and numbered**, and turns it into an asserted test suite. Future
> regenerations MUST honour it; do not delete it on the assumption "the spec doesn't
> mention it."

---

## 1. Feasibility legend

Every entry carries a `feasible` classification (column **Feas.** in §5). These were
produced by an adversarially-verified per-section classification against TinyLanguage's
exact capabilities (no closures, no bitwise operators, no map literal, BCL-only,
single-threaded, deterministic-output-only).

| Value | Meaning | Test outcome |
|-------|---------|--------------|
| `yes` | Directly implementable as a faithful class-based structure with real operations. | Real passing test. |
| `approx` | Implementable as a **faithful approximation** — the `approach` field states exactly what is approximated (almost always: a bitwise trick simulated arithmetically with `div`/`mod` by powers of two, per the corpus's standing policy; or a randomized structure made deterministic via an in-program seeded LCG; or a distributed structure modeled single-process). | Real passing test. |
| `primitive` | The entry **is** one of TinyLanguage's own built-in primitive/representation types (Boolean, Integer, Floating-point, Character, String, Array, Reference). There is no separate structure to build; the demo is a minimal *usage* program (declare / use / print). | Real passing test. |
| `no` | **NOT IMPLEMENTABLE** in a single-process tree-walking BCL-only interpreter. | `[Ignore("NOT IMPLEMENTABLE: <reason>")]`, still counted in the 220. |

**Current split: `yes`=168, `approx`=44, `primitive`=8, `no`=0.** Every structure on the
page is implementable at least as a faithful approximation, so **every numbered test is a
real, passing test today**. The `no` / `[Ignore]` mechanism is specified so the numbered
list stays complete and stable if a future page revision adds something genuinely
infeasible.

---

## 2. The test project — `TinyLanguage.DataStructures.Tests`

A new, **eighth .NET project** in `TinyLanguage.slnx` (scaffolded empty by Phase 1A,
filled in by Phase 4G — see §4). It is an end-to-end suite: each data structure is
implemented as a `.tlg` demo, run through the interpreter in-process, and its stdout is
byte-compared to the demo's golden `.expected` file.

**Project conventions (all enforced by the 0-warning / `Failed:0` gates):**

- MSTest 4.x, `net10.0`, **no nullable, no implicit usings**, BCL + MSTest only.
- `<ProjectReference>`s (solution-relative): `..\TinyLanguage.Interpreter\TinyLanguage.Interpreter.csproj`
  and `..\TinyLanguage.Lexer\TinyLanguage.Lexer.csproj`. It runs `.tlg` **in-process**
  through the interpreter (mirroring `InterpreterIntegrationTests`); it does **not**
  depend on the published `TinyLanguage.exe`, so it needs no prior `dotnet publish`.
- `MSTestSettings.cs`: `[assembly: Microsoft.VisualStudio.TestTools.UnitTesting.Parallelize(Workers = 1, Scope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope.MethodLevel)]`
  (MSTEST0001 + console-redirection-race avoidance — see the "MSTest 4.x" preamble in `Build.md`).
- `TestLog.cs`: the **exact** `TestLog` helper from the "Test output formatting —
  non-negotiable" preamble of `Build.md` (D8), namespace
  `TinyLanguage.DataStructures.Tests`. Every test prints its input and result through it.
- Use the MSTest 4.x assertion APIs (`Assert.AreEqual`, `Assert.Throws<T>`, …) per the
  "MSTest 4.x" preamble; alias the lexer type (`using LexerCore = TinyLanguage.Lexer.Lexer;`)
  if a test file opens the `TinyLanguage.Lexer` namespace.

**Shared runner — `DataStructureGoldenRunner.cs`:**

- `ResolveDemoDirectory()` — walk **up** from `System.AppContext.BaseDirectory` until a
  directory containing `TinyLanguage.slnx` is found, then return its
  `TinyLanguage.DemoFiles` subdirectory. (If `InterpreterIntegrationTests` already has a
  DemoFiles-resolution helper, reuse the same logic so both projects agree.)
- `Run(basename)` — read `<basename>.tlg` (streamed), lex → parse → interpret capturing
  stdout, return the output normalised (CRLF→LF, trailing newline trimmed).
- `Golden(basename)` — read `<basename>.expected`, normalised the same way.
- `AssertMatchesGolden(demoBasename)` — `TestLog.Input(demoBasename, source)`; run;
  `TestLog.Result(actual)`; `Assert.AreEqual(Golden(demoBasename), actual)`. This is the
  same golden contract the Phase 4.5 sweep enforces, so the in-process result and the
  file-mode `.expected` agree by construction (one `Stringify`, LF newlines).

**Test classes & method naming:**

- One class `DataStructureCatalogueIntegrationTests` (or several `*IntegrationTests`
  partial classes split by category for readability). Class names end in
  `IntegrationTests` per `Build.Solution.md` naming; the **project** name carries the
  user-facing label "Data Structures".
- **One `[TestMethod]` per catalogue entry**, named from the catalogue `test_method`
  field plus an outcome suffix, e.g.
  `public void Ds079_SplayTree_MatchesGoldenOutput()` → `AssertMatchesGolden("00528.splay_tree")`.
  The `Ds###` prefix encodes the catalogue number (stable, unique) and is followed by the
  `Subject_Action_ExpectedOutcome` form the spec requires.
- A `primitive` entry asserts its minimal usage demo's golden exactly like any other.
- A future `no` entry gets `[Ignore("NOT IMPLEMENTABLE: <reason from catalogue>")]` and a
  body that documents the blocker — it is still one of the 220 numbered methods.

---

## 3. Where the `.tlg` implementations come from (reuse + extend Tier C)

Each catalogue entry maps to **exactly one** demo — its authentic implementation:

1. **Reuse** an existing demo when the catalogue's `existing_demo` is the authentic 1:1
   implementation of that exact structure (e.g. `00528.splay_tree` ↔ Splay tree,
   `00580.binomial_heap` ↔ Binomial heap) **and** that demo already satisfies the
   **load contract (§7)**. Reuse its `.tlg` + `.expected` verbatim. If the existing demo is
   toy-scale (a handful of literal operations on a 3–5 element instance), it does **not**
   qualify for verbatim reuse: extend it to the load floor (regenerating its `.expected`) or
   author a load-bearing replacement in `00700–00899` and rebind the entry in the manifest.
2. **Author a new faithful demo** when `existing_demo` is `_new_` **or** is a
   *closest-related* demo of a **different** structure (clear from the entry's `approach`
   text, e.g. "extend the R-tree class…", "specialization of the d-ary heap", "combine the
   B+-tree with Morton keys"). New demos go in the **reserved band `00700–00899`**
   ("Tier C catalogue-completion"), named `007NN.<snake_name>.tlg`, and MUST satisfy the
   **Tier C per-demo contract** (header `# Structure / # Category / # Operations (Big-O) /
   # Reference`; class-based; ≥3 named subroutines; ≥8 distinct operation **kinds**; a
   **non-trivial workload per the load contract (§7)** — loop/seed-generated to the
   structure's scale floor, triggering its characteristic behavior, with ≥2 post-load
   invariant checks and a traversal checksum; deterministic labelled output (a **compact
   post-load summary**, never a dump of the N elements); 30–250 lines; `;` separators; a
   golden `.expected`; a matching `.cmd`). Approximations follow the entry's `approach` (arithmetic
   bit-slicing, seeded LCG, single-process modeling) and note the deviation in the header.
3. **Duplicates/synonyms** (the **Dup-of** column / `duplicate_of` field): reuse the
   canonical entry's demo. Their test still runs and asserts that demo's golden — a real
   passing test, flagged in §5 so the redundancy is visible.

**The manifest (entry → demo binding).** Phase 1D writes
`TinyLanguage.DemoFiles\catalogue.manifest.tsv` — one tab-separated row per catalogue `n`:

```
n<TAB>structure<TAB>demo_basename<TAB>test_method
```

The catalogue's `existing_demo` is only a **hint**; `catalogue.manifest.tsv` is the
**authoritative** binding Phase 4G consumes to wire each test to its demo. The manifest
MUST have exactly **220 rows**, every `demo_basename` MUST exist as a `<basename>.tlg`
with a matching `<basename>.expected`, and every new basename MUST fall in `00700–00899`.

All new demos participate in the Phase 4.5 golden sweep and `run-all-demos.cmd` like any
other demo — they are ordinary members of the demo corpus.

---

## 4. Orchestration wiring (what each phase does)

| Phase | Responsibility for this feature |
|-------|--------------------------------|
| **1A** | Scaffold an empty `TinyLanguage.DataStructures.Tests\TinyLanguage.DataStructures.Tests.csproj` (MSTest 4.x, refs to Interpreter + Lexer) and add `<Project Path="TinyLanguage.DataStructures.Tests\TinyLanguage.DataStructures.Tests.csproj" />` to `TinyLanguage.slnx` (alongside the other test projects). |
| **1D** | Author every catalogue-completion `.tlg` + `.expected` (band `00700–00899`) that is not an authentic reuse, per §3; write `catalogue.manifest.tsv` (220 rows). **Every authored (and every rebound) demo meets the load contract (§7)** — workload loop/seed-generated to the tier floor, characteristic behavior triggered, ≥2 post-load invariant checks + a traversal checksum, runtime < 2 s. Hardcode the canonical absolute DemoFiles path into each fan-out agent prompt (per the `args`-don't-reach-subagents lesson). |
| **4.5** | The golden sweep now covers the expanded demo set; gate stays `FAILED=0 TIMEOUT=0 GOLD=0`. New demos are triaged like any other. Load demos (§7) MUST stay under the sweep's 5 s per-demo timeout at their chosen scale — a load demo that TIMEs out is a triage item (drop N to the next tier down, prefer indexed-assign growth over `arr + [x]`), never a reason to relax the timeout. |
| **4G** *(new)* | Read `tools\data-structures.catalogue.json` + `catalogue.manifest.tsv`; generate `TinyLanguage.DataStructures.Tests` (TestLog, runner, `MSTestSettings`, exactly 220 `[TestMethod]`s). Runs after 3A (interpreter) and 4.5 (demos green); serialized with the other build-running test phases. `dotnet test TinyLanguage.DataStructures.Tests` → `Failed: 0`. |
| **5** | Gate: project builds 0/0; `dotnet test` `Failed: 0`; **method count == 220 == catalogue size == manifest rows**; structural check for the new csproj at the canonical path; golden sweep still `0/0/0`; **spot-audit the load contract (§7)** on a sample of demos (workload is loop/seed-generated, characteristic behavior is triggered, ≥2 invariant checks + a checksum are printed — a toy demo printing only a few literal operations fails the audit). |

---

## 5. The catalogue (220 numbered entries, page order)

Legend — **Feas.**: see §1. **Nearest demo**: authentic-reuse hint, or `_new_` (author in
`00700–00899`); a related-but-different demo is still listed as a starting template but per
§3 should be replaced by a faithful new demo. **Dup-of**: this entry re-lists/synonymises an
earlier entry (reuse its demo).

Per-section counts: Primitive 9 · Composite 6 · Abstract data types 15 · Arrays 20 ·
Lists 14 · Binary trees 23 · B-trees 9 · Heaps 19 · Bit-slice trees 12 · Multi-way trees 16 ·
Space-partitioning trees 28 · Application-specific trees 10 · Hash-based 18 · Graphs 14 ·
Other 7  =  **220**.

| # | Structure | Category | Feas. | Nearest demo | Test method | Dup-of |
|---|-----------|----------|-------|--------------|-------------|--------|
| 1 | Boolean | Data types - Primitive types | primitive | _new_ | `Ds001_BooleanPrimitive` |  |
| 2 | Character | Data types - Primitive types | primitive | _new_ | `Ds002_CharacterPrimitive` |  |
| 3 | Floating-point | Data types - Primitive types | primitive | _new_ | `Ds003_FloatingPointPrimitive` |  |
| 4 | Fixed-point | Data types - Primitive types | approx | _new_ | `Ds004_FixedPoint` |  |
| 5 | Integer | Data types - Primitive types | primitive | _new_ | `Ds005_IntegerPrimitive` |  |
| 6 | Reference | Data types - Primitive types | primitive | `00657.tagged_pointer` | `Ds006_ReferencePrimitive` |  |
| 7 | Symbol | Data types - Primitive types | approx | _new_ | `Ds007_SymbolIntern` |  |
| 8 | Enumerated type | Data types - Primitive types | approx | `00651.tagged_union` | `Ds008_EnumeratedType` |  |
| 9 | Complex | Data types - Primitive types | approx | _new_ | `Ds009_ComplexNumber` |  |
| 10 | Array | Data types - Composite types | primitive | `00511.array_list_growable` | `Ds010_ArrayPrimitive` |  |
| 11 | Record | Data types - Composite types | yes | `00650.record_struct` | `Ds011_Record` |  |
| 12 | Product type | Data types - Composite types | yes | `00652.tuple_pair` | `Ds012_ProductType` | = Record |
| 13 | String | Data types - Composite types | primitive | `00654.string_view` | `Ds013_StringPrimitive` |  |
| 14 | Union | Data types - Composite types | approx | `00659.union_with_discriminator` | `Ds014_Union` |  |
| 15 | Tagged union | Data types - Composite types | yes | `00651.tagged_union` | `Ds015_TaggedUnion` |  |
| 16 | Container | Data types - Abstract data types | yes | `00511.array_list_growable` | `Ds016_Container` |  |
| 17 | List | Data types - Abstract data types | yes | `00511.array_list_growable` | `Ds017_ListAdt` |  |
| 18 | Tuple | Data types - Abstract data types | yes | `00652.tuple_pair` | `Ds018_Tuple` |  |
| 19 | Associative array | Data types - Abstract data types | yes | `00643.ordered_map` | `Ds019_AssociativeArray` |  |
| 20 | Map | Data types - Abstract data types | yes | `00590.hash_table_chained_resize` | `Ds020_MapAdt` | = Associative array |
| 21 | Multimap | Data types - Abstract data types | yes | `00640.multimap` | `Ds021_Multimap` |  |
| 22 | Set | Data types - Abstract data types | yes | `00642.ordered_set` | `Ds022_SetAdt` |  |
| 23 | Multiset (bag) | Data types - Abstract data types | yes | `00641.multiset` | `Ds023_MultisetBag` |  |
| 24 | Stack | Data types - Abstract data types | yes | _new_ | `Ds024_Stack` |  |
| 25 | Queue | Data types - Abstract data types | yes | _new_ | `Ds025_Queue` |  |
| 26 | Priority queue | Data types - Abstract data types | yes | `00586.binary_heap_in_array` | `Ds026_PriorityQueue` |  |
| 27 | Double-ended queue | Data types - Abstract data types | yes | `00588.double_ended_priority_queue` | `Ds027_Deque` |  |
| 28 | Graph | Data types - Abstract data types | yes | `00600.graph_adjacency_matrix` | `Ds028_GraphAdt` |  |
| 29 | Tree | Data types - Abstract data types | yes | `00521.binary_tree_traversals` | `Ds029_TreeAdt` |  |
| 30 | Heap | Data types - Abstract data types | yes | `00586.binary_heap_in_array` | `Ds030_HeapAdt` |  |
| 31 | Array | Linear - Arrays | primitive | _new_ | `Ds031_Array` |  |
| 32 | Associative array | Linear - Arrays | yes | `00643.ordered_map` | `Ds032_AssociativeArray` |  |
| 33 | Bit array | Linear - Arrays | approx | `00564.bit_trie` | `Ds033_BitArray` |  |
| 34 | Bit field | Linear - Arrays | approx | `00657.tagged_pointer` | `Ds034_BitField` |  |
| 35 | Bitboard | Linear - Arrays | approx | _new_ | `Ds035_Bitboard` |  |
| 36 | Bitmap | Linear - Arrays | approx | `00564.bit_trie` | `Ds036_Bitmap` | = Bit array |
| 37 | Circular buffer | Linear - Arrays | yes | _new_ | `Ds037_CircularBuffer` |  |
| 38 | Control table | Linear - Arrays | yes | _new_ | `Ds038_ControlTable` |  |
| 39 | Image | Linear - Arrays | approx | _new_ | `Ds039_Image` |  |
| 40 | Dope vector | Linear - Arrays | yes | _new_ | `Ds040_DopeVector` |  |
| 41 | Dynamic array | Linear - Arrays | yes | `00511.array_list_growable` | `Ds041_DynamicArray` |  |
| 42 | Gap buffer | Linear - Arrays | yes | _new_ | `Ds042_GapBuffer` |  |
| 43 | Hashed array tree | Linear - Arrays | yes | _new_ | `Ds043_HashedArrayTree` |  |
| 44 | Lookup table | Linear - Arrays | yes | _new_ | `Ds044_LookupTable` |  |
| 45 | Matrix | Linear - Arrays | yes | _new_ | `Ds045_Matrix` |  |
| 46 | Parallel array | Linear - Arrays | yes | `00658.struct_array_of_structs` | `Ds046_ParallelArray` |  |
| 47 | Sorted array | Linear - Arrays | yes | _new_ | `Ds047_SortedArray` |  |
| 48 | Sparse matrix | Linear - Arrays | yes | `00603.graph_csr` | `Ds048_SparseMatrix` |  |
| 49 | Iliffe vector | Linear - Arrays | yes | _new_ | `Ds049_IliffeVector` |  |
| 50 | Variable-length array | Linear - Arrays | yes | `00511.array_list_growable` | `Ds050_VariableLengthArray` | = Dynamic array |
| 51 | Doubly linked list | Linear - Lists | yes | _new_ | `Ds051_DoublyLinkedList` |  |
| 52 | Array list | Linear - Lists | yes | `00511.array_list_growable` | `Ds052_ArrayList` |  |
| 53 | Linked list | Linear - Lists | yes | `00512.singly_linked_list_advanced` | `Ds053_SinglyLinkedList` |  |
| 54 | Association list | Linear - Lists | yes | `00500.assoc_list` | `Ds054_AssociationList` |  |
| 55 | Self-organizing list | Linear - Lists | yes | `00501.self_organizing_list` | `Ds055_SelfOrganizingList` |  |
| 56 | Skip list | Linear - Lists | approx | `00502.skip_list` | `Ds056_SkipList` |  |
| 57 | Unrolled linked list | Linear - Lists | yes | `00503.unrolled_linked_list` | `Ds057_UnrolledLinkedList` |  |
| 58 | VList | Linear - Lists | yes | `00504.vlist` | `Ds058_VList` |  |
| 59 | Conc-tree list | Linear - Lists | yes | `00505.conc_tree` | `Ds059_ConcTreeList` |  |
| 60 | Xor linked list | Linear - Lists | approx | `00506.xor_linked_list` | `Ds060_XorLinkedList` |  |
| 61 | Zipper | Linear - Lists | yes | `00507.zipper_list` | `Ds061_ZipperList` |  |
| 62 | Doubly connected edge list | Linear - Lists | yes | `00508.dcel` | `Ds062_Dcel` |  |
| 63 | Difference list | Linear - Lists | approx | `00509.difference_list` | `Ds063_DifferenceList` |  |
| 64 | Free list | Linear - Lists | yes | `00510.free_list` | `Ds064_FreeList` |  |
| 65 | AA tree | Trees - Binary trees | yes | `00520.aa_tree` | `Ds065_AaTree` |  |
| 66 | AVL tree | Trees - Binary trees | yes | _new_ | `Ds066_AvlTree` |  |
| 67 | Binary search tree | Trees - Binary trees | yes | `00534.binary_search_tree_iterative` | `Ds067_BinarySearchTree` |  |
| 68 | Binary tree | Trees - Binary trees | yes | `00521.binary_tree_traversals` | `Ds068_BinaryTree` |  |
| 69 | Cartesian tree | Trees - Binary trees | yes | `00522.cartesian_tree` | `Ds069_CartesianTree` |  |
| 70 | Conc-tree list | Trees - Binary trees | yes | `00505.conc_tree` | `Ds070_ConcTreeList` |  |
| 71 | Left-child right-sibling binary tree | Trees - Binary trees | yes | `00523.left_child_right_sibling` | `Ds071_LeftChildRightSibling` |  |
| 72 | Order statistic tree | Trees - Binary trees | yes | `00524.order_statistic_tree` | `Ds072_OrderStatisticTree` |  |
| 73 | Pagoda | Trees - Binary trees | yes | _new_ | `Ds073_Pagoda` |  |
| 74 | Randomized binary search tree | Trees - Binary trees | yes | `00525.randomized_bst` | `Ds074_RandomizedBst` |  |
| 75 | Red–black tree | Trees - Binary trees | yes | _new_ | `Ds075_RedBlackTree` |  |
| 76 | Rope | Trees - Binary trees | yes | `00526.rope` | `Ds076_Rope` |  |
| 77 | Scapegoat tree | Trees - Binary trees | yes | `00527.scapegoat_tree` | `Ds077_ScapegoatTree` |  |
| 78 | Self-balancing binary search tree | Trees - Binary trees | yes | `00520.aa_tree` | `Ds078_SelfBalancingBst` | = AA tree |
| 79 | Splay tree | Trees - Binary trees | yes | `00528.splay_tree` | `Ds079_SplayTree` |  |
| 80 | T-tree | Trees - Binary trees | yes | _new_ | `Ds080_TTree` |  |
| 81 | Tango tree | Trees - Binary trees | yes | _new_ | `Ds081_TangoTree` |  |
| 82 | Threaded binary tree | Trees - Binary trees | yes | `00529.threaded_binary_tree` | `Ds082_ThreadedBinaryTree` |  |
| 83 | Top tree | Trees - Binary trees | yes | _new_ | `Ds083_TopTree` |  |
| 84 | Treap | Trees - Binary trees | yes | `00530.treap` | `Ds084_Treap` |  |
| 85 | WAVL tree | Trees - Binary trees | yes | `00531.wavl_tree` | `Ds085_WavlTree` |  |
| 86 | Weight-balanced tree | Trees - Binary trees | yes | `00532.weight_balanced_tree` | `Ds086_WeightBalancedTree` |  |
| 87 | Zip tree | Trees - Binary trees | yes | `00533.zip_tree` | `Ds087_ZipTree` |  |
| 88 | B-tree | Trees - B-trees | yes | `00570.btree` | `Ds088_BTree` |  |
| 89 | B+ tree | Trees - B-trees | yes | `00571.bplus_tree` | `Ds089_BPlusTree` |  |
| 90 | B*-tree | Trees - B-trees | yes | `00570.btree` | `Ds090_BStarTree` |  |
| 91 | Dancing tree | Trees - B-trees | approx | `00570.btree` | `Ds091_DancingTree` |  |
| 92 | 2–3 tree | Trees - B-trees | yes | `00572.btree_two_three` | `Ds092_TwoThreeTree` |  |
| 93 | 2–3–4 tree | Trees - B-trees | yes | `00573.btree_two_three_four` | `Ds093_TwoThreeFourTree` |  |
| 94 | Queap | Trees - B-trees | yes | `00586.binary_heap_in_array` | `Ds094_Queap` |  |
| 95 | Fusion tree | Trees - B-trees | approx | `00570.btree` | `Ds095_FusionTree` |  |
| 96 | Bx-tree | Trees - B-trees | approx | `00571.bplus_tree` | `Ds096_BxTree` |  |
| 97 | Heap | Trees - Heaps | yes | `00586.binary_heap_in_array` | `Ds097_Heap` |  |
| 98 | Min-max heap | Trees - Heaps | yes | `00588.double_ended_priority_queue` | `Ds098_MinMaxHeap` |  |
| 99 | Binary heap | Trees - Heaps | yes | `00586.binary_heap_in_array` | `Ds099_BinaryHeap` | = Heap |
| 100 | B-heap | Trees - Heaps | approx | `00586.binary_heap_in_array` | `Ds100_BHeap` |  |
| 101 | Weak heap | Trees - Heaps | yes | `00586.binary_heap_in_array` | `Ds101_WeakHeap` |  |
| 102 | Binomial heap | Trees - Heaps | yes | `00580.binomial_heap` | `Ds102_BinomialHeap` |  |
| 103 | Fibonacci heap | Trees - Heaps | yes | `00581.fibonacci_heap` | `Ds103_FibonacciHeap` |  |
| 104 | AF-heap | Trees - Heaps | approx | `00580.binomial_heap` | `Ds104_AfHeap` |  |
| 105 | Leonardo heap | Trees - Heaps | yes | `00586.binary_heap_in_array` | `Ds105_LeonardoHeap` |  |
| 106 | 2–3 heap | Trees - Heaps | yes | `00580.binomial_heap` | `Ds106_TwoThreeHeap` |  |
| 107 | Soft heap | Trees - Heaps | yes | `00580.binomial_heap` | `Ds107_SoftHeap` |  |
| 108 | Pairing heap | Trees - Heaps | yes | `00582.pairing_heap` | `Ds108_PairingHeap` |  |
| 109 | Leftist heap | Trees - Heaps | yes | `00583.leftist_heap` | `Ds109_LeftistHeap` |  |
| 110 | Treap | Trees - Heaps | yes | `00530.treap` | `Ds110_Treap` |  |
| 111 | Beap | Trees - Heaps | yes | `00586.binary_heap_in_array` | `Ds111_Beap` |  |
| 112 | Skew heap | Trees - Heaps | yes | `00584.skew_heap` | `Ds112_SkewHeap` |  |
| 113 | Ternary heap | Trees - Heaps | yes | `00585.d_ary_heap` | `Ds113_TernaryHeap` | = D-ary heap |
| 114 | D-ary heap | Trees - Heaps | yes | `00585.d_ary_heap` | `Ds114_DaryHeap` |  |
| 115 | Brodal queue | Trees - Heaps | approx | `00581.fibonacci_heap` | `Ds115_BrodalQueue` |  |
| 116 | Radix tree | Trees - Bit-slice trees | yes | `00560.radix_tree` | `Ds116_RadixTree` |  |
| 117 | Suffix tree | Trees - Bit-slice trees | yes | `00561.suffix_tree` | `Ds117_SuffixTree` |  |
| 118 | Suffix array | Trees - Bit-slice trees | yes | `00568.suffix_array` | `Ds118_SuffixArray` |  |
| 119 | Compressed suffix array | Trees - Bit-slice trees | approx | `00568.suffix_array` | `Ds119_CompressedSuffixArray` |  |
| 120 | FM-index | Trees - Bit-slice trees | approx | `00568.suffix_array` | `Ds120_FmIndex` |  |
| 121 | Generalised suffix tree | Trees - Bit-slice trees | yes | `00561.suffix_tree` | `Ds121_GeneralisedSuffixTree` | = Suffix tree |
| 122 | B-tree | Trees - Bit-slice trees | yes | `00570.btree` | `Ds122_BTreeBitSlice` | = B-tree |
| 123 | Judy array | Trees - Bit-slice trees | approx | `00560.radix_tree` | `Ds123_JudyArray` |  |
| 124 | Trie | Trees - Bit-slice trees | yes | `00565.trie_autocomplete` | `Ds124_Trie` |  |
| 125 | X-fast trie | Trees - Bit-slice trees | approx | `00564.bit_trie` | `Ds125_XFastTrie` |  |
| 126 | Y-fast trie | Trees - Bit-slice trees | approx | `00564.bit_trie` | `Ds126_YFastTrie` |  |
| 127 | Merkle tree | Trees - Bit-slice trees | approx | `00604.blockchain` | `Ds127_MerkleTree` |  |
| 128 | Ternary search tree | Trees - Multi-way trees | yes | `00562.ternary_search_tree` | `Ds128_TernarySearchTree` |  |
| 129 | Ternary tree | Trees - Multi-way trees | yes | _new_ | `Ds129_TernaryTree` |  |
| 130 | K-ary tree | Trees - Multi-way trees | yes | _new_ | `Ds130_KaryTree` |  |
| 131 | And–or tree | Trees - Multi-way trees | yes | _new_ | `Ds131_AndOrTree` |  |
| 132 | (a,b)-tree | Trees - Multi-way trees | yes | `00570.btree` | `Ds132_ABTree` |  |
| 133 | Link/cut tree | Trees - Multi-way trees | approx | `00528.splay_tree` | `Ds133_LinkCutTree` |  |
| 134 | SPQR-tree | Trees - Multi-way trees | approx | _new_ | `Ds134_SpqrTree` |  |
| 135 | Spaghetti stack | Trees - Multi-way trees | yes | _new_ | `Ds135_SpaghettiStack` |  |
| 136 | Disjoint-set data structure | Trees - Multi-way trees | yes | `00644.disjoint_set_union_advanced` | `Ds136_DisjointSet` |  |
| 137 | Union-find data structure | Trees - Multi-way trees | yes | `00644.disjoint_set_union_advanced` | `Ds137_UnionFind` | = Disjoint-set data structure |
| 138 | Fusion tree | Trees - Multi-way trees | approx | `00570.btree` | `Ds138_FusionTree` |  |
| 139 | Enfilade | Trees - Multi-way trees | yes | _new_ | `Ds139_Enfilade` |  |
| 140 | Exponential tree | Trees - Multi-way trees | yes | `00570.btree` | `Ds140_ExponentialTree` |  |
| 141 | Fenwick tree | Trees - Multi-way trees | approx | _new_ | `Ds141_FenwickTree` |  |
| 142 | Van Emde Boas tree | Trees - Multi-way trees | approx | _new_ | `Ds142_VanEmdeBoasTree` |  |
| 143 | Rose tree | Trees - Multi-way trees | yes | _new_ | `Ds143_RoseTree` |  |
| 144 | Segment tree | Trees - Space-partitioning trees | yes | _new_ | `Ds144_SegmentTree` |  |
| 145 | Interval tree | Trees - Space-partitioning trees | yes | `00615.interval_tree` | `Ds145_IntervalTree` |  |
| 146 | Range tree | Trees - Space-partitioning trees | yes | `00616.range_tree` | `Ds146_RangeTree` |  |
| 147 | Bin | Trees - Space-partitioning trees | yes | `00617.bin_grid` | `Ds147_BinSpatial` |  |
| 148 | K-d tree | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds148_KdTree` |  |
| 149 | Implicit k-d tree | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds149_ImplicitKdTree` |  |
| 150 | Min/max k-d tree | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds150_MinMaxKdTree` |  |
| 151 | Relaxed k-d tree | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds151_RelaxedKdTree` |  |
| 152 | Adaptive k-d tree | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds152_AdaptiveKdTree` |  |
| 153 | Quadtree | Trees - Space-partitioning trees | yes | `00619.quadtree` | `Ds153_Quadtree` |  |
| 154 | Octree | Trees - Space-partitioning trees | yes | `00619.quadtree` | `Ds154_Octree` |  |
| 155 | Linear octree | Trees - Space-partitioning trees | approx | `00629.morton_order_sort` | `Ds155_LinearOctree` |  |
| 156 | Z-order | Trees - Space-partitioning trees | approx | `00620.z_order_curve` | `Ds156_ZOrderCurve` |  |
| 157 | UB-tree | Trees - Space-partitioning trees | approx | `00571.bplus_tree` | `Ds157_UbTree` |  |
| 158 | R-tree | Trees - Space-partitioning trees | yes | `00622.r_tree_2d` | `Ds158_RTree` |  |
| 159 | R+ tree | Trees - Space-partitioning trees | yes | `00622.r_tree_2d` | `Ds159_RPlusTree` |  |
| 160 | R* tree | Trees - Space-partitioning trees | yes | `00622.r_tree_2d` | `Ds160_RStarTree` |  |
| 161 | Hilbert R-tree | Trees - Space-partitioning trees | approx | `00622.r_tree_2d` | `Ds161_HilbertRTree` |  |
| 162 | X-tree | Trees - Space-partitioning trees | yes | `00622.r_tree_2d` | `Ds162_XTree` |  |
| 163 | Metric tree | Trees - Space-partitioning trees | yes | `00624.vp_tree` | `Ds163_MetricTree` |  |
| 164 | Cover tree | Trees - Space-partitioning trees | yes | `00624.vp_tree` | `Ds164_CoverTree` |  |
| 165 | M-tree | Trees - Space-partitioning trees | yes | `00624.vp_tree` | `Ds165_MTree` |  |
| 166 | VP-tree | Trees - Space-partitioning trees | yes | `00624.vp_tree` | `Ds166_VpTree` |  |
| 167 | BK-tree | Trees - Space-partitioning trees | yes | `00621.bk_tree` | `Ds167_BkTree` |  |
| 168 | Bounding interval hierarchy | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds168_BoundingIntervalHierarchy` |  |
| 169 | Bounding volume hierarchy | Trees - Space-partitioning trees | yes | `00622.r_tree_2d` | `Ds169_BoundingVolumeHierarchy` |  |
| 170 | BSP tree | Trees - Space-partitioning trees | yes | `00618.kd_tree_2d` | `Ds170_BspTree` |  |
| 171 | Rapidly exploring random tree | Trees - Space-partitioning trees | approx | `00618.kd_tree_2d` | `Ds171_RrtTree` |  |
| 172 | Abstract syntax tree | Trees - Application-specific trees | yes | _new_ | `Ds172_AbstractSyntaxTree` |  |
| 173 | Parse tree | Trees - Application-specific trees | yes | _new_ | `Ds173_ParseTree` |  |
| 174 | Decision tree | Trees - Application-specific trees | yes | _new_ | `Ds174_DecisionTree` |  |
| 175 | Alternating decision tree | Trees - Application-specific trees | yes | _new_ | `Ds175_AlternatingDecisionTree` |  |
| 176 | Game tree | Trees - Application-specific trees | yes | _new_ | `Ds176_GameTree` |  |
| 177 | Expectiminimax tree | Trees - Application-specific trees | yes | _new_ | `Ds177_ExpectiminimaxTree` |  |
| 178 | Finger tree | Trees - Application-specific trees | yes | `00513.persistent_list` | `Ds178_FingerTree` |  |
| 179 | Expression tree | Trees - Application-specific trees | yes | _new_ | `Ds179_ExpressionTree` |  |
| 180 | Log-structured merge-tree | Trees - Application-specific trees | yes | `00590.hash_table_chained_resize` | `Ds180_LogStructuredMergeTree` |  |
| 181 | PQ tree | Trees - Application-specific trees | yes | _new_ | `Ds181_PqTree` |  |
| 182 | Approximate Membership Query Filter | Hash-based structures | yes | `00599.invertible_bloom_filter` | `Ds182_ApproximateMembershipQueryFilter` |  |
| 183 | Bloom filter | Hash-based structures | yes | `00599.invertible_bloom_filter` | `Ds183_BloomFilter` |  |
| 184 | Cuckoo filter | Hash-based structures | approx | `00593.cuckoo_hashing` | `Ds184_CuckooFilter` |  |
| 185 | Quotient filter | Hash-based structures | approx | `00593.cuckoo_hashing` | `Ds185_QuotientFilter` |  |
| 186 | Count–min sketch | Hash-based structures | yes | `00592.count_min_sketch` | `Ds186_CountMinSketch` |  |
| 187 | Distributed hash table | Hash-based structures | approx | `00595.consistent_hashing` | `Ds187_DistributedHashTable` |  |
| 188 | Double hashing | Hash-based structures | yes | `00596.linear_probing` | `Ds188_DoubleHashing` |  |
| 189 | Dynamic perfect hash table | Hash-based structures | yes | `00590.hash_table_chained_resize` | `Ds189_DynamicPerfectHashTable` |  |
| 190 | Hash array mapped trie | Hash-based structures | approx | `00591.hamt` | `Ds190_HashArrayMappedTrie` |  |
| 191 | Hash list | Hash-based structures | yes | _new_ | `Ds191_HashList` |  |
| 192 | Hash table | Hash-based structures | yes | `00590.hash_table_chained_resize` | `Ds192_HashTable` |  |
| 193 | Hash tree | Hash-based structures | yes | `00604.blockchain` | `Ds193_HashTree` |  |
| 194 | Hash trie | Hash-based structures | approx | `00591.hamt` | `Ds194_HashTrie` | = Hash array mapped trie |
| 195 | Koorde | Hash-based structures | approx | `00595.consistent_hashing` | `Ds195_Koorde` | = Distributed hash table |
| 196 | Prefix hash tree | Hash-based structures | approx | `00595.consistent_hashing` | `Ds196_PrefixHashTree` | = Distributed hash table |
| 197 | Rolling hash | Hash-based structures | yes | _new_ | `Ds197_RollingHash` |  |
| 198 | MinHash | Hash-based structures | yes | _new_ | `Ds198_MinHash` |  |
| 199 | Ctrie | Hash-based structures | approx | `00591.hamt` | `Ds199_Ctrie` | = Hash array mapped trie |
| 200 | Graph | Graphs | yes | `00601.graph_edge_list` | `Ds200_Graph` |  |
| 201 | Adjacency list | Graphs | yes | _new_ | `Ds201_AdjacencyList` |  |
| 202 | Adjacency matrix | Graphs | yes | `00600.graph_adjacency_matrix` | `Ds202_AdjacencyMatrix` |  |
| 203 | Graph-structured stack | Graphs | yes | `00605.directed_graph` | `Ds203_GraphStructuredStack` |  |
| 204 | Scene graph | Graphs | yes | `00605.directed_graph` | `Ds204_SceneGraph` |  |
| 205 | Decision tree | Graphs | yes | `00521.binary_tree_traversals` | `Ds205_DecisionTree` |  |
| 206 | Binary decision diagram | Graphs | yes | `00564.bit_trie` | `Ds206_BinaryDecisionDiagram` |  |
| 207 | Zero-suppressed decision diagram | Graphs | yes | `00564.bit_trie` | `Ds207_ZeroSuppressedDecisionDiagram` |  |
| 208 | And-inverter graph | Graphs | yes | `00605.directed_graph` | `Ds208_AndInverterGraph` |  |
| 209 | Directed graph | Graphs | yes | `00605.directed_graph` | `Ds209_DirectedGraph` |  |
| 210 | Directed acyclic graph | Graphs | yes | `00605.directed_graph` | `Ds210_DirectedAcyclicGraph` |  |
| 211 | Propositional directed acyclic graph | Graphs | yes | `00605.directed_graph` | `Ds211_PropositionalDag` |  |
| 212 | Multigraph | Graphs | yes | `00609.multigraph` | `Ds212_Multigraph` |  |
| 213 | Hypergraph | Graphs | yes | `00610.hypergraph` | `Ds213_Hypergraph` |  |
| 214 | Lightmap | Other | yes | _new_ | `Ds214_Lightmap` |  |
| 215 | Winged edge | Other | yes | `00508.dcel` | `Ds215_WingedEdge` |  |
| 216 | Quad-edge | Other | approx | `00508.dcel` | `Ds216_QuadEdge` |  |
| 217 | Routing table | Other | yes | _new_ | `Ds217_RoutingTable` |  |
| 218 | Symbol table | Other | yes | `00643.ordered_map` | `Ds218_SymbolTable` |  |
| 219 | Piece table | Other | yes | _new_ | `Ds219_PieceTable` |  |
| 220 | E-graph | Other | yes | `00644.disjoint_set_union_advanced` | `Ds220_EGraph` |  |

---

## 6. Duplicates, synonyms & completeness audit

A completeness critic (one workflow agent) fetched the live page and diffed it against the
enumerated list above: **0 missing**, and the only "extra" is the deliberate split of
**#136 Disjoint-set** / **#137 Union-find**, which the page lists as a single bullet
"Disjoint-set data structure (Union-find data structure)". We keep both numbered (faithful
to anyone scanning for either name) with #137 marked `= Disjoint-set data structure`.

The page intentionally lists several structures under more than one section; those repeats
are mirrored and tagged in the **Dup-of** column (14 in total): Product type (=Record),
Map (=Associative array), Bitmap (=Bit array), Variable-length array (=Dynamic array),
Self-balancing BST (=AA tree), Binary heap (=Heap), Ternary heap (=D-ary heap),
Generalised suffix tree (=Suffix tree), B-tree [bit-slice] (=B-tree),
Union-find (=Disjoint-set), Hash trie (=HAMT), Koorde (=Distributed hash table),
Prefix hash tree (=Distributed hash table), Ctrie (=HAMT). Each still gets its own numbered
test (which reuses the canonical entry's demo) so the suite is a faithful 1:1 mirror of
the page.

> **Regenerating the catalogue.** The catalogue was built by an adversarially-verified
> per-section classification. If Wikipedia's page changes, re-run that classification and
> regenerate `tools\data-structures.catalogue.json` (numbering stays page-order; pinned
> `Ds###` test-method names must not be re-rolled for existing entries — append new ones).

---

## 7. Load & stress contract — non-trivial workloads (every bound demo)

**Why this exists.** A 3-element toy demo can pass green while the structure it claims to
implement is subtly broken at scale: a rotation bug that only corrupts the 400th node, a
hash-table resize that drops a key, a B-tree split that never fires because the instance is
too small, a heap sift that is only ever one level deep. An "8 operations on a tiny
instance" gate ships those bugs silently. This contract makes every numbered test exercise
its structure under a workload large enough to **trigger the structure's characteristic
behavior many times**, and to **assert invariants over the whole result** — so a break that
only manifests under load flips the golden output. The contract binds the demo *bound to
each catalogue entry* (reused, rebound, or new), not just the `00700–00899` newcomers.

### 7.1 What "non-trivial workload" means

- The workload is generated **programmatically** — a `for` loop over `0..N-1`, and/or a
  **seeded LCG** (the corpus's standing idiom `seed := (seed * 1103515245 + 12345) mod
  2147483648`) for pseudo-random keys and operation order. Because the sequence is fixed,
  the golden `.expected` stays exact and stable. Do **not** enumerate the workload as N
  literals — that is just a bigger toy and it bloats the source.
- The structure is driven to a **scale floor** (§7.2) that forces its signature behavior to
  fire **repeatedly**, then put through a **mixed operation sequence** (inserts interleaved
  with deletes / updates / queries), not a single monotone fill.

### 7.2 Scale tiers (floors, not targets — go larger when it stays under budget)

Pick the **smallest N that triggers the characteristic behavior several times**, then raise
it only as far as the runtime budget (§7.5) allows.

| Tier | Families | Floor |
|------|----------|-------|
| **A** | dynamic/array/array-list/hashed-array-tree/VList, hash table/set/map/filter, simple lists, simple ADTs (stack/queue/deque/circular buffer), `primitive` accumulation | **N ≥ 1000** elements via a loop; must trigger ≥1 growth/resize and (for hashes) real collisions |
| **B** | self-balancing & search trees, heaps/priority queues, tries/radix/ternary-search, union-find, graphs | **N ≥ 256**; must fire the structure's rebalancing / sifting / split-merge / path-compression / traversal many times (a balanced tree at N=256 is ≥8 levels deep) |
| **C** | heavy spatial / string / persistent structures (R*-tree, k-d/quad/BSP/VP/M/BK tree, suffix tree/array, FM-index, finger tree, rope, …) | **N ≥ 64** points/characters/nodes **plus a mixed query batch of ≥ 32** signature queries (range / k-NN / substring / split-concat) |

### 7.3 Characteristic-behavior triggers (the load MUST fire these ≥ once, ideally often)

| Family | Must be triggered under load |
|--------|------------------------------|
| dynamic array / array list / hashed array tree / VList | ≥1 capacity growth (resize/realloc) |
| hash table / Bloom / cuckoo / quotient / HAMT | real key collisions; ≥1 rehash/resize (filters: reach a realistic load factor) |
| BST / AVL / red-black / AA / splay / treap / scapegoat / WAVL / zip | many rotations/re-balances; final height within the structure's bound |
| B-tree / B+ / B* / 2-3 / 2-3-4 / (a,b) | ≥1 node **split** on insert **and** ≥1 **merge/borrow** on delete |
| heaps (binary / d-ary / binomial / Fibonacci / pairing / leftist / skew / …) | sift across ≥3 levels; ≥1 meld / decrease-key where the variant supports it |
| trie / radix / suffix / ternary search | shared-prefix branching; insert+lookup of ≥ the floor distinct keys |
| union-find / disjoint-set | path compression + union-by-rank across ≥ floor unions, then a component-count check |
| graphs | ≥ floor edges; run a real traversal (BFS/DFS/topo) and assert a graph-wide property |
| spatial (k-d / quad / R-tree / BSP / VP / M / BK) | bulk insert to the floor, then a range or k-NN query **batch** with verified results |
| circular buffer / queue / deque / gap buffer / piece table | wrap-around / overwrite / many splice ops — not a single fill |

### 7.4 Deterministic output — the load-summary convention (keeps `.expected` small)

The summary is what makes load demos diff-able and fast to gate. **Never dump the N
elements.** Each demo prints a handful of labelled lines that are a *fingerprint* of the
entire post-load structure:

- the final **`size` / element count**;
- **≥2 structural-invariant checks** as `label: true`/`false` (§7.4.1);
- a deterministic **traversal checksum/fold** — e.g. `checksum := (checksum * 31 + value)
  mod 1000000007` over a full in-order/level-order walk — so a single wrong element anywhere
  in the structure flips the printed number;
- a few **spot probes**: min, max, a couple of point/range queries, the final height/depth,
  the node/bucket count.

Because every printed value is derived from the *whole* structure after the *full* workload,
a scale-only break (corrupted element #400, a dropped key on resize, a missed rotation)
changes the checksum or flips an invariant → the golden gate (§4 Phase 4.5/5) catches it. A
toy demo cannot produce this signal.

#### 7.4.1 Mandatory invariants (≥2 per demo, each one that FAILS if the structure breaks)

- **Ordered** structures: iteration/in-order is sorted → print `sorted: true`.
- **Heaps**: heap property holds for all internal nodes; K extracts come out monotonically
  ordered.
- **Balanced trees**: final height ≤ the structure's bound for N (e.g. AVL `height ≤
  1.44·log2(N+2)`), printed as a pass/fail.
- **Hash / set / map / filter**: membership round-trips — every inserted key is found; a
  disjoint probe set is (mostly, for filters) absent; **count conservation**
  (`size == inserts − deletes`).
- **Union-find**: component count equals the expected value after the union sequence.
- **Graphs**: a global traversal property (reachable count, valid topological order, no
  cycle where a DAG is asserted).

### 7.5 Runtime budget (the binding constraint)

Each demo MUST finish in **well under the sweep's 5 s per-demo timeout**
(`tools\sweep-demos.ps1`, file mode, published single-file exe) — **target < 2 s**. The
interpreter's `arr + [x]` append is **O(n)** (it copies the array), so building N by repeated
concatenation is **O(n²)**. For the larger floors, prefer **indexed-assign growth**
(`arr[i] := v` appends at `i == Length`, deviation D22) or pre-size the backing array, and
keep N to the smallest value that still triggers the characteristic behavior several times.
If a structure genuinely cannot reach its tier floor under budget, **drop to the next tier
down** and record the chosen N in the header `# Operations` note — never exceed the timeout
to chase a bigger N.

### 7.6 Carve-outs

- **`primitive`** entries (Boolean, Integer, …) have no structure to stress: they stay
  minimal *usage* demos but must still exercise the type in a **loop** (e.g. accumulate /
  fold over `0..999`) and print the folded result, not a single literal.
- **`approx`** entries apply the load to the faithful approximation exactly as above (the
  approximation is what ships); the bit-slicing / seeded-LCG / single-process modeling is
  unchanged.
- **`no` / `[Ignore]`** entries are exempt (they never run).

---

## 8. Acceptance (Phase 5 gate for this feature)

- `TinyLanguage.DataStructures.Tests` exists at the canonical path, builds **0 errors / 0
  warnings**, and `dotnet test TinyLanguage.DataStructures.Tests` reports **`Failed: 0`**.
- The project exposes **exactly 220 `[TestMethod]`s**, one per catalogue entry
  (`Ds001`…`Ds220`); `catalogue.manifest.tsv` has **220 rows**; every referenced demo
  basename exists with a `.tlg` **and** a `.expected`; every new demo is in `00700–00899`.
- The full golden sweep (`tools\sweep-demos.ps1`) over the expanded demo corpus still
  reports **`FAILED=0 TIMEOUT=0 GOLD=0`** — including the load demos, which must stay under
  the 5 s per-demo timeout at their chosen scale.
- **Every bound demo meets the load contract (§7):** its workload is loop/seed-generated to
  at least its tier floor (§7.2), triggers the structure's characteristic behavior (§7.3),
  and prints ≥2 post-load invariant checks plus a traversal checksum (§7.4) — not a few
  literal operations on a toy instance. Spot-audited in Phases 4.5/5.
- Any `[Ignore]`d entry (feasible `no`) carries a `NOT IMPLEMENTABLE: <reason>` message and
  is still one of the 220 numbered methods.
