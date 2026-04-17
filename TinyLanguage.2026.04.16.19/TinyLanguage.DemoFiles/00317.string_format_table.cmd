@echo off
echo Running 00317.string_format_table.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000317.string_format_table.tlg output.txt
) else (
    TinyLanguage.exe %~dp000317.string_format_table.tlg %2
)
