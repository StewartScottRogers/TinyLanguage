@echo off
echo Running 00118.function_higher_order.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000118.function_higher_order.tlg output.txt
) else (
    TinyLanguage.exe %~dp000118.function_higher_order.tlg %2
)
