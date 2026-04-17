@echo off
echo Running 00033.compare_less.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000033.compare_less.tlg output.txt
) else (
    TinyLanguage.exe %~dp000033.compare_less.tlg %2
)
