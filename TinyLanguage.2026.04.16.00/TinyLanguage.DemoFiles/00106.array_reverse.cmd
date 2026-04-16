@echo off
echo Running 00106.array_reverse.tlg
if "%2"=="" (
    TinyLanguage.exe 00106.array_reverse.tlg output.txt
) else (
    TinyLanguage.exe 00106.array_reverse.tlg %2
)
