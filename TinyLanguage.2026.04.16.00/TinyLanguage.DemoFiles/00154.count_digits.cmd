@echo off
echo Running 00154.count_digits.tlg
if "%2"=="" (
    TinyLanguage.exe 00154.count_digits.tlg output.txt
) else (
    TinyLanguage.exe 00154.count_digits.tlg %2
)
