@echo off
echo Running 00014.assignment.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000014.assignment.tlg output.txt
) else (
    TinyLanguage.exe %~dp000014.assignment.tlg %2
)
