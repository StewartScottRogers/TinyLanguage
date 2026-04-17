@echo off
echo Running 00237.selection_sort.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000237.selection_sort.tlg output.txt
) else (
    TinyLanguage.exe %~dp000237.selection_sort.tlg %2
)
