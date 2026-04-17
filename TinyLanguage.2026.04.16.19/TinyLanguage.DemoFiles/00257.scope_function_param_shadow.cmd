@echo off
echo Running 00257.scope_function_param_shadow.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000257.scope_function_param_shadow.tlg output.txt
) else (
    TinyLanguage.exe %~dp000257.scope_function_param_shadow.tlg %2
)
