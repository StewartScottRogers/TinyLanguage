@echo off
echo Running 00202.deeply_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000202.deeply_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000202.deeply_nested.tlg %2
)
