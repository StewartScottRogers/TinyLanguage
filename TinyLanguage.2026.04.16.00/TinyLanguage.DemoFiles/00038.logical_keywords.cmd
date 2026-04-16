@echo off
echo Running 00038.logical_keywords.tlg
if "%2"=="" (
    TinyLanguage.exe 00038.logical_keywords.tlg output.txt
) else (
    TinyLanguage.exe 00038.logical_keywords.tlg %2
)
