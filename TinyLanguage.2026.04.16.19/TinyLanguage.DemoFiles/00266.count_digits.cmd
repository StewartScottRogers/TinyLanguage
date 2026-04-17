@echo off
echo Running 00266.count_digits.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000266.count_digits.tlg output.txt
) else (
    TinyLanguage.exe %~dp000266.count_digits.tlg %2
)
