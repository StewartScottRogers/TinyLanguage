@echo off
echo Running 00043.boolean_not.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000043.boolean_not.tlg output.txt
) else (
    TinyLanguage.exe %~dp000043.boolean_not.tlg %2
)
