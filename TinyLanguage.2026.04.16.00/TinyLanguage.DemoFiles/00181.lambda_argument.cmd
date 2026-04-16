@echo off
echo Running 00181.lambda_argument.tlg
if "%2"=="" (
    TinyLanguage.exe 00181.lambda_argument.tlg output.txt
) else (
    TinyLanguage.exe 00181.lambda_argument.tlg %2
)
