@echo off
echo Running 00244.string_reverse.tlg
if "%2"=="" (
    TinyLanguage.exe 00244.string_reverse.tlg output.txt
) else (
    TinyLanguage.exe 00244.string_reverse.tlg %2
)
