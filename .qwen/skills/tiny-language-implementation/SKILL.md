---
name: tiny-language-implementation
description: Overall approach for implementing the TinyLanguage project following the build plan
source: auto-skill
extracted_at: '2026-06-02T05:36:42.494Z'
---

# TinyLanguage Implementation Approach

This skill documents the overall approach for implementing the TinyLanguage project following the detailed build plan specified in the project documentation.

## Project Overview

TinyLanguage is a complete .NET 10.0 implementation of a small programming language with:
- Lexer, Parser, AST, and tree-walking Interpreter
- Support for a wide range of language features
- Comprehensive test suite with 500+ demo files
- Visual Studio Code and Visual Studio debugging support

## Implementation Phases

The implementation follows a structured build plan with the following phases:

### Phase 0: Architect Analysis
- Analyze Build.Solution.md specification
- Create Plan.md with work-unit breakdown
- Establish dependency graph and critical path

### Phase 1A: Solution Scaffold
- Create directory structure and project files
- Set up SDK pinning with global.json
- Configure long-path support
- Establish editor extension structure

### Phase 1B: Token & Lexer
- Implement TokenType enumeration
- Create Token class for lexical analysis
- Develop Lexer class for tokenizing source code
- Handle line comments and special tokens

### Phase 1C: AST Nodes + Pretty Printer
- Create AST node classes for all language constructs
- Implement INodeVisitor interface
- Develop AstPrettyPrinter for debugging and testing

### Phase 1D: Demo Files
- Create Tier A feature coverage demos (00001-00399)
- Create Tier B advanced data structures demos (00400-00499)
- Create Tier C Wikipedia-style data structure demos (00500-00659)
- Generate corresponding .cmd and .expected files

### Phase 2: Parser
- Implement recursive-descent parser
- Handle all BNF grammar productions
- Address disambiguation rules and implementation notes

### Phase 3A: Interpreter
- Implement tree-walking interpreter
- Create Scope class for variable binding
- Handle all language semantics and built-in functions

### Phase 3B: Unit Tests
- Create LexerUnitTests for token coverage
- Develop ParserUnitTests for grammar coverage
- Use TestLog helper for consistent test output

### Phase 4A: Integration Tests
- Implement InterpreterIntegrationTests
- Test end-to-end programs for all language features
- Validate interpreter behavior with golden outputs

### Phase 4B: Console Application
- Implement Program.cs with stdin/file modes
- Handle command-line arguments properly
- Ensure no demo walking mode (deviation D1)

### Phase 4C: Debugger Engine + DAP Adapter
- Add debugger engine hooks to Interpreter
- Create DebugAdapter project for DAP implementation
- Implement VS Code and VS18 editor shims

### Phase 4D: VS Code Extension
- Create VS Code extension files
- Implement DebugConfigurationProvider and DebugAdapterDescriptorFactory
- Create installer script

### Phase 4E: Wiki Generation
- Generate comprehensive user documentation
- Include language tour with runnable snippets
- Document debugging and building instructions

### Phase 4F: VS18 Editor Shim + Shared Project
- Create Visual Studio extension files
- Implement IAdapterLauncher for VS Debug Adapter Host
- Create Shared Project for extension files

### Phase 5: Final Validation & Delivery
- Verify all build and test requirements
- Run demo suite validation
- Ensure Visual Studio Batch Rebuild compatibility
- Deliver to canonical solution path

## Key Implementation Principles

### Follow Specifications Exactly
- Adhere strictly to Build.Solution.md when not overridden by deviations
- Implement all 23 implementation notes from the specification
- Maintain compliance with .NET Standards section

### Handle Deliberate Deviations
- D1: No demo mode in exe - two modes only (stdin/file)
- D2: Demo aggregator script - run-all-demos.cmd
- D4: Interactive debugger via DAP (additive)
- D5: User-facing wiki (additive)
- D6: Long-path support (additive)
- D7: SDK pin via global.json (additive)
- D8: TestLog helper (additive)
- D9: Editor shims under Shared Project (additive)
- D10-D22: Parser/interpreter leniencies beyond BNF

### Code Quality Standards
- Use UpperCamelCase for types/members, lowerCamelCase for locals
- Apply `readonly` wherever possible
- Use Records over Classes except for AST nodes (which need inheritance)
- Follow BCL-only policy (no NuGet packages)
- Implement one file per type rule

## Critical Success Factors

1. **Proper SDK Pinning**: Use global.json with newest STABLE SDK
2. **Long-Path Support**: Implement all three layers (registry, Directory.Build.props, app.manifest)
3. **Test Output Consistency**: Use TestLog helper for all test output
4. **Editor Extension Compatibility**: Maintain both VS Code and VS18 support
5. **Build System Compliance**: Ensure 0 errors, 0 warnings on build
6. **Demo Suite Completeness**: ≥500 .tlg files with matching .cmd and .expected
7. **Regression Prevention**: Validate no breaking changes to existing functionality

## Verification Approach

After each phase:
1. Run `dotnet build` - verify 0 errors, 0 warnings
2. Run `dotnet test` - verify Failed: 0
3. Validate project-specific acceptance criteria from build plan
4. Run demo suite validation where applicable

## Common Pitfalls to Avoid

1. Incorrect JSON syntax in configuration files
2. Missing project references between components
3. Inadequate error handling in parser/interpreter
4. Insufficient test coverage for edge cases
5. Incorrect implementation of disambiguation rules
6. Failure to handle deliberate deviations from specification
7. Incompatible changes to existing functionality
8. Poor error messages that don't meet runtime error policy

This approach ensures a systematic, compliant implementation of the TinyLanguage project that meets all specified requirements.