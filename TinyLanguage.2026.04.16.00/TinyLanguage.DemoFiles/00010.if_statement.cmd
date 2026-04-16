@echo off
echo Running 00010.if_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00010.if_statement.tlg output.txt
) else (
    TinyLanguage.exe 00010.if_statement.tlg %2
)
