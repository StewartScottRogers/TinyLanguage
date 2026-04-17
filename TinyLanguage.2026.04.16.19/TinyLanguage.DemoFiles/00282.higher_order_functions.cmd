@echo off
echo Running 00282.higher_order_functions.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000282.higher_order_functions.tlg output.txt
) else (
    TinyLanguage.exe %~dp000282.higher_order_functions.tlg %2
)
