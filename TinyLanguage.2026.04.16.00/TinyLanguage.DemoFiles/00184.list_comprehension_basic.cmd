@echo off
echo Running 00184.list_comprehension_basic.tlg
if "%2"=="" (
    TinyLanguage.exe 00184.list_comprehension_basic.tlg output.txt
) else (
    TinyLanguage.exe 00184.list_comprehension_basic.tlg %2
)
