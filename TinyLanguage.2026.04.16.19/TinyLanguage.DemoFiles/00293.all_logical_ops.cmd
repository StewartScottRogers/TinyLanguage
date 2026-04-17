@echo off
echo Running 00293.all_logical_ops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000293.all_logical_ops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000293.all_logical_ops.tlg %2
)
