@echo off
echo Running 00221.list_comprehension.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000221.list_comprehension.tlg output.txt
) else (
    TinyLanguage.exe %~dp000221.list_comprehension.tlg %2
)
