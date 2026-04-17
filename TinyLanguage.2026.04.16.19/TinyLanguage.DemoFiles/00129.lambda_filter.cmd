@echo off
echo Running 00129.lambda_filter.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000129.lambda_filter.tlg output.txt
) else (
    TinyLanguage.exe %~dp000129.lambda_filter.tlg %2
)
