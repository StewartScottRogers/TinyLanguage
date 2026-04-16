@echo off
echo Running 00152.celsius_scale.tlg
if "%2"=="" (
    TinyLanguage.exe 00152.celsius_scale.tlg output.txt
) else (
    TinyLanguage.exe 00152.celsius_scale.tlg %2
)
