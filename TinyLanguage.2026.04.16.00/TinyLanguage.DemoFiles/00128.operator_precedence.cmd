@echo off
echo Running 00128.operator_precedence.tlg
if "%2"=="" (
    TinyLanguage.exe 00128.operator_precedence.tlg output.txt
) else (
    TinyLanguage.exe 00128.operator_precedence.tlg %2
)
