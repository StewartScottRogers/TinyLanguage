@echo off
echo Running 00275.abs_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000275.abs_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000275.abs_function.tlg %2
)
