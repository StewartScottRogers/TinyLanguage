@echo off
echo Running 00037.compare_strings.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000037.compare_strings.tlg output.txt
) else (
    TinyLanguage.exe %~dp000037.compare_strings.tlg %2
)
