@echo off
echo Running 00185.list_comprehension_transform.tlg
if "%2"=="" (
    TinyLanguage.exe 00185.list_comprehension_transform.tlg output.txt
) else (
    TinyLanguage.exe 00185.list_comprehension_transform.tlg %2
)
