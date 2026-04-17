@echo off
echo Running 00059.string_number_mix.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000059.string_number_mix.tlg output.txt
) else (
    TinyLanguage.exe %~dp000059.string_number_mix.tlg %2
)
