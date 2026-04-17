@echo off
echo Running 00080.while_gcd.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000080.while_gcd.tlg output.txt
) else (
    TinyLanguage.exe %~dp000080.while_gcd.tlg %2
)
