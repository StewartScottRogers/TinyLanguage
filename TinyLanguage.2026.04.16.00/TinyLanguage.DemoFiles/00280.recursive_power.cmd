@echo off
echo Running 00280.recursive_power.tlg
if "%2"=="" (
    TinyLanguage.exe 00280.recursive_power.tlg output.txt
) else (
    TinyLanguage.exe 00280.recursive_power.tlg %2
)
