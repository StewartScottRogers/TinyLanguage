@echo off
echo Running 00158.abs_function.tlg
if "%2"=="" (
    TinyLanguage.exe 00158.abs_function.tlg output.txt
) else (
    TinyLanguage.exe 00158.abs_function.tlg %2
)
