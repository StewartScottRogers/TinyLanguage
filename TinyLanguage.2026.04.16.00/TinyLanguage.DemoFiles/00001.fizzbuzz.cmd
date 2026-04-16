@echo off
echo Running 00001.fizzbuzz.tlg
if "%2"=="" (
    TinyLanguage.exe 00001.fizzbuzz.tlg output.txt
) else (
    TinyLanguage.exe 00001.fizzbuzz.tlg %2
)
