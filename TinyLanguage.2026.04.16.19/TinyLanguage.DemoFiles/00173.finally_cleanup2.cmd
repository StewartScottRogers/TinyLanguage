@echo off
echo Running 00173.finally_cleanup2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000173.finally_cleanup2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000173.finally_cleanup2.tlg %2
)
