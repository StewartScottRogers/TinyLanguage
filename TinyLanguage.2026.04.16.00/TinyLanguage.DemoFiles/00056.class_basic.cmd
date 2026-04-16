@echo off
echo Running 00056.class_basic.tlg
if "%2"=="" (
    TinyLanguage.exe 00056.class_basic.tlg output.txt
) else (
    TinyLanguage.exe 00056.class_basic.tlg %2
)
