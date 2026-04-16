@echo off
echo Running 00004.factorial_recursive.tlg
if "%2"=="" (
    TinyLanguage.exe 00004.factorial_recursive.tlg output.txt
) else (
    TinyLanguage.exe 00004.factorial_recursive.tlg %2
)
