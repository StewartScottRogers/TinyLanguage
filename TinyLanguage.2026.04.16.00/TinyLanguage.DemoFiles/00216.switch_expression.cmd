@echo off
echo Running 00216.switch_expression.tlg
if "%2"=="" (
    TinyLanguage.exe 00216.switch_expression.tlg output.txt
) else (
    TinyLanguage.exe 00216.switch_expression.tlg %2
)
