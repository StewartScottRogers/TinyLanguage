@echo off
echo Running 00089.boolean_literals.tlg
if "%2"=="" (
    TinyLanguage.exe 00089.boolean_literals.tlg output.txt
) else (
    TinyLanguage.exe 00089.boolean_literals.tlg %2
)
