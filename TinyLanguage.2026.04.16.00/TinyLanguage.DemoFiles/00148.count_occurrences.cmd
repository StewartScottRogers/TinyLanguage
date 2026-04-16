@echo off
echo Running 00148.count_occurrences.tlg
if "%2"=="" (
    TinyLanguage.exe 00148.count_occurrences.tlg output.txt
) else (
    TinyLanguage.exe 00148.count_occurrences.tlg %2
)
