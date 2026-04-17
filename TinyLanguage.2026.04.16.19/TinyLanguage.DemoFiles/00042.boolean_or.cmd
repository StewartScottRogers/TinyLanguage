@echo off
echo Running 00042.boolean_or.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000042.boolean_or.tlg output.txt
) else (
    TinyLanguage.exe %~dp000042.boolean_or.tlg %2
)
