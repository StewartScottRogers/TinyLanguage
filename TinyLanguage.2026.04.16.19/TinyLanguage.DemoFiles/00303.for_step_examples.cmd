@echo off
echo Running 00303.for_step_examples.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000303.for_step_examples.tlg output.txt
) else (
    TinyLanguage.exe %~dp000303.for_step_examples.tlg %2
)
