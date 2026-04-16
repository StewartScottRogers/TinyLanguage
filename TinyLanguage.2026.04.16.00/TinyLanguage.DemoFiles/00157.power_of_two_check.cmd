@echo off
echo Running 00157.power_of_two_check.tlg
if "%2"=="" (
    TinyLanguage.exe 00157.power_of_two_check.tlg output.txt
) else (
    TinyLanguage.exe 00157.power_of_two_check.tlg %2
)
