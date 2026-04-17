@echo off
echo Running 00229.try_catch_finally.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000229.try_catch_finally.tlg output.txt
) else (
    TinyLanguage.exe %~dp000229.try_catch_finally.tlg %2
)
