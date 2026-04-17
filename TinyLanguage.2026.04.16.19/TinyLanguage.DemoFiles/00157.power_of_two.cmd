@echo off
echo Running 00157.power_of_two.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000157.power_of_two.tlg output.txt
) else (
    TinyLanguage.exe %~dp000157.power_of_two.tlg %2
)
