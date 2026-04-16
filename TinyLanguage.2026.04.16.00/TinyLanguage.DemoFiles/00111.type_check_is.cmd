@echo off
echo Running 00111.type_check_is.tlg
if "%2"=="" (
    TinyLanguage.exe 00111.type_check_is.tlg output.txt
) else (
    TinyLanguage.exe 00111.type_check_is.tlg %2
)
