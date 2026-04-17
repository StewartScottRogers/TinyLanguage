@echo off
echo Running 00032.compare_not_equal.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000032.compare_not_equal.tlg output.txt
) else (
    TinyLanguage.exe %~dp000032.compare_not_equal.tlg %2
)
