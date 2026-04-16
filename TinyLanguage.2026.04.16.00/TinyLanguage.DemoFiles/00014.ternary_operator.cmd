@echo off
echo Running 00014.ternary_operator.tlg
if "%2"=="" (
    TinyLanguage.exe 00014.ternary_operator.tlg output.txt
) else (
    TinyLanguage.exe 00014.ternary_operator.tlg %2
)
