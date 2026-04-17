@echo off
echo Running 00085.for_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000085.for_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000085.for_nested.tlg %2
)
