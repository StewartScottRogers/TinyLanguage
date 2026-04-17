@echo off
echo Running 00061.if_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000061.if_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000061.if_simple.tlg %2
)
