@echo off
echo Running 00111.function_basic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000111.function_basic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000111.function_basic.tlg %2
)
