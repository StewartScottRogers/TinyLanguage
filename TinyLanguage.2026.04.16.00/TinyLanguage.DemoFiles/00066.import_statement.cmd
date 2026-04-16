@echo off
echo Running 00066.import_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00066.import_statement.tlg output.txt
) else (
    TinyLanguage.exe 00066.import_statement.tlg %2
)
