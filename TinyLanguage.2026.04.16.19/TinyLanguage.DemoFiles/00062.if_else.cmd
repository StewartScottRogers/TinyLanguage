@echo off
echo Running 00062.if_else.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000062.if_else.tlg output.txt
) else (
    TinyLanguage.exe %~dp000062.if_else.tlg %2
)
