@echo off
echo Running 00087.string_double_quote.tlg
if "%2"=="" (
    TinyLanguage.exe 00087.string_double_quote.tlg output.txt
) else (
    TinyLanguage.exe 00087.string_double_quote.tlg %2
)
