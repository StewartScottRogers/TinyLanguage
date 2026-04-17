@echo off
echo Running 00082.for_sum.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000082.for_sum.tlg output.txt
) else (
    TinyLanguage.exe %~dp000082.for_sum.tlg %2
)
