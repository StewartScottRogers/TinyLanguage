@echo off
echo Running 00159.max_min_functions.tlg
if "%2"=="" (
    TinyLanguage.exe 00159.max_min_functions.tlg output.txt
) else (
    TinyLanguage.exe 00159.max_min_functions.tlg %2
)
