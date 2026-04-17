@echo off
echo Running 00036.compare_greater_equal.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000036.compare_greater_equal.tlg output.txt
) else (
    TinyLanguage.exe %~dp000036.compare_greater_equal.tlg %2
)
