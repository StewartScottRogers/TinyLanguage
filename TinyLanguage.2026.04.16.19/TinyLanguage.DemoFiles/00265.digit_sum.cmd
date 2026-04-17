@echo off
echo Running 00265.digit_sum.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000265.digit_sum.tlg output.txt
) else (
    TinyLanguage.exe %~dp000265.digit_sum.tlg %2
)
