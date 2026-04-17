@echo off
echo Running 00137.array_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000137.array_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000137.array_nested.tlg %2
)
