@echo off
echo Running 00123.lambda_stored.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000123.lambda_stored.tlg output.txt
) else (
    TinyLanguage.exe %~dp000123.lambda_stored.tlg %2
)
