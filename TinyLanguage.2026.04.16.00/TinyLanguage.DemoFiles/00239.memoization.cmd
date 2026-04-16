@echo off
echo Running 00239.memoization.tlg
if "%2"=="" (
    TinyLanguage.exe 00239.memoization.tlg output.txt
) else (
    TinyLanguage.exe 00239.memoization.tlg %2
)
