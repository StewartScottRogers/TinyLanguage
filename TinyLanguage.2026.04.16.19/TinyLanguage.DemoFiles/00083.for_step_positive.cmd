@echo off
echo Running 00083.for_step_positive.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000083.for_step_positive.tlg output.txt
) else (
    TinyLanguage.exe %~dp000083.for_step_positive.tlg %2
)
