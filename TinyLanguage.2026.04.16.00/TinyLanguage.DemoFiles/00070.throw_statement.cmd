@echo off
echo Running 00070.throw_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00070.throw_statement.tlg output.txt
) else (
    TinyLanguage.exe 00070.throw_statement.tlg %2
)
