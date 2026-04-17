@echo off
echo Running 00147.class_empty.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000147.class_empty.tlg output.txt
) else (
    TinyLanguage.exe %~dp000147.class_empty.tlg %2
)
