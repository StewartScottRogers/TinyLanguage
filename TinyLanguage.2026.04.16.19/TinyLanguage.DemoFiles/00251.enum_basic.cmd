@echo off
echo Running 00251.enum_basic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000251.enum_basic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000251.enum_basic.tlg %2
)
