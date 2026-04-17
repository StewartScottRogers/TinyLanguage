@echo off
echo Running 00019.multiple_variables.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000019.multiple_variables.tlg output.txt
) else (
    TinyLanguage.exe %~dp000019.multiple_variables.tlg %2
)
