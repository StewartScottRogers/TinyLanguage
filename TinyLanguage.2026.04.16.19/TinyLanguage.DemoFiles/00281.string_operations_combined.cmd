@echo off
echo Running 00281.string_operations_combined.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000281.string_operations_combined.tlg output.txt
) else (
    TinyLanguage.exe %~dp000281.string_operations_combined.tlg %2
)
