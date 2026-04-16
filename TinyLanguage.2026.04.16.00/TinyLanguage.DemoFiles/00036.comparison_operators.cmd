@echo off
echo Running 00036.comparison_operators.tlg
if "%2"=="" (
    TinyLanguage.exe 00036.comparison_operators.tlg output.txt
) else (
    TinyLanguage.exe 00036.comparison_operators.tlg %2
)
