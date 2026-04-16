@echo off
echo Running 00032.arithmetic_operators.tlg
if "%2"=="" (
    TinyLanguage.exe 00032.arithmetic_operators.tlg output.txt
) else (
    TinyLanguage.exe 00032.arithmetic_operators.tlg %2
)
