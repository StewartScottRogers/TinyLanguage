@echo off
echo Running 00287.complex_fizzbuzz_extended.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000287.complex_fizzbuzz_extended.tlg output.txt
) else (
    TinyLanguage.exe %~dp000287.complex_fizzbuzz_extended.tlg %2
)
