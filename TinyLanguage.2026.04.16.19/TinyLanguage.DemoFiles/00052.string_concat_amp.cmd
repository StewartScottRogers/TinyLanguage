@echo off
echo Running 00052.string_concat_amp.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000052.string_concat_amp.tlg output.txt
) else (
    TinyLanguage.exe %~dp000052.string_concat_amp.tlg %2
)
