@echo off
echo Running 00053.string_len.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000053.string_len.tlg output.txt
) else (
    TinyLanguage.exe %~dp000053.string_len.tlg %2
)
