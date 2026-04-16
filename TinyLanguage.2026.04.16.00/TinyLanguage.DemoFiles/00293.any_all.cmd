@echo off
echo Running 00293.any_all.tlg
if "%2"=="" (
    TinyLanguage.exe 00293.any_all.tlg output.txt
) else (
    TinyLanguage.exe 00293.any_all.tlg %2
)
