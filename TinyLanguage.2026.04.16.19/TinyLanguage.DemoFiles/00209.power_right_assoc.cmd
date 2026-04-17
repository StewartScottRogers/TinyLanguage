@echo off
echo Running 00209.power_right_assoc.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000209.power_right_assoc.tlg output.txt
) else (
    TinyLanguage.exe %~dp000209.power_right_assoc.tlg %2
)
