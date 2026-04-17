@echo off
echo Running 00161.range_array.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000161.range_array.tlg output.txt
) else (
    TinyLanguage.exe %~dp000161.range_array.tlg %2
)
