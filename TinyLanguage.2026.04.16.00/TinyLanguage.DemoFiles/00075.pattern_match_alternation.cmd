@echo off
echo Running 00075.pattern_match_alternation.tlg
if "%2"=="" (
    TinyLanguage.exe 00075.pattern_match_alternation.tlg output.txt
) else (
    TinyLanguage.exe 00075.pattern_match_alternation.tlg %2
)
