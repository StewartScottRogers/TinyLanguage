@echo off
echo Running 00141.collatz_sequence.tlg
if "%2"=="" (
    TinyLanguage.exe 00141.collatz_sequence.tlg output.txt
) else (
    TinyLanguage.exe 00141.collatz_sequence.tlg %2
)
