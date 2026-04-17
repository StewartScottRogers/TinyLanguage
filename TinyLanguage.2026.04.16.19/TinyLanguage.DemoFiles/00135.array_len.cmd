@echo off
echo Running 00135.array_len.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000135.array_len.tlg output.txt
) else (
    TinyLanguage.exe %~dp000135.array_len.tlg %2
)
