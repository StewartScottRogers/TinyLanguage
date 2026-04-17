@echo off
echo Running 00289.multiple_modules.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000289.multiple_modules.tlg output.txt
) else (
    TinyLanguage.exe %~dp000289.multiple_modules.tlg %2
)
