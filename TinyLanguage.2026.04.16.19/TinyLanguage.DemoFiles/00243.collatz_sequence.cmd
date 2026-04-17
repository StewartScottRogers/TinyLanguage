@echo off
echo Running 00243.collatz_sequence.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000243.collatz_sequence.tlg output.txt
) else (
    TinyLanguage.exe %~dp000243.collatz_sequence.tlg %2
)
