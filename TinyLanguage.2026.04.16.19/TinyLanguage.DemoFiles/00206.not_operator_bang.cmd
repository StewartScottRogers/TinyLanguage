@echo off
echo Running 00206.not_operator_bang.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000206.not_operator_bang.tlg output.txt
) else (
    TinyLanguage.exe %~dp000206.not_operator_bang.tlg %2
)
