@echo off
echo Running 00084.for_step_5.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000084.for_step_5.tlg output.txt
) else (
    TinyLanguage.exe %~dp000084.for_step_5.tlg %2
)
