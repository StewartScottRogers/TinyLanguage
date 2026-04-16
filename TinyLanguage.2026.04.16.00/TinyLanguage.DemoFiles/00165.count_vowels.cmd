@echo off
echo Running 00165.count_vowels.tlg
if "%2"=="" (
    TinyLanguage.exe 00165.count_vowels.tlg output.txt
) else (
    TinyLanguage.exe 00165.count_vowels.tlg %2
)
