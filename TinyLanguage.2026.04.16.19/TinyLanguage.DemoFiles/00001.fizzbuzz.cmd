@echo off
echo Running 00001.fizzbuzz.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000001.fizzbuzz.tlg output.txt
) else (
    TinyLanguage.exe %~dp000001.fizzbuzz.tlg %2
)
