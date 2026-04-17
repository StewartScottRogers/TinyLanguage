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
| **Outputs** | `.sln`, all `.csproj`, `.shproj`, `.projitems`, `Directory.Build.props` |
| **Dependencies** | None |
| **Assignee Role** | Build Engineer |

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

### WU-08: Integration Tests
| Field | Value |
|---|---|
| **Title** | Implement Layers 4–5 Test Classes |
| **Inputs** | Sec 1.6.4–1.6.5, 1.3.1 |
| **Outputs** | `InterpreterIntegrationTests.cs` |
| **Dependencies** | WU-06 |
| **Assignee Role** | QA Engineer |

### WU-09: Console Application
| Field | Value |
|---|---|
| **Title** | Implement TinyLanguage.exe with File-Processor and Demo Modes |
| **Inputs** | Sec 1.2.6 |
| **Outputs** | `Program.cs` |
| **Dependencies** | WU-06 |
| **Assignee Role** | Application Engineer |

### WU-10: Demo File Suite
| Field | Value |
|---|---|
| **Title** | Generate 300+ Demo .tlg Files and Matching .cmd Scripts |
| **Inputs** | Sec 1.2.7, 1.5.8, full BNF |
| **Outputs** | `TinyLanguage.DemoFiles/00001.*.tlg` … `003xx.*.tlg` + matching `.cmd` |
| **Dependencies** | WU-04 (grammar finalized) |
| **Assignee Role** | Content Engineer |

### WU-11: Validation and Assembly
| Field | Value |
|---|---|
| **Title** | Execute Build, Test, Demo Validation and Fix-and-Retry Loop |
| **Inputs** | Sec 1.3.2–1.3.4, 1.6.6, 1.7 |
| **Outputs** | Green build, green tests, green demo |
| **Dependencies** | WU-07, WU-08, WU-09, WU-10 |
| **Assignee Role** | Build Engineer |

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

The generated folder is created as a **sibling of the repo** (one level above), e.g.
`Z:\repos\TinyLanguage.YYYY.MM.DD.HH\` alongside `Z:\repos\TinyLanguage\`.

```
..\TinyLanguage.2026.04.16.19\   (sibling of the TinyLanguage repo)

├── TinyLanguage.sln
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
└── TinyLanguage.DemoFiles/
    ├── TinyLanguage.DemoFiles.shproj
    ├── TinyLanguage.DemoFiles.projitems
    ├── 00001.fizzbuzz.tlg
    ├── 00001.fizzbuzz.cmd
    ├── 00002.fibonacci.tlg
    ├── 00002.fibonacci.cmd
    └── ... (300+ pairs)
```
