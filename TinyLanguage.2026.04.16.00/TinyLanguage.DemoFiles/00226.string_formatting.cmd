@echo off
echo Running 00226.string_formatting.tlg
if "%2"=="" (
    TinyLanguage.exe 00226.string_formatting.tlg output.txt
) else (
    TinyLanguage.exe 00226.string_formatting.tlg %2
)
