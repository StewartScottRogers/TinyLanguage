@echo off
echo Running 00103.max_in_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00103.max_in_array.tlg output.txt
) else (
    TinyLanguage.exe 00103.max_in_array.tlg %2
)
