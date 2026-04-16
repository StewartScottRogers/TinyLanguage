@echo off
echo Running 00072.pattern_match_basic.tlg
if "%2"=="" (
    TinyLanguage.exe 00072.pattern_match_basic.tlg output.txt
) else (
    TinyLanguage.exe 00072.pattern_match_basic.tlg %2
)
