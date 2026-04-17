@echo off
echo Running 00309.as_operator.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000309.as_operator.tlg output.txt
) else (
    TinyLanguage.exe %~dp000309.as_operator.tlg %2
)
