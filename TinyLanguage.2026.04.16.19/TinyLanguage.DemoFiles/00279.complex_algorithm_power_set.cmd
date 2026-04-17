@echo off
echo Running 00279.complex_algorithm_power_set.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000279.complex_algorithm_power_set.tlg output.txt
) else (
    TinyLanguage.exe %~dp000279.complex_algorithm_power_set.tlg %2
)
