@echo off
echo Running 00132.array_index.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000132.array_index.tlg output.txt
) else (
    TinyLanguage.exe %~dp000132.array_index.tlg %2
)
