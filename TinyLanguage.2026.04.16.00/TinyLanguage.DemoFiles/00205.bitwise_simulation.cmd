@echo off
echo Running 00205.bitwise_simulation.tlg
if "%2"=="" (
    TinyLanguage.exe 00205.bitwise_simulation.tlg output.txt
) else (
    TinyLanguage.exe 00205.bitwise_simulation.tlg %2
)
