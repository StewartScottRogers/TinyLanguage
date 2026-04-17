@echo off
echo Running 00223.pattern_match_string.tlg
if "%2"=="" (
    TinyLanguage.exe %~dp000223.pattern_match_string.tlg output.txt
) else (
    TinyLanguage.exe %~dp000223.pattern_match_string.tlg %2
)
