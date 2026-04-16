@echo off
echo Running 00025.nested_while.tlg
if "%2"=="" (
    TinyLanguage.exe 00025.nested_while.tlg output.txt
) else (
    TinyLanguage.exe 00025.nested_while.tlg %2
)
