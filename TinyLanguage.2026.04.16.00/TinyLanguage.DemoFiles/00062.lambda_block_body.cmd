@echo off
echo Running 00062.lambda_block_body.tlg
if "%2"=="" (
    TinyLanguage.exe 00062.lambda_block_body.tlg output.txt
) else (
    TinyLanguage.exe 00062.lambda_block_body.tlg %2
)
