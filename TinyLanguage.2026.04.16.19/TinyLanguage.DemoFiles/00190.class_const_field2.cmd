@echo off
echo Running 00190.class_const_field2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000190.class_const_field2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000190.class_const_field2.tlg %2
)
