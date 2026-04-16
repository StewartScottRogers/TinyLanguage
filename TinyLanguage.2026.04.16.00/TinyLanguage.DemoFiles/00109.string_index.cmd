@echo off
echo Running 00109.string_index.tlg
if "%2"=="" (
    TinyLanguage.exe 00109.string_index.tlg output.txt
) else (
    TinyLanguage.exe 00109.string_index.tlg %2
)
