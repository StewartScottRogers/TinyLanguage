@echo off
echo Running 00304.switch_boolean.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000304.switch_boolean.tlg output.txt
) else (
    TinyLanguage.exe %~dp000304.switch_boolean.tlg %2
)
