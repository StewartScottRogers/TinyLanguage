@echo off
echo Running 00206.mixed_arithmetic.tlg
if "%2"=="" (
    TinyLanguage.exe 00206.mixed_arithmetic.tlg output.txt
) else (
    TinyLanguage.exe 00206.mixed_arithmetic.tlg %2
)
