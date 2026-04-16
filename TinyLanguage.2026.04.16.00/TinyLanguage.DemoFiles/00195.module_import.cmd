@echo off
echo Running 00195.module_import.tlg
if "%2"=="" (
    TinyLanguage.exe 00195.module_import.tlg output.txt
) else (
    TinyLanguage.exe 00195.module_import.tlg %2
)
