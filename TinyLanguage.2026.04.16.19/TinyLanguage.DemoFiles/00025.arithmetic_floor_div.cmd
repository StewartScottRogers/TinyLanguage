@echo off
echo Running 00025.arithmetic_floor_div.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000025.arithmetic_floor_div.tlg output.txt
) else (
    TinyLanguage.exe %~dp000025.arithmetic_floor_div.tlg %2
)
