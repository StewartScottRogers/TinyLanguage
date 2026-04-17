@echo off
echo Running 00146.class_inheritance.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000146.class_inheritance.tlg output.txt
) else (
    TinyLanguage.exe %~dp000146.class_inheritance.tlg %2
)
