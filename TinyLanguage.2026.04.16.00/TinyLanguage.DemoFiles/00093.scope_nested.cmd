@echo off
echo Running 00093.scope_nested.tlg
if "%2"=="" (
    TinyLanguage.exe 00093.scope_nested.tlg output.txt
) else (
    TinyLanguage.exe 00093.scope_nested.tlg %2
)
