@echo off
echo Running 00099.palindrome_check.tlg
if "%2"=="" (
    TinyLanguage.exe 00099.palindrome_check.tlg output.txt
) else (
    TinyLanguage.exe 00099.palindrome_check.tlg %2
)
