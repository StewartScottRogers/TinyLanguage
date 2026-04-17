@echo off
echo Running 00302.while_do_comparison.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000302.while_do_comparison.tlg output.txt
) else (
    TinyLanguage.exe %~dp000302.while_do_comparison.tlg %2
)
