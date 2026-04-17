@echo off
echo Running 00150.class_var_field.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000150.class_var_field.tlg output.txt
) else (
    TinyLanguage.exe %~dp000150.class_var_field.tlg %2
)
