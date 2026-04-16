@echo off
echo Running 00168.flatten_nested.tlg
if "%2"=="" (
    TinyLanguage.exe 00168.flatten_nested.tlg output.txt
) else (
    TinyLanguage.exe 00168.flatten_nested.tlg %2
)
