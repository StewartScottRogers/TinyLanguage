@echo off
echo Running 00115.function_bare_return.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000115.function_bare_return.tlg output.txt
) else (
    TinyLanguage.exe %~dp000115.function_bare_return.tlg %2
)
