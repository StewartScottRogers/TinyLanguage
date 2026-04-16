@echo off
echo Running 00178.static_module_functions.tlg
if "%2"=="" (
    TinyLanguage.exe 00178.static_module_functions.tlg output.txt
) else (
    TinyLanguage.exe 00178.static_module_functions.tlg %2
)
