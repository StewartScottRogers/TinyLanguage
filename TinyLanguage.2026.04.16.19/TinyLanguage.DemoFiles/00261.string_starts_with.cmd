@echo off
echo Running 00261.string_starts_with.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000261.string_starts_with.tlg output.txt
) else (
    TinyLanguage.exe %~dp000261.string_starts_with.tlg %2
)
