@echo off
echo Running 00307.exception_typed_catch.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000307.exception_typed_catch.tlg output.txt
) else (
    TinyLanguage.exe %~dp000307.exception_typed_catch.tlg %2
)
