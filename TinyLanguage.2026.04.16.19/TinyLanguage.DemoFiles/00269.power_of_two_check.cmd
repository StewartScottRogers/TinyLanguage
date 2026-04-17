@echo off
echo Running 00269.power_of_two_check.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000269.power_of_two_check.tlg output.txt
) else (
    TinyLanguage.exe %~dp000269.power_of_two_check.tlg %2
)
