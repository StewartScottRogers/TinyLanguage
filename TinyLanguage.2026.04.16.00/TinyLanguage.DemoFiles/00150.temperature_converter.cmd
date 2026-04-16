@echo off
echo Running 00150.temperature_converter.tlg
if "%2"=="" (
    TinyLanguage.exe 00150.temperature_converter.tlg output.txt
) else (
    TinyLanguage.exe 00150.temperature_converter.tlg %2
)
