@echo off
echo Running 00294.count_if.tlg
if "%2"=="" (
    TinyLanguage.exe 00294.count_if.tlg output.txt
) else (
    TinyLanguage.exe 00294.count_if.tlg %2
)
