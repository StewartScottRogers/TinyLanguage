@echo off
echo Running 00010.print_expression.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000010.print_expression.tlg output.txt
) else (
    TinyLanguage.exe %~dp000010.print_expression.tlg %2
)
