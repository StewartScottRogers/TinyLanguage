@echo off
echo Running 00185.conditional_expr_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000185.conditional_expr_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000185.conditional_expr_nested.tlg %2
)
