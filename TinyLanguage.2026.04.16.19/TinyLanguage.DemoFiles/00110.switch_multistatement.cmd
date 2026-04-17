@echo off
echo Running 00110.switch_multistatement.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000110.switch_multistatement.tlg output.txt
) else (
    TinyLanguage.exe %~dp000110.switch_multistatement.tlg %2
)
