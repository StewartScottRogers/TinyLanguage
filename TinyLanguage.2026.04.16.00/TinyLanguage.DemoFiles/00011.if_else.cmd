@echo off
echo Running 00011.if_else.tlg
if "%2"=="" (
    TinyLanguage.exe 00011.if_else.tlg output.txt
) else (
    TinyLanguage.exe 00011.if_else.tlg %2
)
