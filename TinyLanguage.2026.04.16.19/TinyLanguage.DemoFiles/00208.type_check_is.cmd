@echo off
echo Running 00208.type_check_is.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000208.type_check_is.tlg output.txt
) else (
    TinyLanguage.exe %~dp000208.type_check_is.tlg %2
)
