@echo off
echo Running 00002.fibonacci_recursive.tlg
if "%2"=="" (
    TinyLanguage.exe 00002.fibonacci_recursive.tlg output.txt
) else (
    TinyLanguage.exe 00002.fibonacci_recursive.tlg %2
)
