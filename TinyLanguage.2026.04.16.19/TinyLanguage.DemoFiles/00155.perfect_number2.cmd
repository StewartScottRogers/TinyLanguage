@echo off
echo Running 00155.perfect_number2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000155.perfect_number2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000155.perfect_number2.tlg %2
)
