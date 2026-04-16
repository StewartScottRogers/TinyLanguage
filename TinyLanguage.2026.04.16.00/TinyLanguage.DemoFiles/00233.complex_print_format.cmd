@echo off
echo Running 00233.complex_print_format.tlg
if "%2"=="" (
    TinyLanguage.exe 00233.complex_print_format.tlg output.txt
) else (
    TinyLanguage.exe 00233.complex_print_format.tlg %2
)
