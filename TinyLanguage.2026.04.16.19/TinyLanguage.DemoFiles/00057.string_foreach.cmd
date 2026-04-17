@echo off
echo Running 00057.string_foreach.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000057.string_foreach.tlg output.txt
) else (
    TinyLanguage.exe %~dp000057.string_foreach.tlg %2
)
