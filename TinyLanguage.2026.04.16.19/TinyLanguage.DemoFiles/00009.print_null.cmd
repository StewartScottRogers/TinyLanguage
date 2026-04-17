@echo off
echo Running 00009.print_null.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000009.print_null.tlg output.txt
) else (
    TinyLanguage.exe %~dp000009.print_null.tlg %2
)
