@echo off
echo Running 00127.array_comprehension.tlg
if "%2"=="" (
    TinyLanguage.exe 00127.array_comprehension.tlg output.txt
) else (
    TinyLanguage.exe 00127.array_comprehension.tlg %2
)
