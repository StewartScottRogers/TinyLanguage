@echo off
echo Running 00268.triangle_numbers.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000268.triangle_numbers.tlg output.txt
) else (
    TinyLanguage.exe %~dp000268.triangle_numbers.tlg %2
)
