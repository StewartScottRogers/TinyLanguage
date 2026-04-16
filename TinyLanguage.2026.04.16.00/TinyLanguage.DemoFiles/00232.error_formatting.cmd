@echo off
echo Running 00232.error_formatting.tlg
if "%2"=="" (
    TinyLanguage.exe 00232.error_formatting.tlg output.txt
) else (
    TinyLanguage.exe 00232.error_formatting.tlg %2
)
