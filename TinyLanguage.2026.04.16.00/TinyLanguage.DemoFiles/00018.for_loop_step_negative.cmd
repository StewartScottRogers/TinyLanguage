@echo off
echo Running 00018.for_loop_step_negative.tlg
if "%2"=="" (
    TinyLanguage.exe 00018.for_loop_step_negative.tlg output.txt
) else (
    TinyLanguage.exe 00018.for_loop_step_negative.tlg %2
)
