@echo off
echo Running 00122.filter_like.tlg
if "%2"=="" (
    TinyLanguage.exe 00122.filter_like.tlg output.txt
) else (
    TinyLanguage.exe 00122.filter_like.tlg %2
)
