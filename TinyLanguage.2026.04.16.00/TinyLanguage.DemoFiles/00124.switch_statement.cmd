@echo off
echo Running 00124.switch_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00124.switch_statement.tlg output.txt
) else (
    TinyLanguage.exe 00124.switch_statement.tlg %2
)
