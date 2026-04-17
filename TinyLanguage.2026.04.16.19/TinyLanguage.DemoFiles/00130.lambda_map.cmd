@echo off
echo Running 00130.lambda_map.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000130.lambda_map.tlg output.txt
) else (
    TinyLanguage.exe %~dp000130.lambda_map.tlg %2
)
