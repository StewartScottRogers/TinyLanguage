@echo off
echo Running 00207.chained_logical.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000207.chained_logical.tlg output.txt
) else (
    TinyLanguage.exe %~dp000207.chained_logical.tlg %2
)
