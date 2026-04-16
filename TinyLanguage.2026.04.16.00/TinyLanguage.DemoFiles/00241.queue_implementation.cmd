@echo off
echo Running 00241.queue_implementation.tlg
if "%2"=="" (
    TinyLanguage.exe 00241.queue_implementation.tlg output.txt
) else (
    TinyLanguage.exe 00241.queue_implementation.tlg %2
)
