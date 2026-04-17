@echo off
echo Running 00136.array_strings.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000136.array_strings.tlg output.txt
) else (
    TinyLanguage.exe %~dp000136.array_strings.tlg %2
)
