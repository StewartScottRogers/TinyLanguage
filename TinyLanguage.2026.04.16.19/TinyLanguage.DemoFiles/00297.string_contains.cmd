@echo off
echo Running 00297.string_contains.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000297.string_contains.tlg output.txt
) else (
    TinyLanguage.exe %~dp000297.string_contains.tlg %2
)
