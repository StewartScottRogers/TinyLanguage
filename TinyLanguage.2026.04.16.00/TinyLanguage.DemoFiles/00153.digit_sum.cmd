@echo off
echo Running 00153.digit_sum.tlg
if "%2"=="" (
    TinyLanguage.exe 00153.digit_sum.tlg output.txt
) else (
    TinyLanguage.exe 00153.digit_sum.tlg %2
)
