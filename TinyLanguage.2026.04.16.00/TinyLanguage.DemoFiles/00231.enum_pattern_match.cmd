@echo off
echo Running 00231.enum_pattern_match.tlg
if "%2"=="" (
    TinyLanguage.exe 00231.enum_pattern_match.tlg output.txt
) else (
    TinyLanguage.exe 00231.enum_pattern_match.tlg %2
)
