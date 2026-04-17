@echo off
echo Running 00107.switch_string.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000107.switch_string.tlg output.txt
) else (
    TinyLanguage.exe %~dp000107.switch_string.tlg %2
)
