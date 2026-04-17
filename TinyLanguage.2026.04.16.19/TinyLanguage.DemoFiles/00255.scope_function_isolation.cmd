@echo off
echo Running 00255.scope_function_isolation.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000255.scope_function_isolation.tlg output.txt
) else (
    TinyLanguage.exe %~dp000255.scope_function_isolation.tlg %2
)
