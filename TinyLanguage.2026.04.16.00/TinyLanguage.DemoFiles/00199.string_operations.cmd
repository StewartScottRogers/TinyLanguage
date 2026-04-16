@echo off
echo Running 00199.string_operations.tlg
if "%2"=="" (
    TinyLanguage.exe 00199.string_operations.tlg output.txt
) else (
    TinyLanguage.exe 00199.string_operations.tlg %2
)
