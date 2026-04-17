@echo off
echo Running 00194.power_operator_float.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000194.power_operator_float.tlg output.txt
) else (
    TinyLanguage.exe %~dp000194.power_operator_float.tlg %2
)
