@echo off
echo Running 00238.insertion_sort.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000238.insertion_sort.tlg output.txt
) else (
    TinyLanguage.exe %~dp000238.insertion_sort.tlg %2
)
