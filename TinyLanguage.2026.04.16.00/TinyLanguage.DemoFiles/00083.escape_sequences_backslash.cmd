@echo off
echo Running 00083.escape_sequences_backslash.tlg
if "%2"=="" (
    TinyLanguage.exe 00083.escape_sequences_backslash.tlg output.txt
) else (
    TinyLanguage.exe 00083.escape_sequences_backslash.tlg %2
)
