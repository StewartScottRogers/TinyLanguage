@echo off
echo Running 00051.string_concat_plus.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000051.string_concat_plus.tlg output.txt
) else (
    TinyLanguage.exe %~dp000051.string_concat_plus.tlg %2
)
