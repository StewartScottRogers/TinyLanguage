@echo off
echo Running 00108.switch_no_default.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000108.switch_no_default.tlg output.txt
) else (
    TinyLanguage.exe %~dp000108.switch_no_default.tlg %2
)
