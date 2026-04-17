@echo off
echo Running 00231.throw_in_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000231.throw_in_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000231.throw_in_function.tlg %2
)
