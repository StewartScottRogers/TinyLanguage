@echo off
echo Running 00066.if_as_expression.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000066.if_as_expression.tlg output.txt
) else (
    TinyLanguage.exe %~dp000066.if_as_expression.tlg %2
)
