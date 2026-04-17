@echo off
echo Running 00296.recursive_countdown.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000296.recursive_countdown.tlg output.txt
) else (
    TinyLanguage.exe %~dp000296.recursive_countdown.tlg %2
)
