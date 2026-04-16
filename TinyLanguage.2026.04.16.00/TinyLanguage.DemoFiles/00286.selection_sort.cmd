@echo off
echo Running 00286.selection_sort.tlg
if "%2"=="" (
    TinyLanguage.exe 00286.selection_sort.tlg output.txt
) else (
    TinyLanguage.exe 00286.selection_sort.tlg %2
)
