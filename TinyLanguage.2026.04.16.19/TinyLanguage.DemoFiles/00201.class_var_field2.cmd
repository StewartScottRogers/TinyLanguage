@echo off
echo Running 00201.class_var_field2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000201.class_var_field2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000201.class_var_field2.tlg %2
)
