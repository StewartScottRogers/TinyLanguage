@echo off
echo Running 00064.if_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000064.if_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000064.if_nested.tlg %2
)
