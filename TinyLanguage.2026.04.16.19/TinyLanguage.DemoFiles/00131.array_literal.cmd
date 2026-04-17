@echo off
echo Running 00131.array_literal.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000131.array_literal.tlg output.txt
) else (
    TinyLanguage.exe %~dp000131.array_literal.tlg %2
)
