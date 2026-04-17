@echo off
echo Running 00035.compare_less_equal.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000035.compare_less_equal.tlg output.txt
) else (
    TinyLanguage.exe %~dp000035.compare_less_equal.tlg %2
)
