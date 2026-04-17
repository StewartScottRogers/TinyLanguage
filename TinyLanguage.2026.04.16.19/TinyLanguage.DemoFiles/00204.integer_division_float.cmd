@echo off
echo Running 00204.integer_division_float.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000204.integer_division_float.tlg output.txt
) else (
    TinyLanguage.exe %~dp000204.integer_division_float.tlg %2
)
