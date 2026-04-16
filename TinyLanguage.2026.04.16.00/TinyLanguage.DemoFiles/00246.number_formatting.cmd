@echo off
echo Running 00246.number_formatting.tlg
if "%2"=="" (
    TinyLanguage.exe 00246.number_formatting.tlg output.txt
) else (
    TinyLanguage.exe 00246.number_formatting.tlg %2
)
