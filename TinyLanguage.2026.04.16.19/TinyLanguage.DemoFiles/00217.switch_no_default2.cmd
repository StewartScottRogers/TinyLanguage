@echo off
echo Running 00217.switch_no_default2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000217.switch_no_default2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000217.switch_no_default2.tlg %2
)
