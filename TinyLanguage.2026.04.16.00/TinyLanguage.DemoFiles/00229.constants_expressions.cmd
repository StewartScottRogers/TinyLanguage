@echo off
echo Running 00229.constants_expressions.tlg
if "%2"=="" (
    TinyLanguage.exe 00229.constants_expressions.tlg output.txt
) else (
    TinyLanguage.exe 00229.constants_expressions.tlg %2
)
