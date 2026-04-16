@echo off
echo Running 00108.string_length.tlg
if "%2"=="" (
    TinyLanguage.exe 00108.string_length.tlg output.txt
) else (
    TinyLanguage.exe 00108.string_length.tlg %2
)
