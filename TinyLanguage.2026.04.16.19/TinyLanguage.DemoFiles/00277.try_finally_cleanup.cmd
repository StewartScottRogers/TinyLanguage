@echo off
echo Running 00277.try_finally_cleanup.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000277.try_finally_cleanup.tlg output.txt
) else (
    TinyLanguage.exe %~dp000277.try_finally_cleanup.tlg %2
)
