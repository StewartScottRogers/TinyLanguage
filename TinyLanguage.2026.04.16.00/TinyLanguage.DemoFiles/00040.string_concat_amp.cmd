@echo off
echo Running 00040.string_concat_amp.tlg
if "%2"=="" (
    TinyLanguage.exe 00040.string_concat_amp.tlg output.txt
) else (
    TinyLanguage.exe 00040.string_concat_amp.tlg %2
)
