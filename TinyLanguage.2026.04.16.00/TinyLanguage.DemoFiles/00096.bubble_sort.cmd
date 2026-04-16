@echo off
echo Running 00096.bubble_sort.tlg
if "%2"=="" (
    TinyLanguage.exe 00096.bubble_sort.tlg output.txt
) else (
    TinyLanguage.exe 00096.bubble_sort.tlg %2
)
