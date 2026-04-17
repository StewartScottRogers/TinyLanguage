@echo off
echo Running 00298.const_enum_usage.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000298.const_enum_usage.tlg output.txt
) else (
    TinyLanguage.exe %~dp000298.const_enum_usage.tlg %2
)
