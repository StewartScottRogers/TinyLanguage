@echo off
echo Running 00071.while_simple.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000071.while_simple.tlg output.txt
) else (
    TinyLanguage.exe %~dp000071.while_simple.tlg %2
)
