@echo off
echo Running 00226.module_basic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000226.module_basic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000226.module_basic.tlg %2
)
