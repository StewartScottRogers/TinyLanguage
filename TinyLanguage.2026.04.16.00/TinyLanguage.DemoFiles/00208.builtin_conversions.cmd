@echo off
echo Running 00208.builtin_conversions.tlg
if "%2"=="" (
    TinyLanguage.exe 00208.builtin_conversions.tlg output.txt
) else (
    TinyLanguage.exe 00208.builtin_conversions.tlg %2
)
