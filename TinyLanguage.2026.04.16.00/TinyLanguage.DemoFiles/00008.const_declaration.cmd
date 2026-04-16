@echo off
echo Running 00008.const_declaration.tlg
if "%2"=="" (
    TinyLanguage.exe 00008.const_declaration.tlg output.txt
) else (
    TinyLanguage.exe 00008.const_declaration.tlg %2
)
