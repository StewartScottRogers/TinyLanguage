@echo off
echo Running 00306.module_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000306.module_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000306.module_nested.tlg %2
)
