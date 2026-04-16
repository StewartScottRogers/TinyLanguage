@echo off
echo Running 00074.pattern_match_wildcard.tlg
if "%2"=="" (
    TinyLanguage.exe 00074.pattern_match_wildcard.tlg output.txt
) else (
    TinyLanguage.exe 00074.pattern_match_wildcard.tlg %2
)
