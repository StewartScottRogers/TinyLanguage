@echo off
echo Running 00041.bitwise_and.tlg
if "%2"=="" (
    TinyLanguage.exe 00041.bitwise_and.tlg output.txt
) else (
    TinyLanguage.exe 00041.bitwise_and.tlg %2
)
