@echo off
echo Running 00319.comprehensive_array_ops.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000319.comprehensive_array_ops.tlg output.txt
) else (
    TinyLanguage.exe %~dp000319.comprehensive_array_ops.tlg %2
)
