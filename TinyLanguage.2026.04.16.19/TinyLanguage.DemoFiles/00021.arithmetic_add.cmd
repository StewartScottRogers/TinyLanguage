@echo off
echo Running 00021.arithmetic_add.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000021.arithmetic_add.tlg output.txt
) else (
    TinyLanguage.exe %~dp000021.arithmetic_add.tlg %2
)
