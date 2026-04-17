@echo off
echo Running 00292.all_comparison_ops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000292.all_comparison_ops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000292.all_comparison_ops.tlg %2
)
