@echo off
echo Running 00106.switch_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000106.switch_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000106.switch_simple.tlg %2
)
