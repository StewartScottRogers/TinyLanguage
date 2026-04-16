@echo off
echo Running 00120.tail_recursion_count.tlg
if "%2"=="" (
    TinyLanguage.exe 00120.tail_recursion_count.tlg output.txt
) else (
    TinyLanguage.exe 00120.tail_recursion_count.tlg %2
)
