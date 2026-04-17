@echo off
echo Running 00007.print_boolean.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000007.print_boolean.tlg output.txt
) else (
    TinyLanguage.exe %~dp000007.print_boolean.tlg %2
)
