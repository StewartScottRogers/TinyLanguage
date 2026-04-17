@echo off
echo Running 00065.if_complex_condition.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000065.if_complex_condition.tlg output.txt
) else (
    TinyLanguage.exe %~dp000065.if_complex_condition.tlg %2
)
