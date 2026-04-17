@echo off
echo Running 00247.palindrome_check.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000247.palindrome_check.tlg output.txt
) else (
    TinyLanguage.exe %~dp000247.palindrome_check.tlg %2
)
