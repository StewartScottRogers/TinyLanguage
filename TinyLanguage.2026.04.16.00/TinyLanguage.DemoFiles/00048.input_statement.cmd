@echo off
echo Running 00048.input_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00048.input_statement.tlg output.txt
) else (
    TinyLanguage.exe 00048.input_statement.tlg %2
)
