@echo off
echo Running 00031.compare_equal.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000031.compare_equal.tlg output.txt
) else (
    TinyLanguage.exe %~dp000031.compare_equal.tlg %2
)
