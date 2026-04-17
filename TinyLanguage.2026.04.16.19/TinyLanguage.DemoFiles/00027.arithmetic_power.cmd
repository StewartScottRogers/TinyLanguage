@echo off
echo Running 00027.arithmetic_power.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000027.arithmetic_power.tlg output.txt
) else (
    TinyLanguage.exe %~dp000027.arithmetic_power.tlg %2
)
