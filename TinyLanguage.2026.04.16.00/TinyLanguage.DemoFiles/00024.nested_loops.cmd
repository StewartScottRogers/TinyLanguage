@echo off
echo Running 00024.nested_loops.tlg
if "%2"=="" (
    TinyLanguage.exe 00024.nested_loops.tlg output.txt
) else (
    TinyLanguage.exe 00024.nested_loops.tlg %2
)
