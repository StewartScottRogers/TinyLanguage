@echo off
echo Running 00039.string_concat_plus.tlg
if "%2"=="" (
    TinyLanguage.exe 00039.string_concat_plus.tlg output.txt
) else (
    TinyLanguage.exe 00039.string_concat_plus.tlg %2
)
