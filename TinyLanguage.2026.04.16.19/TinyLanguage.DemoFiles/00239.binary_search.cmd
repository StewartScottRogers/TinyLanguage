@echo off
echo Running 00239.binary_search.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000239.binary_search.tlg output.txt
) else (
    TinyLanguage.exe %~dp000239.binary_search.tlg %2
)
