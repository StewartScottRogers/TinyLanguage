@echo off
echo Running 00113.function_return_string.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000113.function_return_string.tlg output.txt
) else (
    TinyLanguage.exe %~dp000113.function_return_string.tlg %2
)
