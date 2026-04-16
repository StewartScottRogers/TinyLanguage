@echo off
echo Running 00100.string_reversal.tlg
if "%2"=="" (
    TinyLanguage.exe 00100.string_reversal.tlg output.txt
) else (
    TinyLanguage.exe 00100.string_reversal.tlg %2
)
