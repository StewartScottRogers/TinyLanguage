@echo off
echo Running 00049.boolean_variables.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000049.boolean_variables.tlg output.txt
) else (
    TinyLanguage.exe %~dp000049.boolean_variables.tlg %2
)
