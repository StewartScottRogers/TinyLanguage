@echo off
echo Running 00259.recursive_max.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000259.recursive_max.tlg output.txt
) else (
    TinyLanguage.exe %~dp000259.recursive_max.tlg %2
)
