@echo off
echo Running 00037.logical_operators.tlg
if "%2"=="" (
    TinyLanguage.exe 00037.logical_operators.tlg output.txt
) else (
    TinyLanguage.exe 00037.logical_operators.tlg %2
)
