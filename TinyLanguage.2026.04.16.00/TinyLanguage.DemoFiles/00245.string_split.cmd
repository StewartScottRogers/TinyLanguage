@echo off
echo Running 00245.string_split.tlg
if "%2"=="" (
    TinyLanguage.exe 00245.string_split.tlg output.txt
) else (
    TinyLanguage.exe 00245.string_split.tlg %2
)
