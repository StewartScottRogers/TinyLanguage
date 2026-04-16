@echo off
echo Running 00105.array_contains.tlg
if "%2"=="" (
    TinyLanguage.exe 00105.array_contains.tlg output.txt
) else (
    TinyLanguage.exe 00105.array_contains.tlg %2
)
