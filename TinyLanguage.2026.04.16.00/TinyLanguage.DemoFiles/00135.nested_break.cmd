@echo off
echo Running 00135.nested_break.tlg
if "%2"=="" (
    TinyLanguage.exe 00135.nested_break.tlg output.txt
) else (
    TinyLanguage.exe 00135.nested_break.tlg %2
)
