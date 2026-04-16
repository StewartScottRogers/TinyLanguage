@echo off
echo Running 00104.min_in_array.tlg
if "%2"=="" (
    TinyLanguage.exe 00104.min_in_array.tlg output.txt
) else (
    TinyLanguage.exe 00104.min_in_array.tlg %2
)
