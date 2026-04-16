@echo off
echo Running 00047.print_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00047.print_statement.tlg output.txt
) else (
    TinyLanguage.exe 00047.print_statement.tlg %2
)
