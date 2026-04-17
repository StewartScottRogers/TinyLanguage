@echo off
echo Running 00252.enum_with_values.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000252.enum_with_values.tlg output.txt
) else (
    TinyLanguage.exe %~dp000252.enum_with_values.tlg %2
)
