@echo off
echo Running 00256.power_set.tlg
if "%2"=="" (
    TinyLanguage.exe 00256.power_set.tlg output.txt
) else (
    TinyLanguage.exe 00256.power_set.tlg %2
)
