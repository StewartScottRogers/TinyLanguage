@echo off
echo Running 00137.prime_check.tlg
if "%2"=="" (
    TinyLanguage.exe 00137.prime_check.tlg output.txt
) else (
    TinyLanguage.exe 00137.prime_check.tlg %2
)
