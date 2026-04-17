@echo off
echo Running 00273.max_min_functions.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000273.max_min_functions.tlg output.txt
) else (
    TinyLanguage.exe %~dp000273.max_min_functions.tlg %2
)
