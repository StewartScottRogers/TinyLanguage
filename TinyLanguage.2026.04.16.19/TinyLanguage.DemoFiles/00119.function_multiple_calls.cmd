@echo off
echo Running 00119.function_multiple_calls.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000119.function_multiple_calls.tlg output.txt
) else (
    TinyLanguage.exe %~dp000119.function_multiple_calls.tlg %2
)
