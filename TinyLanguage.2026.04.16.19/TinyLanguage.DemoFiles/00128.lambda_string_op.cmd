@echo off
echo Running 00128.lambda_string_op.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000128.lambda_string_op.tlg output.txt
) else (
    TinyLanguage.exe %~dp000128.lambda_string_op.tlg %2
)
