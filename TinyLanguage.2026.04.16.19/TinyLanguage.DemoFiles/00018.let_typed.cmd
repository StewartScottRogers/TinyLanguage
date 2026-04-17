@echo off
echo Running 00018.let_typed.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000018.let_typed.tlg output.txt
) else (
    TinyLanguage.exe %~dp000018.let_typed.tlg %2
)
