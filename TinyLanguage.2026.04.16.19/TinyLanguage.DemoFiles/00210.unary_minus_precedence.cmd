@echo off
echo Running 00210.unary_minus_precedence.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000210.unary_minus_precedence.tlg output.txt
) else (
    TinyLanguage.exe %~dp000210.unary_minus_precedence.tlg %2
)
