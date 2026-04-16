@echo off
echo Running 00121.higher_order_functions.tlg
if "%2"=="" (
    TinyLanguage.exe 00121.higher_order_functions.tlg output.txt
) else (
    TinyLanguage.exe 00121.higher_order_functions.tlg %2
)
