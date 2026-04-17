@echo off
echo Running 00044.boolean_and_or_combined.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000044.boolean_and_or_combined.tlg output.txt
) else (
    TinyLanguage.exe %~dp000044.boolean_and_or_combined.tlg %2
)
