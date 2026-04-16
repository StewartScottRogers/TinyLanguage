@echo off
echo Running 00034.power_operator.tlg
if "%2"=="" (
    TinyLanguage.exe 00034.power_operator.tlg output.txt
) else (
    TinyLanguage.exe 00034.power_operator.tlg %2
)
