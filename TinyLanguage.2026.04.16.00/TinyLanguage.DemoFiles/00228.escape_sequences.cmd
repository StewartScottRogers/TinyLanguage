@echo off
echo Running 00228.escape_sequences.tlg
if "%2"=="" (
    TinyLanguage.exe 00228.escape_sequences.tlg output.txt
) else (
    TinyLanguage.exe 00228.escape_sequences.tlg %2
)
