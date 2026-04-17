@echo off
echo Running 00086.for_break.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000086.for_break.tlg output.txt
) else (
    TinyLanguage.exe %~dp000086.for_break.tlg %2
)
