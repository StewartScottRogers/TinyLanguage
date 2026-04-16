@echo off
echo Running 00202.large_numbers.tlg
if "%2"=="" (
    TinyLanguage.exe 00202.large_numbers.tlg output.txt
) else (
    TinyLanguage.exe 00202.large_numbers.tlg %2
)
