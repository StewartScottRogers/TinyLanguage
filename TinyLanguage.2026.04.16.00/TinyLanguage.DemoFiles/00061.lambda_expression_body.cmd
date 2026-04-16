@echo off
echo Running 00061.lambda_expression_body.tlg
if "%2"=="" (
    TinyLanguage.exe 00061.lambda_expression_body.tlg output.txt
) else (
    TinyLanguage.exe 00061.lambda_expression_body.tlg %2
)
