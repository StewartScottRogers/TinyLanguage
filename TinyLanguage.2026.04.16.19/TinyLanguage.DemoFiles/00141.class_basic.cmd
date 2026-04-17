@echo off
echo Running 00141.class_basic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000141.class_basic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000141.class_basic.tlg %2
)
