@echo off
echo Running 00164.is_vowel.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000164.is_vowel.tlg output.txt
) else (
    TinyLanguage.exe %~dp000164.is_vowel.tlg %2
)
