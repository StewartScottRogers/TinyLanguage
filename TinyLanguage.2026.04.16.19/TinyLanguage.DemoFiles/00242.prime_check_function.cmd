@echo off
echo Running 00242.prime_check_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000242.prime_check_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000242.prime_check_function.tlg %2
)
