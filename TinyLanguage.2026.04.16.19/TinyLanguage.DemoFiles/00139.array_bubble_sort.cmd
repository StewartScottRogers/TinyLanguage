@echo off
echo Running 00139.array_bubble_sort.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000139.array_bubble_sort.tlg output.txt
) else (
    TinyLanguage.exe %~dp000139.array_bubble_sort.tlg %2
)
