@echo off
echo Running 00023.continue_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00023.continue_statement.tlg output.txt
) else (
    TinyLanguage.exe 00023.continue_statement.tlg %2
)
