@echo off
echo Running 00006.let_declaration.tlg
if "%2"=="" (
    TinyLanguage.exe 00006.let_declaration.tlg output.txt
) else (
    TinyLanguage.exe 00006.let_declaration.tlg %2
)
