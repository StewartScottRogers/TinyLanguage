@echo off
echo Running 00017.for_loop_step.tlg
if "%2"=="" (
    TinyLanguage.exe 00017.for_loop_step.tlg output.txt
) else (
    TinyLanguage.exe 00017.for_loop_step.tlg %2
)
