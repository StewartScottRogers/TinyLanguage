@echo off
echo Running 00121.lambda_basic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000121.lambda_basic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000121.lambda_basic.tlg %2
)
