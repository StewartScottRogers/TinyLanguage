@echo off
echo Running 00028.operator_precedence.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000028.operator_precedence.tlg output.txt
) else (
    TinyLanguage.exe %~dp000028.operator_precedence.tlg %2
)
