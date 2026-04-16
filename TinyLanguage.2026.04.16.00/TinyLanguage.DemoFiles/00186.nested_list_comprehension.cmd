@echo off
echo Running 00186.nested_list_comprehension.tlg
if "%2"=="" (
    TinyLanguage.exe 00186.nested_list_comprehension.tlg output.txt
) else (
    TinyLanguage.exe 00186.nested_list_comprehension.tlg %2
)
