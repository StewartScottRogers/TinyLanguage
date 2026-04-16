@echo off
echo Running 00050.array_index_access.tlg
if "%2"=="" (
    TinyLanguage.exe 00050.array_index_access.tlg output.txt
) else (
    TinyLanguage.exe 00050.array_index_access.tlg %2
)
