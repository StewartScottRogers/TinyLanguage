@echo off
echo Running 00176.generic_type.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000176.generic_type.tlg output.txt
) else (
    TinyLanguage.exe %~dp000176.generic_type.tlg %2
)
