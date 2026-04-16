@echo off
echo Running 00065.module_with_functions.tlg
if "%2"=="" (
    TinyLanguage.exe 00065.module_with_functions.tlg output.txt
) else (
    TinyLanguage.exe 00065.module_with_functions.tlg %2
)
