@echo off
echo Running 00230.try_no_throw.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000230.try_no_throw.tlg output.txt
) else (
    TinyLanguage.exe %~dp000230.try_no_throw.tlg %2
)
