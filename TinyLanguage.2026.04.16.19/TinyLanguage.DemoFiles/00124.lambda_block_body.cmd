@echo off
echo Running 00124.lambda_block_body.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000124.lambda_block_body.tlg output.txt
) else (
    TinyLanguage.exe %~dp000124.lambda_block_body.tlg %2
)
