@echo off
echo Running 00174.annotation_basic.tlg
if "%2"=="" (
    TinyLanguage.exe 00174.annotation_basic.tlg output.txt
) else (
    TinyLanguage.exe 00174.annotation_basic.tlg %2
)
