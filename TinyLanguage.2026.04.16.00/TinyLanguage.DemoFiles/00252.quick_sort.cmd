@echo off
echo Running 00252.quick_sort.tlg
if "%2"=="" (
    TinyLanguage.exe 00252.quick_sort.tlg output.txt
) else (
    TinyLanguage.exe 00252.quick_sort.tlg %2
)
