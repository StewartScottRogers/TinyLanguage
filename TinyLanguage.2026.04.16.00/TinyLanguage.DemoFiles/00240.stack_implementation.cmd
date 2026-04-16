@echo off
echo Running 00240.stack_implementation.tlg
if "%2"=="" (
    TinyLanguage.exe 00240.stack_implementation.tlg output.txt
) else (
    TinyLanguage.exe 00240.stack_implementation.tlg %2
)
