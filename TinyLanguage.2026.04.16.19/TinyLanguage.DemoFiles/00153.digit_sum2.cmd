@echo off
echo Running 00153.digit_sum2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000153.digit_sum2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000153.digit_sum2.tlg %2
)
