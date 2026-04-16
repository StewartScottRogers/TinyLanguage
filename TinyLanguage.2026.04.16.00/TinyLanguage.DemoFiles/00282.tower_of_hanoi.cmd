@echo off
echo Running 00282.tower_of_hanoi.tlg
if "%2"=="" (
    TinyLanguage.exe 00282.tower_of_hanoi.tlg output.txt
) else (
    TinyLanguage.exe 00282.tower_of_hanoi.tlg %2
)
