@echo off
echo Running 00284.roman_numerals.tlg
if "%2"=="" (
    TinyLanguage.exe 00284.roman_numerals.tlg output.txt
) else (
    TinyLanguage.exe 00284.roman_numerals.tlg %2
)
