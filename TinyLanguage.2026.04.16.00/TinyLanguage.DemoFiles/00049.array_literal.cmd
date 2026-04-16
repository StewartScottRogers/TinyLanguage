@echo off
echo Running 00049.array_literal.tlg
if "%2"=="" (
    TinyLanguage.exe 00049.array_literal.tlg output.txt
) else (
    TinyLanguage.exe 00049.array_literal.tlg %2
)
