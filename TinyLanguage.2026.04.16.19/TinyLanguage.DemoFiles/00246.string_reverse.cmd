@echo off
echo Running 00246.string_reverse.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000246.string_reverse.tlg output.txt
) else (
    TinyLanguage.exe %~dp000246.string_reverse.tlg %2
)
