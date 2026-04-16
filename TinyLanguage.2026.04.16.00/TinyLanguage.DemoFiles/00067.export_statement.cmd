@echo off
echo Running 00067.export_statement.tlg
if "%2"=="" (
    TinyLanguage.exe 00067.export_statement.tlg output.txt
) else (
    TinyLanguage.exe 00067.export_statement.tlg %2
)
