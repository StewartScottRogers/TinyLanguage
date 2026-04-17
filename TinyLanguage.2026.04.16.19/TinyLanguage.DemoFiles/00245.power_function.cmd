@echo off
echo Running 00245.power_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000245.power_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000245.power_function.tlg %2
)
