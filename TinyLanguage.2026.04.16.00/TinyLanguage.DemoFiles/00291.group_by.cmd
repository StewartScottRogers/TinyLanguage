@echo off
echo Running 00291.group_by.tlg
if "%2"=="" (
    TinyLanguage.exe 00291.group_by.tlg output.txt
) else (
    TinyLanguage.exe 00291.group_by.tlg %2
)
