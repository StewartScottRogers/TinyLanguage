@echo off
echo Running 00224.filter_lambda.tlg
if "%2"=="" (
    TinyLanguage.exe 00224.filter_lambda.tlg output.txt
) else (
    TinyLanguage.exe 00224.filter_lambda.tlg %2
)
