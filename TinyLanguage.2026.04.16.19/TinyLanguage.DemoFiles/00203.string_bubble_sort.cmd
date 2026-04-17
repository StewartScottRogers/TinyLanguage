@echo off
echo Running 00203.string_bubble_sort.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000203.string_bubble_sort.tlg output.txt
) else (
    TinyLanguage.exe %~dp000203.string_bubble_sort.tlg %2
)
