@echo off
echo Running 00005.factorial_iterative.tlg
if "%2"=="" (
    TinyLanguage.exe 00005.factorial_iterative.tlg output.txt
) else (
    TinyLanguage.exe 00005.factorial_iterative.tlg %2
)
