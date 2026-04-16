@echo off
echo Running 00144.insertion_sort.tlg
if "%2"=="" (
    TinyLanguage.exe 00144.insertion_sort.tlg output.txt
) else (
    TinyLanguage.exe 00144.insertion_sort.tlg %2
)
