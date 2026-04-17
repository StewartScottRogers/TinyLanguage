@echo off
echo Running 00174.annotation_basic2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000174.annotation_basic2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000174.annotation_basic2.tlg %2
)
