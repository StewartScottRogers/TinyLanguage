@echo off
echo Running 00276.exception_validate.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000276.exception_validate.tlg output.txt
) else (
    TinyLanguage.exe %~dp000276.exception_validate.tlg %2
)
