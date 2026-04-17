@echo off
echo Running 00006.print_float.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000006.print_float.tlg output.txt
) else (
    TinyLanguage.exe %~dp000006.print_float.tlg %2
)
