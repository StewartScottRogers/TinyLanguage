@echo off
echo Running 00008.print_string.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000008.print_string.tlg output.txt
) else (
    TinyLanguage.exe %~dp000008.print_string.tlg %2
)
