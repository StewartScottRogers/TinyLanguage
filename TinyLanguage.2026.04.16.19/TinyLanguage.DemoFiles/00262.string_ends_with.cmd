@echo off
echo Running 00262.string_ends_with.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000262.string_ends_with.tlg output.txt
) else (
    TinyLanguage.exe %~dp000262.string_ends_with.tlg %2
)
