@echo off
echo Running 00197.import_alias.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000197.import_alias.tlg output.txt
) else (
    TinyLanguage.exe %~dp000197.import_alias.tlg %2
)
