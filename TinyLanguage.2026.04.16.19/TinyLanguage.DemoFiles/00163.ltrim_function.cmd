@echo off
echo Running 00163.ltrim_function.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000163.ltrim_function.tlg output.txt
) else (
    TinyLanguage.exe %~dp000163.ltrim_function.tlg %2
)
