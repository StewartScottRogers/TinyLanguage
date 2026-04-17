@echo off
echo Running 00158.abs_function2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000158.abs_function2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000158.abs_function2.tlg %2
)
