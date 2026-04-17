@echo off
echo Running 00225.pattern_bool.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000225.pattern_bool.tlg output.txt
) else (
    TinyLanguage.exe %~dp000225.pattern_bool.tlg %2
)
