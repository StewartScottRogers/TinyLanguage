@echo off
echo Running 00225.reduce_lambda.tlg
if "%2"=="" (
    TinyLanguage.exe 00225.reduce_lambda.tlg output.txt
) else (
    TinyLanguage.exe 00225.reduce_lambda.tlg %2
)
