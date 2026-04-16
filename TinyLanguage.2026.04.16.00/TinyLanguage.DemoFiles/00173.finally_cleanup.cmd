@echo off
echo Running 00173.finally_cleanup.tlg
if "%2"=="" (
    TinyLanguage.exe 00173.finally_cleanup.tlg output.txt
) else (
    TinyLanguage.exe 00173.finally_cleanup.tlg %2
)
