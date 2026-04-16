@echo off
echo Running 00115.static_function.tlg
if "%2"=="" (
    TinyLanguage.exe 00115.static_function.tlg output.txt
) else (
    TinyLanguage.exe 00115.static_function.tlg %2
)
