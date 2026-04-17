@echo off
echo Running 00038.compare_mixed.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000038.compare_mixed.tlg output.txt
) else (
    TinyLanguage.exe %~dp000038.compare_mixed.tlg %2
)
