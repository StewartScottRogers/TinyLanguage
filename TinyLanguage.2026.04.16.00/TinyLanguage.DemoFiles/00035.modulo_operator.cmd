@echo off
echo Running 00035.modulo_operator.tlg
if "%2"=="" (
    TinyLanguage.exe 00035.modulo_operator.tlg output.txt
) else (
    TinyLanguage.exe 00035.modulo_operator.tlg %2
)
