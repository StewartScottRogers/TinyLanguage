@echo off
echo Running 00127.lambda_in_array.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000127.lambda_in_array.tlg output.txt
) else (
    TinyLanguage.exe %~dp000127.lambda_in_array.tlg %2
)
