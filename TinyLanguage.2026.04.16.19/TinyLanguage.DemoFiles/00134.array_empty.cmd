@echo off
echo Running 00134.array_empty.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000134.array_empty.tlg output.txt
) else (
    TinyLanguage.exe %~dp000134.array_empty.tlg %2
)
