@echo off
echo Running 00187.pattern_wildcard.tlg
if "%2"=="" (
    TinyLanguage.exe 00187.pattern_wildcard.tlg output.txt
) else (
    TinyLanguage.exe 00187.pattern_wildcard.tlg %2
)
