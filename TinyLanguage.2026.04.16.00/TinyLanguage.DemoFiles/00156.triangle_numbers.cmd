@echo off
echo Running 00156.triangle_numbers.tlg
if "%2"=="" (
    TinyLanguage.exe 00156.triangle_numbers.tlg output.txt
) else (
    TinyLanguage.exe 00156.triangle_numbers.tlg %2
)
