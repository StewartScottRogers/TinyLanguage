@echo off
echo Running 00215.complex_switch.tlg
if "%2"=="" (
    TinyLanguage.exe 00215.complex_switch.tlg output.txt
) else (
    TinyLanguage.exe 00215.complex_switch.tlg %2
)
