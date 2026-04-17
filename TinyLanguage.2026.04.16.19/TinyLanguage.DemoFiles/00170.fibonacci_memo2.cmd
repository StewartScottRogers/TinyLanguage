@echo off
echo Running 00170.fibonacci_memo2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000170.fibonacci_memo2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000170.fibonacci_memo2.tlg %2
)
