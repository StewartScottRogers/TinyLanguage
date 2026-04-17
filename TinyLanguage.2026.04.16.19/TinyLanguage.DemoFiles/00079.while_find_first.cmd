@echo off
echo Running 00079.while_find_first.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000079.while_find_first.tlg output.txt
) else (
    TinyLanguage.exe %~dp000079.while_find_first.tlg %2
)
