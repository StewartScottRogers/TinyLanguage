@echo off
echo Running 00095.recursive_gcd.tlg
if "%2"=="" (
    TinyLanguage.exe 00095.recursive_gcd.tlg output.txt
) else (
    TinyLanguage.exe 00095.recursive_gcd.tlg %2
)
