@echo off
echo Running 00122.lambda_two_params.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000122.lambda_two_params.tlg output.txt
) else (
    TinyLanguage.exe %~dp000122.lambda_two_params.tlg %2
)
