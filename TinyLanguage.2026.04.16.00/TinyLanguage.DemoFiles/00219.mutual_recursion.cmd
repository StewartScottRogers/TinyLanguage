@echo off
echo Running 00219.mutual_recursion.tlg
if "%2"=="" (
    TinyLanguage.exe 00219.mutual_recursion.tlg output.txt
) else (
    TinyLanguage.exe 00219.mutual_recursion.tlg %2
)
