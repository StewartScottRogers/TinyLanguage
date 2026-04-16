@echo off
echo Running 00260.string_contains.tlg
if "%2"=="" (
    TinyLanguage.exe 00260.string_contains.tlg output.txt
) else (
    TinyLanguage.exe 00260.string_contains.tlg %2
)
