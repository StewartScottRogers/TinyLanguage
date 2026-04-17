@echo off
echo Running 00041.boolean_and.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000041.boolean_and.tlg output.txt
) else (
    TinyLanguage.exe %~dp000041.boolean_and.tlg %2
)
