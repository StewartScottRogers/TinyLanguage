@echo off
echo Running 00241.fibonacci_iterative.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000241.fibonacci_iterative.tlg output.txt
) else (
    TinyLanguage.exe %~dp000241.fibonacci_iterative.tlg %2
)
