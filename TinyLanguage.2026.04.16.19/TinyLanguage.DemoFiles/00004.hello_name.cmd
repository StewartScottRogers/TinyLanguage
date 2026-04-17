@echo off
echo Running 00004.hello_name.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000004.hello_name.tlg output.txt
) else (
    TinyLanguage.exe %~dp000004.hello_name.tlg %2
)
