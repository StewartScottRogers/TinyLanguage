@echo off
echo Running 00078.pattern_match_null.tlg
if "%2"=="" (
    TinyLanguage.exe 00078.pattern_match_null.tlg output.txt
) else (
    TinyLanguage.exe 00078.pattern_match_null.tlg %2
)
