@echo off
echo Running 00152.celsius_scale.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000152.celsius_scale.tlg output.txt
) else (
    TinyLanguage.exe %~dp000152.celsius_scale.tlg %2
)
