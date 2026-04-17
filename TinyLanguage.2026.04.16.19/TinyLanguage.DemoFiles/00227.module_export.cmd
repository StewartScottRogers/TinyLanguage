@echo off
echo Running 00227.module_export.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000227.module_export.tlg output.txt
) else (
    TinyLanguage.exe %~dp000227.module_export.tlg %2
)
