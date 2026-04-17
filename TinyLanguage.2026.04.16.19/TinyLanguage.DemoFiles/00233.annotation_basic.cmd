@echo off
echo Running 00233.annotation_basic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000233.annotation_basic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000233.annotation_basic.tlg %2
)
