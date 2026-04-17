@echo off
echo Running 00314.pattern_match_wildcard.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000314.pattern_match_wildcard.tlg output.txt
) else (
    TinyLanguage.exe %~dp000314.pattern_match_wildcard.tlg %2
)
