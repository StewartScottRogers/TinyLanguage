@echo off
echo Running 00299.complex_nested_functions.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000299.complex_nested_functions.tlg output.txt
) else (
    TinyLanguage.exe %~dp000299.complex_nested_functions.tlg %2
)
