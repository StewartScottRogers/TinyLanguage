@echo off
echo Running 00024.arithmetic_divide.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000024.arithmetic_divide.tlg output.txt
) else (
    TinyLanguage.exe %~dp000024.arithmetic_divide.tlg %2
)
