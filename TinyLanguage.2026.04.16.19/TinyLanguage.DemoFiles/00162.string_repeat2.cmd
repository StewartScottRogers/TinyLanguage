@echo off
echo Running 00162.string_repeat2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000162.string_repeat2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000162.string_repeat2.tlg %2
)
