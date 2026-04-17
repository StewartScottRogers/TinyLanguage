@echo off
echo Running 00192.pattern_match_alternation.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000192.pattern_match_alternation.tlg output.txt
) else (
    TinyLanguage.exe %~dp000192.pattern_match_alternation.tlg %2
)
