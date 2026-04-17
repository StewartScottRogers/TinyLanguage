@echo off
echo Running 00148.class_const_field.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000148.class_const_field.tlg output.txt
) else (
    TinyLanguage.exe %~dp000148.class_const_field.tlg %2
)
