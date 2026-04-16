@echo off
echo Running 00118.nested_functions.tlg
if "%2"=="" (
    TinyLanguage.exe 00118.nested_functions.tlg output.txt
) else (
    TinyLanguage.exe 00118.nested_functions.tlg %2
)
