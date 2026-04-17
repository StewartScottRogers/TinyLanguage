@echo off
echo Running 00216.array_mixed_types2.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000216.array_mixed_types2.tlg output.txt
) else (
    TinyLanguage.exe %~dp000216.array_mixed_types2.tlg %2
)
