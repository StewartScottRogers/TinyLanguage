@echo off
echo Running 00012.var_declaration.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000012.var_declaration.tlg output.txt
) else (
    TinyLanguage.exe %~dp000012.var_declaration.tlg %2
)
