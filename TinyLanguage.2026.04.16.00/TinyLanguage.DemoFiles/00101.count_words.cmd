@echo off
echo Running 00101.count_words.tlg
if "%2"=="" (
    TinyLanguage.exe 00101.count_words.tlg output.txt
) else (
    TinyLanguage.exe 00101.count_words.tlg %2
)
