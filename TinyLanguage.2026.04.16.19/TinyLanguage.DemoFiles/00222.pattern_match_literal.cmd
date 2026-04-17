@echo off
echo Running 00222.pattern_match_literal.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000222.pattern_match_literal.tlg output.txt
) else (
    TinyLanguage.exe %~dp000222.pattern_match_literal.tlg %2
)
