@echo off
echo Running 00238.partial_application.tlg
if "%2"=="" (
    TinyLanguage.exe 00238.partial_application.tlg output.txt
) else (
    TinyLanguage.exe 00238.partial_application.tlg %2
)
