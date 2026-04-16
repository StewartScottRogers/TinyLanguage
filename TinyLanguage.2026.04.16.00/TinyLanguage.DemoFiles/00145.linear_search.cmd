@echo off
echo Running 00145.linear_search.tlg
if "%2"=="" (
    TinyLanguage.exe 00145.linear_search.tlg output.txt
) else (
    TinyLanguage.exe 00145.linear_search.tlg %2
)
