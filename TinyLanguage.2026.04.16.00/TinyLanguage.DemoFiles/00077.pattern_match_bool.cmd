@echo off
echo Running 00077.pattern_match_bool.tlg
if "%2"=="" (
    TinyLanguage.exe 00077.pattern_match_bool.tlg output.txt
) else (
    TinyLanguage.exe 00077.pattern_match_bool.tlg %2
)
