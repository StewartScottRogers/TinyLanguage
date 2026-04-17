@echo off
echo Running 00248.string_count_char.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000248.string_count_char.tlg output.txt
) else (
    TinyLanguage.exe %~dp000248.string_count_char.tlg %2
)
