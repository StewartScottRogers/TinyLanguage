@echo off
echo Running 00315.function_recursion_depth.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000315.function_recursion_depth.tlg output.txt
) else (
    TinyLanguage.exe %~dp000315.function_recursion_depth.tlg %2
)
