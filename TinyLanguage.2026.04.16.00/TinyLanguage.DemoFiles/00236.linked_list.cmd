@echo off
echo Running 00236.linked_list.tlg
if "%2"=="" (
    TinyLanguage.exe 00236.linked_list.tlg output.txt
) else (
    TinyLanguage.exe 00236.linked_list.tlg %2
)
