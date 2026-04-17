@echo off
echo Running 00114.function_recursion.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000114.function_recursion.tlg output.txt
) else (
    TinyLanguage.exe %~dp000114.function_recursion.tlg %2
)
