@echo off
echo Running 00155.is_perfect_number.tlg
if "%2"=="" (
    TinyLanguage.exe 00155.is_perfect_number.tlg output.txt
) else (
    TinyLanguage.exe 00155.is_perfect_number.tlg %2
)
