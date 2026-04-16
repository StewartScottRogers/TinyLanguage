@echo off
echo Running 00130.string_plus_int.tlg
if "%2"=="" (
    TinyLanguage.exe 00130.string_plus_int.tlg output.txt
) else (
    TinyLanguage.exe 00130.string_plus_int.tlg %2
)
