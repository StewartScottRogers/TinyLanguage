@echo off
echo Running 00196.module_export.tlg
if "%2"=="" (
    TinyLanguage.exe 00196.module_export.tlg output.txt
) else (
    TinyLanguage.exe 00196.module_export.tlg %2
)
