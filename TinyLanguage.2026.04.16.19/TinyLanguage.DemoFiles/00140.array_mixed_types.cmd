@echo off
echo Running 00140.array_mixed_types.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000140.array_mixed_types.tlg output.txt
) else (
    TinyLanguage.exe %~dp000140.array_mixed_types.tlg %2
)
