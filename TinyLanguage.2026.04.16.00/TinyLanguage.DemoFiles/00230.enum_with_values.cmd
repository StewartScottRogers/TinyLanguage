@echo off
echo Running 00230.enum_with_values.tlg
if "%2"=="" (
    TinyLanguage.exe 00230.enum_with_values.tlg output.txt
) else (
    TinyLanguage.exe 00230.enum_with_values.tlg %2
)
