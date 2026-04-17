@echo off
echo Running 00310.comprehensive_fizzbuzz.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000310.comprehensive_fizzbuzz.tlg output.txt
) else (
    TinyLanguage.exe %~dp000310.comprehensive_fizzbuzz.tlg %2
)
