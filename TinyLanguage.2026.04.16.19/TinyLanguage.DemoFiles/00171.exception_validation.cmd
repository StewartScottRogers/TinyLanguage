@echo off
echo Running 00171.exception_validation.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000171.exception_validation.tlg output.txt
) else (
    TinyLanguage.exe %~dp000171.exception_validation.tlg %2
)
