@echo off
echo Running 00311.number_base_literals.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000311.number_base_literals.tlg output.txt
) else (
    TinyLanguage.exe %~dp000311.number_base_literals.tlg %2
)
