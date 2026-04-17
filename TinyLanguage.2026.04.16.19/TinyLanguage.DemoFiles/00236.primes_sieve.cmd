@echo off
echo Running 00236.primes_sieve.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000236.primes_sieve.tlg output.txt
) else (
    TinyLanguage.exe %~dp000236.primes_sieve.tlg %2
)
