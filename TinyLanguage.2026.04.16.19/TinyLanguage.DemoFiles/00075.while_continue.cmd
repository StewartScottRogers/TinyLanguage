@echo off
echo Running 00075.while_continue.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000075.while_continue.tlg output.txt
) else (
    TinyLanguage.exe %~dp000075.while_continue.tlg %2
)
