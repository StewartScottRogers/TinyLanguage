@echo off
echo Running 00154.count_digits2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000154.count_digits2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000154.count_digits2.tlg %2
)
