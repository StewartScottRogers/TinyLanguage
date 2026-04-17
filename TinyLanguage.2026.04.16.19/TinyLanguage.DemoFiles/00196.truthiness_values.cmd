@echo off
echo Running 00196.truthiness_values.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000196.truthiness_values.tlg output.txt
) else (
    TinyLanguage.exe %~dp000196.truthiness_values.tlg %2
)
