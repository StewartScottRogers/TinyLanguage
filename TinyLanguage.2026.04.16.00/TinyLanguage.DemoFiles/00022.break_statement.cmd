@echo off
echo Running 00022.break_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00022.break_statement.tlg output.txt
) else (
    TinyLanguage.exe 00022.break_statement.tlg %2
)
