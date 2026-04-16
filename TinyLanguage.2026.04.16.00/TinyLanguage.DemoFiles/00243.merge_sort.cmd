@echo off
echo Running 00243.merge_sort.tlg
if "%2"=="" (
    TinyLanguage.exe 00243.merge_sort.tlg output.txt
) else (
    TinyLanguage.exe 00243.merge_sort.tlg %2
)
