@echo off
echo Running 00081.cast_expression.tlg
if "%2"=="" (
    TinyLanguage.exe 00081.cast_expression.tlg output.txt
) else (
    TinyLanguage.exe 00081.cast_expression.tlg %2
)
