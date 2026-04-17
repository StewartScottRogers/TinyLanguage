@echo off
echo Running 00290.string_number_formatting.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000290.string_number_formatting.tlg output.txt
) else (
    TinyLanguage.exe %~dp000290.string_number_formatting.tlg %2
)
