@echo off
echo Running 00290.unique_elements.tlg
if "%2"=="" (
    TinyLanguage.exe 00290.unique_elements.tlg output.txt
) else (
    TinyLanguage.exe 00290.unique_elements.tlg %2
)
