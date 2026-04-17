@echo off
echo Running 00117.function_multiple_return.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000117.function_multiple_return.tlg output.txt
) else (
    TinyLanguage.exe %~dp000117.function_multiple_return.tlg %2
)
