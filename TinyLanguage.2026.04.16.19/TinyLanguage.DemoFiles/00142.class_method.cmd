@echo off
echo Running 00142.class_method.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000142.class_method.tlg output.txt
) else (
    TinyLanguage.exe %~dp000142.class_method.tlg %2
)
