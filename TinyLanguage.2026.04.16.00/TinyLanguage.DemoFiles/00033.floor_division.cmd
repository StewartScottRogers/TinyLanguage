@echo off
echo Running 00033.floor_division.tlg
if "%2"=="" (
    TinyLanguage.exe 00033.floor_division.tlg output.txt
) else (
    TinyLanguage.exe 00033.floor_division.tlg %2
)
