@echo off
echo Running 00186.ternary_nested.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000186.ternary_nested.tlg output.txt
) else (
    TinyLanguage.exe %~dp000186.ternary_nested.tlg %2
)
