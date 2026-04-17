@echo off
echo Running 00224.pattern_null.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000224.pattern_null.tlg output.txt
) else (
    TinyLanguage.exe %~dp000224.pattern_null.tlg %2
)
