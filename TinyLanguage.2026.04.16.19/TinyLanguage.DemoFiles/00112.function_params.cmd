@echo off
echo Running 00112.function_params.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000112.function_params.tlg output.txt
) else (
    TinyLanguage.exe %~dp000112.function_params.tlg %2
)
