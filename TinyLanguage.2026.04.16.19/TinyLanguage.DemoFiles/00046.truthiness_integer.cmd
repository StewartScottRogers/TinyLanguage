@echo off
echo Running 00046.truthiness_integer.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000046.truthiness_integer.tlg output.txt
) else (
    TinyLanguage.exe %~dp000046.truthiness_integer.tlg %2
)
