@echo off
echo Running 00223.map_lambda.tlg
if "%2"=="" (
    TinyLanguage.exe 00223.map_lambda.tlg output.txt
) else (
    TinyLanguage.exe 00223.map_lambda.tlg %2
)
