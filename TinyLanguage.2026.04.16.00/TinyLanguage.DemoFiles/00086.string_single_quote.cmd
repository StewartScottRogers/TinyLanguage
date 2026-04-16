@echo off
echo Running 00086.string_single_quote.tlg
if "%2"=="" (
    TinyLanguage.exe 00086.string_single_quote.tlg output.txt
) else (
    TinyLanguage.exe 00086.string_single_quote.tlg %2
)
