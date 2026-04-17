@echo off
echo Running 00069.if_not_condition.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000069.if_not_condition.tlg output.txt
) else (
    TinyLanguage.exe %~dp000069.if_not_condition.tlg %2
)
