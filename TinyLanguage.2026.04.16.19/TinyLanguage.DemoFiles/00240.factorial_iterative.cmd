@echo off
echo Running 00240.factorial_iterative.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000240.factorial_iterative.tlg output.txt
) else (
    TinyLanguage.exe %~dp000240.factorial_iterative.tlg %2
)
