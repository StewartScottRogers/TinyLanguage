@echo off
echo Running 00213.cast_int_expression.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000213.cast_int_expression.tlg output.txt
) else (
    TinyLanguage.exe %~dp000213.cast_int_expression.tlg %2
)
