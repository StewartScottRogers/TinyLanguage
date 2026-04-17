@echo off
echo Running 00172.nested_try_catch2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000172.nested_try_catch2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000172.nested_try_catch2.tlg %2
)
