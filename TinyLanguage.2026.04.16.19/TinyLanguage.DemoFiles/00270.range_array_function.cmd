@echo off
echo Running 00270.range_array_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000270.range_array_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000270.range_array_function.tlg %2
)
