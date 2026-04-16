@echo off
echo Running 00027.function_with_params.tlg
if "%2"=="" (
    TinyLanguage.exe 00027.function_with_params.tlg output.txt
) else (
    TinyLanguage.exe 00027.function_with_params.tlg %2
)
