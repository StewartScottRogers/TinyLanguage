@echo off
echo Running 00312.recursive_ackermann.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000312.recursive_ackermann.tlg output.txt
) else (
    TinyLanguage.exe %~dp000312.recursive_ackermann.tlg %2
)
