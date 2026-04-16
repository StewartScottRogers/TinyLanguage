@echo off
echo Running 00281.collatz.tlg
if "%2"=="" (
    TinyLanguage.exe 00281.collatz.tlg output.txt
) else (
    TinyLanguage.exe 00281.collatz.tlg %2
)
