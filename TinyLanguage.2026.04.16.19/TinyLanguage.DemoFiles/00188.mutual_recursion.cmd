@echo off
echo Running 00188.mutual_recursion.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000188.mutual_recursion.tlg output.txt
) else (
    TinyLanguage.exe %~dp000188.mutual_recursion.tlg %2
)
