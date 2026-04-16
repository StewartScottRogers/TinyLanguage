@echo off
echo Running 00026.function_no_params.tlg
if "%2"=="" (
    TinyLanguage.exe 00026.function_no_params.tlg output.txt
) else (
    TinyLanguage.exe 00026.function_no_params.tlg %2
)
