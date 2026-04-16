@echo off
echo Running 00069.try_catch_finally.tlg
if "%2"=="" (
    TinyLanguage.exe 00069.try_catch_finally.tlg output.txt
) else (
    TinyLanguage.exe 00069.try_catch_finally.tlg %2
)
