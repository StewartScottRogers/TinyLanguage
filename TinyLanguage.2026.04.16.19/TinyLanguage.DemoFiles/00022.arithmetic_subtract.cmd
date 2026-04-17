@echo off
echo Running 00022.arithmetic_subtract.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000022.arithmetic_subtract.tlg output.txt
) else (
    TinyLanguage.exe %~dp000022.arithmetic_subtract.tlg %2
)
