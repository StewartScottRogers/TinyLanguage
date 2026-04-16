@echo off
echo Running 00063.lambda_as_argument.tlg
if "%2"=="" (
    TinyLanguage.exe 00063.lambda_as_argument.tlg output.txt
) else (
    TinyLanguage.exe 00063.lambda_as_argument.tlg %2
)
