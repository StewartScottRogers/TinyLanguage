@echo off
echo Running 00073.pattern_match_strings.tlg
if "%2"=="" (
    TinyLanguage.exe 00073.pattern_match_strings.tlg output.txt
) else (
    TinyLanguage.exe 00073.pattern_match_strings.tlg %2
)
