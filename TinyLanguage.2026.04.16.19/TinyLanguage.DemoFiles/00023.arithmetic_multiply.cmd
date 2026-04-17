@echo off
echo Running 00023.arithmetic_multiply.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000023.arithmetic_multiply.tlg output.txt
) else (
    TinyLanguage.exe %~dp000023.arithmetic_multiply.tlg %2
)
