@echo off
echo Running 00195.floor_div_float.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000195.floor_div_float.tlg output.txt
) else (
    TinyLanguage.exe %~dp000195.floor_div_float.tlg %2
)
