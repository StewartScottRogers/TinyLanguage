@echo off
echo Running 00138.array_compute.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000138.array_compute.tlg output.txt
) else (
    TinyLanguage.exe %~dp000138.array_compute.tlg %2
)
