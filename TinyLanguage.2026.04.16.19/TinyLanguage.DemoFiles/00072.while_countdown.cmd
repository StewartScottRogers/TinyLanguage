@echo off
echo Running 00072.while_countdown.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000072.while_countdown.tlg output.txt
) else (
    TinyLanguage.exe %~dp000072.while_countdown.tlg %2
)
