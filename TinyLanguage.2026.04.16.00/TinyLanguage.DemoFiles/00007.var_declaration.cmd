@echo off
echo Running 00007.var_declaration.tlg
if "%2"=="" (
    TinyLanguage.exe 00007.var_declaration.tlg output.txt
) else (
    TinyLanguage.exe 00007.var_declaration.tlg %2
)
