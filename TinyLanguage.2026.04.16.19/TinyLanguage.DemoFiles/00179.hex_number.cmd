@echo off
echo Running 00179.hex_number.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000179.hex_number.tlg output.txt
) else (
    TinyLanguage.exe %~dp000179.hex_number.tlg %2
)
