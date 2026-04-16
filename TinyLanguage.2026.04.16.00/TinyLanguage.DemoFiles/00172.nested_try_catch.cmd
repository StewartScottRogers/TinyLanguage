@echo off
echo Running 00172.nested_try_catch.tlg
if "%2"=="" (
    TinyLanguage.exe 00172.nested_try_catch.tlg output.txt
) else (
    TinyLanguage.exe 00172.nested_try_catch.tlg %2
)
