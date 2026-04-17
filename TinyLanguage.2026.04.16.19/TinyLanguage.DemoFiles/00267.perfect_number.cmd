@echo off
echo Running 00267.perfect_number.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000267.perfect_number.tlg output.txt
) else (
    TinyLanguage.exe %~dp000267.perfect_number.tlg %2
)
