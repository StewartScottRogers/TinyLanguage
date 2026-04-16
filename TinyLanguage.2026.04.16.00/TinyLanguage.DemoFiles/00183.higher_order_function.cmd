@echo off
echo Running 00183.higher_order_function.tlg
if "%2"=="" (
    TinyLanguage.exe 00183.higher_order_function.tlg output.txt
) else (
    TinyLanguage.exe 00183.higher_order_function.tlg %2
)
