@echo off
echo Running 00071.try_catch_typed.tlg
if "%2"=="" (
    TinyLanguage.exe 00071.try_catch_typed.tlg output.txt
) else (
    TinyLanguage.exe 00071.try_catch_typed.tlg %2
)
