@echo off
echo Running 00171.exception_hierarchy.tlg
if "%2"=="" (
    TinyLanguage.exe 00171.exception_hierarchy.tlg output.txt
) else (
    TinyLanguage.exe 00171.exception_hierarchy.tlg %2
)
