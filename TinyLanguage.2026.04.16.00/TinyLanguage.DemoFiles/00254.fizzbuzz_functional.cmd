@echo off
echo Running 00254.fizzbuzz_functional.tlg
if "%2"=="" (
    TinyLanguage.exe 00254.fizzbuzz_functional.tlg output.txt
) else (
    TinyLanguage.exe 00254.fizzbuzz_functional.tlg %2
)
