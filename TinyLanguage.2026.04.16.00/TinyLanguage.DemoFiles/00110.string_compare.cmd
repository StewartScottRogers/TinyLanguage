@echo off
echo Running 00110.string_compare.tlg
if "%2"=="" (
    TinyLanguage.exe 00110.string_compare.tlg output.txt
) else (
    TinyLanguage.exe 00110.string_compare.tlg %2
)
