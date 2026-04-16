@echo off
echo Running 00003.fibonacci_iterative.tlg
if "%2"=="" (
    TinyLanguage.exe 00003.fibonacci_iterative.tlg output.txt
) else (
    TinyLanguage.exe 00003.fibonacci_iterative.tlg %2
)
