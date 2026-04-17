@echo off
echo Running 00228.try_catch_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000228.try_catch_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000228.try_catch_simple.tlg %2
)
