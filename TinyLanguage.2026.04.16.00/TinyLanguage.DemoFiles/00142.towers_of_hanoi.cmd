@echo off
echo Running 00142.towers_of_hanoi.tlg
if "%2"=="" (
    TinyLanguage.exe 00142.towers_of_hanoi.tlg output.txt
) else (
    TinyLanguage.exe 00142.towers_of_hanoi.tlg %2
)
