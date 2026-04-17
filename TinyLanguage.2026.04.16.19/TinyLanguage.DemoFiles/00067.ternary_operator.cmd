@echo off
echo Running 00067.ternary_operator.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000067.ternary_operator.tlg output.txt
) else (
    TinyLanguage.exe %~dp000067.ternary_operator.tlg %2
)
