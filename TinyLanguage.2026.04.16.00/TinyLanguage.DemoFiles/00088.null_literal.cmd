@echo off
echo Running 00088.null_literal.tlg
if "%2"=="" (
    TinyLanguage.exe 00088.null_literal.tlg output.txt
) else (
    TinyLanguage.exe 00088.null_literal.tlg %2
)
