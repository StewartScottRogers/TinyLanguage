@echo off
echo Running 00087.for_continue.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000087.for_continue.tlg output.txt
) else (
    TinyLanguage.exe %~dp000087.for_continue.tlg %2
)
