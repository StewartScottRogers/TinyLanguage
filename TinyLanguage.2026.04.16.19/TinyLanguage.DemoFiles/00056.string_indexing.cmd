@echo off
echo Running 00056.string_indexing.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000056.string_indexing.tlg output.txt
) else (
    TinyLanguage.exe %~dp000056.string_indexing.tlg %2
)
