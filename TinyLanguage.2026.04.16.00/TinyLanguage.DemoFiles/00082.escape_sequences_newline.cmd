@echo off
echo Running 00082.escape_sequences_newline.tlg
if "%2"=="" (
    TinyLanguage.exe 00082.escape_sequences_newline.tlg output.txt
) else (
    TinyLanguage.exe 00082.escape_sequences_newline.tlg %2
)
