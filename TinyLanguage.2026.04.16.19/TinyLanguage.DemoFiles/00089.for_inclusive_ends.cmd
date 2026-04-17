@echo off
echo Running 00089.for_inclusive_ends.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000089.for_inclusive_ends.tlg output.txt
) else (
    TinyLanguage.exe %~dp000089.for_inclusive_ends.tlg %2
)
