@echo off
echo Running 00291.all_arithmetic_ops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000291.all_arithmetic_ops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000291.all_arithmetic_ops.tlg %2
)
