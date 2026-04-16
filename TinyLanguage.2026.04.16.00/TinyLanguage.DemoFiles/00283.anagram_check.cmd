@echo off
echo Running 00283.anagram_check.tlg
if "%2"=="" (
    TinyLanguage.exe 00283.anagram_check.tlg output.txt
) else (
    TinyLanguage.exe 00283.anagram_check.tlg %2
)
