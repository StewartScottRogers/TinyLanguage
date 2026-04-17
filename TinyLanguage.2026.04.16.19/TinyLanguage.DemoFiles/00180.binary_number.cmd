@echo off
echo Running 00180.binary_number.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000180.binary_number.tlg output.txt
) else (
    TinyLanguage.exe %~dp000180.binary_number.tlg %2
)
