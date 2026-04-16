@echo off
echo Running 00136.fibonacci_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00136.fibonacci_array.tlg output.txt
) else (
    TinyLanguage.exe 00136.fibonacci_array.tlg %2
)
