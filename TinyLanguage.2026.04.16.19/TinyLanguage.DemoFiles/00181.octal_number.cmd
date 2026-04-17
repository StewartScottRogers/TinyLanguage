@echo off
echo Running 00181.octal_number.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000181.octal_number.tlg output.txt
) else (
    TinyLanguage.exe %~dp000181.octal_number.tlg %2
)
