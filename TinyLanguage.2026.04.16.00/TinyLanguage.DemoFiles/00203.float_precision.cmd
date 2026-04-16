@echo off
echo Running 00203.float_precision.tlg
if "%2"=="" (
    TinyLanguage.exe 00203.float_precision.tlg output.txt
) else (
    TinyLanguage.exe 00203.float_precision.tlg %2
)
