@echo off
echo Running 00259.prime_sieve.tlg
if "%2"=="" (
    TinyLanguage.exe 00259.prime_sieve.tlg output.txt
) else (
    TinyLanguage.exe 00259.prime_sieve.tlg %2
)
