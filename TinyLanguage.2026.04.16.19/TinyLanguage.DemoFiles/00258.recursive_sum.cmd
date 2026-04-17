@echo off
echo Running 00258.recursive_sum.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000258.recursive_sum.tlg output.txt
) else (
    TinyLanguage.exe %~dp000258.recursive_sum.tlg %2
)
