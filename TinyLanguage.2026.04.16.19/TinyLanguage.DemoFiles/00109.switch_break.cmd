@echo off
echo Running 00109.switch_break.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000109.switch_break.tlg output.txt
) else (
    TinyLanguage.exe %~dp000109.switch_break.tlg %2
)
