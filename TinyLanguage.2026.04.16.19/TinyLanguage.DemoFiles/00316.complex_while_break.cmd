@echo off
echo Running 00316.complex_while_break.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000316.complex_while_break.tlg output.txt
) else (
    TinyLanguage.exe %~dp000316.complex_while_break.tlg %2
)
