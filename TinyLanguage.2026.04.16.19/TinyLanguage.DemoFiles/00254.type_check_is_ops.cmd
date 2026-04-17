@echo off
echo Running 00254.type_check_is_ops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000254.type_check_is_ops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000254.type_check_is_ops.tlg %2
)
