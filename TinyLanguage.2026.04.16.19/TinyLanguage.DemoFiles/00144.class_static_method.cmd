@echo off
echo Running 00144.class_static_method.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000144.class_static_method.tlg output.txt
) else (
    TinyLanguage.exe %~dp000144.class_static_method.tlg %2
)
