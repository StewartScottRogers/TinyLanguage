@echo off
echo Running 00060.new_expression.tlg
if "%2"=="" (
    TinyLanguage.exe 00060.new_expression.tlg output.txt
) else (
    TinyLanguage.exe 00060.new_expression.tlg %2
)
