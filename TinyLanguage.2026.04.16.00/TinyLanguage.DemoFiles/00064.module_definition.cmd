@echo off
echo Running 00064.module_definition.tlg
if "%2"=="" (
    TinyLanguage.exe 00064.module_definition.tlg output.txt
) else (
    TinyLanguage.exe 00064.module_definition.tlg %2
)
