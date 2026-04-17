@echo off
echo Running 00081.for_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000081.for_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000081.for_simple.tlg %2
)
