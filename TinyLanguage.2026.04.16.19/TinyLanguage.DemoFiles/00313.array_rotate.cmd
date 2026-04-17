@echo off
echo Running 00313.array_rotate.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000313.array_rotate.tlg output.txt
) else (
    TinyLanguage.exe %~dp000313.array_rotate.tlg %2
)
