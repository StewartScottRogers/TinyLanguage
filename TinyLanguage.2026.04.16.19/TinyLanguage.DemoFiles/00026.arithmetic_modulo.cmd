@echo off
echo Running 00026.arithmetic_modulo.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000026.arithmetic_modulo.tlg output.txt
) else (
    TinyLanguage.exe %~dp000026.arithmetic_modulo.tlg %2
)
