@echo off
echo Running 00076.pattern_match_when.tlg
if "%2"=="" (
    TinyLanguage.exe 00076.pattern_match_when.tlg output.txt
) else (
    TinyLanguage.exe 00076.pattern_match_when.tlg %2
)
