@echo off
echo Running 00002.fibonacci.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000002.fibonacci.tlg output.txt
) else (
    TinyLanguage.exe %~dp000002.fibonacci.tlg %2
)
