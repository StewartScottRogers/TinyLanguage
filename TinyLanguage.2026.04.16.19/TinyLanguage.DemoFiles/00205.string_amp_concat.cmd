@echo off
echo Running 00205.string_amp_concat.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000205.string_amp_concat.tlg output.txt
) else (
    TinyLanguage.exe %~dp000205.string_amp_concat.tlg %2
)
