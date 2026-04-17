@echo off
echo Running 00219.for_to_inclusive2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000219.for_to_inclusive2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000219.for_to_inclusive2.tlg %2
)
