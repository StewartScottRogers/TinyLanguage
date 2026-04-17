@echo off
echo Running 00183.for_each_with_index.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000183.for_each_with_index.tlg output.txt
) else (
    TinyLanguage.exe %~dp000183.for_each_with_index.tlg %2
)
