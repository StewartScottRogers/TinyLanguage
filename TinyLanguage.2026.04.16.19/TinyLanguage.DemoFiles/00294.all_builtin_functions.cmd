@echo off
echo Running 00294.all_builtin_functions.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000294.all_builtin_functions.tlg output.txt
) else (
    TinyLanguage.exe %~dp000294.all_builtin_functions.tlg %2
)
