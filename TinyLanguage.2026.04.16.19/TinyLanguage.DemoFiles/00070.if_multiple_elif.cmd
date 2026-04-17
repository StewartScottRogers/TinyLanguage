@echo off
echo Running 00070.if_multiple_elif.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000070.if_multiple_elif.tlg output.txt
) else (
    TinyLanguage.exe %~dp000070.if_multiple_elif.tlg %2
)
