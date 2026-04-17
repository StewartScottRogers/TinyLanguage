@echo off
echo Running 00047.truthiness_string.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000047.truthiness_string.tlg output.txt
) else (
    TinyLanguage.exe %~dp000047.truthiness_string.tlg %2
)
