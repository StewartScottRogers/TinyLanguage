@echo off
echo Running 00097.binary_search.tlg
if "%2"=="" (
    TinyLanguage.exe 00097.binary_search.tlg output.txt
) else (
    TinyLanguage.exe 00097.binary_search.tlg %2
)
