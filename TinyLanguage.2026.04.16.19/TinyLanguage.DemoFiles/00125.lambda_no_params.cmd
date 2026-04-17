@echo off
echo Running 00125.lambda_no_params.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000125.lambda_no_params.tlg output.txt
) else (
    TinyLanguage.exe %~dp000125.lambda_no_params.tlg %2
)
