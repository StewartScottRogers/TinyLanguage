@echo off
echo Running 00212.parenthesized_expr.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000212.parenthesized_expr.tlg output.txt
) else (
    TinyLanguage.exe %~dp000212.parenthesized_expr.tlg %2
)
