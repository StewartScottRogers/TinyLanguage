@echo off
echo Running 00180.lambda_variable.tlg
if "%2"=="" (
    TinyLanguage.exe 00180.lambda_variable.tlg output.txt
) else (
    TinyLanguage.exe 00180.lambda_variable.tlg %2
)
