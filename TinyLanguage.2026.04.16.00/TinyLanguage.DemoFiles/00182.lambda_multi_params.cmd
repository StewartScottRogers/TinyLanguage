@echo off
echo Running 00182.lambda_multi_params.tlg
if "%2"=="" (
    TinyLanguage.exe 00182.lambda_multi_params.tlg output.txt
) else (
    TinyLanguage.exe 00182.lambda_multi_params.tlg %2
)
