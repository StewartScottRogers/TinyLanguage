@echo off
echo Running 00166.reduce_pattern.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000166.reduce_pattern.tlg output.txt
) else (
    TinyLanguage.exe %~dp000166.reduce_pattern.tlg %2
)
