@echo off
echo Running 00285.word_count.tlg
if "%2"=="" (
    TinyLanguage.exe 00285.word_count.tlg output.txt
) else (
    TinyLanguage.exe 00285.word_count.tlg %2
)
