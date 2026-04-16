@echo off
echo Running 00200.array_operations.tlg
if "%2"=="" (
    TinyLanguage.exe 00200.array_operations.tlg output.txt
) else (
    TinyLanguage.exe 00200.array_operations.tlg %2
)
