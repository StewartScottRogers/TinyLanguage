@echo off
echo Running 00120.function_no_params.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000120.function_no_params.tlg output.txt
) else (
    TinyLanguage.exe %~dp000120.function_no_params.tlg %2
)
