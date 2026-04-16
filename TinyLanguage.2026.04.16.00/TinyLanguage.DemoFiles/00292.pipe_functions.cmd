@echo off
echo Running 00292.pipe_functions.tlg
if "%2"=="" (
    TinyLanguage.exe 00292.pipe_functions.tlg output.txt
) else (
    TinyLanguage.exe 00292.pipe_functions.tlg %2
)
