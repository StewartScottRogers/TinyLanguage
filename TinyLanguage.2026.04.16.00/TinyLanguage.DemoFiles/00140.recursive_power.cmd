@echo off
echo Running 00140.recursive_power.tlg
if "%2"=="" (
    TinyLanguage.exe 00140.recursive_power.tlg output.txt
) else (
    TinyLanguage.exe 00140.recursive_power.tlg %2
)
