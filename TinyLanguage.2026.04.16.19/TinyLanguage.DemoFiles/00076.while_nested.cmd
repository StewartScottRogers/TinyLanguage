@echo off
echo Running 00076.while_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000076.while_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000076.while_nested.tlg %2
)
