@echo off
echo Running 00232.nested_try_catch.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000232.nested_try_catch.tlg output.txt
) else (
    TinyLanguage.exe %~dp000232.nested_try_catch.tlg %2
)
