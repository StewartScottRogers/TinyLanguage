@echo off
echo Running 00030.mixed_arithmetic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000030.mixed_arithmetic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000030.mixed_arithmetic.tlg %2
)
