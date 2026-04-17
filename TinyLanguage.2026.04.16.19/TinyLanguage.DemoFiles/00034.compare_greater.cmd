@echo off
echo Running 00034.compare_greater.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000034.compare_greater.tlg output.txt
) else (
    TinyLanguage.exe %~dp000034.compare_greater.tlg %2
)
