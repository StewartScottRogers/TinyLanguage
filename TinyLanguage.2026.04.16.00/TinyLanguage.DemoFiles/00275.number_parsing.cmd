@echo off
echo Running 00275.number_parsing.tlg
if "%2"=="" (
    TinyLanguage.exe 00275.number_parsing.tlg output.txt
) else (
    TinyLanguage.exe 00275.number_parsing.tlg %2
)
