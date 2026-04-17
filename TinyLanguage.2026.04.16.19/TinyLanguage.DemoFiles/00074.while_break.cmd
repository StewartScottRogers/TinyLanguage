@echo off
echo Running 00074.while_break.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000074.while_break.tlg output.txt
) else (
    TinyLanguage.exe %~dp000074.while_break.tlg %2
)
