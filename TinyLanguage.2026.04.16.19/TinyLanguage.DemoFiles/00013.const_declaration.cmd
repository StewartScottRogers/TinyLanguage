@echo off
echo Running 00013.const_declaration.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000013.const_declaration.tlg output.txt
) else (
    TinyLanguage.exe %~dp000013.const_declaration.tlg %2
)
