@echo off
echo Running 00113.enum_basic.tlg
if "%2"=="" (
    TinyLanguage.exe 00113.enum_basic.tlg output.txt
) else (
    TinyLanguage.exe 00113.enum_basic.tlg %2
)
