@echo off
echo Running 00060.string_comparison_ops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000060.string_comparison_ops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000060.string_comparison_ops.tlg %2
)
