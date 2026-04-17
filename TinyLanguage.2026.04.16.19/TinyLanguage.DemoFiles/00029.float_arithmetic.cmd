@echo off
echo Running 00029.float_arithmetic.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000029.float_arithmetic.tlg output.txt
) else (
    TinyLanguage.exe %~dp000029.float_arithmetic.tlg %2
)
