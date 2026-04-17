@echo off
echo Running 00005.print_integer.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000005.print_integer.tlg output.txt
) else (
    TinyLanguage.exe %~dp000005.print_integer.tlg %2
)
