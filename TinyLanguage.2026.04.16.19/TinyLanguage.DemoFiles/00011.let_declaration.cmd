@echo off
echo Running 00011.let_declaration.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000011.let_declaration.tlg output.txt
) else (
    TinyLanguage.exe %~dp000011.let_declaration.tlg %2
)
