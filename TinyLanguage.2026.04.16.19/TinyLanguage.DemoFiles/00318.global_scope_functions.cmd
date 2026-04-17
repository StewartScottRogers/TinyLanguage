@echo off
echo Running 00318.global_scope_functions.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000318.global_scope_functions.tlg output.txt
) else (
    TinyLanguage.exe %~dp000318.global_scope_functions.tlg %2
)
