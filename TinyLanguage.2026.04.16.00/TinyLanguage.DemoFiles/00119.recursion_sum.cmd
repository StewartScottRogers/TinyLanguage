@echo off
echo Running 00119.recursion_sum.tlg
if "%2"=="" (
    TinyLanguage.exe 00119.recursion_sum.tlg output.txt
) else (
    TinyLanguage.exe 00119.recursion_sum.tlg %2
)
